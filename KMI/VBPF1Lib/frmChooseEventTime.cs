namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Windows.Forms;

    public class frmChooseEventTime : Form
    {
        private Button btnCancel;
        private Button btnHelp;
        private MonthCalendar Cal;
        private ListBox cboEnd;
        private ListBox cboStart;
        private Container components = null;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Label label1;
        private Button button1;
        private Label label3;
        private TextBox GotoDate;
        private Button btnOK;
        private Label label2;
        private Button button2;

        static int[] Remember_Times = new int[2] { 0, 0 };
        private Label label4;
        private CheckBox monCheck;
        private CheckBox tuesCheck;
        private CheckBox wednesCheck;
        private CheckBox thursCheck;
        private CheckBox friCheck;
        private CheckBox saturCheck;
        private CheckBox sunCheck;
        private Button Jump_bt;
        private Label label5;
        private TextBox jumpToDate_TB;
        static DateTime Remember_Date = new DateTime(1,1,1);
        private bool DatesBusy = false;

        public frmChooseEventTime()
        {
            this.InitializeComponent();
            for (int i = 0; i < 0x30; i++)
            {
                DateTime time = new DateTime(1, 1, 1);
                this.cboStart.Items.Add(time.AddHours((double) (((float) i) / 2f)).ToShortTimeString());
                time = new DateTime(1, 1, 1);
                this.cboEnd.Items.Add(time.AddHours((double) (((float) i) / 2f)).ToShortTimeString());
            }
            this.Cal.TodayDate = A.MF.Now.AddDays(1.0);
            this.Cal.SetDate(this.Cal.TodayDate);
            this.Cal.MinDate = this.Cal.TodayDate;
            if (Remember_Date.CompareTo(Cal.MinDate) < 0)
                Remember_Date = Cal.MinDate;
            else
                Cal.SelectionStart = Remember_Date;
            GetAllowedDays();
            Update_Date();
            Recall_Times();
        }

        private void GetAllowedDays()
        {
            DatesBusy = true;
            string allowed;
            if (!Settings.GetValue<string>("AllowedDays", out allowed))
                allowed = "1111111";
            monCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[0].ToString()));
            tuesCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[1].ToString()));
            wednesCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[2].ToString()));
            thursCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[3].ToString()));
            friCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[4].ToString()));
            saturCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[5].ToString()));
            sunCheck.Checked = Convert.ToBoolean(Convert.ToInt32(allowed[6].ToString()));
            DatesBusy = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("Socializing"));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Save_Times();
            try {
                if ((this.cboStart.SelectedIndex == -1) || (this.cboEnd.SelectedIndex == -1))
                    MessageBox.Show(A.R.GetString("You must selected a starting and ending time. Please try again."), A.R.GetString("Input Required"));
                else if (this.cboStart.SelectedIndex == this.cboEnd.SelectedIndex)
                    MessageBox.Show(A.R.GetString("Events must last at least one-half hour. Please try again."), A.R.GetString("Input Required"));
                else {
                    A.SA.SetParty(A.MF.CurrentEntityID, this.Cal.SelectionStart, this.cboStart.SelectedIndex, this.cboEnd.SelectedIndex);
                }
            }
            catch (Exception exception) { frmExceptionHandler.Handle(exception); }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboEnd = new System.Windows.Forms.ListBox();
            this.cboStart = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Jump_bt = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.jumpToDate_TB = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.GotoDate = new System.Windows.Forms.TextBox();
            this.Cal = new System.Windows.Forms.MonthCalendar();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.monCheck = new System.Windows.Forms.CheckBox();
            this.tuesCheck = new System.Windows.Forms.CheckBox();
            this.wednesCheck = new System.Windows.Forms.CheckBox();
            this.thursCheck = new System.Windows.Forms.CheckBox();
            this.friCheck = new System.Windows.Forms.CheckBox();
            this.saturCheck = new System.Windows.Forms.CheckBox();
            this.sunCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cboEnd);
            this.groupBox1.Controls.Add(this.cboStart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(244, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 192);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time";
            // 
            // cboEnd
            // 
            this.cboEnd.Location = new System.Drawing.Point(120, 32);
            this.cboEnd.Name = "cboEnd";
            this.cboEnd.Size = new System.Drawing.Size(72, 147);
            this.cboEnd.TabIndex = 4;
            // 
            // cboStart
            // 
            this.cboStart.Location = new System.Drawing.Point(16, 32);
            this.cboStart.Name = "cboStart";
            this.cboStart.Size = new System.Drawing.Size(72, 147);
            this.cboStart.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start Time:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(120, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 24);
            this.label2.TabIndex = 2;
            this.label2.Text = "End Time:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Jump_bt);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.jumpToDate_TB);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.GotoDate);
            this.groupBox2.Controls.Add(this.Cal);
            this.groupBox2.Location = new System.Drawing.Point(16, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(210, 247);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Date";
            // 
            // Jump_bt
            // 
            this.Jump_bt.Location = new System.Drawing.Point(126, 211);
            this.Jump_bt.Name = "Jump_bt";
            this.Jump_bt.Size = new System.Drawing.Size(46, 24);
            this.Jump_bt.TabIndex = 17;
            this.Jump_bt.Text = "Jump";
            this.Jump_bt.Click += new System.EventHandler(this.Jump_bt_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(1, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 27);
            this.label5.TabIndex = 16;
            this.label5.Text = "Jump # days";
            // 
            // jumpToDate_TB
            // 
            this.jumpToDate_TB.Location = new System.Drawing.Point(54, 214);
            this.jumpToDate_TB.Name = "jumpToDate_TB";
            this.jumpToDate_TB.Size = new System.Drawing.Size(66, 20);
            this.jumpToDate_TB.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(126, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(46, 24);
            this.button2.TabIndex = 14;
            this.button2.Text = "Now";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(172, 185);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(38, 24);
            this.button1.TabIndex = 13;
            this.button1.Text = "Go";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 191);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Go to:";
            // 
            // GotoDate
            // 
            this.GotoDate.Location = new System.Drawing.Point(54, 188);
            this.GotoDate.Name = "GotoDate";
            this.GotoDate.Size = new System.Drawing.Size(66, 20);
            this.GotoDate.TabIndex = 1;
            // 
            // Cal
            // 
            this.Cal.Location = new System.Drawing.Point(16, 20);
            this.Cal.MaxSelectionCount = 1;
            this.Cal.Name = "Cal";
            this.Cal.ShowToday = false;
            this.Cal.ShowTodayCircle = false;
            this.Cal.TabIndex = 0;
            this.Cal.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.Cal_DateChanged);
            this.Cal.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.Cal_DateSelected);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(310, 313);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(96, 24);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(190, 313);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 24);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Close";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(70, 313);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 24);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "Set Party Date";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 269);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Party Days:";
            // 
            // monCheck
            // 
            this.monCheck.AutoSize = true;
            this.monCheck.Checked = true;
            this.monCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.monCheck.Location = new System.Drawing.Point(79, 269);
            this.monCheck.Name = "monCheck";
            this.monCheck.Size = new System.Drawing.Size(64, 17);
            this.monCheck.TabIndex = 14;
            this.monCheck.Text = "Monday";
            this.monCheck.UseVisualStyleBackColor = true;
            this.monCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // tuesCheck
            // 
            this.tuesCheck.AutoSize = true;
            this.tuesCheck.Checked = true;
            this.tuesCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tuesCheck.Location = new System.Drawing.Point(149, 269);
            this.tuesCheck.Name = "tuesCheck";
            this.tuesCheck.Size = new System.Drawing.Size(67, 17);
            this.tuesCheck.TabIndex = 15;
            this.tuesCheck.Text = "Tuesday";
            this.tuesCheck.UseVisualStyleBackColor = true;
            this.tuesCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // wednesCheck
            // 
            this.wednesCheck.AutoSize = true;
            this.wednesCheck.Checked = true;
            this.wednesCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.wednesCheck.Location = new System.Drawing.Point(222, 269);
            this.wednesCheck.Name = "wednesCheck";
            this.wednesCheck.Size = new System.Drawing.Size(83, 17);
            this.wednesCheck.TabIndex = 16;
            this.wednesCheck.Text = "Wednesday";
            this.wednesCheck.UseVisualStyleBackColor = true;
            this.wednesCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // thursCheck
            // 
            this.thursCheck.AutoSize = true;
            this.thursCheck.Checked = true;
            this.thursCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.thursCheck.Location = new System.Drawing.Point(310, 269);
            this.thursCheck.Name = "thursCheck";
            this.thursCheck.Size = new System.Drawing.Size(70, 17);
            this.thursCheck.TabIndex = 17;
            this.thursCheck.Text = "Thursday";
            this.thursCheck.UseVisualStyleBackColor = true;
            this.thursCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // friCheck
            // 
            this.friCheck.AutoSize = true;
            this.friCheck.Checked = true;
            this.friCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.friCheck.Location = new System.Drawing.Point(79, 290);
            this.friCheck.Name = "friCheck";
            this.friCheck.Size = new System.Drawing.Size(54, 17);
            this.friCheck.TabIndex = 18;
            this.friCheck.Text = "Friday";
            this.friCheck.UseVisualStyleBackColor = true;
            this.friCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // saturCheck
            // 
            this.saturCheck.AutoSize = true;
            this.saturCheck.Checked = true;
            this.saturCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.saturCheck.Location = new System.Drawing.Point(149, 290);
            this.saturCheck.Name = "saturCheck";
            this.saturCheck.Size = new System.Drawing.Size(68, 17);
            this.saturCheck.TabIndex = 19;
            this.saturCheck.Text = "Saturday";
            this.saturCheck.UseVisualStyleBackColor = true;
            this.saturCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // sunCheck
            // 
            this.sunCheck.AutoSize = true;
            this.sunCheck.Checked = true;
            this.sunCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.sunCheck.Location = new System.Drawing.Point(222, 290);
            this.sunCheck.Name = "sunCheck";
            this.sunCheck.Size = new System.Drawing.Size(62, 17);
            this.sunCheck.TabIndex = 20;
            this.sunCheck.Text = "Sunday";
            this.sunCheck.UseVisualStyleBackColor = true;
            this.sunCheck.CheckedChanged += new System.EventHandler(this.AllowedDaysChanged);
            // 
            // frmChooseEventTime
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(472, 343);
            this.Controls.Add(this.sunCheck);
            this.Controls.Add(this.saturCheck);
            this.Controls.Add(this.friCheck);
            this.Controls.Add(this.thursCheck);
            this.Controls.Add(this.wednesCheck);
            this.Controls.Add(this.tuesCheck);
            this.Controls.Add(this.monCheck);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmChooseEventTime";
            this.ShowInTaskbar = false;
            this.Text = "Plan Your Party";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        void Recall_Times()
        {
            int Start, End = -1;
            DateTime rDate = new DateTime(1, 1, 1);
            int days = 0;
            if (Settings.GetValue<int>("JumpDays", out days))
                jumpToDate_TB.Text = days.ToString();
            Settings.GetValue<int>("AllowedTime0", out Start);
            Settings.GetValue<int>("AllowedTime1", out End);
            Settings.GetValue<DateTime>("RDate", out rDate);
            cboStart.SelectedIndex = Start;
            cboEnd.SelectedIndex = End;
            Cal.SelectionStart = (Cal.MinDate < rDate) ? rDate : Cal.MinDate;
            GoToAllowedDate();
        }

        void Save_Times()
        {
            Settings.SetValue<int>("AllowedTime0", cboStart.SelectedIndex);
            Settings.SetValue<int>("AllowedTime1", cboEnd.SelectedIndex);
        }

        void Update_Date()
        {
            int[] Date = new int[] { Cal.SelectionStart.Year, Cal.SelectionStart.Month, Cal.SelectionStart.Day };
            GotoDate.Text = Date[1] + "/" + Date[2] + "/" + Date[0];
        }

        private void Cal_DateSelected(object sender, DateRangeEventArgs e)
        {
            Update_Date();
            Settings.SetValue<DateTime>("RDate", Cal.SelectionStart);
        }

        bool IsValidDate(string TEXT, out DateTime DATE)
        {
            DATE = new DateTime(1, 1, 1); 
            try {
                string[] parts = TEXT.Split('/');
                int[] SET = new int[parts.Length];
                for (int i = 0; i != 3; i++)
                    SET[i] = Convert.ToInt32(parts[i]);
                return CheckDateExceptions(SET, out DATE);
            }
            catch { return false; }
        }

        bool CheckDateExceptions(int[] DateInfo, out DateTime SET)
        {
            SET = new DateTime(1, 1, 1);
            if (DateInfo.Length != 3)
                return false;
            for (int i = 0; i != 3; i++)
                if (DateInfo[i] == 0)
                    return false;
            DateTime Temp = new DateTime(DateInfo[2], DateInfo[0], DateInfo[1]);
            int LeapOffset = 0; 
            if (DateTime.IsLeapYear(DateInfo[2]))
                LeapOffset++;
            int[] Month_Lengths = new int[]{ 31, 28+LeapOffset, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            if (DateInfo[0] > 12 || DateInfo[0] == 0)
                return false;
            if (DateInfo[1] > Month_Lengths[DateInfo[0]])
                return false;
            if (Temp.CompareTo(Cal.MinDate) < 0)
                return false;
            SET = Temp;
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime ToSet;
            if (IsValidDate(GotoDate.Text, out ToSet))
                Cal.SetDate(ToSet);
            GoToAllowedDate();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cal.SelectionStart = Cal.MinDate;
            Remember_Date = Cal.MinDate;
            GoToAllowedDate();
            Update_Date();
        }

        private void GoToAllowedDate() {
            while (!Allowed(Cal.SelectionStart)) {
                Cal.SelectionStart = Cal.SelectionStart.AddDays(1);
            }
        }

        private bool Allowed(DateTime day) {
            foreach (Control UC in this.Controls)
                if (UC is CheckBox && (UC as CheckBox).Text == day.DayOfWeek.ToString())
                    return (UC as CheckBox).Checked;
            return false;
        }

        private void AllowedDaysChanged(object sender, EventArgs e)
        {
            string allowed = ((monCheck.Checked) ? "1" : "0") +
                ((tuesCheck.Checked) ? "1" : "0") + ((wednesCheck.Checked) ? "1" : "0") +
                ((thursCheck.Checked) ? "1" : "0") + ((friCheck.Checked) ? "1" : "0") +
                ((saturCheck.Checked) ? "1" : "0") + ((sunCheck.Checked) ? "1" : "0");
            if (allowed == "0000000")
                ((CheckBox)sender).Checked = true;
            else {
                Settings.SetValue<string>("AllowedDays", allowed);
                GoToAllowedDate();
            }
        }

        private void Cal_DateChanged(object sender, DateRangeEventArgs e)
        {
            GoToAllowedDate();
        }

        private void Jump_bt_Click(object sender, EventArgs e)
        {
            if (DatesBusy)
                return;
            int days = 0;
            if (!int.TryParse(jumpToDate_TB.Text, out days) && days > 0)
                return;
            Cal.SelectionStart = Cal.SelectionStart.AddDays(days);
            Settings.SetValue<int>("JumpDays", days);
            GoToAllowedDate();
        }
    }
}

