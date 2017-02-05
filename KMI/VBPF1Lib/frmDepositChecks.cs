namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using KMI.VBPF1Lib.Custom_Controls;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmDepositChecks : Form
    {
        private SortedList accounts;
        private Button btnAccept;
        private Button btnBack;
        private Button btnCancel;
        private Button btnHelp;
        private Button btnNext;
        private ComboBox cboAccounts;
        private SortedList checks;
        private CheckControl chkCheck;
        private Container components = null;
        private int currentIndex;
        private GroupBox groupBox1;
        private bool hasCheckingAccount;
        private Label labNoOffers;
        private Label labWarning;
        private RadioButton optCash;
        private RadioButton optDeposit;
        private CheckBox DoToAll;
        private Panel panChecks;

        public frmDepositChecks()
        {
            this.InitializeComponent();
            this.chkCheck = new CheckControl();
            this.chkCheck.Location = new Point(16, 0x18);
            this.panChecks.Controls.Add(this.chkCheck);
            for (int i = 0; i < 4; i++)
            {
                Label label = new Label {
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = this.chkCheck.BackColor,
                    Size = this.chkCheck.Size,
                    Location = new Point(20 + (i * 4), 20 - (i * 4))
                };
                this.panChecks.Controls.Add(label);
            }
            this.accounts = A.SA.GetBankAccounts(A.MF.CurrentEntityID);
            foreach (BankAccount account in this.accounts.Values)
            {
                this.cboAccounts.Items.Add(account);
                if (account is CheckingAccount)
                {
                    this.hasCheckingAccount = true;
                }
            }
            if (this.cboAccounts.Items.Count > 0)
            {
                this.optDeposit.Enabled = true;
                this.cboAccounts.SelectedIndex = 0;
            }
            this.labWarning.Visible = !this.hasCheckingAccount;
            this.RefreshData();
            this.currentIndex = 0;
        }

        public void cashAll(float fee)
        {
            while (checks.Count != 0)
                A.SA.CashCheck(A.MF.CurrentEntityID, (Check)checks.GetByIndex(0), fee);
        }

        public void depositAll()
        {
            while (checks.Count != 0)
                A.SA.DepositCheck(A.MF.CurrentEntityID, (Check)checks.GetByIndex(0), ((BankAccount)this.cboAccounts.SelectedItem).AccountNumber);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.optCash.Checked)
                {

                    float fee = 0f;
                    if (!this.hasCheckingAccount)
                        fee = 0.1f;
                    if (DoToAll.Checked)
                        cashAll(fee);
                    else
                        A.SA.CashCheck(A.MF.CurrentEntityID, this.currentCheck, fee);
                }
                else
                {
                    if (DoToAll.Checked)
                        depositAll();
                    else
                        A.SA.DepositCheck(A.MF.CurrentEntityID, this.currentCheck, ((BankAccount) this.cboAccounts.SelectedItem).AccountNumber);
                }
                A.MF.UpdateView();
                this.RefreshData();
                if (checks.Count == 0)
                    this.Close();
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception, this);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.currentIndex--;
            this.SwitchCheck();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("Deposit Funds"));
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.currentIndex++;
            this.SwitchCheck();
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.labNoOffers = new System.Windows.Forms.Label();
            this.panChecks = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DoToAll = new System.Windows.Forms.CheckBox();
            this.labWarning = new System.Windows.Forms.Label();
            this.cboAccounts = new System.Windows.Forms.ComboBox();
            this.optDeposit = new System.Windows.Forms.RadioButton();
            this.optCash = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(504, 256);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(96, 24);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Go";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(504, 288);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(504, 320);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(96, 24);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(24, 280);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(48, 24);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "<<";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(88, 280);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(48, 24);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = ">>";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // labNoOffers
            // 
            this.labNoOffers.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labNoOffers.ForeColor = System.Drawing.Color.Gray;
            this.labNoOffers.Location = new System.Drawing.Point(176, 64);
            this.labNoOffers.Name = "labNoOffers";
            this.labNoOffers.Size = new System.Drawing.Size(264, 120);
            this.labNoOffers.TabIndex = 7;
            this.labNoOffers.Text = "There are no more checks.";
            // 
            // panChecks
            // 
            this.panChecks.Location = new System.Drawing.Point(0, 8);
            this.panChecks.Name = "panChecks";
            this.panChecks.Size = new System.Drawing.Size(600, 232);
            this.panChecks.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DoToAll);
            this.groupBox1.Controls.Add(this.labWarning);
            this.groupBox1.Controls.Add(this.cboAccounts);
            this.groupBox1.Controls.Add(this.optDeposit);
            this.groupBox1.Controls.Add(this.optCash);
            this.groupBox1.Location = new System.Drawing.Point(160, 248);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 96);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Cash or Deposit";
            // 
            // DoToAll
            // 
            this.DoToAll.AutoSize = true;
            this.DoToAll.Location = new System.Drawing.Point(6, 72);
            this.DoToAll.Name = "DoToAll";
            this.DoToAll.Size = new System.Drawing.Size(77, 17);
            this.DoToAll.TabIndex = 5;
            this.DoToAll.Text = "Apply to all";
            this.DoToAll.UseVisualStyleBackColor = true;
            // 
            // labWarning
            // 
            this.labWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labWarning.ForeColor = System.Drawing.Color.Red;
            this.labWarning.Location = new System.Drawing.Point(112, 18);
            this.labWarning.Name = "labWarning";
            this.labWarning.Size = new System.Drawing.Size(200, 24);
            this.labWarning.TabIndex = 4;
            this.labWarning.Text = "Since you do not have a checking account in town, a 10% check cashing fee will be" +
    " charged.";
            // 
            // cboAccounts
            // 
            this.cboAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccounts.Location = new System.Drawing.Point(90, 52);
            this.cboAccounts.Name = "cboAccounts";
            this.cboAccounts.Size = new System.Drawing.Size(216, 21);
            this.cboAccounts.TabIndex = 1;
            // 
            // optDeposit
            // 
            this.optDeposit.Enabled = false;
            this.optDeposit.Location = new System.Drawing.Point(4, 45);
            this.optDeposit.Name = "optDeposit";
            this.optDeposit.Size = new System.Drawing.Size(80, 24);
            this.optDeposit.TabIndex = 3;
            this.optDeposit.Text = "Deposit to";
            this.optDeposit.CheckedChanged += new System.EventHandler(this.optDeposit_CheckedChanged);
            // 
            // optCash
            // 
            this.optCash.Checked = true;
            this.optCash.Location = new System.Drawing.Point(6, 30);
            this.optCash.Name = "optCash";
            this.optCash.Size = new System.Drawing.Size(96, 16);
            this.optCash.TabIndex = 2;
            this.optCash.TabStop = true;
            this.optCash.Text = "Cash Check";
            this.optCash.CheckedChanged += new System.EventHandler(this.optCash_CheckedChanged);
            // 
            // frmDepositChecks
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(610, 352);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panChecks);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.labNoOffers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmDepositChecks";
            this.ShowInTaskbar = false;
            this.Text = "Cash or Deposit Checks";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void optDeposit_CheckedChanged(object sender, EventArgs e)
        {
            this.cboAccounts.Enabled = this.optDeposit.Checked;
            if (optDeposit.Checked)
                DoToAll.Text = "Deposit all";
        }

        protected void RefreshData()
        {
            this.checks = A.SA.GetChecks(A.MF.CurrentEntityID);
            this.currentIndex = Math.Min(this.currentIndex, this.checks.Count - 1);
            this.panChecks.Visible = this.checks.Count > 0;
            this.btnAccept.Enabled = this.checks.Count > 0;
            for (int i = 0; i < this.panChecks.Controls.Count; i++)
            {
                this.panChecks.Controls[i].Visible = i < this.checks.Count;
            }
            if (this.checks.Count > 0)
            {
                this.SwitchCheck();
            }
        }

        private void SwitchCheck()
        {
            this.chkCheck.Check = this.currentCheck;
            this.btnBack.Enabled = this.currentIndex > 0;
            this.btnNext.Enabled = this.currentIndex < (this.checks.Count - 1);
        }

        public KMI.VBPF1Lib.Check currentCheck
        {
            get
            {
                return (KMI.VBPF1Lib.Check) this.checks.GetByIndex(this.currentIndex);
            }
        }

        private void optCash_CheckedChanged(object sender, EventArgs e)
        {
            if (optCash.Checked)
                DoToAll.Text = "Cash all";
        }
    }
}

