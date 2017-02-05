namespace KMI.Sim
{
    using KMI.Sim.Academics;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.CompilerServices;
    using System.Runtime.Remoting;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading;
    using System.Windows.Forms;

    public class frmMainBase : Form
    {
        protected Bitmap backBuffer;
        protected Graphics backBufferGraphics;
        public System.Windows.Forms.Timer backgroundMusicTimer;
        protected SimSettings cacheDesignerSimSettings;
        protected SimSettings cachedSimSettings;
        protected string[] commandLineArgs;
        private IContainer components;
        private StatusBarPanel CreateMessagePanel;
        protected long currentEntityID;
        protected StatusBarPanel CurrentEntityNamePanel;
        protected StatusBarPanel CurrentEntityPanel;
        protected string currentFilePath;
        protected internal int currentMacroActionIndex;
        protected KMI.Sim.View currentView;
        protected string currentViewName;
        protected int currentWeek;
        public static DateTime DateNotSet = new DateTime(0x7d0, 1, 1);
        protected StatusBarPanel DatePanel;
        protected StatusBarPanel DayOfWeekPanel;
        protected bool designerMode;
        protected bool dirtySimState;
        protected StatusBarPanel EntityCriticalResourceNamePanel;
        protected StatusBarPanel EntityCriticalResourcePanel;
        private Hashtable entityNames;
        protected ImageList ilsMainToolBar;
        protected static frmMainBase instance;
        protected bool isWin98;
        protected bool lessonLoadedPromptSaveAs;
        private StatusBarPanel Level;
        protected internal ArrayList macroActions;
        protected internal string macroFilename;
        protected internal bool macroPlayingOn;
        protected internal bool macroRecordingOn;
        protected MainMenu mainMenu1;
        private MenuItem menuItem3;
        protected MenuItem menuMessagesSep;
        protected frmMessages messagesForm;
        protected MenuItem mnuActions;
        private MenuItem mnuFile;
        public MenuItem mnuFileExit;
        protected MenuItem mnuFileMultiplayer;
        public MenuItem mnuFileMultiplayerJoin;
        protected MenuItem mnuFileMultiplayerScoreboard;
        public MenuItem mnuFileMultiplayerStart;
        private MenuItem mnuFileMultiplayerTeamList;
        public MenuItem mnuFileNew;
        public MenuItem mnuFileOpenLesson;
        public MenuItem mnuFileOpenSavedSim;
        private MenuItem mnuFilePrintView;
        private MenuItem mnuFileSave;
        public MenuItem mnuFileSaveAs;
        private MenuItem mnuFileUploadSeparator;
        private MenuItem mnuHelp;
        private MenuItem mnuHelpAbout;
        private MenuItem mnuHelpAssignment;
        private MenuItem mnuHelpSearch;
        private MenuItem mnuHelpTopicsAndIndex;
        public MenuItem mnuHelpTutorial;
        protected MenuItem mnuOptions;
        public MenuItem mnuOptionsBackgroundMusic;
        private MenuItem mnuOptionsChangeOwner;
        private MenuItem mnuOptionsFaster;
        protected internal MenuItem mnuOptionsGoStop;
        protected MenuItem mnuOptionsIA;
        private MenuItem mnuOptionsIACustomizeYourSim;
        private MenuItem mnuOptionsIAProvideCash;
        private MenuItem mnuOptionsMacros;
        private MenuItem mnuOptionsMacrosPlayMacro;
        private MenuItem mnuOptionsMacrosRecordMacro;
        private MenuItem mnuOptionsMacrosStopPlaying;
        private MenuItem mnuOptionsMacroStopRecording;
        private MenuItem mnuOptionsRenameEntity;
        private MenuItem mnuOptionsRunTo;
        protected MenuItem mnuOptionsShowMessages;
        private MenuItem mnuOptionsSlower;
        private MenuItem mnuOptionsSoundEffects;
        private MenuItem mnuOptionsTestResults;
        private MenuItem mnuOptionsTuning;
        protected MenuItem mnuReports;
        private MenuItem mnuSep1;
        private MenuItem mnuSep3;
        private MenuItem mnuSep4;
        private MenuItem mnuSep6;
        private MenuItem mnuSep7;
        private MenuItem mnuSep8;
        private MenuItem mnuSep9385;
        protected MenuItem mnuView;
        protected StatusBarPanel NewMessagesPanel;
        protected internal DateTime nextMacroPlayTime;
        protected DateTime now;
        public PictureBox picMain;
        protected internal long playIntervalMilliseconds;
        protected internal bool playLooping;
        private Panel pnlMain;
        protected string printStudentName;
        protected ToolBarButton ScoreboardButton;
        protected StatusBarPanel SpacerPanel;
        protected internal StatusBar staMain;
        protected StatusBarPanel TimePanel;
        protected ToolbarSponsored tlbMain;
        private ToolTip viewToolTip;
        ResourceManager manager;

        public event EventHandler EntityChanged;

        public event EventHandler NewDay;

        public event EventHandler NewHour;

        public event EventHandler NewWeek;

        public event EventHandler NewYear;

        public frmMainBase()
        {
            this.entityNames = new Hashtable();
            this.currentEntityID = -1L;
            this.printStudentName = "";
            this.macroRecordingOn = false;
            this.macroPlayingOn = false;
            this.playLooping = true;
            this.playIntervalMilliseconds = 0x3e8L;
            this.currentMacroActionIndex = 0;
            this.macroActions = new ArrayList();
            this.InitializeComponent();
        }

        public frmMainBase(string[] args, bool demo, bool vbc, bool academic)
        {
            this.entityNames = new Hashtable();
            this.currentEntityID = -1L;
            this.printStudentName = "";
            this.macroRecordingOn = false;
            this.macroPlayingOn = false;
            this.playLooping = true;
            this.playIntervalMilliseconds = 0x3e8L;
            this.currentMacroActionIndex = 0;
            this.macroActions = new ArrayList();
            frmLanguage language = new frmLanguage();
            if (language.LanguageCount > 0) 
                language.ShowDialog(this); 
            this.InitializeComponent();
            this.isWin98 = Environment.OSVersion.Platform != PlatformID.Win32NT;
            instance = this;
            this.InitRemotingConfiguration();
            if ((base.CreateGraphics().DpiX != 96f) || (base.CreateGraphics().DpiY != 96f))
            {
                MessageBox.Show("A DPI setting other than 96 was detected. This program is designed for 96 DPI and may not function properly. To correct this problem, change the DPI setting under Display on the Control Panel.", "Warning");
            }
            Cursor.Current = Cursors.WaitCursor;
            this.commandLineArgs = args;
            this.ConstructSimulator();
            S.I.VBC = vbc;
            S.I.Demo = demo;
            S.I.Academic = academic;
            if (S.I.VBC)
            {
                int vBCStudentOrgCode = this.GetVBCStudentOrgCode();
                if (vBCStudentOrgCode > 0)
                {
                    string str = Utilities.GetWebPage(WebRequest.Create("http://vbc.knowledgematters.com/vbccommon/vbcvalidate.php?ss=" + vBCStudentOrgCode), S.I.UserAdminSettings.ProxyAddress, S.I.UserAdminSettings.ProxyBypassList);
                    if (str != "1")
                    {
                        if (str == "")
                        {
                            MessageBox.Show("This special version of " + Application.ProductName + " could not connect to the Internet to confirm that a Virtual Business Challenge is currently running. Please check your Internet connection and try again.", "No Internet Connection");
                            Application.Exit();
                        }
                        else
                        {
                            MessageBox.Show("There is no live challenge for " + Application.ProductName + " at this time.", "No Valid Virtual Business Challenge");
                            Application.Exit();
                        }
                    }
                }
            }
            this.DesignerMode = false;
            foreach (KMI.Sim.View view in S.I.Views)
            {
                MenuItem item = new MenuItem(S.R.GetString(view.Name), new EventHandler(this.mnuViewView_Click));
                this.mnuView.MenuItems.Add(item);
            }
            this.mnuView.MenuItems.Add(new MenuItem("-"));
            this.CurrentEntityNamePanel.Text = S.R.GetString("Current") + " " + S.I.EntityName + ":";

            //Settings Management
            this.mnuOptionsSoundEffects.Checked = !S.I.UserAdminSettings.NoSound;
            this.mnuOptionsBackgroundMusic.Checked = !S.I.UserAdminSettings.NoSound; 
            this.mnuOptionsSoundEffects.Enabled = !S.I.UserAdminSettings.NoSound; 
            this.mnuOptionsBackgroundMusic.Enabled = !S.I.UserAdminSettings.NoSound;

            this.mnuFileNew.Enabled = !S.I.VBC;
            this.mnuFileOpenLesson.Enabled = !S.I.VBC;
            this.mnuFileMultiplayerStart.Enabled = !S.I.VBC; 
            this.mnuFileMultiplayerJoin.Enabled = !S.I.VBC;

            this.mnuFileOpenLesson.Visible = !S.I.Academic;
            this.mnuFileMultiplayer.Visible = !S.I.Academic;
            this.mnuOptionsTestResults.Visible = S.I.Academic;

            Thread.CurrentThread.Priority = ThreadPriority.Highest;
        }

        public void AbortSession()
        {
            this.dirtySimState = false;
            this.mnuFileExit_Click(null, null);
        }

        public void AddPlayerMessage(PlayerMessage message)
        {
            if (this.messagesForm != null)
            {
                this.messagesForm.AddMessage(message);
                if (!(this.messagesForm.Visible || (this.NewMessagesPanel.Width == 120)))
                {
                    this.NewMessagesPanel.Width = 120;
                }
            }
        }

        private void backgroundMusicTimer_Tick(object sender, EventArgs e)
        {
            this.StartMusic();
        }

        protected bool CanShowForm(Form f)
        {
            if (f is IConstrainedForm)
            {
                string text = ((IConstrainedForm) f).CanUse();
                if (text.Equals(""))
                {
                    return true;
                }
                MessageBox.Show(text, S.R.GetString("Action Not Allowed"));
                return false;
            }
            return true;
        }

        protected virtual void ClientJoinHook(Player p)
        {
        }

        private string ClientOrHost()
        {
            if (Simulator.Instance.Client)
            {
                return "Client";
            }
            return "Host";
        }

        protected void CloseActionForms()
        {
            foreach (Form form in base.OwnedForms)
            {
                if (form is IActionForm)
                {
                    form.Close();
                }
            }
        }

        protected void CloseOwnedForms()
        {
            foreach (Form form in base.OwnedForms)
            {
                form.Close();
            }
        }

        protected virtual void ConstructSimulator()
        {
            Simulator.InitSimulator(new SimFactory());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        public virtual void EnableDisable()
        {
            MenuItem item;
            ToolBarButton button;
            this.cachedSimSettings = S.I.SimStateAdapter.getSimSettings();
            bool flag = false;
            if (this.CurrentEntityID != -1L)
            {
                flag = S.I.ThisPlayerName == S.I.SimStateAdapter.GetEntityPlayer(this.CurrentEntityID);
            }
            this.mnuHelpAssignment.Enabled = this.cachedSimSettings.PdfAssignment != null;
            Utilities.FindButtonEquivalent(this.tlbMain, this.mnuHelpAssignment.Text).Enabled = this.mnuHelpAssignment.Enabled;
            bool flag2 = this.CurrentEntityID == -1L;
            bool flag3 = S.I.Multiplayer && !S.I.Client;
            PropertyInfo[] properties = this.cachedSimSettings.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                string str;
                if (flag)
                {
                    str = "Owner";
                }
                else
                {
                    str = "NonOwner";
                }
                int index = info.Name.IndexOf("EnabledFor" + str);
                if (index > -1)
                {
                    string str2 = info.Name.Substring(0, index);
                    item = Utilities.FindMenuEquivalent(this.mainMenu1, Utilities.AddSpaces(str2));
                    if (item != null)
                    {
                        bool flag4 = (bool) info.GetGetMethod().Invoke(this.cachedSimSettings, new object[0]);
                        if (flag2)
                        {
                            flag4 = false;
                        }
                        else if (this.DesignerMode)
                        {
                            flag4 = true;
                        }
                        else if (flag3)
                        {
                            flag4 = this.IsReportMenuItem(item);
                        }
                        item.Enabled = flag4;
                        button = Utilities.FindButtonEquivalent(this.tlbMain, item.Text);
                        if (button != null)
                        {
                            button.Enabled = item.Enabled;
                        }
                    }
                }
            }
            if (S.I.MultiplayerRole != null)
            {
                string[] disableList = S.I.MultiplayerRole.DisableList;
                foreach (string str2 in disableList)
                {
                    item = Utilities.FindMenuEquivalent(this.mainMenu1, str2);
                    if (item != null)
                    {
                        item.Enabled = false;
                        button = Utilities.FindButtonEquivalent(this.tlbMain, item.Text);
                        if (button != null)
                        {
                            button.Enabled = item.Enabled;
                        }
                    }
                }
            }
            foreach (MenuItem item2 in this.mainMenu1.MenuItems)
            {
                foreach (MenuItem item3 in item2.MenuItems)
                {
                    if (item3.MenuItems.Count > 0)
                    {
                        item3.Enabled = false;
                        foreach (MenuItem item4 in item3.MenuItems)
                        {
                            if (item4.Text != "-")
                            {
                                item3.Enabled |= item4.Enabled;
                            }
                        }
                    }
                }
            }
            if (S.I.Client)
            {
                this.mnuOptionsIA.Enabled = false;
            }
            if (!((this.cachedSimSettings.StudentOrg <= 0) || S.MF.DesignerMode))
            {
                this.mnuOptionsIA.Enabled = false;
            }
        }

        public void EnableMenuAndSubMenus(MenuItem m)
        {
            m.Enabled = true;
            foreach (MenuItem item in m.MenuItems)
            {
                this.EnableMenuAndSubMenus(item);
            }
        }

        public string EntityIDToName(long ID)
        {
            if (this.entityNames.ContainsKey(ID))
            {
                return (string) this.entityNames[ID];
            }
            return "";
        }

        public long EntityNameToID(string entityName)
        {
            foreach (long num in this.entityNames.Keys)
            {
                if (((string) this.entityNames[num]).ToUpper() == entityName.ToUpper())
                {
                    return num;
                }
            }
            return -1L;
        }

        public void ExplainNoFunctionality()
        {
            MessageBox.Show(this, "You will be receiving a technical update shortly that implements this functionality");
        }

        protected Form FindOwnedForm(System.Type type)
        {
            foreach (Form form in base.OwnedForms)
            {
                if (form.GetType() == type)
                {
                    return form;
                }
            }
            return null;
        }

        private void FireNewTimeEvents(SimStateAdapter.ViewUpdate viewUpdate)
        {
            bool flag = (this.Now != DateNotSet) && (viewUpdate.Now.Year != this.Now.Year);
            bool flag2 = (this.CurrentWeek != -1) && (viewUpdate.CurrentWeek != this.CurrentWeek);
            bool flag3 = (this.Now != DateNotSet) && (((viewUpdate.Now.Day != this.Now.Day) || (viewUpdate.Now.Month != this.Now.Month)) || flag);
            if (((this.Now != DateNotSet) && ((viewUpdate.Now.Hour != this.Now.Hour) || flag3)) && (this.NewHour != null))
            {
                this.NewHour(this, new EventArgs());
            }
            if (flag3 && (this.NewDay != null))
            {
                this.NewDay(this, new EventArgs());
            }
            if (flag2 && (this.NewWeek != null))
            {
                this.NewWeek(this, new EventArgs());
                if (this.SoundOn)
                {
                    Sound.PlaySoundFromFile(@"sounds\NewWeek.wav");
                }
            }
            if (flag && (this.NewYear != null))
            {
                this.NewYear(this, new EventArgs());
            }
        }

        private void frmMainBase_Closed(object sender, EventArgs e)
        {
            this.UnregisterClientEventHandlers();
            Application.Exit();
        }

        private void frmMainBase_Closing(object sender, CancelEventArgs e)
        {
            if (!this.QuerySave())
            {
                e.Cancel = true;
            }
        }

        private void frmMainBase_Load(object sender, EventArgs e)
        {
            this.Text = Application.ProductName;
            if (!base.DesignMode)
            {
                if (S.I.Academic)
                {
                    Simulator i = S.I;
                    i.DataFileTypeExtension = i.DataFileTypeExtension + "A";
                }
                if ((this.commandLineArgs == null) || (this.commandLineArgs.Length == 0))
                {
                    this.GetCorrectStartChoices().ShowDialog(this);
                }
                else
                {
                    this.OpenSavedSim(this.commandLineArgs[0]);
                }
            }
        }

        private void frmMainBase_Resize(object sender, EventArgs e)
        {
            int num = 0;
            if (this.picMain.Size.Height > this.pnlMain.Size.Height)
            {
                num = 0x11;
            }
            this.pnlMain.AutoScrollPosition = new Point(0, 0);
            this.picMain.Location = new Point(Math.Max(0, ((this.pnlMain.Size.Width - num) - this.picMain.Size.Width) / 2), Math.Max(0, (this.pnlMain.Size.Height - this.picMain.Size.Height) / 2));
        }

        public Form GetCorrectStartChoices()
        {
            return new frmStartChoices();
        }

        public bool GetEnablingFor(string menuCaption)
        {
            return Utilities.FindMenuEquivalent(this.mainMenu1, menuCaption).Enabled;
        }

        public virtual int GetVBCStudentOrgCode()
        {
            return 0;
        }

        public void HideMessageWindow()
        {
            this.mnuOptionsShowMessages.Checked = false;
            this.messagesForm.Hide();
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            manager = new ResourceManager(typeof(frmMainBase));
            this.mainMenu1 = new MainMenu();
            this.mnuFile = new MenuItem();
            this.mnuFileNew = new MenuItem();
            this.mnuFileOpenLesson = new MenuItem();
            this.mnuFileOpenSavedSim = new MenuItem();
            this.mnuSep1 = new MenuItem();
            this.mnuFileSave = new MenuItem();
            this.mnuFileSaveAs = new MenuItem();
            this.mnuFileUploadSeparator = new MenuItem();
            this.mnuFileMultiplayer = new MenuItem();
            this.mnuFileMultiplayerJoin = new MenuItem();
            this.mnuFileMultiplayerStart = new MenuItem();
            this.mnuFileMultiplayerScoreboard = new MenuItem();
            this.mnuFileMultiplayerTeamList = new MenuItem();
            this.menuItem3 = new MenuItem();
            this.mnuFilePrintView = new MenuItem();
            this.mnuSep3 = new MenuItem();
            this.mnuFileExit = new MenuItem();
            this.mnuView = new MenuItem();
            this.mnuReports = new MenuItem();
            this.mnuActions = new MenuItem();
            this.mnuOptions = new MenuItem();
            this.mnuOptionsGoStop = new MenuItem();
            this.mnuOptionsFaster = new MenuItem();
            this.mnuOptionsSlower = new MenuItem();
            this.mnuSep9385 = new MenuItem();
            this.mnuOptionsRunTo = new MenuItem();
            this.menuMessagesSep = new MenuItem();
            this.mnuOptionsShowMessages = new MenuItem();
            this.mnuSep4 = new MenuItem();
            this.mnuOptionsBackgroundMusic = new MenuItem();
            this.mnuOptionsSoundEffects = new MenuItem();
            this.mnuSep6 = new MenuItem();
            this.mnuOptionsIA = new MenuItem();
            this.mnuOptionsIACustomizeYourSim = new MenuItem();
            this.mnuOptionsIAProvideCash = new MenuItem();
            this.mnuOptionsTuning = new MenuItem();
            this.mnuOptionsChangeOwner = new MenuItem();
            this.mnuOptionsRenameEntity = new MenuItem();
            this.mnuOptionsMacros = new MenuItem();
            this.mnuOptionsMacrosRecordMacro = new MenuItem();
            this.mnuOptionsMacroStopRecording = new MenuItem();
            this.mnuOptionsMacrosPlayMacro = new MenuItem();
            this.mnuOptionsMacrosStopPlaying = new MenuItem();
            this.mnuOptionsTestResults = new MenuItem();
            this.mnuHelp = new MenuItem();
            this.mnuHelpTopicsAndIndex = new MenuItem();
            this.mnuHelpTutorial = new MenuItem();
            this.mnuHelpSearch = new MenuItem();
            this.mnuSep7 = new MenuItem();
            this.mnuHelpAssignment = new MenuItem();
            this.mnuSep8 = new MenuItem();
            this.mnuHelpAbout = new MenuItem();
            this.staMain = new StatusBar();
            this.DatePanel = new StatusBarPanel();
            this.DayOfWeekPanel = new StatusBarPanel();
            this.TimePanel = new StatusBarPanel();
            this.NewMessagesPanel = new StatusBarPanel();
            this.CreateMessagePanel = new StatusBarPanel();
            this.Level = new StatusBarPanel();
            this.SpacerPanel = new StatusBarPanel();
            this.CurrentEntityNamePanel = new StatusBarPanel();
            this.CurrentEntityPanel = new StatusBarPanel();
            this.EntityCriticalResourceNamePanel = new StatusBarPanel();
            this.EntityCriticalResourcePanel = new StatusBarPanel();
            this.tlbMain = new ToolbarSponsored();
            this.ilsMainToolBar = new ImageList(this.components);
            this.pnlMain = new Panel();
            this.picMain = new PictureBox();
            this.viewToolTip = new ToolTip(this.components);
            this.backgroundMusicTimer = new System.Windows.Forms.Timer(this.components);
            this.DatePanel.BeginInit();
            this.DayOfWeekPanel.BeginInit();
            this.TimePanel.BeginInit();
            this.NewMessagesPanel.BeginInit();
            this.CreateMessagePanel.BeginInit();
            this.Level.BeginInit();
            this.SpacerPanel.BeginInit();
            this.CurrentEntityNamePanel.BeginInit();
            this.CurrentEntityPanel.BeginInit();
            this.EntityCriticalResourceNamePanel.BeginInit();
            this.EntityCriticalResourcePanel.BeginInit();
            this.pnlMain.SuspendLayout();
            base.SuspendLayout();
            this.mainMenu1.MenuItems.AddRange(new MenuItem[] { this.mnuFile, this.mnuView, this.mnuReports, this.mnuActions, this.mnuOptions, this.mnuHelp });
            this.mnuFile.Index = 0;
            this.mnuFile.MenuItems.AddRange(new MenuItem[] { this.mnuFileNew, this.mnuFileOpenLesson, this.mnuFileOpenSavedSim, this.mnuSep1, this.mnuFileSave, this.mnuFileSaveAs, this.mnuFileUploadSeparator, this.mnuFileMultiplayer, this.menuItem3, this.mnuFilePrintView, this.mnuSep3, this.mnuFileExit });
            this.mnuFile.Text = "&File";
            this.mnuFileNew.Index = 0;
            this.mnuFileNew.Text = "&New...";
            this.mnuFileNew.Click += new EventHandler(this.mnuFileNew_Click);
            this.mnuFileOpenLesson.Index = 1;
            this.mnuFileOpenLesson.Text = "&Open Lesson...";
            this.mnuFileOpenLesson.Click += new EventHandler(this.mnuFileOpenLesson_Click);
            this.mnuFileOpenSavedSim.Index = 2;
            this.mnuFileOpenSavedSim.Text = "Open S&aved Sim...";
            this.mnuFileOpenSavedSim.Click += new EventHandler(this.mnuFileOpenSavedSim_Click);
            this.mnuSep1.Index = 3;
            this.mnuSep1.Text = "-";
            this.mnuFileSave.Index = 4;
            this.mnuFileSave.Text = "Sa&ve";
            this.mnuFileSave.Click += new EventHandler(this.mnuFileSave_Click);
            this.mnuFileSaveAs.Index = 5;
            this.mnuFileSaveAs.Text = "Sav&e As...";
            this.mnuFileSaveAs.Click += new EventHandler(this.mnuFileSaveAs_Click);
            this.mnuFileUploadSeparator.Index = 6;
            this.mnuFileUploadSeparator.Text = "-";
            this.mnuFileUploadSeparator.Visible = false;
            this.mnuFileMultiplayer.Index = 7;
            this.mnuFileMultiplayer.MenuItems.AddRange(new MenuItem[] { this.mnuFileMultiplayerJoin, this.mnuFileMultiplayerStart, this.mnuFileMultiplayerScoreboard, this.mnuFileMultiplayerTeamList });
            this.mnuFileMultiplayer.Text = "&Multiplayer";
            this.mnuFileMultiplayerJoin.Index = 0;
            this.mnuFileMultiplayerJoin.Text = "&Join Session";
            this.mnuFileMultiplayerJoin.Click += new EventHandler(this.mnuFileMultiplayerJoin_Click);
            this.mnuFileMultiplayerStart.Index = 1;
            this.mnuFileMultiplayerStart.Text = "&Start Session";
            this.mnuFileMultiplayerStart.Click += new EventHandler(this.mnuFileMultiplayerStart_Click);
            this.mnuFileMultiplayerScoreboard.Index = 2;
            this.mnuFileMultiplayerScoreboard.Text = "S&coreboard";
            this.mnuFileMultiplayerScoreboard.Click += new EventHandler(this.mnuFileMultiplayerScoreboard_Click);
            this.mnuFileMultiplayerTeamList.Index = 3;
            this.mnuFileMultiplayerTeamList.Text = "&Team List";
            this.mnuFileMultiplayerTeamList.Click += new EventHandler(this.mnuFileMultiplayerTeamList_Click);
            this.menuItem3.Index = 8;
            this.menuItem3.Text = "-";
            this.mnuFilePrintView.Index = 9;
            this.mnuFilePrintView.Text = "&Print View...";
            this.mnuFilePrintView.Click += new EventHandler(this.mnuFilePrintView_Click);
            this.mnuSep3.Index = 10;
            this.mnuSep3.Text = "-";
            this.mnuFileExit.Index = 11;
            this.mnuFileExit.Text = "E&xit";
            this.mnuFileExit.Click += new EventHandler(this.mnuFileExit_Click);
            this.mnuView.Index = 1;
            this.mnuView.Text = "&View";
            this.mnuReports.Index = 2;
            this.mnuReports.Text = "&Reports";
            this.mnuActions.Index = 3;
            this.mnuActions.Text = "&Actions";
            this.mnuActions.Select += new EventHandler(this.mnuActions_Select);
            this.mnuOptions.Index = 4;
            this.mnuOptions.MenuItems.AddRange(new MenuItem[] { 
                this.mnuOptionsGoStop, this.mnuOptionsFaster, this.mnuOptionsSlower, this.mnuSep9385, this.mnuOptionsRunTo, this.menuMessagesSep, this.mnuOptionsShowMessages, this.mnuSep4, this.mnuOptionsBackgroundMusic, this.mnuOptionsSoundEffects, this.mnuSep6, this.mnuOptionsIA, this.mnuOptionsTuning, this.mnuOptionsChangeOwner, this.mnuOptionsRenameEntity, this.mnuOptionsMacros, 
                this.mnuOptionsTestResults
             });
            this.mnuOptions.Text = "&Options";
            this.mnuOptionsGoStop.Index = 0;
            this.mnuOptionsGoStop.Text = "&Go";
            this.mnuOptionsGoStop.Click += new EventHandler(this.mnuOptionsGoStop_Click);
            this.mnuOptionsFaster.Index = 1;
            this.mnuOptionsFaster.Text = "&Faster";
            this.mnuOptionsFaster.Click += new EventHandler(this.mnuOptionsFaster_Click);
            this.mnuOptionsSlower.Index = 2;
            this.mnuOptionsSlower.Text = "&Slower";
            this.mnuOptionsSlower.Click += new EventHandler(this.mnuOptionsSlower_Click);
            this.mnuSep9385.Index = 3;
            this.mnuSep9385.Text = "-";
            this.mnuOptionsRunTo.Index = 4;
            this.mnuOptionsRunTo.Text = "Run to...";
            this.mnuOptionsRunTo.Click += new EventHandler(this.mnuOptionsRunTo_Click);
            this.menuMessagesSep.Index = 5;
            this.menuMessagesSep.Text = "-";
            this.mnuOptionsShowMessages.Checked = true;
            this.mnuOptionsShowMessages.Index = 6;
            this.mnuOptionsShowMessages.Text = "Show Messages";
            this.mnuOptionsShowMessages.Click += new EventHandler(this.mnuOptionsShowMessages_Click);
            this.mnuSep4.Index = 7;
            this.mnuSep4.Text = "-";
            this.mnuOptionsBackgroundMusic.Index = 8;
            this.mnuOptionsBackgroundMusic.Text = "&Background Music";
            this.mnuOptionsBackgroundMusic.Click += new EventHandler(this.mnuOptionsBackgroundMusic_Click);
            this.mnuOptionsSoundEffects.Checked = true;
            this.mnuOptionsSoundEffects.Index = 9;
            this.mnuOptionsSoundEffects.Text = "S&ound Effects";
            this.mnuOptionsSoundEffects.Click += new EventHandler(this.mnuOptionsSoundEffects_Click);
            this.mnuSep6.Index = 10;
            this.mnuSep6.Text = "-";
            this.mnuOptionsIA.Index = 11;
            this.mnuOptionsIA.MenuItems.AddRange(new MenuItem[] { this.mnuOptionsIACustomizeYourSim, this.mnuOptionsIAProvideCash });
            this.mnuOptionsIA.Text = "&Instructor's Area";
            this.mnuOptionsIACustomizeYourSim.Index = 0;
            this.mnuOptionsIACustomizeYourSim.Text = "Enable/Disable &Features...";
            this.mnuOptionsIACustomizeYourSim.Click += new EventHandler(this.mnuOptionsIACustomizeYourSim_Click);
            this.mnuOptionsIAProvideCash.Index = 1;
            this.mnuOptionsIAProvideCash.Text = "&Provide Cash...";
            this.mnuOptionsIAProvideCash.Click += new EventHandler(this.mnuOptionsIAProvideCash_Click);
            this.mnuOptionsTuning.Index = 12;
            this.mnuOptionsTuning.Text = "Tuning";
            this.mnuOptionsTuning.Click += new EventHandler(this.mnuOptionsTuning_Click);
            this.mnuOptionsChangeOwner.Index = 13;
            this.mnuOptionsChangeOwner.Text = "Change Owner";
            this.mnuOptionsChangeOwner.Click += new EventHandler(this.mnuOptionsChangeOwner_Click);
            this.mnuOptionsRenameEntity.Index = 14;
            this.mnuOptionsRenameEntity.Text = "Rename Entity";
            this.mnuOptionsRenameEntity.Click += new EventHandler(this.mnuOptionsRenameEntity_Click);
            this.mnuOptionsMacros.Index = 15;
            this.mnuOptionsMacros.MenuItems.AddRange(new MenuItem[] { this.mnuOptionsMacrosRecordMacro, this.mnuOptionsMacroStopRecording, this.mnuOptionsMacrosPlayMacro, this.mnuOptionsMacrosStopPlaying });
            this.mnuOptionsMacros.Text = "&Macros";
            this.mnuOptionsMacrosRecordMacro.Index = 0;
            this.mnuOptionsMacrosRecordMacro.Text = "&Record Macro";
            this.mnuOptionsMacrosRecordMacro.Click += new EventHandler(this.mnuOptionsMacrosRecordMacro_Click);
            this.mnuOptionsMacroStopRecording.Enabled = false;
            this.mnuOptionsMacroStopRecording.Index = 1;
            this.mnuOptionsMacroStopRecording.Text = "&Stop Recording";
            this.mnuOptionsMacroStopRecording.Click += new EventHandler(this.mnuOptionsMacroStopRecording_Click);
            this.mnuOptionsMacrosPlayMacro.Index = 2;
            this.mnuOptionsMacrosPlayMacro.Text = "&Play Macro";
            this.mnuOptionsMacrosPlayMacro.Click += new EventHandler(this.mnuOptionsMacrosPlayMacro_Click);
            this.mnuOptionsMacrosStopPlaying.Enabled = false;
            this.mnuOptionsMacrosStopPlaying.Index = 3;
            this.mnuOptionsMacrosStopPlaying.Text = "S&top Playing";
            this.mnuOptionsMacrosStopPlaying.Click += new EventHandler(this.mnuOptionsMacrosStopPlaying_Click);
            this.mnuOptionsTestResults.Index = 16;
            this.mnuOptionsTestResults.Text = "Test Results...";
            this.mnuOptionsTestResults.Visible = false;
            this.mnuOptionsTestResults.Click += new EventHandler(this.mnuOptionsTestResults_Click);
            this.mnuHelp.Index = 5;
            this.mnuHelp.MenuItems.AddRange(new MenuItem[] { this.mnuHelpTopicsAndIndex, this.mnuHelpTutorial, this.mnuHelpSearch, this.mnuSep7, this.mnuHelpAssignment, this.mnuSep8, this.mnuHelpAbout });
            this.mnuHelp.Text = "&Help";
            this.mnuHelpTopicsAndIndex.Index = 0;
            this.mnuHelpTopicsAndIndex.Text = "&Topics && Index...";
            this.mnuHelpTopicsAndIndex.Click += new EventHandler(this.mnuHelpTopicsAndIndex_Click);
            this.mnuHelpTutorial.Index = 1;
            this.mnuHelpTutorial.Text = "T&utorial...";
            this.mnuHelpTutorial.Click += new EventHandler(this.mnuHelpTutorial_Click);
            this.mnuHelpSearch.Index = 2;
            this.mnuHelpSearch.Text = "Sear&ch...";
            this.mnuHelpSearch.Click += new EventHandler(this.mnuHelpSearch_Click);
            this.mnuSep7.Index = 3;
            this.mnuSep7.Text = "-";
            this.mnuHelpAssignment.Index = 4;
            this.mnuHelpAssignment.Text = "A&ssignment...";
            this.mnuHelpAssignment.Click += new EventHandler(this.mnuHelpAssignment_Click);
            this.mnuSep8.Index = 5;
            this.mnuSep8.Text = "-";
            this.mnuHelpAbout.Index = 6;
            this.mnuHelpAbout.Text = "&About...";
            this.mnuHelpAbout.Click += new EventHandler(this.mnuHelpAbout_Click);
            this.staMain.Location = new Point(0, 0x13e);
            this.staMain.Name = "staMain";
            this.staMain.Panels.AddRange(new StatusBarPanel[] { this.DatePanel, this.DayOfWeekPanel, this.TimePanel, this.NewMessagesPanel, this.CreateMessagePanel, this.Level, this.SpacerPanel, this.CurrentEntityNamePanel, this.CurrentEntityPanel, this.EntityCriticalResourceNamePanel, this.EntityCriticalResourcePanel });
            this.staMain.ShowPanels = true;
            this.staMain.Size = new Size(0x278, 16);
            this.staMain.SizingGrip = false;
            this.staMain.TabIndex = 2;
            this.staMain.PanelClick += new StatusBarPanelClickEventHandler(this.staMain_PanelClick);
            this.DatePanel.Alignment = HorizontalAlignment.Right;
            this.DayOfWeekPanel.Width = 40;
            this.TimePanel.Alignment = HorizontalAlignment.Right;
            this.TimePanel.Width = 60;
            this.NewMessagesPanel.BorderStyle = StatusBarPanelBorderStyle.None;
            this.NewMessagesPanel.Icon = (Icon) manager.GetObject("NewMessagesPanel.Icon");
            this.NewMessagesPanel.MinWidth = 0;
            this.NewMessagesPanel.Text = "New Messages";
            this.NewMessagesPanel.Width = 0;
            this.CreateMessagePanel.Icon = (Icon) manager.GetObject("CreateMessagePanel.Icon");
            this.CreateMessagePanel.MinWidth = 0;
            this.CreateMessagePanel.ToolTipText = "Send Memo to Team";
            this.CreateMessagePanel.Width = 0;
            this.Level.BorderStyle = StatusBarPanelBorderStyle.None;
            this.Level.Width = 70;
            this.SpacerPanel.AutoSize = StatusBarPanelAutoSize.Spring;
            this.SpacerPanel.BorderStyle = StatusBarPanelBorderStyle.None;
            this.SpacerPanel.Width = 0x65;
            this.CurrentEntityNamePanel.Alignment = HorizontalAlignment.Right;
            this.CurrentEntityNamePanel.AutoSize = StatusBarPanelAutoSize.Contents;
            this.CurrentEntityNamePanel.BorderStyle = StatusBarPanelBorderStyle.None;
            this.CurrentEntityNamePanel.Text = "#";
            this.CurrentEntityNamePanel.Width = 20;
            this.EntityCriticalResourceNamePanel.Alignment = HorizontalAlignment.Right;
            this.EntityCriticalResourceNamePanel.AutoSize = StatusBarPanelAutoSize.Contents;
            this.EntityCriticalResourceNamePanel.BorderStyle = StatusBarPanelBorderStyle.None;
            this.EntityCriticalResourceNamePanel.Text = "Cash";
            this.EntityCriticalResourceNamePanel.Width = 0x29;
            this.EntityCriticalResourcePanel.Alignment = HorizontalAlignment.Right;
            this.tlbMain.Appearance = ToolBarAppearance.Flat;
            this.tlbMain.DropDownArrows = true;
            this.tlbMain.ImageList = this.ilsMainToolBar;
            this.tlbMain.Location = new Point(0, 0);
            this.tlbMain.Name = "tlbMain";
            this.tlbMain.ShowToolTips = true;
            this.tlbMain.Size = new Size(0x278, 0x2a);
            this.tlbMain.TabIndex = 0;
            this.tlbMain.ButtonClick += new ToolBarButtonClickEventHandler(this.tlbMain_ButtonClick);
            this.ilsMainToolBar.ColorDepth = ColorDepth.Depth24Bit;
            this.ilsMainToolBar.ImageSize = new Size(0x18, 0x18);
            this.ilsMainToolBar.TransparentColor = Color.Transparent;
            this.pnlMain.AutoScroll = true;
            this.pnlMain.BackColor = Color.White;
            this.pnlMain.Controls.Add(this.picMain);
            this.pnlMain.Dock = DockStyle.Fill;
            this.pnlMain.Location = new Point(0, 0x2a);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new Size(0x278, 0x114);
            this.pnlMain.TabIndex = 1;
            this.picMain.BackColor = Color.White;
            this.picMain.Location = new Point(0x48, 40);
            this.picMain.Name = "picMain";
            this.picMain.Size = new Size(0x138, 0xb8);
            this.picMain.TabIndex = 0;
            this.picMain.TabStop = false;
            this.picMain.Click += new EventHandler(this.picMain_Click);
            this.picMain.Paint += new PaintEventHandler(this.picMain_Paint);
            this.picMain.MouseUp += new MouseEventHandler(this.picMain_MouseUp);
            this.picMain.DoubleClick += new EventHandler(this.picMain_DoubleClick);
            this.picMain.MouseMove += new MouseEventHandler(this.picMain_MouseMove);
            this.picMain.MouseDown += new MouseEventHandler(this.picMain_MouseDown);
            this.viewToolTip.AutomaticDelay = 200;
            this.backgroundMusicTimer.Tick += new EventHandler(this.backgroundMusicTimer_Tick);
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x278, 0x14e);
            base.Controls.Add(this.pnlMain);
            base.Controls.Add(this.tlbMain);
            base.Controls.Add(this.staMain);
            base.Menu = this.mainMenu1;
            base.Name = "frmMainBase";
            this.Text = "#";
            base.Resize += new EventHandler(this.frmMainBase_Resize);
            base.Closing += new CancelEventHandler(this.frmMainBase_Closing);
            base.Load += new EventHandler(this.frmMainBase_Load);
            base.Closed += new EventHandler(this.frmMainBase_Closed);
            this.DatePanel.EndInit();
            this.DayOfWeekPanel.EndInit();
            this.TimePanel.EndInit();
            this.NewMessagesPanel.EndInit();
            this.CreateMessagePanel.EndInit();
            this.Level.EndInit();
            this.SpacerPanel.EndInit();
            this.CurrentEntityNamePanel.EndInit();
            this.CurrentEntityPanel.EndInit();
            this.EntityCriticalResourceNamePanel.EndInit();
            this.EntityCriticalResourcePanel.EndInit();
            this.pnlMain.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        protected void InitRemotingConfiguration()
        {
            AssemblyName name = Assembly.GetEntryAssembly().GetName();
            int startIndex = 1 + name.CodeBase.LastIndexOf("/");
            RemotingConfiguration.Configure(Application.StartupPath + @"\" + name.CodeBase.Substring(startIndex) + ".config");
        }

        public virtual bool IsActionMenuItem(string menuItemText)
        {
            MenuItem item = Utilities.FindMenuEquivalent(this.mainMenu1, menuItemText);
            if (item != null)
            {
                MenuItem parent = (MenuItem) item.Parent;
                while (parent.Parent != this.mainMenu1)
                {
                    parent = (MenuItem) parent.Parent;
                }
                return (parent == this.mnuActions);
            }
            return false;
        }

        public bool IsMenuItemEnabled(string menuItemText)
        {
            MenuItem item = Utilities.FindMenuEquivalent(this.mainMenu1, menuItemText);
            if (item == null)
            {
                throw new Exception("Can't find menu item " + menuItemText + " in IsMenuItemEnabled.");
            }
            return item.Enabled;
        }

        protected virtual bool IsReportMenuItem(MenuItem menuItem)
        {
            MenuItem parent = (MenuItem) menuItem.Parent;
            return ((parent == this.mnuReports) || (parent == this.mnuView));
        }

        protected virtual void LoadStateHook()
        {
        }

        private void mnuActions_Select(object sender, EventArgs e)
        {
            this.dirtySimState = true;
            this.CloseActionForms();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            base.Close();
        }

        protected virtual void mnuFileMultiplayerJoin_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            if (this.QuerySave())
            {
                frmJoinMultiplayerSession session = new frmJoinMultiplayerSession();
                if (session.ShowDialog() != DialogResult.Cancel)
                {
                    this.ReInit();
                    S.I.Client = true;
                    S.I.SimStateAdapter = session.RemoteAdapter;
                    S.I.ThisPlayerName = session.TeamName;
                    S.I.SessionName = session.SessionName;
                    S.I.MultiplayerRoleName = session.MultiplayerRoleName;
                    if ((S.I.MultiplayerRoleName != "") && S.I.AllowIntraTeamMessaging)
                    {
                        this.CreateMessagePanel.Width = 60;
                    }
                    this.ClientJoinHook(session.Player);
                    this.OnStateChanged();
                    this.mnuOptionsGoStop.PerformClick();
                }
            }
        }

        private void mnuFileMultiplayerScoreboard_Click(object sender, EventArgs e)
        {
            Form form = this.FindOwnedForm(typeof(frmScoreboard));
            if (form == null)
            {
                form = new frmScoreboard {
                    Owner = this
                };
                form.Show();
            }
            else
            {
                form.Focus();
            }
        }

        protected virtual void mnuFileMultiplayerStart_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            if (this.QuerySave())
            {
                this.ReInit();
                this.currentFilePath = null;
                if (!(this.DesignerMode && (this.cacheDesignerSimSettings != null)))
                {
                    S.I.NewState(S.I.DefaultSimSettings, true);
                }
                else
                {
                    S.I.NewState(this.cacheDesignerSimSettings, true);
                }
                this.ServerStartHook();
                this.OnStateChanged();
                this.mnuOptionsGoStop.PerformClick();
            }
        }

        private void mnuFileMultiplayerTeamList_Click(object sender, EventArgs e)
        {
            string text = "";
            foreach (Player player in S.ST.Player.Values)
            {
                if ((player.PlayerName != S.I.ThisPlayerName) && (player.PlayerType != PlayerType.AI))
                {
                    text = text + S.R.GetString("Team Name: ") + player.PlayerName;
                    if (S.I.UserAdminSettings.PasswordsForMultiplayer)
                    {
                        text = text + "     " + S.R.GetString("Password: ") + S.ST.GetMultiplayerTeamPassword(player.PlayerName);
                    }
                    text = text + "\r\n";
                }
            }
            MessageBox.Show(text, S.R.GetString("Teams That Have Logged In To This Session"));
        }

        private void mnuFileNew_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            if (this.QuerySave())
            {
                if (S.I.Academic)
                {
                    new frmChooseMathPack().ShowDialog();
                }
                DialogResult result = new frmDualChoiceDialog("What kind of " + S.I.NewWhatName.ToLower() + " would you like?", "Standard " + S.I.NewWhatName, "Random " + S.I.NewWhatName, true) { Text = "New " + S.I.EntityName + " Project" }.ShowDialog(this);
                if (result != DialogResult.Cancel)
                {
                    SimSettings defaultSimSettings = S.I.DefaultSimSettings;
                    if (result == DialogResult.Yes)
                    {
                        if (S.I.NewStandardProjectFromFile)
                        {
                            string filepath = Application.StartupPath + @"\Project\New " + S.I.EntityName + " Project." + S.I.DataFileTypeExtension;
                            if (S.I.Academic)
                            {
                                filepath = string.Concat(new object[] { AcademicGod.PageBankPath, Path.DirectorySeparatorChar, "Project.", S.I.DataFileTypeExtension });
                            }
                            this.OpenSavedSim(filepath);
                            this.lessonLoadedPromptSaveAs = true;
                            return;
                        }
                        defaultSimSettings.RandomSeed = 0;
                    }
                    this.ReInit();
                    this.currentFilePath = null;
                    if (!(this.DesignerMode && (this.cacheDesignerSimSettings != null)))
                    {
                        S.I.NewState(defaultSimSettings, false);
                    }
                    else
                    {
                        S.I.NewState(this.cacheDesignerSimSettings, false);
                    }
                    this.OnStateChanged();
                    this.mnuOptionsGoStop.PerformClick();
                }
            }
        }

        private void mnuFileOpenLesson_Click(object sender, EventArgs e)
        {
            if (S.I.Demo)
            {
                MessageBox.Show(this, S.R.GetString("Only a limited number of lessons are available in this demo edition."), S.R.GetString("Demo Edition"));
            }
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            if (this.QuerySave())
            {
                frmLessonsSimple simple;
                try
                {
                    simple = new frmLessonsSimple();
                }
                catch (DirectoryNotFoundException)
                {
                    MessageBox.Show("The Lessons directory is missing from the directory in which " + Application.ProductName + " was installed. Re-install " + Application.ProductName + ".");
                    return;
                }
                if (simple.ShowDialog() == DialogResult.OK)
                {
                    this.OpenSavedSim(simple.LessonFileName);
                    this.lessonLoadedPromptSaveAs = true;
                }
            }
        }

        private void mnuFileOpenSavedSim_Click(object sender, EventArgs e)
        {
            if (S.I.Demo)
            {
                MessageBox.Show(this, S.R.GetString("This feature is disabled in this demo edition."), S.R.GetString("Demo Edition"));
            }
            else
            {
                if (S.I.SimTimeRunning)
                {
                    this.mnuOptionsGoStop_Click(new object(), new EventArgs());
                }
                if (this.QuerySave())
                {
                    string startdir = Environment.CurrentDirectory + "\\";
                    if (Directory.Exists(Environment.CurrentDirectory + "\\saves\\"))
                        startdir += "saves\\";
                    OpenFileDialog dialog = new OpenFileDialog {
                        InitialDirectory = startdir,
                        Filter = S.I.DataFileTypeName + " files (*." + S.I.DataFileTypeExtension + ")|*." + S.I.DataFileTypeExtension + "|All files (*.*)|*.*",
                        DefaultExt = S.I.DataFileTypeExtension
                    };
                    if (S.I.UserAdminSettings.DefaultDirectory != null)
                    {
                        dialog.InitialDirectory = S.I.UserAdminSettings.DefaultDirectory;
                    }
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        this.OpenSavedSim(dialog.FileName);
                    }
                }
            }
        }

        private void mnuFilePrintView_Click(object sender, EventArgs e)
        {
            frmInputString str = new frmInputString(S.R.GetString("Student Name"), S.R.GetString("Enter your name to help identify your printout on a shared printer:"), this.printStudentName);
            str.ShowDialog(this);
            this.printStudentName = str.Response;
            Utilities.PrintWithExceptionHandling(this.Text, new PrintPageEventHandler(this.View_PrintPage));
        }

        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            if (S.I.Demo)
            {
                MessageBox.Show(this, S.R.GetString("This feature is disabled in this demo edition."), S.R.GetString("Demo Edition"));
            }
            else if ((this.currentFilePath == null) | this.lessonLoadedPromptSaveAs)
            {
                this.mnuFileSaveAs_Click(new object(), new EventArgs());
            }
            else
            {
                try
                {
                    this.SaveSim(this.currentFilePath);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(S.R.GetString("Could not save file. File may be read-only or in use by another application.") + "\r\n\r\n" + S.R.GetString("Error details") + ": " + exception.Message, S.R.GetString("Could Not Save"));
                    this.mnuFileSaveAs_Click(new object(), new EventArgs());
                }
            }
        }

        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            if (S.I.Demo)
            {
                MessageBox.Show(this, S.R.GetString("This feature is disabled in this demo edition."), S.R.GetString("Demo Edition"));
            }
            else
            {
                bool flag = false;
                if (S.I.SimTimeRunning)
                {
                    flag = true;
                    this.mnuOptionsGoStop_Click(new object(), new EventArgs());
                }
                string startdir = Environment.CurrentDirectory + "\\";
                if (Directory.Exists(Environment.CurrentDirectory + "\\saves\\"))
                    startdir += "saves\\";
                SaveFileDialog dialog = new SaveFileDialog {
                    InitialDirectory = startdir,
                    Filter = S.I.DataFileTypeName + " files (*." + S.I.DataFileTypeExtension + ")|*." + S.I.DataFileTypeExtension + "|All files (*.*)|*.*",
                    DefaultExt = S.I.DataFileTypeExtension
                };
                if (S.I.UserAdminSettings.DefaultDirectory != null)
                {
                    dialog.InitialDirectory = S.I.UserAdminSettings.DefaultDirectory;
                }
                while (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        this.SaveSim(dialog.FileName);
                        this.lessonLoadedPromptSaveAs = false;
                        this.currentFilePath = dialog.FileName;
                        this.Text = Path.GetFileNameWithoutExtension(this.currentFilePath) + " - " + Application.ProductName;
                        if (S.I.SimStateAdapter.getMultiplayer())
                        {
                            string text = this.Text;
                            this.Text = text + " Multiplayer   Role: " + this.ClientOrHost() + "   Session Name: " + Simulator.Instance.SessionName;
                        }
                        break;
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(S.R.GetString("Could not save file. File may be read-only or in use by another application.") + "\r\n\r\n" + S.R.GetString("Error details") + ": " + exception.Message, S.R.GetString("Could Not Save"));
                    }
                }
                if (flag)
                {
                    this.mnuOptionsGoStop_Click(new object(), new EventArgs());
                }
            }
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog(this);
        }

        private void mnuHelpAssignment_Click(object sender, EventArgs e)
        {
            byte[] buffer = S.SA.getPdfAssignment();
            if (Thread.CurrentThread.CurrentUICulture.Name != "")
            {
                Hashtable hashtable = (Hashtable) S.ST.Reserved["LocalLanguageAssignments"];
                if (hashtable != null)
                {
                    byte[] buffer2 = (byte[]) hashtable[Thread.CurrentThread.CurrentUICulture.Name];
                    if (buffer2 != null)
                    {
                        buffer = buffer2;
                    }
                }
            }
            string tempPath = Path.GetTempPath();
            string path = null;
            int num = 100;
            int num2 = 0;
            while (num2 < num)
            {
                try
                {
                    path = string.Concat(new object[] { tempPath, "Printable Virtual Business Assignment.pdf", num2, "q.pdf" });
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    break;
                }
                catch (IOException)
                {
                }
                num2++;
            }
            if (num2 >= num)
            {
                MessageBox.Show("Too many assignment files are being viewed at one time.  Try closing some first, then try viewing this assignment again.");
            }
            else
            {
                try
                {
                    FileStream output = new FileStream(path, FileMode.Create);
                    new BinaryWriter(output).Write(buffer);
                    output.Close();
                    Process.Start(path);
                }
                catch (Win32Exception)
                {
                    MessageBox.Show("Could not find Adobe Acrobat Reader to display assignment. Either download and install Acrobat Reader from www.Adobe.com or have your instructor copy the assignment out of the Instructor's Manual.");
                }
                catch (Exception exception)
                {
                    MessageBox.Show("Could not display assignment. Please have your instructor copy the assignment out of the Instructor's Manual.\r\n\r\nDetailed problem: " + exception.Message);
                }
            }
        }

        private void mnuHelpSearch_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenSearch();
        }

        private void mnuHelpTopicsAndIndex_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp();
        }

        private void mnuHelpTutorial_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp();
        }

        public virtual void mnuOptionsBackgroundMusic_Click(object sender, EventArgs e)
        {
            this.mnuOptionsBackgroundMusic.Checked = !this.mnuOptionsBackgroundMusic.Checked;
            if (!this.mnuOptionsBackgroundMusic.Checked)
            {
                this.StopMusic();
            }
            if (this.mnuOptionsBackgroundMusic.Checked && S.I.SimTimeRunning)
            {
                this.backgroundMusicTimer.Enabled = true;
                this.StartMusic();
            }
        }

        private void mnuOptionsChangeOwner_Click(object sender, EventArgs e)
        {
            frmInputString errantForm = new frmInputString("Change Owner", "Enter the player name of the new owner. Enter USER for single player user. Enter AI for a new AI player.", "");
            if (errantForm.ShowDialog() == DialogResult.OK)
            {
                string response = errantForm.Response;
                if (response.ToUpper() == "USER")
                {
                    response = "";
                }
                if (response.ToUpper() == "AI")
                {
                    response = S.SA.CreatePlayer(Guid.NewGuid().ToString(), PlayerType.AI).PlayerName;
                }
                try
                {
                    S.SA.ChangeEntityOwner(S.MF.CurrentEntityID, response);
                }
                catch (SimApplicationException exception)
                {
                    MessageBox.Show(exception.Message, "Couldn't change owner");
                }
                catch (Exception exception2)
                {
                    frmExceptionHandler.Handle(exception2, errantForm);
                }
            }
        }

        private void mnuOptionsFaster_Click(object sender, EventArgs e)
        {
            SimState simState = S.I.SimState;
            simState.SpeedIndex++;
            this.OnSpeedChange();
        }

        public void mnuOptionsGoStop_Click(object sender, EventArgs e)
        {
            if (!S.I.SimTimeRunning) {
                this.dirtySimState = true;
                S.I.StartSimTimeRunning();
            }
            else 
                S.I.StopSimTimeRunning(); 
            this.SynchGoStop();
        }

        protected void mnuOptionsIACustomizeYourSim_Click(object sender, EventArgs e)
        {
            frmPassword password = new frmPassword(S.I.UserAdminSettings.GetP());
            if (this.DesignerMode || (password.ShowDialog(this) == DialogResult.OK))
            {
                frmEditSimSettings errantForm = null;
                try
                {
                    errantForm = new frmEditSimSettings();
                    errantForm.ShowDialog();
                    this.EnableDisable();
                }
                catch (Exception exception)
                {
                    frmExceptionHandler.Handle(exception, errantForm);
                }
            }
        }

        protected virtual void mnuOptionsIAProvideCash_Click(object sender, EventArgs e)
        {
            frmPassword password = new frmPassword(S.I.UserAdminSettings.GetP());
            if (this.DesignerMode || (password.ShowDialog(this) == DialogResult.OK))
            {
                try
                {
                    new frmProvideCash().ShowDialog(this);
                }
                catch (Exception exception)
                {
                    frmExceptionHandler.Handle(exception);
                }
            }
        }

        private void mnuOptionsLanguage_Click(object sender, EventArgs e)
        {
        }

        private void mnuOptionsMacrosPlayMacro_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.macroActions.Clear();
                FileStream serializationStream = null;
                try
                {
                    serializationStream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    IFormatter formatter = new BinaryFormatter();
                    while (serializationStream.Position < serializationStream.Length)
                    {
                        MacroAction action = (MacroAction) formatter.Deserialize(serializationStream);
                        this.macroActions.Add(action);
                    }
                }
                catch (Exception)
                {
                    this.macroActions.Clear();
                    MessageBox.Show("Had problems deserializing from " + dialog.FileName);
                }
                finally
                {
                    if (serializationStream != null)
                    {
                        serializationStream.Close();
                    }
                }
                frmPlayMacro macro = new frmPlayMacro();
                if (macro.ShowDialog(this) == DialogResult.OK)
                {
                    this.playLooping = macro.optContinuously.Checked;
                    this.playIntervalMilliseconds = (long) macro.updInterval.Value;
                    this.mnuOptionsMacrosPlayMacro.Enabled = false;
                    this.mnuOptionsMacrosStopPlaying.Enabled = true;
                    this.macroPlayingOn = true;
                    this.mnuOptionsMacrosRecordMacro.Enabled = false;
                    this.mnuOptionsMacrosStopPlaying.Enabled = true;
                }
            }
        }

        private void mnuOptionsMacrosRecordMacro_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                this.macroFilename = dialog.FileName;
                this.mnuOptionsMacrosRecordMacro.Enabled = false;
                this.mnuOptionsMacroStopRecording.Enabled = true;
                this.mnuOptionsMacrosPlayMacro.Enabled = false;
                this.macroRecordingOn = true;
            }
        }

        private void mnuOptionsMacrosStopPlaying_Click(object sender, EventArgs e)
        {
            this.macroPlayingOn = false;
            this.macroActions = null;
            this.mnuOptionsMacrosStopPlaying.Enabled = false;
            this.mnuOptionsMacrosRecordMacro.Enabled = true;
        }

        private void mnuOptionsMacroStopRecording_Click(object sender, EventArgs e)
        {
            this.macroRecordingOn = false;
            this.mnuOptionsMacroStopRecording.Enabled = false;
            this.mnuOptionsMacrosRecordMacro.Enabled = true;
            this.mnuOptionsMacrosPlayMacro.Enabled = true;
        }

        private void mnuOptionsRenameEntity_Click(object sender, EventArgs e)
        {
            frmInputString str = new frmInputString("Rename " + S.I.EntityName, "Enter new name:", "");
            str.ShowDialog();
            if ((str.Response != null) && (str.Response != ""))
            {
                S.SA.RenameEntity(S.MF.CurrentEntityID, str.Response);
            }
            this.UpdateView();
        }

        private void mnuOptionsRunTo_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning) 
                this.mnuOptionsGoStop.PerformClick(); 
            new frmRunTo().ShowDialog(this);
        }

        protected virtual void mnuOptionsShowMessages_Click(object sender, EventArgs e)
        {
            if (!this.mnuOptionsShowMessages.Checked)
            {
                this.ShowMessageWindow();
            }
            else
            {
                this.HideMessageWindow();
            }
        }

        public void mnuOptionsSlower_Click(object sender, EventArgs e)
        {
            SimState simState = S.I.SimState;
            simState.SpeedIndex--;
            this.OnSpeedChange();
        }

        private void mnuOptionsSoundEffects_Click(object sender, EventArgs e)
        {
            this.mnuOptionsSoundEffects.Checked = !this.mnuOptionsSoundEffects.Checked;
        }

        private void mnuOptionsTestResults_Click(object sender, EventArgs e)
        {
            if (S.I.SimTimeRunning)
            {
                this.mnuOptionsGoStop.PerformClick();
            }
            new frmTestResults().ShowDialog(this);
        }

        protected virtual void mnuOptionsTuning_Click(object sender, EventArgs e)
        {
        }

        private void mnuViewEntity_Click(object sender, EventArgs e)
        {
            this.OnCurrentEntityChange(this.EntityNameToID(((MenuItem) sender).Text));
            this.UpdateView();
        }

        private void mnuViewView_Click(object sender, EventArgs e)
        {
            this.OnViewChange(((MenuItem) sender).Text);
            if (this.SoundOn)
            {
                Sound.PlaySoundFromFile(@"sounds\viewchange.wav");
            }
        }

        public void OnCurrentEntityChange(long entityID)
        {
            this.CurrentEntityID = entityID;
            this.CloseActionForms();
            foreach (MenuItem item in this.mnuView.MenuItems)
            {
                item.Checked = item.Text == this.EntityIDToName(this.CurrentEntityID);
            }
            this.CurrentEntityPanel.Text = this.EntityIDToName(this.CurrentEntityID);
            try
            {
                this.EnableDisable();
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
            }
            MenuItem item2 = Utilities.FindMenuEquivalent(this.mainMenu1, this.CurrentViewName);
            if ((item2 != null) && !item2.Enabled)
            {
                this.OnViewChange(S.I.Views[0].Name);
            }
            if (this.EntityChanged != null)
            {
                this.EntityChanged(this, new EventArgs());
            }
        }

        private void OnSpeedChange()
        {
            this.mnuOptionsFaster.Enabled = S.I.SimState.SpeedIndex < (S.ST.Speeds.Length - 1);
            this.mnuOptionsSlower.Enabled = S.I.SimState.SpeedIndex > 0;
            this.ReenableButtons();
        }

        private void OnStateChanged()
        {
            if (!(S.I.Client || !S.I.Demo))
            {
                S.SS.StopDate = S.SS.StartDate.AddDays((double) S.I.DemoDuration);
            }
            this.Text = Application.ProductName;
            if (this.currentFilePath != null)
            {
                this.Text = Path.GetFileNameWithoutExtension(this.currentFilePath) + " - " + this.Text;
            }
            bool flag = S.I.SimStateAdapter.getMultiplayer();
            if (this.ScoreboardButton != null)
            {
                this.ScoreboardButton.Visible = flag;
            }
            this.mnuFileMultiplayerScoreboard.Enabled = flag;
            this.mnuFileMultiplayerTeamList.Visible = flag && S.I.Host;
            if (flag)
            {
                if (S.I.Client)
                {
                    this.CurrentEntityID = S.SA.GetAnEntityIdForPlayer(S.I.ThisPlayerName);
                }
                else
                {
                    frmStartMultiplayerSession session = new frmStartMultiplayerSession();
                    if (session.ShowDialog() == DialogResult.Cancel)
                    {
                        this.GetCorrectStartChoices().ShowDialog(this);
                        return;
                    }
                    S.I.ThisPlayerName = "";
                    S.ST.RoleBasedMultiplayer = session.chkRequireRoles.Checked;
                }
                string text = this.Text;
                this.Text = text + " Multiplayer " + this.ClientOrHost() + "   Session Name: " + Simulator.Instance.SessionName;
                if (S.I.MultiplayerRoleName != "")
                {
                    this.Text = this.Text + "   Role: " + S.I.MultiplayerRoleName;
                }
            }
            this.OnCurrentEntityChange(this.CurrentEntityID);
            if (!S.I.Client)
            {
                this.OnSpeedChange();
            }
            else
            {
                S.I.SetSimEngineSpeed(new SimSpeed(S.I.UserAdminSettings.ClientDrawStepPeriod, 1));
                this.mnuFileSave.Enabled = false;
                this.mnuFileSaveAs.Enabled = false;
                this.mnuOptionsFaster.Enabled = false;
                this.mnuOptionsSlower.Enabled = false;
                this.mnuOptionsRunTo.Enabled = false;
                this.ReenableButtons();
            }
            if (this.CurrentViewName == "")
            {
                this.CurrentViewName = S.I.Views[0].Name;
            }
            this.OnViewChange(this.CurrentViewName);
            if (this.mnuHelpAssignment.Enabled)
            {
                Form form = this.FindOwnedForm(typeof(frmStartChoices));
                if (form != null)
                {
                    form.Location = new Point(0, 0x7530);
                }
                if (MessageBox.Show("Do you want to view or print your assignment?", "View Assignment", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.mnuHelpAssignment.PerformClick();
                }
            }
            this.RegisterClientEventHandlers();
            this.OnStateChangedHook();
        }

        protected virtual void OnStateChangedHook()
        {
        }

        public void OnViewChange(string viewName)
        {
            this.CloseActionForms();
            KMI.Sim.View.ClearCurrentHit();
            this.CurrentViewName = viewName;
            this.picMain.Visible = false;
            this.backBuffer = new Bitmap(this.currentView.Size.Width, this.currentView.Size.Height, this.picMain.CreateGraphics());
            this.backBufferGraphics = Graphics.FromImage(this.backBuffer);
            this.picMain.Size = this.currentView.Size;
            this.frmMainBase_Resize(new object(), new EventArgs());
            this.UpdateView();
            this.picMain.Visible = true;
        }

        public virtual void OpenSavedSim(string filepath)
        {
            this.Cursor = Cursors.WaitCursor;
            this.ReInit();
            try
            {
                S.I.LoadState(filepath);
                this.currentViewName = S.ST.SavedViewName;
                this.currentEntityID = S.ST.SavedEntityID;
                S.ST.SpeedIndex = S.ST.SpeedIndex;
                this.LoadStateHook();
            }
            catch (Exception exception)
            {
                MessageBox.Show(S.R.GetString("An error occurred while opening the file. Check that the file is a valid {0} (.{1}) file. If the error continues, the file may have been corrupted.", new string[] { Application.ProductName, Simulator.Instance.DataFileTypeExtension }) + "\r\n\r\n" + S.R.GetString("Error details") + ": " + exception.Message, S.R.GetString("Error Opening File"));
                this.GetCorrectStartChoices().ShowDialog(this);
                return;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
            this.currentFilePath = filepath;
            if (S.SS.StudentOrg > 0)
            {
                MessageBox.Show("You have opened a Virtual Business Challenge file. The Instructor's Area will be disabled.", "Virtual Business Challenge");
            }
            else if (S.I.VBC)
            {
                MessageBox.Show(S.R.GetString("The file you are trying to load was not built for this Edition. It can be loaded only by the classroom version."));
                this.GetCorrectStartChoices().ShowDialog(this);
            }
            this.OnStateChanged();
        }

        public virtual bool OptOutModalMessageHook(ModalMessage message)
        {
            return false;
        }

        private void picMain_Click(object sender, EventArgs e)
        {
            this.currentView.View_Click(sender, e);
        }

        private void picMain_DoubleClick(object sender, EventArgs e)
        {
            this.currentView.View_DoubleClick(sender, e);
        }

        private void picMain_MouseDown(object sender, MouseEventArgs e)
        {
            this.currentView.View_MouseDown(sender, e);
        }

        private void picMain_MouseMove(object sender, MouseEventArgs e)
        {
            this.currentView.UpdateCurrentHit(e);
            this.currentView.View_MouseMove(sender, e);
        }

        private void picMain_MouseUp(object sender, MouseEventArgs e)
        {
            this.currentView.View_MouseUp(sender, e);
        }

        private void picMain_Paint(object sender, PaintEventArgs e)
        {
            if (!(base.DesignMode || (this.backBuffer == null)))
            {
                e.Graphics.DrawImageUnscaled(this.backBuffer, 0, 0);
            }
        }

        public void PlayMacroAction()
        {
            if (this.macroPlayingOn)
            {
                MacroAction action;
                if (this.playLooping)
                {
                    if (this.currentMacroActionIndex >= this.macroActions.Count)
                    {
                        this.currentMacroActionIndex = 0;
                    }
                    action = (MacroAction) this.macroActions[this.currentMacroActionIndex];
                    if (this.nextMacroPlayTime < DateTime.Now)
                    {
                        action.Method.Invoke(S.SA, action.ArgumentValues);
                        this.nextMacroPlayTime = this.nextMacroPlayTime.AddMilliseconds((double) this.playIntervalMilliseconds);
                        this.currentMacroActionIndex++;
                    }
                }
                else if (!S.I.Client)
                {
                    while (this.currentMacroActionIndex < this.macroActions.Count)
                    {
                        action = (MacroAction) this.macroActions[this.currentMacroActionIndex];
                        if (action.Timestamp >= S.ST.Now)
                        {
                            break;
                        }
                        action.Method.Invoke(S.SA, action.ArgumentValues);
                        this.currentMacroActionIndex++;
                    }
                }
            }
        }

        private bool QuerySave()
        {
            if (S.I.Demo)
            {
                return true;
            }
            if (!(this.dirtySimState && !S.I.Client))
            {
                return true;
            }
            switch (MessageBox.Show(S.I.Resource.GetString("MsgQuerySave"), Application.ProductName, MessageBoxButtons.YesNoCancel))
            {
                case DialogResult.Yes:
                    this.mnuFileSave_Click(new object(), new EventArgs());
                    return !this.dirtySimState;

                case DialogResult.No:
                    this.dirtySimState = false;
                    return true;

                case DialogResult.Cancel:
                    return false;
            }
            return false;
        }

        private void ReenableButtons()
        {
            foreach (ToolBarButton button in this.tlbMain.Buttons)
            {
                MenuItem item = Utilities.FindMenuEquivalent(this.mainMenu1, button.Text);
                if (item != null)
                {
                    button.Enabled = item.Enabled;
                }
            }
        }

        protected virtual void RegisterClientEventHandlers()
        {
            S.SA.PlaySoundEvent += new PlaySoundDelegate(ClientEventHandlers.Instance.PlaySoundHandler);
            S.SA.PlayerMessageEvent += new PlayerMessageDelegate(ClientEventHandlers.Instance.PlayerMessageHandler);
            S.SA.ModalMessageEvent += new ModalMessageDelegate(ClientEventHandlers.Instance.ModalMessageHandler);
        }

        private void ReInit()
        {
            this.CloseOwnedForms();
            if (S.SA != null)
            {
                this.UnregisterClientEventHandlers();
            }
            if ((this.DesignerMode && !S.I.Client) && (S.I.SimState != null))
            {
                this.cacheDesignerSimSettings = S.I.SimState.SimSettings;
            }
            S.I.SimState = null;
            S.I.PeriodicMessageTable.Clear();
            try
            {
                if (!((S.SA == null) || S.I.Client))
                {
                    RemotingServices.Disconnect(S.SA);
                }
            }
            catch
            {
                throw new Exception("The state of the SimStateAdapter and the Client flag are somehow out of sync.");
            }
            S.I.Client = false;
            S.I.MultiplayerRoleName = "";
            this.CreateMessagePanel.Width = 0;
            S.I.SessionName = "";
            S.I.SimStateAdapter = S.I.SimFactory.CreateSimStateAdapter();
            S.I.ThisPlayerName = "";
            this.mnuFileSave.Enabled = true;
            this.mnuFileSaveAs.Enabled = true;
            this.mnuOptionsGoStop.Enabled = true;
            this.mnuOptionsFaster.Enabled = true;
            this.mnuOptionsSlower.Enabled = true;
            this.mnuOptionsIA.Enabled = true;
            this.mnuOptionsRunTo.Enabled = true;
            this.EnableMenuAndSubMenus(this.mnuReports);
            this.EnableMenuAndSubMenus(this.mnuActions);
            this.lessonLoadedPromptSaveAs = false;
            this.currentWeek = -1;
            this.Now = DateNotSet;
            for (int i = this.mnuView.MenuItems.Count - 1; (i >= 0) && (this.mnuView.MenuItems[i].Text != "-"); i--)
            {
                this.mnuView.MenuItems.RemoveAt(i);
            }
            this.CurrentViewName = S.I.Views[0].Name;
            this.CurrentEntityID = -1L;
            this.NewMessagesPanel.Width = 0;
            if (S.I.Messages)
            {
                this.messagesForm = new frmMessages();
                this.messagesForm.Controller.Add(this);
                this.messagesForm.Owner = this;
                this.ShowMessageWindow();
            }
        }

        public void SaveMacroAction(MacroAction action)
        {
            if (this.macroRecordingOn)
            {
                FileStream serializationStream = null;
                serializationStream = new FileStream(this.macroFilename, FileMode.Append, FileAccess.Write, FileShare.None);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(serializationStream, action);
                serializationStream.Close();
            }
        }

        private void SaveSim(string filename)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                S.ST.SavedEntityID = this.CurrentEntityID;
                S.ST.SavedViewName = this.CurrentViewName;
                S.I.SaveState(filename);
                this.dirtySimState = false;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        protected virtual void ServerStartHook()
        {
        }

        public void SetDirty()
        {
            this.dirtySimState = true;
        }

        protected void ShowMessageWindow()
        {
            this.mnuOptionsShowMessages.Checked = true;
            this.messagesForm.Show();
            this.messagesForm.Location = base.PointToScreen(new Point(0, this.staMain.Top - this.messagesForm.Height));
            this.NewMessagesPanel.Width = 0;
        }

        public void ShowModalMessage(ModalMessage message)
        {
            bool flag = false;
            if (!(!S.I.SimTimeRunning || S.I.Multiplayer))
            {
                flag = true;
                this.mnuOptionsGoStop_Click(new object(), new EventArgs());
            }
            if (message is RunToDateReachedMessage)
            {
                this.UpdateView();
                this.SynchGoStop();
            }
            else if (message is StopDateReachedMessage)
            {
                this.UpdateView();
                new frmUpload().ShowDialog(S.MF);
                this.SynchGoStop();
            }
            else
            {
                if (message is ShowPageMessage)
                {
                    new frmPage(((ShowPageMessage) message).Page).ShowDialog();
                }
                else if (message is LevelEndTestMessage)
                {
                    LevelEndTestMessage m = (LevelEndTestMessage) message;
                    AcademicGod.HandleLevelEnd(m);
                    if (m.LastLevel)
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(message.Message, message.Title, MessageBoxButtons.OK, message.Icon);
                    if (message is GameOverMessage)
                    {
                        if (!S.I.Client)
                        {
                            this.SynchGoStop();
                        }
                        this.DirtySimState = false;
                        this.GetCorrectStartChoices().ShowDialog(this);
                        return;
                    }
                }
                if (flag)
                {
                    this.mnuOptionsGoStop_Click(new object(), new EventArgs());
                }
            }
        }

        protected void ShowNonModalForm(Form frm)
        {
            Form form = this.FindOwnedForm(frm.GetType());
            if (form == null)
            {
                frm.Owner = this;
                if (this.CanShowForm(frm))
                {
                    frm.Show();
                }
            }
            else
            {
                form.Focus();
            }
        }

        private void staMain_PanelClick(object sender, StatusBarPanelClickEventArgs e)
        {
            if (e.StatusBarPanel == this.NewMessagesPanel)
            {
                this.ShowMessageWindow();
            }
            if (e.StatusBarPanel == this.CreateMessagePanel)
            {
                new frmCreateMessage().ShowDialog(this);
            }
        }

        protected void StartMusic()
        {
            this.backgroundMusicTimer.Stop();
            this.backgroundMusicTimer.Interval = S.I.BackgroundMusicLength;
            Sound.PlayMidiFromFile(@"sounds\Background.Mid");
            this.backgroundMusicTimer.Start();
        }

        public void StopMusic()
        {
            Sound.StopMidi();
            this.backgroundMusicTimer.Stop();
        }

        protected internal void StopSimulation()
        {
            if (S.I.SimTimeRunning)
            {
                S.MF.mnuOptionsGoStop.PerformClick();
            }
        }

        public void SynchGoStop()
        {
            ToolBarButton button;
            if (!S.I.SimTimeRunning && this.mnuOptionsGoStop.Text.EndsWith(S.I.Resource.GetString("Stop"))) {
                button = Utilities.FindButtonEquivalent(this.tlbMain, this.mnuOptionsGoStop.Text);
                this.mnuOptionsGoStop.Text = S.I.Resource.GetString("Go");
                if (button != null) {
                    button.Text = this.mnuOptionsGoStop.Text;
                    button.ImageIndex--;
                }
                this.StopMusic();
            }
            if (S.I.SimTimeRunning && this.mnuOptionsGoStop.Text.EndsWith(S.I.Resource.GetString("Go"))) {
                button = Utilities.FindButtonEquivalent(this.tlbMain, this.mnuOptionsGoStop.Text);
                this.mnuOptionsGoStop.Text = S.I.Resource.GetString("Stop");
                if (button != null) {
                    button.Text = this.mnuOptionsGoStop.Text;
                    button.ImageIndex++;
                }
                if (this.MusicOn) {
                    this.StartMusic();
                }
            }
        }

        private void tlbMain_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            MenuItem item = Utilities.FindMenuEquivalent(this.mainMenu1, e.Button.Text);
            if (item == null)
            {
                throw new Exception("A main toolbar button click was unhandled. Check the toolbar buttons text property.");
            }
            item.PerformClick();
        }

        protected virtual void UnregisterClientEventHandlers()
        {
            try
            {
                S.SA.PlaySoundEvent -= new PlaySoundDelegate(ClientEventHandlers.Instance.PlaySoundHandler);
                S.SA.PlayerMessageEvent -= new PlayerMessageDelegate(ClientEventHandlers.Instance.PlayerMessageHandler);
                S.SA.ModalMessageEvent -= new ModalMessageDelegate(ClientEventHandlers.Instance.ModalMessageHandler);
            }
            catch (Exception)
            {
            }
        }

        protected virtual void UpdateStatusBar(SimStateAdapter.ViewUpdate viewUpdate)
        {
            this.Now = viewUpdate.Now;
            this.CurrentWeek = viewUpdate.CurrentWeek;
            this.EntityCriticalResourcePanel.Text = Utilities.FC(viewUpdate.Cash, S.I.CurrencyConversion);
            if (!(S.I.Client || !S.SS.LevelManagementOn))
            {
                this.Level.Text = S.R.GetString("Level {0}", new object[] { S.SS.Level.ToString() });
            }
            else
            {
                this.Level.Text = "";
            }
        }

        public void UpdateView()
        {
            SimStateAdapter.ViewUpdate viewUpdate = null;
            try
            {
                viewUpdate = S.SA.GetViewUpdate(this.CurrentViewName, this.CurrentEntityID, this.currentView.ViewerOptions);
            }
            catch (EntityNotFoundException exception)
            {
                this.OnCurrentEntityChange(exception.ExistingEntityID);
                this.UpdateView();
                return;
            }
            catch (Exception exception2)
            {
                frmExceptionHandler.Handle(exception2);
                return;
            }
            this.FireNewTimeEvents(viewUpdate);
            this.UpdateStatusBar(viewUpdate);
            this.UpdateViewMenu(viewUpdate);
            this.backBufferGraphics.Clear(Color.White);
            this.currentView.Drawables = viewUpdate.Drawables;
            this.currentView.Draw(this.backBufferGraphics);
            this.picMain.Refresh();
        }

        protected virtual void UpdateViewMenu(SimStateAdapter.ViewUpdate viewUpdate)
        {
            this.entityNames = viewUpdate.EntityNames;
            bool flag = false;
            int index = this.mnuView.MenuItems.Count - 1;
            while ((index >= 0) && (this.mnuView.MenuItems[index].Text != "-"))
            {
                index--;
            }
            int num2 = index + 1;
            if ((this.mnuView.MenuItems.Count - num2) != viewUpdate.EntityNames.Count)
            {
                flag = true;
            }
            else
            {
                int num3 = 0;
                foreach (string str in viewUpdate.EntityNames.Values)
                {
                    if (this.mnuView.MenuItems[num2 + num3++].Text != str)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (flag)
            {
                for (index = this.mnuView.MenuItems.Count - 1; (index >= 0) && (this.mnuView.MenuItems[index].Text != "-"); index--)
                {
                    this.mnuView.MenuItems.RemoveAt(index);
                }
                foreach (string str2 in viewUpdate.EntityNames.Values)
                {
                    MenuItem item = new MenuItem(str2, new EventHandler(this.mnuViewEntity_Click));
                    this.mnuView.MenuItems.Add(item);
                    item.Checked = item.Text == this.EntityIDToName(this.CurrentEntityID);
                }
                this.CurrentEntityPanel.Text = this.EntityIDToName(this.CurrentEntityID);
                if ((this.CurrentEntityID == -1L) && (viewUpdate.EntityNames.Count > 0))
                {
                    foreach (long num4 in viewUpdate.EntityNames.Keys)
                    {
                        this.OnCurrentEntityChange(num4);
                        break;
                    }
                }
            }
        }

        private void View_PrintPage(object sender, PrintPageEventArgs e)
        {
            try
            {
                Utilities.ResetFPU();
                Font font = new Font("Arial", 14f);
                string text = "";
                if (this.printStudentName != "")
                {
                    text = text + "Student Name: " + this.printStudentName + "\r\n\r\n";
                }
                text = text + this.CurrentViewName + " View";
                if (S.I.EntityName != "")
                {
                    string str2 = text;
                    text = str2 + "\r\nCurrent " + S.I.EntityName + ": " + this.EntityIDToName(this.CurrentEntityID);
                }
                int num2 = (int) (e.Graphics.MeasureString(text, font).Height * 1.5f);
                Rectangle marginBounds = e.MarginBounds;
                Rectangle rectangle2 = new Rectangle(marginBounds.Left, marginBounds.Top + num2, marginBounds.Width, marginBounds.Height - num2);
                float sx = Math.Min((float) (((float) rectangle2.Width) / ((float) this.picMain.Width)), (float) (((float) rectangle2.Height) / ((float) this.picMain.Height)));
                float dx = rectangle2.Left + ((rectangle2.Width - (this.picMain.Width * sx)) / 2f);
                float dy = rectangle2.Top + ((rectangle2.Height - (this.picMain.Height * sx)) / 2f);
                e.Graphics.TranslateTransform(dx, dy);
                e.Graphics.ScaleTransform(sx, sx);
                e.Graphics.DrawImageUnscaled(this.backBuffer, 0, 0);
                e.Graphics.ResetTransform();
                e.Graphics.DrawString(text, font, new SolidBrush(Color.Black), (float) e.MarginBounds.Left, (float) e.MarginBounds.Top);
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
            }
        }

        public Graphics BackBufferGraphics
        {
            get
            {
                return this.backBufferGraphics;
            }
        }

        public long CurrentEntityID
        {
            get
            {
                return this.currentEntityID;
            }
            set
            {
                this.currentEntityID = value;
            }
        }

        public string CurrentViewName
        {
            get
            {
                return this.currentViewName;
            }
            set
            {
                this.currentViewName = value;
                this.currentView = S.I.View(this.currentViewName);
            }
        }

        public int CurrentWeek
        {
            get
            {
                return this.currentWeek;
            }
            set
            {
                this.currentWeek = value;
            }
        }

        public bool DesignerMode
        {
            get
            {
                return this.designerMode;
            }
            set
            {
                this.designerMode = value;
                this.mnuOptionsTuning.Visible = this.designerMode;
                this.mnuOptionsChangeOwner.Visible = this.designerMode;
                this.mnuOptionsMacros.Visible = this.designerMode;
                this.mnuOptionsRenameEntity.Visible = this.designerMode;
            }
        }

        public bool DirtySimState
        {
            get
            {
                return this.dirtySimState;
            }
            set
            {
                this.dirtySimState = value;
            }
        }

        public static frmMainBase Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsWin98
        {
            get
            {
                return this.isWin98;
            }
        }

        public Rectangle MainWindowBounds
        {
            get
            {
                return new Rectangle(0, 0, this.picMain.Width, this.picMain.Height);
            }
        }

        public bool MusicOn
        {
            get
            {
                return this.mnuOptionsBackgroundMusic.Checked;
            }
        }

        public DateTime Now
        {
            get
            {
                return this.now;
            }
            set
            {
                this.now = value;
                if (this.now != DateNotSet)
                {
                    this.TimePanel.Text = this.now.ToShortTimeString();
                    this.DayOfWeekPanel.Text = this.now.ToString("ddd");
                    this.DatePanel.Text = this.now.ToString("MMMM dd, yyyy");
                }
            }
        }

        public bool SoundOn
        {
            get
            {
                return this.mnuOptionsSoundEffects.Checked;
            }
        }

        public ToolTip ViewToolTip
        {
            get
            {
                return this.viewToolTip;
            }
        }

        public class MenuItemEnabledNoEntities : MenuItem
        {
        }
    }
}

