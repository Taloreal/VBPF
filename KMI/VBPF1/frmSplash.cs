using System;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO.Compression;
using System.Collections.Generic;

using KMI.VBPF1Lib;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;

namespace KMI.VBPF1
{
    public class frmSplash : Form
    {
        protected bool academic = false;
        private string[] args;
        private IContainer components;
        ComponentResourceManager manager;
        protected bool demo = false;
        private bool exit = false;
        private Label labCopyright;
        private Label labVersion;
        private System.Windows.Forms.Timer timer1;
        protected bool vbc = false;
        public bool DoUpdate = false;
        public bool UpdateAvailable = false;
        frmUpdateProgress Updating = new frmUpdateProgress();
        public string VersionAVailable = Application.ProductVersion;
        int countDown = 40;
        char Modifier = 'M';
        Dictionary<char, Action<char>> StartUp_Type = new Dictionary<char, Action<char>>();

        public frmSplash(string[] args)
        {
            //List<string> DLs = GetUpdateLinks();
            DeleteObselete();
            if (Debugger.IsAttached)
                ReleaseUpdate();
            UpdateAvailable = VersionCheck(out VersionAVailable);
            if (UpdateAvailable)
                OfferUpdate();
            this.InitializeComponent();
            this.args = args;
            this.labVersion.Text = this.labVersion.Text + Application.ProductVersion;
            this.timer1.Start();
        }

        public void DeleteObselete()
        {
            string[] files = Directory.GetFiles(Application.StartupPath + "\\");
            for (int i = 0; i != files.Length; i++)
                if (files[i].Contains("OLD."))
                    File.Delete(files[i]);
        }

        public void ReleaseUpdate()
        {
            DialogResult dialogResult = MessageBox.Show("Release Update?", "New Update?", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
                return;
            try
            {
                ArchiveForUpload();
                UploadVersionInfo();
                UploadArchive();
                return;

                //WC.UploadProgressChanged += new UploadProgressChangedEventHandler(UploadUpdate_Progressed);
                //WebRequest request = WebRequest.Create("http://www.taloreal.com/scripts/Testphp.php");
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //string s = "Successful!";
                //byte[] bytes =  Encoding.ASCII.GetBytes(s);
                //request.ContentLength = bytes.Length;
                //Stream requestStream = request.GetRequestStream();
                //requestStream.Write(bytes, 0, bytes.Length);
                //requestStream.Close();
                //StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                //string str2 = reader.ReadToEnd();
                
                //reader.Close();

                //CookieAwareWebClient WC = new CookieAwareWebClient();
                //WebRequest WRt = WebRequest.Create(@"http://www.taloreal.com/scripts/");
                //WRt.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
                //WebResponse WRe = WRt.GetResponse();
                //WRe.Close();
                //WebClient WC = new WebClient();
                //WC.Credentials = new NetworkCredential("naate", "Deadless@222", "ftp://www.taloreal.com/");
                //WC.UploadFile("ftp://www.taloreal.com/scripts/help.txt", "STOR", "help.txt");
                //WC.Dispose();
            }
            catch (Exception E) { }
        }

        public void UploadArchive()
        {
            Updating.Show();
            WebClient WC = new WebClient();
            WC.Headers.Add("Content-Type", "binary/octet-stream");
            WC.UploadProgressChanged += new UploadProgressChangedEventHandler(Updating.ULProgressChanged);
            WC.UploadFileCompleted += new UploadFileCompletedEventHandler(WC_UploadFileCompleted);
            WC.UploadFileAsync(new Uri("http://www.taloreal.com/vbUpdates/UploadFile.php"), "POST", "Update.zip");
        }

        public void WC_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            File.Delete("Update.zip");
            Updating.Hide();
        }

        public void ArchiveForUpload()
        {
            Directory.CreateDirectory("C:\\Program Data\\");
            FileStream fsOut = File.Create("C:\\Program Data\\Update.zip");
            ZipOutputStream zipStream = new ZipOutputStream(fsOut);
            zipStream.SetLevel(9);
            int folderOffset = Application.StartupPath.Length + (Application.StartupPath.EndsWith("\\") ? 0 : 1);
            CompressFolder(Application.StartupPath, zipStream, folderOffset);
            zipStream.IsStreamOwner = true;
            zipStream.Close();
            File.Move("C:\\Program Data\\Update.zip", Application.StartupPath + "\\Update.zip");
        }

        public void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string filename in files) {
                FileInfo fi = new FileInfo(filename);
                string entryName = filename.Substring(folderOffset); // Makes the name in zip based on the folder
                entryName = ZipEntry.CleanName(entryName); // Removes drive from name and fixes slash direction
                ZipEntry newEntry = new ZipEntry(entryName);
                newEntry.DateTime = fi.LastWriteTime; // Note the zip format stores 2 second granularity
                // Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
                // A password on the ZipOutputStream is required if using AES.
                //   newEntry.AESKeySize = 256;

                // To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
                // you need to do one of the following: Specify UseZip64.Off, or set the Size.
                // If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
                // but the zip will be in Zip64 format which not all utilities can understand.
                //   zipStream.UseZip64 = UseZip64.Off;
                newEntry.Size = fi.Length;
                zipStream.PutNextEntry(newEntry);
                // Zip the file in buffered chunks
                // the "using" will close the stream even if an exception occurs
                byte[] buffer = new byte[4096];
                using (FileStream streamReader = File.OpenRead(filename)) {
                    StreamUtils.Copy(streamReader, zipStream, buffer);
                }
                zipStream.CloseEntry();
            }
            //
            //Recursion Unneeded for updates NODA - 5/12/2016
            //
            //string[] folders = Directory.GetDirectories(path);
            //foreach (string folder in folders) {
            //    CompressFolder(folder, zipStream, folderOffset);
            //}
        }

        public void UploadVersionInfo()
        {
            WebClient WC = new WebClient();
            StreamWriter VerUpdater = new StreamWriter(@"Ver.txt");
            VerUpdater.WriteLine(Application.ProductVersion);
            VerUpdater.Close();
            WC.Headers.Add("Content-Type", "binary/octet-stream");
            WC.UploadFile(new Uri("http://www.taloreal.com/vbUpdates/UploadFile.php"), "POST", "Ver.txt");
            File.Delete("Ver.txt");
        }

        public void OfferUpdate()
        {
            DialogResult DR = MessageBox.Show("Update : " + VersionAVailable.ToString() + "\r\nis available, update?",
                "Update Available", MessageBoxButtons.YesNo);
            if (DR == DialogResult.Yes)
                DownloadUpdate();
        }

        public void DownloadUpdate()
        {
            DoUpdate = true;
            File.Delete(Application.StartupPath + "\\OLD.VBPF1.exe");
            File.Move(Application.StartupPath + "\\KMI.VBPF1.exe", Application.StartupPath + "\\OLD.VBPF1.exe");
            Updating.Show();
            WebClient WC = new WebClient();
            WC.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadComplete);
            WC.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Updating.ProgressChanged);
            WC.DownloadFileAsync(new Uri("http://www.taloreal.com/vbUpdates/Update.zip"), "Update.zip");
        }

        public void DownloadComplete(object sender, AsyncCompletedEventArgs ACEA)
        {
            ExtractZipFile("Update.zip", null, Application.StartupPath + "\\");
            File.Delete("Update.zip");
            Process.Start(Application.StartupPath + "\\KMI.VBPF1.exe");
            Application.Exit();
        }

        public void ExtractZipFile(string archiveFilenameIn, string password, string outFolder)
        {
            ZipFile zf = null;
            try
            {
                FileStream fs = File.OpenRead(archiveFilenameIn);
                zf = new ZipFile(fs);
                if (!String.IsNullOrEmpty(password))
                {
                    zf.Password = password;     // AES encrypted entries are handled automatically
                }
                foreach (ZipEntry zipEntry in zf)
                {
                    if (!zipEntry.IsFile)
                    {
                        continue;           // Ignore directories
                    }
                    String entryFileName = zipEntry.Name;
                    if (entryFileName == "ICSharpCode.SharpZipLib.dll")
                        continue;
                    if (File.Exists(entryFileName))
                        File.Move(entryFileName, "OLD." + entryFileName);
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum
                    Stream zipStream = zf.GetInputStream(zipEntry);

                    // Manipulate the output filename here as desired.
                    String fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    using (FileStream streamWriter = File.Create(fullZipToPath))
                    {
                        StreamUtils.Copy(zipStream, streamWriter, buffer);
                    }
                }
            }
            catch (Exception E) { }
            finally {
                if (zf != null) {
                    zf.IsStreamOwner = true; // Makes close also shut the underlying stream
                    zf.Close(); // Ensure we release resources
                }
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            frmMain.HandleError(e.Exception);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Displays a message regarding the current version being a trial.
        /// Not implamented.
        /// </summary>
        /// <param name="message">The message to display about being a trial.</param>  
        private void DrawTrialText(string message)
        {
            Graphics graphics = Graphics.FromImage(this.BackgroundImage);
            int alpha = 0x7a;
            Color color = Color.FromArgb(alpha, Color.White);
            Rectangle rect = new Rectangle(20, 150, 460, 100);
            Brush brush = new SolidBrush(color);
            graphics.FillRectangle(brush, rect);
            brush = new SolidBrush(Color.Red);
            StringFormat format = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Near
            };
            Font font = new Font("Microsoft Sans Serif", 20f);
            graphics.DrawString(message, font, brush, rect, format);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSplash));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.labVersion = new System.Windows.Forms.Label();
            this.labCopyright = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // labVersion
            // 
            this.labVersion.BackColor = System.Drawing.Color.White;
            this.labVersion.Location = new System.Drawing.Point(24, 48);
            this.labVersion.Name = "labVersion";
            this.labVersion.Size = new System.Drawing.Size(149, 16);
            this.labVersion.TabIndex = 1;
            this.labVersion.Text = "Version ";
            // 
            // labCopyright
            // 
            this.labCopyright.BackColor = System.Drawing.Color.Transparent;
            this.labCopyright.Location = new System.Drawing.Point(23, 64);
            this.labCopyright.Name = "labCopyright";
            this.labCopyright.Size = new System.Drawing.Size(235, 40);
            this.labCopyright.TabIndex = 2;
            this.labCopyright.Text = "Copyright 2008-2010 Knowledge Matters, Inc. All rights reserved worldwide.";
            this.labCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // frmSplash
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImage = global::Properties.Resources.BackgroundImage;
            this.ClientSize = new System.Drawing.Size(500, 320);
            this.Controls.Add(this.labCopyright);
            this.Controls.Add(this.labVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = global::Properties.Resources.Icon;
            /*
             * Old Knowledge Matters Resource Management
             * this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
             */
            this.Name = "frmSplash";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmSplashAbout";
            this.Load += new System.EventHandler(this.frmSplash_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSplash_KeyDown);
            this.ResumeLayout(false);

        }

        #region On Start Up

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Application.ThreadException += new ThreadExceptionEventHandler(frmSplash.Application_ThreadException);
                User Me = new User(true);
                Application.Run(new frmSplash(args));
            }
            catch (Exception exception)
            {
                frmMain.HandleError(exception);
            }
        }

        public static bool VersionCheck(out string NewVer)
        {
            try
            {
                WebClient WC = new WebClient();
                WC.DownloadFile("http://www.taloreal.com/vbUpdates/Ver.txt", "Ver.txt");
                string CurrentVer = Application.ProductVersion;
                TextReader TR = new StreamReader(@"Ver.txt");
                NewVer = TR.ReadLine();
                TR.Close();
                File.Delete(@"Ver.txt");
                return (int.Parse(CurrentVer.Replace(".", "")) < int.Parse(NewVer.Replace(".", "")));
            }
            catch
            {
                MessageBox.Show("Was unable to verify game version.\r\nHowever the game will run as intended in the last update recieved.");
                NewVer = Application.ProductVersion;
                return false;
            }
        }

        #endregion

        private void timer1_Tick(object sender, EventArgs e)
        {
            //GC.Collect();
            countDown--;
            if (countDown > 0)
                return;
            this.timer1.Stop();
            if (this.exit) 
                Application.Exit(); 
            else 
                StartUp_Type[Modifier](Modifier); 
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            StartUp_Type.Add('M', Normal_StartUp);
            StartUp_Type.Add('D', Designer_StartUp);
        }

        private void frmSplash_KeyDown(object sender, KeyEventArgs e)
        {
            if (StartUp_Type.ContainsKey((char)e.KeyValue))
                Modifier = (char)e.KeyValue;
        }

        public void Designer_StartUp(char Start_Param)
        {
            DateTime now = DateTime.Now;
            frmMain main = new frmMain(args, demo, vbc, academic);
            main.DesignerMode = true;
            base.Hide();
            main.Show();
        }

        public void Normal_StartUp(char Start_Param)
        {
            DateTime now = DateTime.Now;
            frmMain main = new frmMain(args, demo, vbc, academic);
            base.Hide();
            main.Show();
        }
    }
}

