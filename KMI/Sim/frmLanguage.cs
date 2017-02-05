namespace KMI.Sim
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;
    using System.Windows.Forms;

    public class frmLanguage : Form
    {
        private Button btnOK;
        private Container components = null;
        private string[] languageCodes;
        private string[] languageNames;
        private ListBox lstLanguages;
        private string preferredLanguageCode;

        public frmLanguage()
        {
            this.InitializeComponent();
            AppSettingsReader reader = new AppSettingsReader();
            this.preferredLanguageCode = (string) reader.GetValue("PreferredLanguageCode", typeof(string));
            this.languageNames = ((string) reader.GetValue("SupportedLanguageNames", typeof(string))).Split(new char[] { '|' });
            this.languageCodes = ((string) reader.GetValue("SupportedLanguageCodes", typeof(string))).Split(new char[] { '|' });
            this.lstLanguages.Items.Add("English");
            for (int i = 0; i < this.languageNames.Length; i++) {
                this.lstLanguages.Items.Add(this.languageNames[i]);
                if (this.preferredLanguageCode == this.languageCodes[i]) {
                    this.lstLanguages.SelectedIndex = this.lstLanguages.Items.Count - 1;
                }
            }
            if (this.lstLanguages.SelectedIndex == -1) {
                this.lstLanguages.SelectedIndex = 0;
            }
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

        private void frmLanguage_Closed(object sender, EventArgs e)
        {
            if (this.lstLanguages.SelectedIndex > 0)
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(this.languageCodes[this.lstLanguages.SelectedIndex - 1]);
            }
        }

        private void InitializeComponent()
        {
            this.lstLanguages = new ListBox();
            this.btnOK = new Button();
            base.SuspendLayout();
            this.lstLanguages.Location = new Point(40, 16);
            this.lstLanguages.Name = "lstLanguages";
            this.lstLanguages.Size = new Size(0xd0, 0x45);
            this.lstLanguages.TabIndex = 0;
            this.btnOK.Location = new Point(0x60, 0x68);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new Size(0x60, 0x18);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            base.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x11a, 0x90);
            base.Controls.Add(this.btnOK);
            base.Controls.Add(this.lstLanguages);
            base.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            base.Name = "frmLanguage";
            base.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Select Language";
            base.Closed += new EventHandler(this.frmLanguage_Closed);
            base.ResumeLayout(false);
        }

        public int LanguageCount
        {
            get
            {
                if (this.languageCodes[0] == "")
                {
                    return 0;
                }
                return this.languageCodes.Length;
            }
        }
    }
}

