namespace KMI.Sim
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public class frmMessages : Form
    {
        protected bool alternateBackground = false;
        protected const int AVAILABLE_WIDTH = 30;
        private IContainer components;
        public static int MAX_MESSAGES = 20;
        public List<frmMainBase> Controller = new List<frmMainBase>();

        public frmMessages()
        {
            this.InitializeComponent();
        }

        public void AddMessage(PlayerMessage message)
        {
            if (message.NotificationColor == NotificationColor.Red || message.NotificationColor == NotificationColor.Yellow)
                this.Visible = true;
            if (message.NotificationColor == NotificationColor.Red)
                ((frmMainBase)this.Owner).mnuOptionsGoStop_Click(null, null);
            if (message.NotificationColor == NotificationColor.Yellow)
                if (!(message.To == "All Players"))
                    if (S.I.SimState.SpeedIndex > 1)
                        ((frmMainBase)this.Owner).mnuOptionsSlower_Click(null, null);
            if (base.Controls.Count == MAX_MESSAGES)
            {
                base.Controls.RemoveAt(0);
            }
            MessageControl control = new MessageControl(message);
            if (this.alternateBackground)
            {
                control.BackColor = Color.Gainsboro;
            }
            this.alternateBackground = !this.alternateBackground;
            base.SuspendLayout();
            base.Controls.Add(control);
            base.ResumeLayout();
            this.frmMessages_Resize(this, new EventArgs());
            base.ScrollControlIntoView(base.Controls[base.Controls.Count - 1]);
        }

        public void Clear()
        {
            base.SuspendLayout();
            base.Controls.Clear();
            base.ResumeLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmMessages_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            S.MF.HideMessageWindow();
        }

        private void frmMessages_Resize(object sender, EventArgs e)
        {
            base.SuspendLayout();
            for (int i = 0; i < base.Controls.Count; i++)
            {
                MessageControl control = (MessageControl) base.Controls[i];
                control.Width = base.Width - 30;
                if (i == 0)
                {
                    control.Location = new Point(base.AutoScrollPosition.X, base.AutoScrollPosition.Y);
                }
                else
                {
                    control.Location = new Point(base.AutoScrollPosition.X, base.Controls[i - 1].Location.Y + base.Controls[i - 1].Height);
                }
            }
            base.ResumeLayout();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmMessages
            // 
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(288, 166);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Location = new System.Drawing.Point(0, 5000);
            this.MinimumSize = new System.Drawing.Size(200, 160);
            this.Name = "frmMessages";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Message Center";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMessages_Closing);
            this.Resize += new System.EventHandler(this.frmMessages_Resize);
            this.ResumeLayout(false);

        }
    }
}

