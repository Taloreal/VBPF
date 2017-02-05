namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class frmSnapshot : Form
    {
        private static Bitmap buffer;
        public MenuItem EnablingReference;
        private static Graphics g;
        private Input input;
        private Panel panMain;
        private ArrayList statusToolTips = new ArrayList();
        private Timer Updater;
        private System.ComponentModel.IContainer components;
        private ToolTip toolTip;

        public frmSnapshot()
        {
            this.InitializeComponent();
            A.MF.NewDay += new EventHandler(this.NewDayHandler);
            A.MF.EntityChanged += new EventHandler(this.EntityChangedHandler);
            this.toolTip = new ToolTip();
            this.toolTip.InitialDelay = 0;
            this.GetData();
        }

        protected void DrawStatusIcon(Graphics g, string image, float val, string toolTip, ref ArrayList toolTips)
        {
            this.statusToolTips.Add(toolTip);
            int x = 230 - (this.statusToolTips.Count * 0x2e);
            int y = 0;
            g.DrawImageUnscaled(A.R.GetImage(image), x, y);
            Color color = Color.FromArgb(0x67, 0xb6, 0x67);
            if (val < 0.1)
            {
                color = Color.FromArgb(0xcf, 0x6f, 0x6f);
            }
            else if (val < 0.66)
            {
                color = Color.FromArgb(0xc3, 0xb1, 0x6b);
            }
            Brush brush = new SolidBrush(color);
            int height = (int) Utilities.Clamp(19f * val, 1f, 19f);
            g.FillRectangle(brush, x + 11, ((y + 7) + 0x12) - height, 7, height);
        }

        protected virtual void EntityChangedHandler(object sender, EventArgs e)
        {
            if (!((this.EnablingReference == null) || this.EnablingReference.Enabled))
            {
                base.Close();
            }
            else if (this.GetData())
            {
                this.panMain.Refresh();
            }
        }

        protected void frmReport_Closed(object sender, EventArgs e)
        {
            A.MF.NewDay -= new EventHandler(this.NewDayHandler);
            A.MF.EntityChanged -= new EventHandler(this.EntityChangedHandler);
        }

        private void frmSnapshot_Load(object sender, EventArgs e)
        {
            base.Location = new Point((A.MF.Bounds.Right - base.Width) - 6, (A.MF.Bounds.Bottom - base.Height) - 20);
        }

        private void frmSnapshot_Resize(object sender, EventArgs e)
        {
            base.Location = new Point((A.MF.Bounds.Right - base.Width) - 6, (A.MF.Bounds.Bottom - base.Height) - 20);
        }

        protected bool GetData()
        {
            try
            {
                this.input = A.SA.GetSnapshot(A.MF.CurrentEntityID);
                return true;
            }
            catch (EntityNotFoundException)
            {
                base.Close();
                return false;
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panMain = new System.Windows.Forms.Panel();
            this.Updater = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // panMain
            // 
            this.panMain.Location = new System.Drawing.Point(0, 2);
            this.panMain.Name = "panMain";
            this.panMain.Size = new System.Drawing.Size(232, 33);
            this.panMain.TabIndex = 0;
            this.panMain.Paint += new System.Windows.Forms.PaintEventHandler(this.panMain_Paint);
            this.panMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panMain_MouseMove);
            // 
            // Updater
            // 
            this.Updater.Enabled = true;
            this.Updater.Interval = 3000;
            this.Updater.Tick += new System.EventHandler(this.Updater_Tick);
            // 
            // frmSnapshot
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(232, 36);
            this.Controls.Add(this.panMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmSnapshot";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Snapshot";
            this.Closed += new System.EventHandler(this.frmReport_Closed);
            this.Load += new System.EventHandler(this.frmSnapshot_Load);
            this.Resize += new System.EventHandler(this.frmSnapshot_Resize);
            this.ResumeLayout(false);

        }

        protected void NewDayHandler(object sender, EventArgs e)
        {
            if (this.GetData())
            {
                this.panMain.Refresh();
            }
        }

        private void panMain_MouseMove(object sender, MouseEventArgs e)
        {
            int num = (this.panMain.Width - e.X) / 0x2e;
            if ((num >= 0) && (num < this.statusToolTips.Count))
            {
                this.toolTip.SetToolTip(this.panMain, (string) this.statusToolTips[num]);
            }
            else
            {
                this.toolTip.SetToolTip(this.panMain, "");
            }
        }

        private void panMain_Paint(object sender, PaintEventArgs e)
        {
            this.statusToolTips = new ArrayList();
            if (buffer == null)
            {
                buffer = new Bitmap(230, this.panMain.Height, e.Graphics);
                g = Graphics.FromImage(buffer);
            }
            g.Clear(this.BackColor);
            if (this.input.gas > -1f)
            {
                string str = "OK";
                if (this.input.carBroken)
                {
                    str = "Broken";
                }
                this.DrawStatusIcon(g, this.input.carImageName + str, -1f, A.R.GetString("Car: {0}", new object[] { str }), ref this.statusToolTips);
                this.DrawStatusIcon(g, "StatusGas", this.input.gas / 60f, A.R.GetString("Gas: {0} gals", new object[] { this.input.gas.ToString("N1") }), ref this.statusToolTips);
            }
            if (this.input.busTokens > -1)
            {
                this.DrawStatusIcon(g, "StatusBusTokens", ((float) this.input.busTokens) / 60f, A.R.GetString("Bus Tokens: {0}", new object[] { this.input.busTokens }), ref this.statusToolTips);
            }
            this.DrawStatusIcon(g, "StatusFood", ((float) this.input.food) / 60f, A.R.GetString("Food: {0} meals", new object[] { this.input.food }), ref this.statusToolTips);
            this.DrawStatusIcon(g, "StatusHealth", this.input.health, A.R.GetString("Health: {0}", new object[] { Utilities.FP(this.input.health) }), ref this.statusToolTips);
            int num = 0x2e * (5 - this.statusToolTips.Count);
            base.Width = 0xee - num;
            this.panMain.Left = -num;
            e.Graphics.DrawImageUnscaled(buffer, 0, 0);
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public int food;
            public float health;
            public int busTokens;
            public float gas;
            public string carImageName;
            public bool carBroken;
        }

        private void Updater_Tick(object sender, EventArgs e)
        {
            if (this.GetData())
            {
                this.panMain.Refresh();
            }
        }
    }
}

