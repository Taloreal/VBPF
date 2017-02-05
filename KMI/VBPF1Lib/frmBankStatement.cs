namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Windows.Forms;

    public class frmBankStatement : Form
    {
        protected SortedList bankAccounts;
        private Button btnHelp;
        private Button btnMonthBack;
        private Button btnMonthNext;
        private Button btnPageBack;
        private Button btnPageNext;
        private Button btnPrint;
        private Container components = null;
        protected BankAccount currentAccount;
        protected int currentMonth;
        protected int currentYear;
        protected PrintDocument d = new PrintDocument();
        private Label label1;
        private Label label2;
        private Label label3;
        private Label labNoAccounts;
        private ListBox lstAccount;
        protected int maxMonth;
        protected int maxYear;
        protected int page = 0;
        protected PictureBox picStatement;
        protected int printerPage = 0;
        protected int TransactionsPerPage = 0x1b;
        protected ArrayList transThisMonth;

        public frmBankStatement()
        {
            this.InitializeComponent();
            this.bankAccounts = this.GetAccounts();
            this.maxMonth = A.MF.Now.Month;
            this.maxYear = A.MF.Now.Year;
            if (this.maxMonth == 1)
            {
                this.maxMonth = 12;
                this.maxYear--;
            }
            else
            {
                this.maxMonth--;
            }
            this.currentMonth = this.maxMonth;
            this.currentYear = this.maxYear;
            foreach (BankAccount account in this.bankAccounts.Values)
            {
                this.lstAccount.Items.Add(account);
            }
            if (this.lstAccount.Items.Count > 0)
            {
                this.lstAccount.SelectedIndex = 0;
                this.picStatement.Visible = true;
            }
            this.btnPrint.Enabled = this.currentAccount != null;
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(this.Text);
        }

        private void btnMonthBack_Click(object sender, EventArgs e)
        {
            if (this.currentMonth == 1)
            {
                this.currentMonth = 12;
                this.currentYear--;
            }
            else
            {
                this.currentMonth--;
            }
            this.picStatement.Refresh();
        }

        private void btnMonthForward_Click(object sender, EventArgs e)
        {
            if (this.currentMonth == 12)
            {
                this.currentMonth = 1;
                this.currentYear++;
            }
            else
            {
                this.currentMonth++;
            }
            this.picStatement.Refresh();
        }

        private void btnPageBack_Click(object sender, EventArgs e)
        {
            this.page--;
            this.picStatement.Refresh();
        }

        private void btnPageNext_Click(object sender, EventArgs e)
        {
            this.page++;
            this.picStatement.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.printerPage = 0;
            Utilities.PrintWithExceptionHandling(this.Text, new PrintPageEventHandler(this.Report_PrintPage));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected virtual SortedList GetAccounts()
        {
            return A.SA.GetBankAccounts(A.MF.CurrentEntityID);
        }

        private void InitializeComponent()
        {
            this.btnPrint = new System.Windows.Forms.Button();
            this.picStatement = new System.Windows.Forms.PictureBox();
            this.btnPageNext = new System.Windows.Forms.Button();
            this.btnPageBack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnMonthBack = new System.Windows.Forms.Button();
            this.btnMonthNext = new System.Windows.Forms.Button();
            this.lstAccount = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.labNoAccounts = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picStatement)).BeginInit();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(384, 384);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(72, 24);
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.button1_Click);
            // 
            // picStatement
            // 
            this.picStatement.BackColor = System.Drawing.Color.White;
            this.picStatement.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picStatement.Location = new System.Drawing.Point(0, 0);
            this.picStatement.Name = "picStatement";
            this.picStatement.Size = new System.Drawing.Size(360, 464);
            this.picStatement.TabIndex = 2;
            this.picStatement.TabStop = false;
            this.picStatement.Visible = false;
            this.picStatement.Paint += new System.Windows.Forms.PaintEventHandler(this.picStatement_Paint);
            // 
            // btnPageNext
            // 
            this.btnPageNext.Location = new System.Drawing.Point(424, 320);
            this.btnPageNext.Name = "btnPageNext";
            this.btnPageNext.Size = new System.Drawing.Size(32, 24);
            this.btnPageNext.TabIndex = 3;
            this.btnPageNext.Text = ">>";
            this.btnPageNext.Click += new System.EventHandler(this.btnPageNext_Click);
            // 
            // btnPageBack
            // 
            this.btnPageBack.Location = new System.Drawing.Point(384, 320);
            this.btnPageBack.Name = "btnPageBack";
            this.btnPageBack.Size = new System.Drawing.Size(32, 24);
            this.btnPageBack.TabIndex = 4;
            this.btnPageBack.Text = "<<";
            this.btnPageBack.Click += new System.EventHandler(this.btnPageBack_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(384, 296);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 5;
            this.label1.Text = "Page:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(384, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 8;
            this.label2.Text = "Month:";
            // 
            // btnMonthBack
            // 
            this.btnMonthBack.Location = new System.Drawing.Point(384, 224);
            this.btnMonthBack.Name = "btnMonthBack";
            this.btnMonthBack.Size = new System.Drawing.Size(32, 24);
            this.btnMonthBack.TabIndex = 7;
            this.btnMonthBack.Text = "<<";
            this.btnMonthBack.Click += new System.EventHandler(this.btnMonthBack_Click);
            // 
            // btnMonthNext
            // 
            this.btnMonthNext.Location = new System.Drawing.Point(424, 224);
            this.btnMonthNext.Name = "btnMonthNext";
            this.btnMonthNext.Size = new System.Drawing.Size(32, 24);
            this.btnMonthNext.TabIndex = 6;
            this.btnMonthNext.Text = ">>";
            this.btnMonthNext.Click += new System.EventHandler(this.btnMonthForward_Click);
            // 
            // lstAccount
            // 
            this.lstAccount.HorizontalScrollbar = true;
            this.lstAccount.Location = new System.Drawing.Point(376, 40);
            this.lstAccount.Name = "lstAccount";
            this.lstAccount.Size = new System.Drawing.Size(88, 121);
            this.lstAccount.TabIndex = 9;
            this.lstAccount.SelectedIndexChanged += new System.EventHandler(this.lstAccount_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(376, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 16);
            this.label3.TabIndex = 10;
            this.label3.Text = "Account:";
            // 
            // labNoAccounts
            // 
            this.labNoAccounts.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labNoAccounts.Location = new System.Drawing.Point(80, 192);
            this.labNoAccounts.Name = "labNoAccounts";
            this.labNoAccounts.Size = new System.Drawing.Size(200, 32);
            this.labNoAccounts.TabIndex = 11;
            this.labNoAccounts.Text = "No Accounts Open";
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(384, 424);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(72, 24);
            this.btnHelp.TabIndex = 12;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // frmBankStatement
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(482, 464);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lstAccount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMonthBack);
            this.Controls.Add(this.btnMonthNext);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnPageBack);
            this.Controls.Add(this.btnPageNext);
            this.Controls.Add(this.picStatement);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.labNoAccounts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmBankStatement";
            this.ShowInTaskbar = false;
            this.Text = "Bank Statements";
            ((System.ComponentModel.ISupportInitialize)(this.picStatement)).EndInit();
            this.ResumeLayout(false);

        }

        private void lstAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.currentAccount = (BankAccount) this.lstAccount.SelectedItem;
            this.picStatement.Refresh();
        }

        private void picStatement_Paint(object sender, PaintEventArgs e)
        {
            this.transThisMonth = this.currentAccount.TransactionsForMonth(this.currentYear, this.currentMonth);
            this.btnPageBack.Enabled = this.page > 0;
            this.btnPageNext.Enabled = this.page < (this.Pages - 1);
            this.btnMonthBack.Enabled = (this.currentYear > this.currentAccount.DateOpened.Year) || ((this.currentMonth > this.currentAccount.DateOpened.Month) && (this.currentYear == this.currentAccount.DateOpened.Year));
            this.btnMonthNext.Enabled = (this.currentYear < A.MF.Now.Year) || (this.currentMonth < A.MF.Now.Month);
            if (!A.MF.DesignerMode)
            {
                this.btnMonthNext.Enabled = (this.currentYear < this.maxYear) || (this.currentMonth < this.maxMonth);
            }
            this.PrintPage(this.page, e.Graphics);
        }

        protected virtual bool PrintPage(int page, Graphics g)
        {
            this.currentAccount.PrintPage(page, g, this.currentYear, this.currentMonth, this.Pages, this.TransactionsPerPage);
            return (page < (this.Pages - 1));
        }

        protected void Report_PrintPage(object sender, PrintPageEventArgs e)
        {
            Utilities.ResetFPU();
            e.Graphics.ScaleTransform(2f, 2f);
            e.HasMorePages = this.PrintPage(this.printerPage, e.Graphics);
            this.printerPage++;
        }

        protected int Pages
        {
            get
            {
                if (this.currentAccount == null)
                {
                    return 0;
                }
                return (int) Math.Max(1.0, Math.Ceiling((double) (((float) this.transThisMonth.Count) / ((float) this.TransactionsPerPage))));
            }
        }
    }
}

