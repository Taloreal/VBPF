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

    public class frmOnlineBanking : Form
    {
        private BankAccount account;
        private SortedList bankAccounts;
        protected static Brush brush = new SolidBrush(Color.Gray);
        private Button btnDoneRecurring;
        private Button btnSchedulePayments;
        private Button btnTransfer;
        private Button button1;
        private ComboBox cboFrom;
        private ComboBox cboPayAccount;
        private ComboBox cboPayAccount2;
        private ComboBox cboTo;
        private ComboBox cboURLs;
        protected int colWidth = 80;
        private Container components = null;
        protected static Font font = new Font("Arial", 10f);
        protected static Font fontS = new Font("Arial", 10f, FontStyle.Bold);
        private Label labAccountBalance;
        private Label labAccountNumber;
        private Label labBankName;
        private Label label1;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label17;
        private Label label18;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label labFromBal;
        private Label labToBal;
        private LinkLabel linkBack2;
        private LinkLabel linkLabel1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
        private LinkLabel linkPaybills;
        private LinkLabel linkRecurring;
        private LinkLabel linkTransfer;
        protected int margin = 5;
        private Panel panAccountDetail;
        private Panel panAccounts;
        private Panel panel1;
        private Panel panHome;
        private Panel panPayBills;
        private Panel panPayees;
        private Panel panPayees2;
        private Panel panRecurring;
        private Panel panTransactions;
        private Panel panTransfer;
        private ArrayList payees;
        protected static Pen pen = new Pen(brush, 1f);
        protected static StringFormat sfc = new StringFormat();
        protected static StringFormat sfr = new StringFormat();
        private NumericUpDown updAmount;
        protected string url = null;

        public frmOnlineBanking()
        {
            this.InitializeComponent();
            sfr.Alignment = StringAlignment.Far;
            base.Size = new Size(0x248, 0x1a8);
            this.bankAccounts = A.SA.GetBankAccounts(A.MF.CurrentEntityID);
            this.payees = A.SA.GetPayees(A.MF.CurrentEntityID);
            if (this.bankAccounts.Count == 0)
            {
                throw new SimApplicationException(A.R.GetString("You do not have any bank accounts open. You need a bank account to do online banking. To open a bank account, click a bank in the City view."));
            }
            foreach (BankAccount account in this.bankAccounts.Values)
            {
                if (!this.cboURLs.Items.Contains(account.BankURL))
                {
                    this.cboURLs.Items.Add(account.BankURL);
                }
            }
            this.cboURLs.SelectedIndex = 0;
        }

        private void btnDoneRecurring_Click(object sender, EventArgs e)
        {
            Exception exception;
            ArrayList payments = new ArrayList();
            foreach (RecurringPaymentControl control in this.panPayees2.Controls)
            {
                try
                {
                    RecurringPayment recurringPayment = control.RecurringPayment;
                    if (recurringPayment != null)
                    {
                        payments.Add(recurringPayment);
                    }
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    MessageBox.Show(A.R.GetString("Invalid amount entered for {0}. Please correct.", new object[] { control.PayeeName.Text }), A.R.GetString("Invalid Entry"));
                    return;
                }
            }
            try
            {
                A.SA.SetRecurringPayments(A.MF.CurrentEntityID, ((BankAccount) this.cboPayAccount2.SelectedItem).AccountNumber, payments);
                MessageBox.Show(A.R.GetString("Your recurring payments have been set up."), A.R.GetString("Success"));
            }
            catch (Exception exception2)
            {
                exception = exception2;
                frmExceptionHandler.Handle(exception, this);
            }
            this.ReturnToHome();
        }

        private void btnSchedulePayments_Click(object sender, EventArgs e)
        {
            Exception exception;
            Hashtable payments = new Hashtable();
            foreach (Control control in this.panPayees.Controls)
            {
                if ((control is TextBox) && (control.Text != ""))
                {
                    float num = 0f;
                    try
                    {
                        num = float.Parse(control.Text);
                    }
                    catch (Exception exception1)
                    {
                        exception = exception1;
                        MessageBox.Show(A.R.GetString("Invalid amount entered for {0}. Please correct.", new object[] { control.Tag }), A.R.GetString("Invalid Entry"));
                        return;
                    }
                    if (num > 0f)
                    {
                        payments.Add(control.Tag, num);
                    }
                }
            }
            try
            {
                A.SA.SchedulePayments(A.MF.CurrentEntityID, ((BankAccount) this.cboPayAccount.SelectedItem).AccountNumber, payments);
                MessageBox.Show(A.R.GetString("Your payments have been scheduled."), A.R.GetString("Success"));
            }
            catch (Exception exception2)
            {
                exception = exception2;
                frmExceptionHandler.Handle(exception, this);
            }
            this.ReturnToHome();
        }

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            try
            {
                BankAccount selectedItem = (BankAccount) this.cboFrom.SelectedItem;
                BankAccount account2 = (BankAccount) this.cboTo.SelectedItem;
                A.SA.TransferFunds(A.MF.CurrentEntityID, selectedItem.AccountNumber, account2.AccountNumber, (float) this.updAmount.Value);
                MessageBox.Show(A.R.GetString("Funds transferred successfully."), A.R.GetString("Transfer Funds"));
                this.ReturnToHome();
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception, this);
            }
        }

        private void cboFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.cboTo.Items.Clear();
            foreach (BankAccount account in this.bankAccounts.Values)
            {
                if (account != this.cboFrom.SelectedItem)
                {
                    this.cboTo.Items.Add(account);
                }
            }
            this.cboTo.SelectedIndex = 0;
            BankAccount selectedItem = (BankAccount) this.cboFrom.SelectedItem;
            this.labFromBal.Text = Utilities.FC(selectedItem.EndingBalance(), 2, A.I.CurrencyConversion);
            this.updAmount.Maximum = (decimal) selectedItem.EndingBalance();
        }

        private void cboPayAccount2_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (RecurringPayment payment in ((CheckingAccount) this.cboPayAccount2.SelectedItem).RecurringPayments)
            {
                foreach (RecurringPaymentControl control in this.panPayees2.Controls)
                {
                    if (payment.PayeeAccountNumber == control.PayeeAccountNumber)
                    {
                        control.RecurringPayment = payment;
                    }
                }
            }
        }

        private void cboTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.labToBal.Text = Utilities.FC(((BankAccount) this.cboTo.SelectedItem).EndingBalance(), 2, A.I.CurrencyConversion);
        }

        private void cboURLs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.URL = this.cboURLs.SelectedItem.ToString();
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
            this.panel1 = new Panel();
            this.button1 = new Button();
            this.label1 = new Label();
            this.cboURLs = new ComboBox();
            this.panHome = new Panel();
            this.panAccounts = new Panel();
            this.linkRecurring = new LinkLabel();
            this.labBankName = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.linkTransfer = new LinkLabel();
            this.linkPaybills = new LinkLabel();
            this.panAccountDetail = new Panel();
            this.labAccountNumber = new Label();
            this.labAccountBalance = new Label();
            this.label14 = new Label();
            this.label13 = new Label();
            this.linkLabel1 = new LinkLabel();
            this.panTransactions = new Panel();
            this.panPayBills = new Panel();
            this.cboPayAccount = new ComboBox();
            this.label8 = new Label();
            this.btnSchedulePayments = new Button();
            this.label5 = new Label();
            this.label4 = new Label();
            this.linkBack2 = new LinkLabel();
            this.panPayees = new Panel();
            this.panTransfer = new Panel();
            this.labToBal = new Label();
            this.labFromBal = new Label();
            this.btnTransfer = new Button();
            this.updAmount = new NumericUpDown();
            this.label12 = new Label();
            this.cboTo = new ComboBox();
            this.cboFrom = new ComboBox();
            this.label10 = new Label();
            this.label11 = new Label();
            this.label9 = new Label();
            this.linkLabel2 = new LinkLabel();
            this.label6 = new Label();
            this.label7 = new Label();
            this.panRecurring = new Panel();
            this.label18 = new Label();
            this.cboPayAccount2 = new ComboBox();
            this.label15 = new Label();
            this.btnDoneRecurring = new Button();
            this.label16 = new Label();
            this.label17 = new Label();
            this.linkLabel3 = new LinkLabel();
            this.panPayees2 = new Panel();
            this.panel1.SuspendLayout();
            this.panHome.SuspendLayout();
            this.panAccountDetail.SuspendLayout();
            this.panPayBills.SuspendLayout();
            this.panTransfer.SuspendLayout();
            this.updAmount.BeginInit();
            this.panRecurring.SuspendLayout();
            base.SuspendLayout();
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cboURLs);
            this.panel1.Dock = DockStyle.Top;
            this.panel1.Location = new Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new Size(0x3e8, 40);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new PaintEventHandler(this.panel1_Paint);
            this.button1.FlatStyle = FlatStyle.Popup;
            this.button1.Location = new Point(480, 8);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x30, 0x18);
            this.button1.TabIndex = 2;
            this.button1.Text = "Go";
            this.button1.TextAlign = ContentAlignment.MiddleRight;
            this.label1.Location = new Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x38, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Addresss";
            this.cboURLs.Location = new Point(80, 8);
            this.cboURLs.Name = "cboURLs";
            this.cboURLs.Size = new Size(0x180, 0x15);
            this.cboURLs.TabIndex = 0;
            this.cboURLs.SelectedIndexChanged += new EventHandler(this.cboURLs_SelectedIndexChanged);
            this.panHome.BackColor = Color.White;
            this.panHome.BorderStyle = BorderStyle.Fixed3D;
            this.panHome.Controls.Add(this.panAccounts);
            this.panHome.Controls.Add(this.linkRecurring);
            this.panHome.Controls.Add(this.labBankName);
            this.panHome.Controls.Add(this.label3);
            this.panHome.Controls.Add(this.label2);
            this.panHome.Controls.Add(this.linkTransfer);
            this.panHome.Controls.Add(this.linkPaybills);
            this.panHome.Location = new Point(0, 40);
            this.panHome.Name = "panHome";
            this.panHome.Size = new Size(0x1a0, 0x120);
            this.panHome.TabIndex = 1;
            this.panAccounts.AutoScroll = true;
            this.panAccounts.Location = new Point(0x18, 0x70);
            this.panAccounts.Name = "panAccounts";
            this.panAccounts.Size = new Size(0x98, 200);
            this.panAccounts.TabIndex = 8;
            this.linkRecurring.Location = new Point(16, 40);
            this.linkRecurring.Name = "linkRecurring";
            this.linkRecurring.Size = new Size(120, 16);
            this.linkRecurring.TabIndex = 7;
            this.linkRecurring.TabStop = true;
            this.linkRecurring.Text = "Recurring Payments";
            this.linkRecurring.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkRecurring_LinkClicked);
            this.labBankName.Font = new Font("Impact", 26.25f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.labBankName.ForeColor = Color.Silver;
            this.labBankName.Location = new Point(0xb8, 0xd8);
            this.labBankName.Name = "labBankName";
            this.labBankName.Size = new Size(0x158, 0x48);
            this.labBankName.TabIndex = 6;
            this.labBankName.Text = "label4";
            this.labBankName.TextAlign = ContentAlignment.TopCenter;
            this.label3.Font = new Font("Impact", 36f, FontStyle.Italic, GraphicsUnit.Point, 0);
            this.label3.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.label3.Location = new Point(0xb8, 16);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x158, 0xc0);
            this.label3.TabIndex = 5;
            this.label3.Text = "Welcome to Online Banking at";
            this.label3.TextAlign = ContentAlignment.TopCenter;
            this.label2.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label2.ForeColor = Color.Gray;
            this.label2.Location = new Point(16, 0x58);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x98, 0x18);
            this.label2.TabIndex = 4;
            this.label2.Text = "Account Details:";
            this.linkTransfer.Location = new Point(16, 0x40);
            this.linkTransfer.Name = "linkTransfer";
            this.linkTransfer.Size = new Size(0x60, 16);
            this.linkTransfer.TabIndex = 1;
            this.linkTransfer.TabStop = true;
            this.linkTransfer.Text = "Transfer Funds";
            this.linkTransfer.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkTransfer_LinkClicked);
            this.linkPaybills.Location = new Point(16, 16);
            this.linkPaybills.Name = "linkPaybills";
            this.linkPaybills.Size = new Size(0x60, 16);
            this.linkPaybills.TabIndex = 0;
            this.linkPaybills.TabStop = true;
            this.linkPaybills.Text = "Pay Bills";
            this.linkPaybills.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkPaybills_LinkClicked);
            this.panAccountDetail.AutoScroll = true;
            this.panAccountDetail.BackColor = Color.White;
            this.panAccountDetail.BorderStyle = BorderStyle.Fixed3D;
            this.panAccountDetail.Controls.Add(this.labAccountNumber);
            this.panAccountDetail.Controls.Add(this.labAccountBalance);
            this.panAccountDetail.Controls.Add(this.label14);
            this.panAccountDetail.Controls.Add(this.label13);
            this.panAccountDetail.Controls.Add(this.linkLabel1);
            this.panAccountDetail.Controls.Add(this.panTransactions);
            this.panAccountDetail.Location = new Point(0x1b0, 0x38);
            this.panAccountDetail.Name = "panAccountDetail";
            this.panAccountDetail.Size = new Size(560, 0xa8);
            this.panAccountDetail.TabIndex = 2;
            this.panAccountDetail.Visible = false;
            this.labAccountNumber.Font = new Font("Franklin Gothic Medium", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labAccountNumber.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.labAccountNumber.Location = new Point(0x130, 8);
            this.labAccountNumber.Name = "labAccountNumber";
            this.labAccountNumber.Size = new Size(0x90, 0x16);
            this.labAccountNumber.TabIndex = 7;
            this.labAccountBalance.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labAccountBalance.ForeColor = Color.Gray;
            this.labAccountBalance.Location = new Point(0x108, 40);
            this.labAccountBalance.Name = "labAccountBalance";
            this.labAccountBalance.Size = new Size(120, 16);
            this.labAccountBalance.TabIndex = 6;
            this.label14.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label14.ForeColor = Color.Gray;
            this.label14.Location = new Point(0x90, 40);
            this.label14.Name = "label14";
            this.label14.Size = new Size(120, 16);
            this.label14.TabIndex = 10;
            this.label14.Text = "Current Balance: ";
            this.label13.Font = new Font("Franklin Gothic Medium", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label13.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.label13.Location = new Point(16, 8);
            this.label13.Name = "label13";
            this.label13.Size = new Size(280, 0x16);
            this.label13.TabIndex = 9;
            this.label13.Text = "Account History for  Account Number";
            this.linkLabel1.Location = new Point(0x1c8, 8);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new Size(80, 16);
            this.linkLabel1.TabIndex = 8;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Back To Main";
            this.linkLabel1.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkBack2_LinkClicked);
            this.panTransactions.Location = new Point(0x18, 0x40);
            this.panTransactions.Name = "panTransactions";
            this.panTransactions.Size = new Size(0x1f8, 0x70);
            this.panTransactions.TabIndex = 0;
            this.panTransactions.Paint += new PaintEventHandler(this.panTransactions_Paint);
            this.panPayBills.AutoScroll = true;
            this.panPayBills.BackColor = Color.White;
            this.panPayBills.BorderStyle = BorderStyle.Fixed3D;
            this.panPayBills.Controls.Add(this.cboPayAccount);
            this.panPayBills.Controls.Add(this.label8);
            this.panPayBills.Controls.Add(this.btnSchedulePayments);
            this.panPayBills.Controls.Add(this.label5);
            this.panPayBills.Controls.Add(this.label4);
            this.panPayBills.Controls.Add(this.linkBack2);
            this.panPayBills.Controls.Add(this.panPayees);
            this.panPayBills.Location = new Point(0x1b0, 320);
            this.panPayBills.Name = "panPayBills";
            this.panPayBills.Size = new Size(560, 0x90);
            this.panPayBills.TabIndex = 3;
            this.panPayBills.Visible = false;
            this.cboPayAccount.Location = new Point(0xd0, 8);
            this.cboPayAccount.Name = "cboPayAccount";
            this.cboPayAccount.Size = new Size(0xa8, 0x15);
            this.cboPayAccount.TabIndex = 14;
            this.label8.Font = new Font("Franklin Gothic Medium", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label8.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.label8.Location = new Point(16, 8);
            this.label8.Name = "label8";
            this.label8.Size = new Size(0xb8, 0x16);
            this.label8.TabIndex = 13;
            this.label8.Text = "Pay Bills from Account";
            this.btnSchedulePayments.BackColor = SystemColors.Control;
            this.btnSchedulePayments.Location = new Point(0xb8, 0x70);
            this.btnSchedulePayments.Name = "btnSchedulePayments";
            this.btnSchedulePayments.Size = new Size(0xb0, 0x18);
            this.btnSchedulePayments.TabIndex = 12;
            this.btnSchedulePayments.Text = "Schedule Payments";
            this.btnSchedulePayments.Click += new EventHandler(this.btnSchedulePayments_Click);
            this.label5.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label5.ForeColor = Color.Gray;
            this.label5.Location = new Point(280, 40);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x58, 16);
            this.label5.TabIndex = 10;
            this.label5.Text = "Amount";
            this.label4.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label4.ForeColor = Color.Gray;
            this.label4.Location = new Point(0x40, 40);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x60, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Payee Name";
            this.linkBack2.Location = new Point(0x1c8, 8);
            this.linkBack2.Name = "linkBack2";
            this.linkBack2.Size = new Size(80, 16);
            this.linkBack2.TabIndex = 8;
            this.linkBack2.TabStop = true;
            this.linkBack2.Text = "Back To Main";
            this.linkBack2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkBack2_LinkClicked);
            this.panPayees.Location = new Point(0x38, 80);
            this.panPayees.Name = "panPayees";
            this.panPayees.Size = new Size(0x180, 16);
            this.panPayees.TabIndex = 0;
            this.panTransfer.AutoScroll = true;
            this.panTransfer.BackColor = Color.White;
            this.panTransfer.BorderStyle = BorderStyle.Fixed3D;
            this.panTransfer.Controls.Add(this.labToBal);
            this.panTransfer.Controls.Add(this.labFromBal);
            this.panTransfer.Controls.Add(this.btnTransfer);
            this.panTransfer.Controls.Add(this.updAmount);
            this.panTransfer.Controls.Add(this.label12);
            this.panTransfer.Controls.Add(this.cboTo);
            this.panTransfer.Controls.Add(this.cboFrom);
            this.panTransfer.Controls.Add(this.label10);
            this.panTransfer.Controls.Add(this.label11);
            this.panTransfer.Controls.Add(this.label9);
            this.panTransfer.Controls.Add(this.linkLabel2);
            this.panTransfer.Controls.Add(this.label6);
            this.panTransfer.Controls.Add(this.label7);
            this.panTransfer.Location = new Point(0x1b0, 0x1f8);
            this.panTransfer.Name = "panTransfer";
            this.panTransfer.Size = new Size(560, 200);
            this.panTransfer.TabIndex = 4;
            this.panTransfer.Visible = false;
            this.labToBal.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labToBal.ForeColor = Color.Gray;
            this.labToBal.Location = new Point(0x1a8, 0x58);
            this.labToBal.Name = "labToBal";
            this.labToBal.Size = new Size(0x68, 16);
            this.labToBal.TabIndex = 0x12;
            this.labToBal.TextAlign = ContentAlignment.TopRight;
            this.labFromBal.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.labFromBal.ForeColor = Color.Gray;
            this.labFromBal.Location = new Point(0x1a8, 0x38);
            this.labFromBal.Name = "labFromBal";
            this.labFromBal.Size = new Size(0x68, 16);
            this.labFromBal.TabIndex = 0x11;
            this.labFromBal.TextAlign = ContentAlignment.TopRight;
            this.btnTransfer.BackColor = SystemColors.Control;
            this.btnTransfer.Location = new Point(200, 0xa8);
            this.btnTransfer.Name = "btnTransfer";
            this.btnTransfer.Size = new Size(0x98, 0x18);
            this.btnTransfer.TabIndex = 16;
            this.btnTransfer.Text = "Perform Transfer";
            this.btnTransfer.Click += new EventHandler(this.btnTransfer_Click);
            this.updAmount.Location = new Point(0xd8, 0x80);
            this.updAmount.Name = "updAmount";
            this.updAmount.Size = new Size(0x70, 20);
            this.updAmount.TabIndex = 15;
            this.updAmount.TextAlign = HorizontalAlignment.Right;
            this.updAmount.ThousandsSeparator = true;
            this.label12.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label12.ForeColor = Color.Gray;
            this.label12.Location = new Point(0x80, 0x80);
            this.label12.Name = "label12";
            this.label12.Size = new Size(0x48, 16);
            this.label12.TabIndex = 14;
            this.label12.Text = "Amount: ";
            this.cboTo.Location = new Point(0x68, 0x58);
            this.cboTo.Name = "cboTo";
            this.cboTo.Size = new Size(0xa8, 0x15);
            this.cboTo.TabIndex = 13;
            this.cboTo.Text = "comboBox2";
            this.cboTo.SelectedIndexChanged += new EventHandler(this.cboTo_SelectedIndexChanged);
            this.cboFrom.Location = new Point(0x68, 0x38);
            this.cboFrom.Name = "cboFrom";
            this.cboFrom.Size = new Size(0xa8, 0x15);
            this.cboFrom.TabIndex = 12;
            this.cboFrom.Text = "comboBox1";
            this.cboFrom.SelectedIndexChanged += new EventHandler(this.cboFrom_SelectedIndexChanged);
            this.label10.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label10.ForeColor = Color.Gray;
            this.label10.Location = new Point(40, 0x58);
            this.label10.Name = "label10";
            this.label10.Size = new Size(0x30, 16);
            this.label10.TabIndex = 11;
            this.label10.Text = "To: ";
            this.label11.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label11.ForeColor = Color.Gray;
            this.label11.Location = new Point(0x128, 0x58);
            this.label11.Name = "label11";
            this.label11.Size = new Size(120, 16);
            this.label11.TabIndex = 10;
            this.label11.Text = "Current Balance: ";
            this.label9.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label9.ForeColor = Color.Gray;
            this.label9.Location = new Point(40, 0x38);
            this.label9.Name = "label9";
            this.label9.Size = new Size(0x30, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "From: ";
            this.linkLabel2.Location = new Point(0x1c8, 8);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new Size(80, 16);
            this.linkLabel2.TabIndex = 8;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Back To Main";
            this.linkLabel2.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkBack2_LinkClicked);
            this.label6.Font = new Font("Franklin Gothic Medium", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label6.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.label6.Location = new Point(16, 8);
            this.label6.Name = "label6";
            this.label6.Size = new Size(0x198, 0x16);
            this.label6.TabIndex = 7;
            this.label6.Text = "Transfer Funds";
            this.label7.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label7.ForeColor = Color.Gray;
            this.label7.Location = new Point(0x128, 0x38);
            this.label7.Name = "label7";
            this.label7.Size = new Size(120, 16);
            this.label7.TabIndex = 6;
            this.label7.Text = "Current Balance: ";
            this.panRecurring.AutoScroll = true;
            this.panRecurring.BackColor = Color.White;
            this.panRecurring.BorderStyle = BorderStyle.Fixed3D;
            this.panRecurring.Controls.Add(this.label18);
            this.panRecurring.Controls.Add(this.cboPayAccount2);
            this.panRecurring.Controls.Add(this.label15);
            this.panRecurring.Controls.Add(this.btnDoneRecurring);
            this.panRecurring.Controls.Add(this.label16);
            this.panRecurring.Controls.Add(this.label17);
            this.panRecurring.Controls.Add(this.linkLabel3);
            this.panRecurring.Controls.Add(this.panPayees2);
            this.panRecurring.Location = new Point(16, 400);
            this.panRecurring.Name = "panRecurring";
            this.panRecurring.Size = new Size(0x188, 0x138);
            this.panRecurring.TabIndex = 5;
            this.panRecurring.Visible = false;
            this.label18.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label18.ForeColor = Color.Gray;
            this.label18.Location = new Point(0x19c, 0x30);
            this.label18.Name = "label18";
            this.label18.Size = new Size(0x58, 32);
            this.label18.TabIndex = 15;
            this.label18.Text = "Day of Month to Pay";
            this.label18.TextAlign = ContentAlignment.TopCenter;
            this.cboPayAccount2.Location = new Point(0xd0, 0x18);
            this.cboPayAccount2.Name = "cboPayAccount2";
            this.cboPayAccount2.Size = new Size(0xa8, 0x15);
            this.cboPayAccount2.TabIndex = 14;
            this.cboPayAccount2.SelectedIndexChanged += new EventHandler(this.cboPayAccount2_SelectedIndexChanged);
            this.label15.Font = new Font("Franklin Gothic Medium", 12f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label15.ForeColor = Color.FromArgb(0, 0xc0, 0);
            this.label15.Location = new Point(16, 8);
            this.label15.Name = "label15";
            this.label15.Size = new Size(0xb8, 40);
            this.label15.TabIndex = 13;
            this.label15.Text = "Setup Recurring Monthly Payments from";
            this.btnDoneRecurring.BackColor = SystemColors.Control;
            this.btnDoneRecurring.Location = new Point(0xb8, 0x88);
            this.btnDoneRecurring.Name = "btnDoneRecurring";
            this.btnDoneRecurring.Size = new Size(0xb0, 0x18);
            this.btnDoneRecurring.TabIndex = 12;
            this.btnDoneRecurring.Text = "Done";
            this.btnDoneRecurring.Click += new EventHandler(this.btnDoneRecurring_Click);
            this.label16.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label16.ForeColor = Color.Gray;
            this.label16.Location = new Point(280, 0x40);
            this.label16.Name = "label16";
            this.label16.Size = new Size(0x58, 16);
            this.label16.TabIndex = 10;
            this.label16.Text = "Amount";
            this.label17.Font = new Font("Franklin Gothic Medium", 9.75f, FontStyle.Bold, GraphicsUnit.Point, 0);
            this.label17.ForeColor = Color.Gray;
            this.label17.Location = new Point(0x40, 0x40);
            this.label17.Name = "label17";
            this.label17.Size = new Size(0x60, 16);
            this.label17.TabIndex = 9;
            this.label17.Text = "Payee Name";
            this.linkLabel3.Location = new Point(0x1c8, 8);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new Size(80, 16);
            this.linkLabel3.TabIndex = 8;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Back To Main";
            this.linkLabel3.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkBack2_LinkClicked);
            this.panPayees2.Location = new Point(0x38, 0x68);
            this.panPayees2.Name = "panPayees2";
            this.panPayees2.Size = new Size(0x1c8, 16);
            this.panPayees2.TabIndex = 0;
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x3e8, 0x2e6);
            base.Controls.Add(this.panHome);
            base.Controls.Add(this.panRecurring);
            base.Controls.Add(this.panTransfer);
            base.Controls.Add(this.panPayBills);
            base.Controls.Add(this.panAccountDetail);
            base.Controls.Add(this.panel1);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.Name = "frmOnlineBanking";
            base.ShowInTaskbar = false;
            this.Text = "Online Banking";
            this.panel1.ResumeLayout(false);
            this.panHome.ResumeLayout(false);
            this.panAccountDetail.ResumeLayout(false);
            this.panPayBills.ResumeLayout(false);
            this.panTransfer.ResumeLayout(false);
            this.updAmount.EndInit();
            this.panRecurring.ResumeLayout(false);
            base.ResumeLayout(false);
        }

        private void linkAccountDetails_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel) sender;
            BankAccount account = (BankAccount) this.bankAccounts[(long) label.Tag];
            this.account = account;
            this.labAccountNumber.Text = account.AccountNumber.ToString();
            this.labAccountBalance.Text = Utilities.FC(account.EndingBalance(), 2, A.I.CurrencyConversion);
            this.panHome.Visible = false;
            this.panAccountDetail.Visible = true;
            this.panAccountDetail.Dock = DockStyle.Fill;
        }

        private void linkBack2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.ReturnToHome();
        }

        private void linkLabel3_Click(object sender, EventArgs e)
        {
            this.ReturnToHome();
        }

        private void linkPaybills_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.panHome.Visible = false;
            this.panPayBills.Visible = true;
            this.panPayBills.Dock = DockStyle.Fill;
            this.panPayees.Controls.Clear();
            this.cboPayAccount.Items.Clear();
            int y = 5;
            if (this.panPayees.Controls.Count == 0)
            {
                foreach (BankAccount account in this.payees)
                {
                    Label label = new Label {
                        AutoSize = true,
                        Location = new Point(5, y),
                        Text = account.BankName
                    };
                    if (account is InstallmentLoan)
                    {
                        label.Text = label.Text + A.R.GetString(" Acct# {0}", new object[] { account.AccountNumber });
                    }
                    TextBox box = new TextBox {
                        TextAlign = HorizontalAlignment.Right,
                        Location = new Point(220, y),
                        Tag = account
                    };
                    this.panPayees.Controls.Add(box);
                    this.panPayees.Controls.Add(label);
                    y += 0x23;
                }
                this.panPayees.Height = y;
                this.btnSchedulePayments.Top = this.panPayees.Bottom + 5;
            }
            foreach (BankAccount account2 in this.bankAccounts.Values)
            {
                if (account2 is CheckingAccount)
                {
                    this.cboPayAccount.Items.Add(account2);
                }
            }
            if (this.cboPayAccount.Items.Count > 0)
            {
                this.cboPayAccount.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show(A.R.GetString("You need a checking account at this bank to pay bills."), A.R.GetString("Pay Bills"));
                this.ReturnToHome();
            }
        }

        private void linkRecurring_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.panHome.Visible = false;
            this.panRecurring.Visible = true;
            this.panRecurring.Dock = DockStyle.Fill;
            this.panPayees2.Controls.Clear();
            this.cboPayAccount2.Items.Clear();
            int y = 5;
            foreach (BankAccount account in this.payees)
            {
                RecurringPaymentControl control = new RecurringPaymentControl {
                    Location = new Point(0, y)
                };
                control.PayeeName.Text = account.BankName;
                if (account is InstallmentLoan)
                {
                    control.PayeeName.Text = control.PayeeName.Text + A.R.GetString(" Acct# {0}", new object[] { account.AccountNumber });
                }
                control.PayeeAccountNumber = account.AccountNumber;
                this.panPayees2.Controls.Add(control);
                y += 0x23;
            }
            this.panPayees2.Height = y;
            this.btnDoneRecurring.Top = this.panPayees2.Bottom + 5;
            foreach (BankAccount account2 in this.bankAccounts.Values)
            {
                if (account2 is CheckingAccount)
                {
                    this.cboPayAccount2.Items.Add(account2);
                }
            }
            if (this.cboPayAccount2.Items.Count > 0)
            {
                this.cboPayAccount2.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show(A.R.GetString("You need a checking account at this bank to pay bills."), A.R.GetString("Pay Bills"));
                this.ReturnToHome();
            }
        }

        private void linkTransfer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (this.bankAccounts.Count < 2)
            {
                MessageBox.Show(A.R.GetString("You need at least two accounts open to transfer funds between them."), A.R.GetString("Cannot Transfer"));
            }
            else
            {
                this.panHome.Visible = false;
                this.panTransfer.Visible = true;
                this.panTransfer.Dock = DockStyle.Fill;
                this.cboFrom.Items.Clear();
                foreach (BankAccount account in this.bankAccounts.Values)
                {
                    this.cboFrom.Items.Add(account);
                }
                this.cboFrom.SelectedIndex = 0;
                this.updAmount.Value = 0M;
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void panTransactions_Paint(object sender, PaintEventArgs e)
        {
            int margin = this.margin;
            Graphics graphics = e.Graphics;
            string[] strArray = new string[] { A.R.GetString("Date"), A.R.GetString("Description"), A.R.GetString("Debit"), A.R.GetString("Credit"), A.R.GetString("Balance") };
            graphics.DrawString(strArray[0], fontS, brush, (float) this.margin, (float) margin);
            graphics.DrawString(strArray[1], fontS, brush, (float) this.colWidth, (float) margin);
            for (int i = 2; i < 5; i++)
            {
                graphics.DrawString(strArray[i], fontS, brush, (float) ((i + 2) * this.colWidth), (float) margin, sfr);
            }
            margin += 20;
            margin += this.margin;
            ArrayList list = (ArrayList) this.account.Transactions.Clone();
            list.Reverse();
            float num3 = this.account.EndingBalance();
            foreach (Transaction transaction in list)
            {
                graphics.DrawString(transaction.Date.ToString("M/d/yyyy"), font, brush, (float) this.margin, (float) margin);
                graphics.DrawString(transaction.Description, font, brush, new RectangleF((float) this.colWidth, (float) margin, (float) ((int) (2.5f * this.colWidth)), 45f));
                if (transaction.Type == Transaction.TranType.Debit)
                {
                    graphics.DrawString(transaction.Amount.ToString("N2"), font, brush, (float) (4 * this.colWidth), (float) margin, sfr);
                }
                else
                {
                    graphics.DrawString(transaction.Amount.ToString("N2"), font, brush, (float) (5 * this.colWidth), (float) margin, sfr);
                }
                graphics.DrawString(num3.ToString("N2"), font, brush, (float) (6 * this.colWidth), (float) margin, sfr);
                if (transaction.Type == Transaction.TranType.Debit)
                {
                    num3 += transaction.Amount;
                }
                else
                {
                    num3 -= transaction.Amount;
                }
                margin += 0x2d;
            }
            this.panTransactions.Height = margin + 0x1f;
        }

        protected void ReturnToHome()
        {
            this.panPayBills.Visible = false;
            this.panAccountDetail.Visible = false;
            this.panTransfer.Visible = false;
            this.panRecurring.Visible = false;
            this.panHome.Visible = true;
            this.panHome.Dock = DockStyle.Fill;
        }

        protected string URL
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
                this.labBankName.Text = this.url;
                this.ReturnToHome();
                int num = 0;
                this.panAccounts.Controls.Clear();
                foreach (BankAccount account in this.bankAccounts.Values)
                {
                    if (account.BankURL == this.URL)
                    {
                        LinkLabel label = new LinkLabel {
                            Text = account.AccountTypeFriendlyName + " # " + account.AccountNumber,
                            Tag = account.AccountNumber,
                            Location = new Point(0, num++ * 0x19)
                        };
                        label.Size = new Size(label.Width + 10, label.Height);
                        label.LinkClicked += new LinkLabelLinkClickedEventHandler(this.linkAccountDetails_LinkClicked);
                        this.panAccounts.Controls.Add(label);
                    }
                }
            }
        }
    }
}

