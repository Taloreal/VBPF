namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using KMI.VBPF1Lib.Custom_Controls;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Windows.Forms;

    public class frmCheckbook : Form
    {
        private CheckingAccount account;
        private bool autofill;
        private Button btnClose;
        private Button btnHelp;
        private Button btnPrint;
        private Button btnSendCheck;
        private ComboBox cboAccount;
        private CheckControl chkCheck;
        private Container components = null;
        protected Bill currentBill;
        private Label label1;
        private Label label2;
        private Label label21;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Panel panel1;
        private Panel panRegister = new Panel();
        private string studentName;

        public frmCheckbook()
        {
            this.InitializeComponent();
            this.chkCheck = new CheckControl();
            this.chkCheck.Location = new Point(40, 200);
            this.chkCheck.Visible = false;
            base.Controls.Add(this.chkCheck);
            this.autofill = ((AppSimSettings) A.SA.getSimSettings()).AutofillCheckRegister;
            SortedList bankAccounts = A.SA.GetBankAccounts(A.MF.CurrentEntityID);
            foreach (BankAccount account in bankAccounts.Values)
            {
                if (account is CheckingAccount)
                {
                    this.cboAccount.Items.Add(account);
                }
            }
            if (this.cboAccount.Items.Count <= 0)
            {
                throw new SimApplicationException(A.R.GetString("You must open a checking account before you can pay by check or view the check register. In the City View, click on a bank to open an account."));
            }
            this.cboAccount.SelectedIndex = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("Check Register"));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            this.studentName = "";
            frmInputString str = new frmInputString(S.R.GetString("Student Name"), S.R.GetString("Enter your name to help identify your printout on a shared printer:"), this.studentName);
            str.ShowDialog(this);
            this.studentName = str.Response;
            Utilities.PrintWithExceptionHandling(this.Text, new PrintPageEventHandler(this.PrintPage));
        }

        private void btnSendCheck_Click(object sender, EventArgs e)
        {
            KMI.VBPF1Lib.Check check = new KMI.VBPF1Lib.Check(this.Account.AccountNumber);
            if ((this.autofill || this.RegisterEntry(this.chkCheck.labCheckNumber.Text)) || (MessageBox.Show("You have not recorded this check number in your check register. You may lose track of checks you've written. Do you want to continue without writing in your check register?", "No Register Entry", MessageBoxButtons.YesNo) != DialogResult.No))
            {
                try
                {
                    check.Amount = float.Parse(this.chkCheck.txtAmount.Text);
                }
                catch
                {
                    MessageBox.Show(A.R.GetString("We couldn't understand the amount. Please try again."), A.R.GetString("Check Writing Error"));
                    this.chkCheck.txtAmount.SelectAll();
                    this.chkCheck.txtAmount.Focus();
                    return;
                }
                check.Number = this.Account.NextCheckNumber;
                check.Payee = this.chkCheck.labPayee.Text;
                A.SA.PayByCheck(A.MF.CurrentEntityID, this.Account.AccountNumber, this.currentBill, check);
                base.DialogResult = DialogResult.OK;
                base.Close();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ArrayList registerEntries = null;
            this.Account = (CheckingAccount) this.cboAccount.SelectedItem;
            if (this.autofill)
            {
                registerEntries = this.Account.BuildRegisterFromTransactions();
            }
            else
            {
                registerEntries = this.Account.RegisterEntries;
            }
            this.panRegister.Controls.Clear();
            for (int i = 0; i < (registerEntries.Count + 20); i++)
            {
                RegisterLine line = new RegisterLine {
                    Top = i * 32
                };
                this.panRegister.Controls.Add(line);
                if (i < registerEntries.Count)
                {
                    line.FillInFields((CheckRegisterEntry) registerEntries[i]);
                }
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

        public void FillIn(Bill bill)
        {
            this.currentBill = bill;
            this.chkCheck.Visible = true;
            this.chkCheck.Bill = bill;
            this.chkCheck.labPayor.Text = this.Account.OwnerName;
            this.chkCheck.labSignature.Text = this.Account.OwnerName;
            this.chkCheck.labCheckNumber.Text = this.Account.NextCheckNumber.ToString();
            this.chkCheck.labCheckNumberBottom.Text = this.Account.NextCheckNumber.ToString();
            this.chkCheck.labBankName.Text = this.Account.BankName;
            this.chkCheck.labAccountNumber.Text = this.Account.AccountNumber.ToString();
            this.btnSendCheck.Enabled = true;
        }

        private void frmCheckbook_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                ArrayList entries = new ArrayList();
                foreach (RegisterLine line in this.panRegister.Controls)
                {
                    CheckRegisterEntry entry = line.CreateCheckRegisterEntry();
                    if (!entry.IsEmpty())
                    {
                        entries.Add(entry);
                    }
                }
                if (!this.autofill)
                {
                    A.SA.SetRegisterEntries(A.MF.CurrentEntityID, "test", this.Account.AccountNumber, entries);
                }
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception, this);
            }
        }

        private void frmCheckbook_Load(object sender, EventArgs e)
        {
            this.chkCheck.txtAmount.Focus();
        }

        private void InitializeComponent()
        {
            this.panRegister = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.cboAccount = new System.Windows.Forms.ComboBox();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSendCheck = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panRegister
            // 
            this.panRegister.AutoScroll = true;
            this.panRegister.Location = new System.Drawing.Point(0, 40);
            this.panRegister.Name = "panRegister";
            this.panRegister.Size = new System.Drawing.Size(632, 160);
            this.panRegister.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 32);
            this.label1.TabIndex = 2;
            this.label1.Text = "NUMBER";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(48, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 32);
            this.label2.TabIndex = 3;
            this.label2.Text = "DATE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(96, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "DESCRIPTION OF TRANSACTION";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(272, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 32);
            this.label4.TabIndex = 5;
            this.label4.Text = "PAYMENT/DEBIT (-)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(360, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 32);
            this.label5.TabIndex = 6;
            this.label5.Text = "T";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.White;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(384, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 32);
            this.label6.TabIndex = 7;
            this.label6.Text = "FEE       (-)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.White;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(432, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 32);
            this.label7.TabIndex = 8;
            this.label7.Text = "DEPOSIT/CREDIT (+)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.White;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(528, 8);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 32);
            this.label8.TabIndex = 9;
            this.label8.Text = "BALANCE";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.label21);
            this.panel1.Controls.Add(this.cboAccount);
            this.panel1.Controls.Add(this.btnHelp);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnSendCheck);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 398);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(632, 40);
            this.panel1.TabIndex = 10;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(384, 8);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(72, 24);
            this.btnPrint.TabIndex = 5;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(16, 12);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(48, 16);
            this.label21.TabIndex = 4;
            this.label21.Text = "Account:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cboAccount
            // 
            this.cboAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccount.Location = new System.Drawing.Point(64, 8);
            this.cboAccount.Name = "cboAccount";
            this.cboAccount.Size = new System.Drawing.Size(176, 21);
            this.cboAccount.TabIndex = 3;
            this.cboAccount.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(544, 7);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(72, 24);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(464, 7);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSendCheck
            // 
            this.btnSendCheck.Enabled = false;
            this.btnSendCheck.Location = new System.Drawing.Point(264, 8);
            this.btnSendCheck.Name = "btnSendCheck";
            this.btnSendCheck.Size = new System.Drawing.Size(88, 24);
            this.btnSendCheck.TabIndex = 0;
            this.btnSendCheck.Text = "Send Check";
            this.btnSendCheck.Click += new System.EventHandler(this.btnSendCheck_Click);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.Black;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Location = new System.Drawing.Point(0, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(632, 3);
            this.label9.TabIndex = 11;
            // 
            // frmCheckbook
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(632, 438);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panRegister);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmCheckbook";
            this.ShowInTaskbar = false;
            this.Text = "Checkbook";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmCheckbook_Closing);
            this.Load += new System.EventHandler(this.frmCheckbook_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        protected void PrintPage(object sender, PrintPageEventArgs e)
        {
            Utilities.ResetFPU();
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush, 1f);
            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            int y = 10;
            if (this.studentName.Length > 0)
            {
                e.Graphics.DrawString(A.R.GetString("This report belongs to: {0}", new object[] { this.studentName }), this.Font, brush, 0f, (float) y);
            }
            y = 0x29;
            int num2 = 0;
            foreach (Control control in base.Controls)
            {
                if ((control is Label) && (control.Text != ""))
                {
                    Rectangle rect = new Rectangle(control.Left, y, control.Width - 1, control.Height);
                    e.Graphics.DrawRectangle(pen, rect);
                    e.Graphics.DrawString(control.Text, control.Font, brush, rect, format);
                    num2++;
                }
            }
            y += 0x1f;
            foreach (RegisterLine line in this.panRegister.Controls)
            {
                line.Print(e.Graphics, y);
                y += 0x1f;
            }
        }

        protected bool RegisterEntry(string checkNumber)
        {
            foreach (RegisterLine line in this.panRegister.Controls)
            {
                if (line.txtNumber.Text == checkNumber)
                {
                    return true;
                }
            }
            return false;
        }

        protected CheckingAccount Account
        {
            get
            {
                return this.account;
            }
            set
            {
                this.account = value;
                this.chkCheck.labCheckNumber.Text = this.account.NextCheckNumber.ToString();
                this.chkCheck.labCheckNumberBottom.Text = this.chkCheck.labCheckNumber.Text;
            }
        }
    }
}

