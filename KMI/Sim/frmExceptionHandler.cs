namespace KMI.Sim
{
    using KMI.Utility;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Remoting;
    using System.Text;
    using System.Windows.Forms;
    using System.Net.Mail;

    public class frmExceptionHandler : Form
    {
        protected static bool alreadyHandling = false;
        public static bool ORIGINAL_REPORT = false;
        private Label cdKeyLabel;
        private Container components = null;
        private Button doneButton;
        private TextBox errorMessageTextBox;
        private TextBox errorTextTextBox;
        private Button reportButton;
        public static string ReportURL = "http://www.knowledgematters.com/reports/bugs.php";
        //public static string SupportEmail = "support@knowledgematters.com";
        public static string SupportEmail = "naate@taloreal.com";
        //public static string SupportPhone = "1-877-965-3276";
        public static string SupportPhone = "1-315-398-4070";
        private TextBox txtSchool;

        protected frmExceptionHandler()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void ExceptionHandler_Load(object sender, EventArgs e)
        {
        }

        private void frmExceptionHandler_Closing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }

        private static string GenerateInformation(Exception e)
        {
            string str = "";
            StackTrace trace = null;
            StackFrame frame = null;
            if (e == null)
            {
                trace = new StackTrace(true);
            }
            else
            {
                trace = new StackTrace(e, true);
            }
            if (trace.FrameCount > 0)
            {
                frame = trace.GetFrame(0);
            }
            OperatingSystem oSVersion = Environment.OSVersion;
            if (oSVersion != null)
            {
                str = str + "OSPlatform: " + oSVersion.Platform.ToString() + "\r\n";
                if (oSVersion.Version != null)
                {
                    str = str + "OSVersion: " + oSVersion.Version.ToString() + "\r\n";
                }
            }
            if (e != null)
            {
                object obj2;
                if ((S.I != null) && (S.ST != null))
                {
                    str = (str + "Multiplayer: " + S.I.Multiplayer.ToString() + "\r\n") + "SimStateID: " + S.ST.GUID.ToString() + "\r\n";
                }
                if ((e.Source != null) && (e.Source.Length > 0))
                {
                    str = str + "Module: " + e.Source + "\r\n";
                }
                if ((frame.GetFileName() != null) && (frame.GetFileName().Length > 0))
                {
                    str = str + "File: " + frame.GetFileName() + "\r\n";
                }
                if ((e.GetType().Name != null) && (e.GetType().Name.Length > 0))
                {
                    str = str + "Exception: " + e.GetType().Name + "\r\n";
                }
                if ((e.Message != null) && (e.Message.Length > 0))
                {
                    str = str + "Reason: " + e.Message + "\r\n";
                }
                if (e.TargetSite != null)
                {
                    str = str + "Target: " + e.TargetSite.ToString() + "\r\n";
                }
                if (frame.GetFileLineNumber() > 0)
                {
                    obj2 = str;
                    str = string.Concat(new object[] { obj2, "Line: ", frame.GetFileLineNumber(), "\r\n" });
                }
                if (frame.GetFileColumnNumber() > 0)
                {
                    obj2 = str;
                    str = string.Concat(new object[] { obj2, "Column: ", frame.GetFileColumnNumber(), "\r\n" });
                }
            }
            if (e != null)
            {
                return (str + "Stack Trace: " + e.StackTrace + "\r\n\r\n");
            }
            return (str + "Stack Trace: " + trace.ToString() + "\r\n\r\n");
        }

        public static void Handle(Exception e)
        {
            Handle(e, null);
        }

        public static void Handle(Exception e, Form errantForm)
        {
            if (!alreadyHandling)
            {
                alreadyHandling = true;
                if (e is SocketException)
                    HandleRemoteError();
                else if (e is RemotingException)
                    HandleRemoteError();
                else if (e is EntityNotFoundException)
                {
                    string caption = S.R.GetString("{0} No Longer Exists", new object[] { S.I.EntityName });
                    MessageBox.Show(e.Message, caption);
                    S.MF.UpdateView();
                    alreadyHandling = false;
                }
                else if (e is SimApplicationException)
                {
                    MessageBox.Show(e.Message, Application.ProductName);
                    alreadyHandling = false;
                }
                else
                {
                    frmExceptionHandler handler = new frmExceptionHandler
                    {
                        Text = S.R.GetString("An unexpected error has occurred."),
                        MessageText = S.R.GetString("The simulation has encountered an unanticipated error from which it cannot recover.") + "\r\n\r\n" + S.R.GetString("If you would like to report this error please use the 'Report' button below.") + "\r\n\r\n" + S.R.GetString("Alternatively, you can send an e-mail to {0}.  Please include the text from the box below as part of your message.", new object[] { SupportEmail }),
                        ErrorText = GenerateInformation(e)
                    };
                    handler.reportButton.Visible = true;
                    handler.doneButton.Text = S.R.GetString("Close");
                    handler.ShowDialog();
                }
                if (errantForm != null)
                {
                    errantForm.Close();
                }
            }
        }

        protected static void HandleRemoteError()
        {
            if (S.I.SimTimeRunning)
            {
                S.MF.mnuOptionsGoStop.PerformClick();
            }
            MessageBox.Show(S.R.GetString("Apparently you have been disconnected from the host session. Either the network connection has failed or the host session is no longer running.  If the host is still running, you might be able to reconnect by doing a Multiplayer Join from the File menu."), S.R.GetString("Disconnected From Host"));
            new frmStartChoices().ShowDialog(S.MF);
            alreadyHandling = false;
        }

        public static bool HandleToLog(Exception e)
        {
            Exception exception;
            try
            {
                if (!EventLog.SourceExists(Application.ProductName) || !EventLog.Exists(Application.ProductName))
                {
                    try
                    {
                        EventLog.CreateEventSource(Application.ProductName, Application.ProductName);
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        Handle(exception);
                        return false;
                    }
                }
                EventLog.WriteEntry(Application.ProductName, GenerateInformation(e), EventLogEntryType.Error, 0, 0);
            }
            catch (Exception exception2)
            {
                exception = exception2;
                Handle(exception);
                return false;
            }
            return true;
        }

        private void InitializeComponent()
        {
            this.errorTextTextBox = new System.Windows.Forms.TextBox();
            this.doneButton = new System.Windows.Forms.Button();
            this.reportButton = new System.Windows.Forms.Button();
            this.errorMessageTextBox = new System.Windows.Forms.TextBox();
            this.txtSchool = new System.Windows.Forms.TextBox();
            this.cdKeyLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // errorTextTextBox
            // 
            this.errorTextTextBox.AcceptsReturn = true;
            this.errorTextTextBox.AcceptsTab = true;
            this.errorTextTextBox.Location = new System.Drawing.Point(16, 184);
            this.errorTextTextBox.Multiline = true;
            this.errorTextTextBox.Name = "errorTextTextBox";
            this.errorTextTextBox.ReadOnly = true;
            this.errorTextTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorTextTextBox.Size = new System.Drawing.Size(448, 112);
            this.errorTextTextBox.TabIndex = 3;
            this.errorTextTextBox.Text = "#error text";
            // 
            // doneButton
            // 
            this.doneButton.Location = new System.Drawing.Point(384, 312);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(80, 24);
            this.doneButton.TabIndex = 5;
            this.doneButton.Text = "#text";
            this.doneButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // reportButton
            // 
            this.reportButton.Location = new System.Drawing.Point(288, 312);
            this.reportButton.Name = "reportButton";
            this.reportButton.Size = new System.Drawing.Size(80, 24);
            this.reportButton.TabIndex = 4;
            this.reportButton.Text = "Report";
            this.reportButton.Visible = false;
            this.reportButton.Click += new System.EventHandler(this.reportButton_Click);
            // 
            // errorMessageTextBox
            // 
            this.errorMessageTextBox.AcceptsReturn = true;
            this.errorMessageTextBox.AcceptsTab = true;
            this.errorMessageTextBox.Location = new System.Drawing.Point(16, 56);
            this.errorMessageTextBox.Multiline = true;
            this.errorMessageTextBox.Name = "errorMessageTextBox";
            this.errorMessageTextBox.ReadOnly = true;
            this.errorMessageTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.errorMessageTextBox.Size = new System.Drawing.Size(448, 112);
            this.errorMessageTextBox.TabIndex = 2;
            this.errorMessageTextBox.Text = "#error message";
            // 
            // txtSchool
            // 
            this.txtSchool.Location = new System.Drawing.Point(96, 16);
            this.txtSchool.MaxLength = 128;
            this.txtSchool.Name = "txtSchool";
            this.txtSchool.Size = new System.Drawing.Size(368, 20);
            this.txtSchool.TabIndex = 1;
            // 
            // cdKeyLabel
            // 
            this.cdKeyLabel.Location = new System.Drawing.Point(0, 9);
            this.cdKeyLabel.Name = "cdKeyLabel";
            this.cdKeyLabel.Size = new System.Drawing.Size(90, 37);
            this.cdKeyLabel.TabIndex = 0;
            this.cdKeyLabel.Text = "Email Address or\r\nText Phone#:";
            this.cdKeyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmExceptionHandler
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(482, 352);
            this.Controls.Add(this.txtSchool);
            this.Controls.Add(this.errorMessageTextBox);
            this.Controls.Add(this.errorTextTextBox);
            this.Controls.Add(this.cdKeyLabel);
            this.Controls.Add(this.reportButton);
            this.Controls.Add(this.doneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmExceptionHandler";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExceptionHandler";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmExceptionHandler_Closing);
            this.Load += new System.EventHandler(this.ExceptionHandler_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        string Title = "report.txt";
        private void reportButton_Click(object sender, EventArgs e)
        {
            if (this.txtSchool.Text.Length == 0)
            {
                MessageBox.Show(S.R.GetString("Please fill in the school name field."));
                this.txtSchool.Focus();
            }
            else
            {
                try
                {
                    if (ORIGINAL_REPORT)
                    {
                        WebRequest request = WebRequest.Create(ReportURL);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        string s = "";
                        s = (((s + "school=" + Utilities.URLEncode(this.txtSchool.Text) + "&") + "product=" + Utilities.URLEncode(Application.ProductName) + "&") + "version=" + Utilities.URLEncode(Application.ProductVersion) + "&") + "error_text=" + Utilities.URLEncode(this.errorTextTextBox.Text);
                        byte[] bytes = Encoding.ASCII.GetBytes(s);
                        request.ContentLength = bytes.Length;
                        Stream requestStream = request.GetRequestStream();
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                        StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                        string str2 = reader.ReadToEnd();
                        reader.Close();
                        if (str2 == "ReportSuccess")
                        {
                            MessageBox.Show(S.R.GetString("ReportSuccess", new object[] { SupportEmail, SupportPhone }));
                            base.Close();
                        }
                        else
                        {
                            MessageBox.Show(S.R.GetString("ReportFailure", new object[] { ReportURL, SupportEmail, SupportPhone }));
                        }
                    }
                    else
                    {
                        try
                        {
                            string[] From = GetComputerInfo();
                            Title = "VBPF Report " + DateTime.Now.ToString().Replace("/", ",").Replace(":", ",") + ".txt";
                            TextWriter Reporter = new StreamWriter(Title);
                            string Message = errorMessageTextBox.Text + "\r\n\r\n-----\r\n\r\n" + errorTextTextBox.Text;
                            foreach (string s in From)
                                Reporter.WriteLine(s);
                            Reporter.WriteLine();
                            Reporter.WriteLine();
                            Reporter.Write(Message);
                            Reporter.WriteLine();
                            Reporter.WriteLine();
                            Reporter.WriteLine("Contact Info: " + txtSchool.Text);
                            Reporter.Close();
                            WebClient WC = new WebClient();
                            WC.Headers.Add("Content-Type", "binary/octet-stream");
                            WC.UploadFileCompleted += new UploadFileCompletedEventHandler(DeleteReportContents);
                            WC.UploadFileAsync(new Uri("http://www.taloreal.com/vbUpdates/Reports/UploadFile.php"), "POST", Title);
                            MessageBox.Show("Feedback sent.  Every bug report helps, Thank You.");
                        }
                        catch
                        {
                            File.Delete(Title);
                            MessageBox.Show("We're sorry but your message could not be sent...");
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show(S.R.GetString("ReportFailure", new object[] { ReportURL, SupportEmail, SupportPhone }));
                }
            }
        }

        public void DeleteReportContents(object sender, EventArgs EAs)
        {
            File.Delete(Title);
        }

        public string[] GetComputerInfo()
        {
            string[] Info = new string[6];
            Info[0] = "Username: " + Environment.UserName.ToString();
            Info[1] = "Current Directory: " + Environment.CurrentDirectory;
            Info[2] = "Computer Name: " + Environment.MachineName.ToString();
            Info[3] = "Operating System: Windows " + new KMI.Utility.OS_Identifier().getOSInfo();
            Info[4] = "LAN IP: " + GetLanIP();
            Info[5] = "WAN IP: " + GetWanIP();
            return Info;
        }

        #region GetIP's
        public string GetLanIP()
        {
            string LAN = "0.0.0.0";
            foreach (IPAddress IP in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                if (IP.AddressFamily == AddressFamily.InterNetwork)
                    LAN = IP.ToString();
            return LAN;
        }

        public string GetWanIP()
        {
            string IP = "0.0.0.0";
            try {
                IP = new UTF8Encoding().GetString(new WebClient().DownloadData("http://taloreal.com/ipaddress.shtml"));
            }
            catch { IP = "0.0.0.0"; }
            return IP.Replace("<title>", "").Replace("</title>", "").Replace("'", "").Replace("Your IP address is ", "");
        }
        #endregion

        public string ErrorText
        {
            get
            {
                return this.errorTextTextBox.Text;
            }
            set
            {
                this.errorTextTextBox.Text = value;
            }
        }

        public string MessageText
        {
            get
            {
                return this.errorMessageTextBox.Text;
            }
            set
            {
                this.errorMessageTextBox.Text = value;
            }
        }
    }
}

