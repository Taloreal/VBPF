namespace KMI.VBPF1Lib
{
    #region References
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;
    #endregion

    public class frmDailyRoutine : Form
    {
        #region Controls and variables

        private LinkLabel AddEat;
        private LinkLabel AddRelaxation;
        private LinkLabel AddSleep;
        private LinkLabel AddWorkout;
        private Button btnChangeTravel;
        private Button btnClose;
        private Button btnHelp;
        private Button btnHostAParty;
        private CheckedListBox chkEvents;
        private Container components = null;
        private DailyRoutine[] dailyRoutines;
        private const int hrSpacing = 7;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private LinkLabel linkLabel4;
        private Panel panel2;
        private Panel[] panMains;
        private Panel panMainWD;
        private Panel panMainWE;
        private Panel panSpecialEvents;
        private LinkLabel CopyWeekdays;
        private LinkLabel CopyWeekends;
        private AppSimSettings simSettings;

        #endregion

        #region Initilization

        public frmDailyRoutine()
        {
            this.InitializeComponent();
            this.DrawSchedule(this.panMainWD);
            this.DrawSchedule(this.panMainWE);
            this.panMains = new Panel[] { this.panMainWD, this.panMainWE };
            this.simSettings = (AppSimSettings) A.SA.getSimSettings();
            this.panSpecialEvents.Visible = this.simSettings.HealthFactorsToConsider > 4;
            if (this.simSettings.ScheduleReadOnly)
            {
                foreach (Control control in base.Controls)
                {
                    if (control is LinkLabel)
                    {
                        control.Enabled = false;
                    }
                }
            }
            this.UpdateForm();
            this.btnChangeTravel.Enabled = A.MF.mnuActionsLivingTransportation.Enabled;
        }

        private void InitializeComponent()
        {
            this.panMainWD = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.AddWorkout = new System.Windows.Forms.LinkLabel();
            this.AddRelaxation = new System.Windows.Forms.LinkLabel();
            this.AddSleep = new System.Windows.Forms.LinkLabel();
            this.AddEat = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnChangeTravel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHelp = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.panMainWE = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnHostAParty = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.chkEvents = new System.Windows.Forms.CheckedListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.panSpecialEvents = new System.Windows.Forms.Panel();
            this.CopyWeekdays = new System.Windows.Forms.LinkLabel();
            this.CopyWeekends = new System.Windows.Forms.LinkLabel();
            this.panel2.SuspendLayout();
            this.panSpecialEvents.SuspendLayout();
            this.SuspendLayout();
            // 
            // panMainWD
            // 
            this.panMainWD.BackColor = System.Drawing.Color.White;
            this.panMainWD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMainWD.Location = new System.Drawing.Point(64, 48);
            this.panMainWD.Name = "panMainWD";
            this.panMainWD.Size = new System.Drawing.Size(72, 336);
            this.panMainWD.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(464, 16);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(152, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "Add time to:";
            // 
            // AddWorkout
            // 
            this.AddWorkout.Location = new System.Drawing.Point(152, 152);
            this.AddWorkout.Name = "AddWorkout";
            this.AddWorkout.Size = new System.Drawing.Size(56, 16);
            this.AddWorkout.TabIndex = 5;
            this.AddWorkout.TabStop = true;
            this.AddWorkout.Tag = "false";
            this.AddWorkout.Text = "Exercise";
            this.AddWorkout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddWorkout_LinkClicked);
            // 
            // AddRelaxation
            // 
            this.AddRelaxation.Location = new System.Drawing.Point(152, 128);
            this.AddRelaxation.Name = "AddRelaxation";
            this.AddRelaxation.Size = new System.Drawing.Size(56, 16);
            this.AddRelaxation.TabIndex = 6;
            this.AddRelaxation.TabStop = true;
            this.AddRelaxation.Tag = "false";
            this.AddRelaxation.Text = "Relax";
            this.AddRelaxation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddRelaxation_LinkClicked);
            // 
            // AddSleep
            // 
            this.AddSleep.Location = new System.Drawing.Point(152, 176);
            this.AddSleep.Name = "AddSleep";
            this.AddSleep.Size = new System.Drawing.Size(56, 16);
            this.AddSleep.TabIndex = 7;
            this.AddSleep.TabStop = true;
            this.AddSleep.Tag = "false";
            this.AddSleep.Text = "Sleep";
            this.AddSleep.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddSleep_LinkClicked);
            // 
            // AddEat
            // 
            this.AddEat.Location = new System.Drawing.Point(152, 104);
            this.AddEat.Name = "AddEat";
            this.AddEat.Size = new System.Drawing.Size(56, 16);
            this.AddEat.TabIndex = 11;
            this.AddEat.TabStop = true;
            this.AddEat.Tag = "false";
            this.AddEat.Text = "Eat";
            this.AddEat.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddEat_LinkClicked);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Violet;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(200, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 24);
            this.label2.TabIndex = 12;
            this.label2.Text = "Est. Travel Time";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 32);
            this.label3.TabIndex = 13;
            this.label3.Text = "To edit or delete an existing activity, click it.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // btnChangeTravel
            // 
            this.btnChangeTravel.Location = new System.Drawing.Point(312, 16);
            this.btnChangeTravel.Name = "btnChangeTravel";
            this.btnChangeTravel.Size = new System.Drawing.Size(128, 24);
            this.btnChangeTravel.TabIndex = 14;
            this.btnChangeTravel.Text = "Change Travel Mode";
            this.btnChangeTravel.Click += new System.EventHandler(this.btnChangeTravel_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnHelp);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnChangeTravel);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Location = new System.Drawing.Point(0, 400);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(632, 56);
            this.panel2.TabIndex = 21;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(552, 16);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(64, 24);
            this.btnHelp.TabIndex = 15;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(208, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(1, 400);
            this.label5.TabIndex = 22;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label4.Location = new System.Drawing.Point(0, 400);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(640, 1);
            this.label4.TabIndex = 23;
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(424, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(1, 400);
            this.label6.TabIndex = 30;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(368, 104);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(56, 16);
            this.linkLabel1.TabIndex = 29;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Tag = "true";
            this.linkLabel1.Text = "Eat";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddEat_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.Location = new System.Drawing.Point(368, 176);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(56, 16);
            this.linkLabel2.TabIndex = 28;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Tag = "true";
            this.linkLabel2.Text = "Sleep";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddSleep_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.Location = new System.Drawing.Point(368, 128);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(56, 16);
            this.linkLabel3.TabIndex = 27;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Tag = "true";
            this.linkLabel3.Text = "Relax";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddRelaxation_LinkClicked);
            // 
            // linkLabel4
            // 
            this.linkLabel4.Location = new System.Drawing.Point(368, 152);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(56, 16);
            this.linkLabel4.TabIndex = 26;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Tag = "true";
            this.linkLabel4.Text = "Exercise";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AddWorkout_LinkClicked);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(368, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 32);
            this.label7.TabIndex = 25;
            this.label7.Text = "Add time to:";
            // 
            // panMainWE
            // 
            this.panMainWE.BackColor = System.Drawing.Color.White;
            this.panMainWE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMainWE.Location = new System.Drawing.Point(280, 48);
            this.panMainWE.Name = "panMainWE";
            this.panMainWE.Size = new System.Drawing.Size(72, 336);
            this.panMainWE.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label8.Location = new System.Drawing.Point(8, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(184, 24);
            this.label8.TabIndex = 31;
            this.label8.Text = "Weekday";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label9.Location = new System.Drawing.Point(224, 8);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(184, 24);
            this.label9.TabIndex = 32;
            this.label9.Text = "Weekend";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label10.Location = new System.Drawing.Point(8, 8);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(184, 24);
            this.label10.TabIndex = 33;
            this.label10.Text = "Special Events";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnHostAParty
            // 
            this.btnHostAParty.Location = new System.Drawing.Point(16, 40);
            this.btnHostAParty.Name = "btnHostAParty";
            this.btnHostAParty.Size = new System.Drawing.Size(160, 24);
            this.btnHostAParty.TabIndex = 34;
            this.btnHostAParty.Text = "Host a Party";
            this.btnHostAParty.Click += new System.EventHandler(this.btnHostAParty_Click);
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(16, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 16);
            this.label11.TabIndex = 35;
            this.label11.Text = "Attend Events:";
            // 
            // chkEvents
            // 
            this.chkEvents.CheckOnClick = true;
            this.chkEvents.HorizontalScrollbar = true;
            this.chkEvents.Location = new System.Drawing.Point(16, 104);
            this.chkEvents.Name = "chkEvents";
            this.chkEvents.Size = new System.Drawing.Size(160, 199);
            this.chkEvents.TabIndex = 36;
            this.chkEvents.SelectedValueChanged += new System.EventHandler(this.chkEvents_SelectedValueChanged);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(32, 312);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(128, 16);
            this.label12.TabIndex = 37;
            this.label12.Text = "Check the box to attend.";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(17, 328);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(168, 64);
            this.label13.TabIndex = 38;
            this.label13.Text = "If the special event begins during a scheduled activity, you will complete that a" +
                "ctivity first then go to the event. If the event has ended by that time, you wil" +
                "l not get credit for attending.";
            // 
            // panSpecialEvents
            // 
            this.panSpecialEvents.Controls.Add(this.label10);
            this.panSpecialEvents.Controls.Add(this.btnHostAParty);
            this.panSpecialEvents.Controls.Add(this.label11);
            this.panSpecialEvents.Controls.Add(this.chkEvents);
            this.panSpecialEvents.Controls.Add(this.label12);
            this.panSpecialEvents.Controls.Add(this.label13);
            this.panSpecialEvents.Location = new System.Drawing.Point(424, 0);
            this.panSpecialEvents.Name = "panSpecialEvents";
            this.panSpecialEvents.Size = new System.Drawing.Size(200, 400);
            this.panSpecialEvents.TabIndex = 39;
            // 
            // CopyWeekdays
            // 
            this.CopyWeekdays.Location = new System.Drawing.Point(142, 351);
            this.CopyWeekdays.Name = "CopyWeekdays";
            this.CopyWeekdays.Size = new System.Drawing.Size(60, 33);
            this.CopyWeekdays.TabIndex = 40;
            this.CopyWeekdays.TabStop = true;
            this.CopyWeekdays.Tag = "false";
            this.CopyWeekdays.Text = "Copy To Weekend";
            this.CopyWeekdays.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CopyWeekdays.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyWeekdays_LinkClicked);
            // 
            // CopyWeekends
            // 
            this.CopyWeekends.Location = new System.Drawing.Point(215, 351);
            this.CopyWeekends.Name = "CopyWeekends";
            this.CopyWeekends.Size = new System.Drawing.Size(60, 33);
            this.CopyWeekends.TabIndex = 41;
            this.CopyWeekends.TabStop = true;
            this.CopyWeekends.Tag = "false";
            this.CopyWeekends.Text = "Copy To Weekday";
            this.CopyWeekends.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CopyWeekends.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CopyWeekends_LinkClicked);
            // 
            // frmDailyRoutine
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(634, 451);
            this.Controls.Add(this.CopyWeekends);
            this.Controls.Add(this.CopyWeekdays);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.linkLabel3);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panMainWE);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.AddEat);
            this.Controls.Add(this.AddSleep);
            this.Controls.Add(this.AddRelaxation);
            this.Controls.Add(this.AddWorkout);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panMainWD);
            this.Controls.Add(this.panSpecialEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmDailyRoutine";
            this.ShowInTaskbar = false;
            this.Text = "Schedule";
            this.panel2.ResumeLayout(false);
            this.panSpecialEvents.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        #region Control functions

        private void AddEat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.AddTask(new Eat(), bool.Parse((string) ((Control) sender).Tag));
        }

        private void AddRelaxation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.AddTask(new Relax(), bool.Parse((string) ((Control) sender).Tag));
        }

        private void AddSleep_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.AddTask(new Sleep(), bool.Parse((string) ((Control) sender).Tag));
        }

        private void AddTask(Task task, bool weekend)
        {
            try
            {
                new frmChangeTask(task, false, weekend).ShowDialog(this);
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
            }
        }

        private void AddWorkout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.AddTask(new Exercise(), bool.Parse((string) ((Control) sender).Tag));
        }

        private void btnChangeTravel_Click(object sender, EventArgs e)
        {
            A.MF.mnuActionsLivingTransportation.PerformClick();
            this.UpdateForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(this.Text);
        }

        private void btnHostAParty_Click(object sender, EventArgs e)
        {
            new frmChooseEventTime().ShowDialog(this);
            this.UpdateForm();
        }

        private void chkEvents_SelectedValueChanged(object sender, EventArgs e)
        {
            ArrayList eventIDs = new ArrayList();
            foreach (Task task in this.chkEvents.CheckedItems)
            {
                eventIDs.Add(task.ID);
            }
            A.SA.SetOneTimeEvents(A.MF.CurrentEntityID, eventIDs);
            this.UpdateForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected void DrawSchedule(Panel p)
        {
            for (int i = 0; i < 0x30; i++) {
                Label label = new Label();
                DateTime time = new DateTime(1, 1, 1);
                label.Text = time.AddHours((double) (((float) i) / 2f)).ToShortTimeString();
                label.Location = new Point(p.Left - 0x30, 0x2d + (i * 7));
                label.Font = new Font("Arial", 7f);
                label.Size = new Size(0x2e, 16);
                label.TextAlign = ContentAlignment.TopRight;
                label.ForeColor = Color.FromArgb(110, 110, 110);
                Label label2 = new Label {
                    BackColor = Color.LightGray
                };
                if ((i % 2) == 0) {
                    label2.BackColor = Color.DarkGray;
                }
                label2.Location = new Point(0, (label.Top - p.Top) + 2);
                label2.Size = new Size(p.Width, 1);
                p.Controls.Add(label2);
                label2.Enabled = false;
                if ((i % 2) == 0)
                {
                    base.Controls.Add(label);
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void Task_Click(object sender, EventArgs e)
        {
            if (this.simSettings.ScheduleReadOnly)
            {
                MessageBox.Show("You can view but not change your schedule in this lesson.", "Changes Disabled");
            }
            else
            {
                try
                {
                    Control control = (Control) sender;
                    Task tag = (Task) control.Tag;
                    new frmChangeTask(tag, true, tag.Weekend).ShowDialog(this);
                }
                catch (Exception exception)
                {
                    frmExceptionHandler.Handle(exception);
                }
            }
        }

        #endregion

        #region Form Update Methods

        /// <summary>
        /// Updates the forms information about tasks to be done.
        /// </summary>
        public void UpdateForm()
        {
            this.dailyRoutines = A.SA.GetDailyRoutines(A.MF.CurrentEntityID);
            for (int i = 0; i < 2; i++)
            {
                RemoveOld(i);
                AddTasks(i);
                TaskStyling(i);
            }
            UpdateParties();
        }

        /// <summary>
        /// Removes the old schedule.
        /// </summary>
        /// <param name="i">Indicates weekday or weekend.</param>
        public void RemoveOld(int i)
        {
            ArrayList list = new ArrayList(this.panMains[i].Controls);
            foreach (Control control in list)
                if (control.Tag != null)
                    this.panMains[i].Controls.Remove(control);
        }

        /// <summary>
        /// Adds the specific tasks to one of the daily routines.
        /// </summary>
        /// <param name="i">Determines if weekday or weekend.</param>
        public void AddTasks(int i)
        {
            IEnumerator enumerator = this.dailyRoutines[i].Tasks.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Task current = (Task)enumerator.Current;
                Label label = new Label {
                    Location = new Point(-1, ((43 + (current.StartPeriod * 7)) - this.panMains[i].Top) + 4),
                    AutoSize = false, Tag = current
                };
                this.panMains[i].Controls.Add(label);
                label.Size = new Size(this.panMains[i].Width - 12, current.Duration * 7);
                if (current.StartPeriod > current.EndPeriod)
                {
                    label.Size = new Size(this.panMains[i].Width - 12, (0x30 - current.StartPeriod) * 7);
                    Label label2 = new Label
                    {
                        Tag = label.Tag, AutoSize = false,
                        Location = new Point(-1, (0x2b - this.panMains[i].Top) + 4)
                    };
                    this.panMains[i].Controls.Add(label2);
                    label2.Size = new Size(this.panMains[i].Width - 12, current.EndPeriod * 7);
                }
            }
        }

        /// <summary>
        /// Styles the tasks in the daily routines.
        /// </summary>
        /// <param name="i">Determines if weekday or weekend.</param>
        public void TaskStyling(int i)
        {
            foreach (Control control in this.panMains[i].Controls)
            {
                if (control.Tag != null) {
                    Task current = (Task)control.Tag;
                    Label label3 = (Label)control;
                    label3.BackColor = current.GetColor();
                    label3.BorderStyle = BorderStyle.FixedSingle;
                    label3.BringToFront();
                    if (current is TravelTask) {
                        label3.Width = 12;
                        label3.Left = this.panMains[i].Width - 12;
                    }
                    else {
                        label3.Click += new EventHandler(this.Task_Click);
                        label3.Cursor = Cursors.Hand;
                        label3.TextAlign = ContentAlignment.MiddleCenter;
                        label3.Text = current.CategoryName();
                    }
                }
            }
        }

        /// <summary>
        /// Updates the information on available parties.
        /// </summary>
        public void UpdateParties()
        {
            SortedList oneTimeEventsInvitedTo = A.SA.GetOneTimeEventsInvitedTo(A.MF.CurrentEntityID);
            SortedList oneTimeEventsAttending = A.SA.GetOneTimeEventsAttending(A.MF.CurrentEntityID);
            int index = 0;
            this.chkEvents.Items.Clear();
            foreach (OneTimeEvent event2 in oneTimeEventsInvitedTo.Values)
            {
                this.chkEvents.Items.Add(event2);
                if (oneTimeEventsAttending.ContainsKey(event2.Key))
                {
                    this.chkEvents.SetItemChecked(index, true);
                }
                index++;
            }
        }

        #endregion

        #region Copy

        public void Transition(int From)
        {
            DailyRoutine ToCopy = dailyRoutines[From];
            List<Task> tasks = new List<Task>();
            foreach (Task T in ToCopy.Tasks.Values)
                if (CanCopy(T, ((From + 1) % 2)))
                    A.SA.AddTask(A.MF.CurrentEntityID, GenerateCopy(T));
        }

        public Task GenerateCopy(Task T)
        {
            List<Task> Ref = new List<Task>();
            if (T.CategoryName() == A.R.GetString("Sleep"))
                Ref.Add(new Sleep());
            if (T.CategoryName() == A.R.GetString("Exercise"))
                Ref.Add(new Exercise());
            if (T.CategoryName() == A.R.GetString("Relax"))
                Ref.Add(new Relax());
            if (T.CategoryName() == A.R.GetString("Eat"))
                Ref.Add(new Eat());
            Ref[0].Weekend = !T.Weekend;
            Ref[0].StartPeriod = T.StartPeriod;
            Ref[0].EndPeriod = T.EndPeriod;
            Ref[0].Building = T.Building;
            return Ref[0];
        }

        public bool CanCopy(Task T, int To)
        {
            bool Can = true;
            if (T.Building.BuildingType.Name != "Apartments")
                Can = false;
            if (!dailyRoutines[To].CheckConflicts(T))
                Can = false;
            if (T.CategoryName() != A.R.GetString("Sleep"))
                if (T.CategoryName() != A.R.GetString("Exercise"))
                    if (T.CategoryName() != A.R.GetString("Relax"))
                        if (T.CategoryName() != A.R.GetString("Eat"))
                            Can = false;
            return Can;
        }

        public void Copy(int From)
        {
            dailyRoutines = A.SA.GetDailyRoutines(A.MF.CurrentEntityID);
            Transition(From);
            UpdateForm();
        }

        private void CopyWeekdays_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Copy(0);
        }

        private void CopyWeekends_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Copy(1);
        }

        #endregion
    }
}

