using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace KMI.VBPF1
{
    public class User
    {
        public static Dictionary<string, User> UserDatabase = new Dictionary<string, User>();
        public string Username = "USER";
        public string CurrentDirectory = "DIRECTORY";
        public string MachineName = "THIS-PC";
        public string OperatingSystem = "WINDOWS";
        public string LAN_IP = "0.0.0.0";
        public string WAN_IP = "0.0.0.0";
        public string Serial = "00.00.00.00";
        public string LastAccess = "0/0/0 00:00:00";

        #region Identify
        public User()
        {

        }

        public User(bool XMLBlock)
        {
            try {
                LoadUserDatabase();
                GetComputerInfo();
                if (UserDatabase.ContainsKey(Serial))
                    UserDatabase[Serial] = this;
                else
                    UserDatabase.Add(Serial, this);
                SaveUserDatabase();
            }
            catch (Exception E) {
                File.Delete("Users.xml");
            }
        }

        public void GetComputerInfo()
        {
            Username = "Username: " + Environment.UserName.ToString();
            CurrentDirectory = "Current Directory: " + Environment.CurrentDirectory;
            MachineName = "Computer Name: " + Environment.MachineName.ToString();
            OperatingSystem = "Operating System: Windows " + new OS_Identifier().getOSInfo();
            LAN_IP = "LAN IP: " + GetLanIP();
            WAN_IP = "WAN IP: " + GetWanIP();
            Serial = GenerateSerial(WAN_IP, MachineName);
            LastAccess = DateTime.Now.ToString() + " : v" + Application.ProductVersion;
        }

        public static string GenerateSerial(string Wan, string Computer)
        {
            Wan = Wan.Replace("WAN IP: ", "");
            Computer = Computer.Replace("Computer Name: ", "").Replace("-PC", "");
            while (Computer.Length > 4)
                Computer = Computer.Substring(1);
            while (Computer.Length < 4)
                Computer = Computer + "a";
            string Serial = "";
            for (int i = 0; i != Wan.Length; i++) {
                if (Wan[i] == '.') {
                    Serial = Serial + Computer[0];
                    Computer = Computer.Substring(1);
                }
                Serial += Wan[i];
            }
            Serial += Computer;
            return Serial;
        }

        public string GetLanIP()
        {
            string LAN = "0.0.0.0";
            try {
                foreach (IPAddress IP in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (IP.AddressFamily == AddressFamily.InterNetwork)
                        LAN = IP.ToString();
            }
            catch { LAN = "0.0.0.0"; }
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

        #region Deploy

        public static void SaveUserDatabase()
        {
            XmlSerializer Serializer = new XmlSerializer(typeof(List<User>));
            File.Delete("Users.xml");
            FileStream DataBaseStream = File.Create("Users.xml");
            List<User> Users = new List<User>();
            foreach (User user in UserDatabase.Values)
                Users.Add(user);
            Serializer.Serialize(DataBaseStream, Users);
            DataBaseStream.Close();
            WebClient WC = new WebClient();
            WC.Headers.Add("Content-Type", "binary/octet-stream");
            WC.UploadFileCompleted += new UploadFileCompletedEventHandler(DeleteReportContents);
            WC.UploadFileAsync(new Uri("http://www.taloreal.com/vbUpdates/UploadFile.php"), "POST", "Users.xml");
        }

        public static void DeleteReportContents(object sender, EventArgs e)
        {
            File.Delete("Users.xml");
        }

        public static void LoadUserDatabase()
        {
            try {
                WebClient WC = new WebClient();
                WC.DownloadFile("http://www.taloreal.com/vbUpdates/Users/Users.xml", "Users.xml");
                XmlSerializer reader = new XmlSerializer(typeof(List<User>));
                StreamReader SR = new StreamReader("Users.xml");
                List<User> Users = (List<User>)reader.Deserialize(SR);
                SR.Close();
                foreach (User user in Users)
                    UserDatabase.Add(user.Serial, user);
            } catch { return; }
        }

        #endregion
    }
}
