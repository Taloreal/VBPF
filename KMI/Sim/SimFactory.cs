namespace KMI.Sim
{
    using KMI.Sim.Surveys;
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Drawing;
    using System.Reflection;
    using System.Resources;
    using System.Windows.Forms;

    public class SimFactory
    {
        protected Bitmap CBmp(System.Type typeFromAssembly, string filename)
        {
            Bitmap image = null;
            Bitmap bitmap2 = new Bitmap(typeFromAssembly, filename);
            bitmap2.SetResolution(96f, 96f);
            if (bitmap2 == null)
            {
                throw new Exception("In SimFactory.CreateCompatibleBitmap, could not get image from filename " + filename);
            }
            image = new Bitmap(bitmap2.Width, bitmap2.Height, S.MF.picMain.CreateGraphics());
            Graphics.FromImage(image).DrawImageUnscaled(bitmap2, 0, 0);
            return image;
        }

        protected Cursor CCursor(System.Type typeFromAssembly, string filename)
        {
            return new Cursor(this.CBmp(typeFromAssembly, filename).GetHicon());
        }

        protected Page CPage(System.Type typeFromAssembly, string filename, int cols, int rows, int anchorX, int anchorY)
        {
            return new Page(this.CBmp(typeFromAssembly, filename), cols, rows, anchorX, anchorY);
        }

        public virtual SortedList CreateCursorTable()
        {
            return null;
        }

        public virtual Entity CreateEntity(Player player, string entityName)
        {
            return new Entity(player, entityName);
        }

        public virtual SortedList CreateImageTable()
        {
            return null;
        }

        public virtual SortedList CreatePageTable()
        {
            return null;
        }

        public virtual Player CreatePlayer(string playerName, PlayerType playerType)
        {
            return new Player(playerName, playerType);
        }

        public virtual Resource CreateResource()
        {
            ResourceManager manager = new ResourceManager("KMI.Sim.Sim", Assembly.GetAssembly(typeof(SimFactory)));
            return new Resource(new ResourceManager[] { manager });
        }

        public virtual SimEngine CreateSimEngine()
        {
            return new SimEngine();
        }

        public virtual SimSettings CreateSimSettings()
        {
            return new SimSettings();
        }

        public virtual SimState CreateSimState(SimSettings simSettings, bool multiplayer)
        {
            return new SimState(simSettings, multiplayer);
        }

        public virtual SimStateAdapter CreateSimStateAdapter()
        {
            return new SimStateAdapter();
        }

        public virtual Simulator CreateSimulator()
        {
            return new Simulator(this);
        }

        public virtual Survey CreateSurvey(long entityID, DateTime date, string[] entityNames, ArrayList surveyQuestions)
        {
            return new Survey(entityID, date, entityNames, surveyQuestions);
        }

        public virtual UserAdminSettings CreateUserAdminSettings()
        {
            UserAdminSettings settings = new UserAdminSettings();
            AppSettingsReader reader = new AppSettingsReader();
            settings.DefaultDirectory = (string) reader.GetValue("DefaultDirectory", typeof(string));
            settings.P = (int) reader.GetValue("P", typeof(int));
            settings.ProxyAddress = (string) reader.GetValue("ProxyAddress", typeof(string));
            settings.ProxyBypassList = (string) reader.GetValue("ProxyBypassList", typeof(string));
            settings.NoSound = (bool) reader.GetValue("NoSound", typeof(bool));
            settings.MultiplayerBasePort = (int) reader.GetValue("MultiplayerBasePort", typeof(int));
            settings.MultiplayerPortCount = (int) reader.GetValue("MultiplayerPortCount", typeof(int));
            settings.ClientDrawStepPeriod = (int) reader.GetValue("ClientDrawStepPeriod", typeof(int));
            settings.PasswordsForMultiplayer = (bool) reader.GetValue("PasswordsForMultiplayer", typeof(bool));
            return settings;
        }

        public virtual KMI.Sim.View[] CreateViews()
        {
            return new KMI.Sim.View[0];
        }

        protected void LoadWith8CompassPoints(SortedList table, System.Type type, string baseResourceName, string fileExtension)
        {
            this.LoadWithCompassPoints(table, type, baseResourceName, fileExtension);
            string[] strArray = baseResourceName.Split(new char[] { '.' });
            string str = strArray[strArray.Length - 1];
            foreach (string str2 in new string[] { "N", "S", "E", "W" })
            {
                table.Add(str + str2, this.CBmp(type, baseResourceName + str2 + "." + fileExtension));
            }
        }

        protected void LoadWithCompassPoints(SortedList table, System.Type type, string baseResourceName, string fileExtension)
        {
            string[] strArray = baseResourceName.Split(new char[] { '.' });
            string str = strArray[strArray.Length - 1];
            foreach (string str2 in new string[] { "N", "S" })
            {
                foreach (string str3 in new string[] { "E", "W" })
                {
                    table.Add(str + str2 + str3, this.CBmp(type, baseResourceName + str2 + str3 + "." + fileExtension));
                }
            }
        }
    }
}

