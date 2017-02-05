namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Resources;
    using System.Windows.Forms;
    using System.Collections.Generic;

    public class frmChangeTask : Form
    {
        private Button btnCancel;
        private Button btnHelp;
        private Button btnOK;
        private Button btnQuit;
        private ListBox cboEnd;
        private ListBox cboStart;
        private bool change;
        private Container components = null;
        private GroupBox groupBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label labMedical;
        private LinkLabel lnk401K;
        private LinkLabel lnkPayment;
        private LinkLabel lnkWitholding;
        private Panel panWorkTask;
        private Task task;
        private ToolTip toolTip;
        private Button bb_ShiftUp;
        private Label lb_Shift;
        private Button bb_ShiftDown;
        private ResourceManager manager;

        #region Init Methods
        public frmChangeTask(Task task, bool change, bool weekend)
        {
            this.InitializeComponent();
            this.task = task;
            this.change = change;
            this.task.Weekend = weekend;
            AddTimes();
            this.cboStart.SelectedIndex = task.StartPeriod;
            this.cboEnd.SelectedIndex = task.EndPeriod;
            this.btnQuit.Visible = change;
            ResourceManager manager = new ResourceManager(typeof(frmChangeTask));
            this.labMedical.Image = (Image)manager.GetObject("labMedical.Image");
            if ((task is WorkTask) || (task is AttendClass)) {
                Make_Unchangable();
                if (task is WorkTask)
                    Make_WorkTask();
            }
        }

        private void AddTimes()
        {
            for (int i = 0; i < 48; i++) {
                DateTime time = new DateTime(1, 1, 1);
                string Stime = time.AddHours((double)(((float)i) / 2f)).ToShortTimeString();
                this.cboStart.Items.Add(Stime);
                this.cboEnd.Items.Add(Stime);
            }
        }

        private void Make_WorkTask()
        {
            this.panWorkTask.Visible = true;
            this.lnkPayment.Enabled = A.MF.mnuActionsIncomePayment.Enabled;
            this.lnkWitholding.Enabled = A.MF.mnuActionsIncomeWitholding.Enabled;
            this.lnk401K.Enabled = A.MF.mnuActionsIncome401K.Enabled;
            this.lnk401K.Visible = ((WorkTask)task).R401KMatch > -1f;
            this.labMedical.Visible = ((WorkTask)task).HealthInsurance != null;
            this.toolTip = new ToolTip();
            this.toolTip.InitialDelay = 0;
            this.toolTip.SetToolTip(this.labMedical, "This job offers health insurance");
        }

        private void Make_Unchangable()
        {
            bb_ShiftDown.Enabled = false;
            bb_ShiftUp.Enabled = false;
            cboStart.Enabled = false;
            cboEnd.Enabled = false;
        }
        #endregion
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("Schedule"));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.cboStart.SelectedIndex == this.cboEnd.SelectedIndex) {
                MessageBox.Show("Tasks must be at least one hour long. Please try again.", "Invalid Entry");
                return;
            }
            try
            {
                if (this.change)
                {
                    if (this.cboStart.Enabled)
                    {
                        A.SA.EditTask(A.MF.CurrentEntityID, this.task.ID, this.cboStart.SelectedIndex, this.cboEnd.SelectedIndex);
                    }
                }
                else
                {
                    this.task.StartPeriod = this.cboStart.SelectedIndex;
                    this.task.EndPeriod = this.cboEnd.SelectedIndex;
                    A.SA.AddTask(A.MF.CurrentEntityID, this.task);
                }
                ((frmDailyRoutine) base.Owner).UpdateForm();
                base.Close();
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
                this.cboStart.SelectedIndex = this.task.StartPeriod;
                this.cboEnd.SelectedIndex = this.task.EndPeriod;
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(A.R.GetString("Are you sure you want to quit this activity?"), A.R.GetString("Confirm Quit"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (this.task is WorkTask)
                {
                    A.SA.DeleteTask(A.MF.CurrentEntityID, this.task.ID, false, true);
                }
                else
                {
                    A.SA.DeleteTask(A.MF.CurrentEntityID, this.task.ID);
                }
                ((frmDailyRoutine) base.Owner).UpdateForm();
                base.Close();
            }
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cboEnd = new System.Windows.Forms.ListBox();
            this.cboStart = new System.Windows.Forms.ListBox();
            this.btnQuit = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lnkWitholding = new System.Windows.Forms.LinkLabel();
            this.lnkPayment = new System.Windows.Forms.LinkLabel();
            this.lnk401K = new System.Windows.Forms.LinkLabel();
            this.panWorkTask = new System.Windows.Forms.Panel();
            this.labMedical = new System.Windows.Forms.Label();
            this.bb_ShiftUp = new System.Windows.Forms.Button();
            this.bb_ShiftDown = new System.Windows.Forms.Button();
            this.lb_Shift = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panWorkTask.SuspendLayout();
            this.SuspendLayout();
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
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_Shift);
            this.groupBox1.Controls.Add(this.bb_ShiftDown);
            this.groupBox1.Controls.Add(this.bb_ShiftUp);
            this.groupBox1.Controls.Add(this.cboEnd);
            this.groupBox1.Controls.Add(this.cboStart);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 256);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time";
            // 
            // cboEnd
            // 
            this.cboEnd.Location = new System.Drawing.Point(123, 32);
            this.cboEnd.Name = "cboEnd";
            this.cboEnd.Size = new System.Drawing.Size(69, 212);
            this.cboEnd.TabIndex = 4;
            // 
            // cboStart
            // 
            this.cboStart.Location = new System.Drawing.Point(19, 32);
            this.cboStart.Name = "cboStart";
            this.cboStart.Size = new System.Drawing.Size(69, 212);
            this.cboStart.TabIndex = 3;
            // 
            // btnQuit
            // 
            this.btnQuit.BackColor = System.Drawing.SystemColors.Control;
            this.btnQuit.Location = new System.Drawing.Point(51, 278);
            this.btnQuit.Name = "btnQuit";
            this.btnQuit.Size = new System.Drawing.Size(132, 24);
            this.btnQuit.TabIndex = 0;
            this.btnQuit.Text = "Stop Doing This Activity";
            this.btnQuit.UseVisualStyleBackColor = false;
            this.btnQuit.Click += new System.EventHandler(this.btnQuit_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(32, 352);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(48, 24);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(96, 352);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(48, 24);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(160, 352);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(48, 24);
            this.btnHelp.TabIndex = 9;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(2, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Change:";
            // 
            // lnkWitholding
            // 
            this.lnkWitholding.Location = new System.Drawing.Point(52, 8);
            this.lnkWitholding.Name = "lnkWitholding";
            this.lnkWitholding.Size = new System.Drawing.Size(64, 16);
            this.lnkWitholding.TabIndex = 11;
            this.lnkWitholding.TabStop = true;
            this.lnkWitholding.Text = "Withholding";
            this.lnkWitholding.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWitholding_LinkClicked);
            // 
            // lnkPayment
            // 
            this.lnkPayment.Location = new System.Drawing.Point(120, 8);
            this.lnkPayment.Name = "lnkPayment";
            this.lnkPayment.Size = new System.Drawing.Size(56, 16);
            this.lnkPayment.TabIndex = 12;
            this.lnkPayment.TabStop = true;
            this.lnkPayment.Text = "Payment";
            this.lnkPayment.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkPayment_LinkClicked);
            // 
            // lnk401K
            // 
            this.lnk401K.Location = new System.Drawing.Point(180, 8);
            this.lnk401K.Name = "lnk401K";
            this.lnk401K.Size = new System.Drawing.Size(32, 16);
            this.lnk401K.TabIndex = 13;
            this.lnk401K.TabStop = true;
            this.lnk401K.Text = "401K";
            this.lnk401K.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnk401K_LinkClicked);
            // 
            // panWorkTask
            // 
            this.panWorkTask.Controls.Add(this.lnkWitholding);
            this.panWorkTask.Controls.Add(this.lnkPayment);
            this.panWorkTask.Controls.Add(this.lnk401K);
            this.panWorkTask.Controls.Add(this.label3);
            this.panWorkTask.Location = new System.Drawing.Point(8, 312);
            this.panWorkTask.Name = "panWorkTask";
            this.panWorkTask.Size = new System.Drawing.Size(224, 32);
            this.panWorkTask.TabIndex = 14;
            this.panWorkTask.Visible = false;
            // 
            // labMedical
            // 
            this.labMedical.Location = new System.Drawing.Point(192, 276);
            this.labMedical.Name = "labMedical";
            this.labMedical.Size = new System.Drawing.Size(32, 32);
            this.labMedical.TabIndex = 15;
            this.labMedical.Visible = false;
            this.labMedical.Click += new System.EventHandler(this.labMedical_Click);
            // 
            // bb_ShiftUp
            // 
            this.bb_ShiftUp.Location = new System.Drawing.Point(95, 109);
            this.bb_ShiftUp.Name = "bb_ShiftUp";
            this.bb_ShiftUp.Size = new System.Drawing.Size(22, 23);
            this.bb_ShiftUp.TabIndex = 5;
            this.bb_ShiftUp.Text = "▲";
            this.bb_ShiftUp.UseVisualStyleBackColor = true;
            this.bb_ShiftUp.Click += new System.EventHandler(this.bb_ShiftUp_Click);
            // 
            // bb_ShiftDown
            // 
            this.bb_ShiftDown.Location = new System.Drawing.Point(95, 138);
            this.bb_ShiftDown.Name = "bb_ShiftDown";
            this.bb_ShiftDown.Size = new System.Drawing.Size(22, 23);
            this.bb_ShiftDown.TabIndex = 6;
            this.bb_ShiftDown.Text = "▼";
            this.bb_ShiftDown.UseVisualStyleBackColor = true;
            this.bb_ShiftDown.Click += new System.EventHandler(this.bb_ShiftDown_Click);
            // 
            // lb_Shift
            // 
            this.lb_Shift.Location = new System.Drawing.Point(88, 77);
            this.lb_Shift.Name = "lb_Shift";
            this.lb_Shift.Size = new System.Drawing.Size(35, 29);
            this.lb_Shift.TabIndex = 7;
            this.lb_Shift.Text = "Shift 30min";
            // 
            // frmChangeTask
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(240, 382);
            this.Controls.Add(this.labMedical);
            this.Controls.Add(this.panWorkTask);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnQuit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmChangeTask";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Change Activity";
            this.groupBox1.ResumeLayout(false);
            this.panWorkTask.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void labMedical_Click(object sender, EventArgs e)
        {
        }

        private void lnk401K_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frm401K(this.task.ID).ShowDialog(this);
        }

        private void lnkPayment_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmMethodOfPay(this.task.ID).ShowDialog(this);
        }

        private void lnkWitholding_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new frmW4(this.task.ID).ShowDialog(this);
        }

        private void bb_ShiftUp_Click(object sender, EventArgs e)
        {
            if (cboStart.SelectedIndex != 0)
                cboStart.SelectedIndex -= 1;
            else
                cboStart.SelectedIndex = 47;

            if (cboEnd.SelectedIndex != 0)
                cboEnd.SelectedIndex -= 1;
            else
                cboEnd.SelectedIndex = 47;
        }

        private void bb_ShiftDown_Click(object sender, EventArgs e)
        {
            if (cboStart.SelectedIndex != 47)
                cboStart.SelectedIndex += 1;
            else
                cboStart.SelectedIndex = 0;

            if (cboEnd.SelectedIndex != 47)
                cboEnd.SelectedIndex += 1;
            else
                cboEnd.SelectedIndex = 0;
        }
    }
}

