namespace KMI.VBPF1Lib
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmMessageBoxWithIcon : Form
    {
        private Button btnOK;
        private Container components = null;
        private Label labIcon;
        private Label labMessage;

        public frmMessageBoxWithIcon(string message, Bitmap icon)
        {
            this.InitializeComponent();
            this.Text = Application.ProductName;
            this.labIcon.Image = icon;
            this.labMessage.Text = message;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnOK = new System.Windows.Forms.Button();
            this.labIcon = new System.Windows.Forms.Label();
            this.labMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(148, 124);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(112, 24);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // labIcon
            // 
            this.labIcon.Location = new System.Drawing.Point(20, 12);
            this.labIcon.Name = "labIcon";
            this.labIcon.Size = new System.Drawing.Size(108, 92);
            this.labIcon.TabIndex = 1;
            // 
            // labMessage
            // 
            this.labMessage.Location = new System.Drawing.Point(144, 12);
            this.labMessage.Name = "labMessage";
            this.labMessage.Size = new System.Drawing.Size(248, 88);
            this.labMessage.TabIndex = 2;
            this.labMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmMessageBoxWithIcon
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(412, 158);
            this.Controls.Add(this.labMessage);
            this.Controls.Add(this.labIcon);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmMessageBoxWithIcon";
            this.ShowInTaskbar = false;
            this.Text = "frmMessageBoxWithIcon";
            this.ResumeLayout(false);

        }
    }
}

