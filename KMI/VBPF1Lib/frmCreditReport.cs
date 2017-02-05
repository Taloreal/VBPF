namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public class frmCreditReport : frmDrawnReport
    {
        protected static Brush brush = new SolidBrush(Color.Black);
        protected int col1Width = 120;
        protected int colWidth = 60;
        protected static Font font = new Font("Arial", 8f);
        protected static Font fontB = new Font("Arial", 8f, FontStyle.Bold);
        protected static Font fontL = new Font("Arial", 16f);
        protected static Font fontS = new Font("Arial", 7f);
        private Input input;
        protected int margin = 10;
        protected static Pen pen = new Pen(brush, 1f);
        protected static StringFormat sfc = new StringFormat();
        protected static StringFormat sfr = new StringFormat();
        protected int spacing = 14;

        public frmCreditReport()
        {
            this.InitializeComponent();
            sfr.Alignment = StringAlignment.Far;
            sfc.Alignment = StringAlignment.Center;
        }

        protected override void DrawReportVirtual(Graphics g)
        {
            int margin = this.margin;
            g.DrawString(A.R.GetString("Credit Report"), fontL, brush, (float) this.margin, (float) margin);
            margin += 3 * this.spacing;
            g.DrawString(A.R.GetString("PERSONAL IDENTIFICATION INFORMATION"), fontB, brush, (float) this.margin, (float) margin);
            margin += this.spacing;
            g.FillRectangle(brush, (float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 3f);
            margin += this.spacing;
            g.DrawString(this.input.Name, font, brush, (float) this.margin, (float) margin);
            g.DrawString(A.R.GetString("Social Security #: {0}", new object[] { this.input.SSN }), font, brush, g.ClipBounds.Width / 2f, (float) margin);
            margin += this.spacing;
            g.DrawString(A.R.GetString("Springfield, USA"), font, brush, (float) this.margin, (float) margin);
            margin += 2 * this.spacing;
            g.DrawString(A.R.GetString("PUBLIC RECORD INFORMATION"), fontB, brush, (float) this.margin, (float) margin);
            margin += this.spacing;
            g.FillRectangle(brush, (float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 3f);
            margin += 2 * this.spacing;
            g.DrawString(A.R.GetString("COLLECTION AGENCY ACCOUNT INFORMATION"), fontB, brush, (float) this.margin, (float) margin);
            margin += this.spacing;
            g.FillRectangle(brush, (float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 3f);
            margin += this.spacing;
            foreach (BankAccount account in this.input.Accounts)
            {
                switch (account.PastDueStatusPassive(this.input.Now.Year, this.input.Now.Month))
                {
                    case BankAccount.Status.NewlyCancelled:
                    case BankAccount.Status.Cancelled:
                        g.DrawString(A.R.GetString("In process of collection. Client: {0} Amount: {1}", new object[] { account.BankName, Utilities.FC(account.EndingBalance(), A.I.CurrencyConversion) }), font, brush, (float) this.margin, (float) margin);
                        margin += this.spacing;
                        break;
                }
            }
            margin += this.spacing;
            g.DrawString(A.R.GetString("CREDIT ACCOUNT INFORMATION"), fontB, brush, (float) this.margin, (float) margin);
            margin += this.spacing;
            g.FillRectangle(brush, (float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 3f);
            margin += 2 * this.spacing;
            int num2 = 0;
            foreach (string str in new string[] { "COMPANY", "ACCOUNT", "HIGH", "", "PAST", "" })
            {
                g.DrawString(str, fontS, brush, (float) ((1 + num2++) * this.colWidth), (float) margin, sfc);
            }
            margin += this.spacing;
            num2 = 0;
            foreach (string str in new string[] { "NAME", "NUMBER", "CREDIT", "BALANCE", "DUE", "STATUS" })
            {
                g.DrawString(str, fontS, brush, (float) ((1 + num2++) * this.colWidth), (float) margin, sfc);
            }
            margin += this.spacing;
            g.FillRectangle(brush, (float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 1f);
            margin += this.spacing;
            int num3 = 0;
            int num4 = 0;
            foreach (BankAccount account2 in this.input.Accounts)
            {
                if ((account2 is InstallmentLoan) || (account2 is CreditCardAccount))
                {
                    float amount = 0f;
                    if (account2 is InstallmentLoan)
                    {
                        amount = ((InstallmentLoan) account2).OriginalBalance;
                        num3 += ((InstallmentLoan) account2).MissedPayments();
                    }
                    else if (account2 is CreditCardAccount)
                    {
                        amount = ((CreditCardAccount) account2).CreditLimit;
                        num4 += ((CreditCardAccount) account2).MissedPayments();
                    }
                    g.DrawString(account2.BankName.Substring(0, Math.Min(15, account2.BankName.Length)), font, brush, (float) this.margin, (float) margin);
                    g.DrawString(account2.AccountNumber.ToString(), font, brush, this.margin + (1.5f * this.colWidth), (float) margin);
                    g.DrawString(Utilities.FC(amount, A.I.CurrencyConversion), font, brush, this.margin + (2.6f * this.colWidth), (float) margin);
                    g.DrawString(Utilities.FC(account2.EndingBalance(), A.I.CurrencyConversion), font, brush, this.margin + (3.6f * this.colWidth), (float) margin);
                    g.DrawString(Utilities.FC(account2.PastDueAmount(this.input.Now), A.I.CurrencyConversion), font, brush, this.margin + (4.6f * this.colWidth), (float) margin);
                    g.DrawString(account2.CRStatus(this.input.Now), font, brush, this.margin + (5.6f * this.colWidth), (float) margin);
                    margin += this.spacing;
                }
            }
            margin += (int) (1.5f * this.spacing);
            g.DrawString(A.R.GetString("Previous Payment History:"), fontB, brush, (float) (2 * this.margin), (float) margin);
            margin += this.spacing;
            g.DrawString(A.R.GetString("{0} times late on loan payments", new object[] { num3 }), font, brush, (float) (2 * this.margin), (float) margin);
            margin += this.spacing;
            g.DrawString(A.R.GetString("{0} times late on credit card payments", new object[] { num4 }), font, brush, (float) (2 * this.margin), (float) margin);
            margin += 2 * this.spacing;
            base.picReport.Height = margin + 30;
        }

        protected override void GetDataVirtual()
        {
            this.input = A.SA.GetCreditReport(A.MF.CurrentEntityID);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.pnlBottom.Location = new Point(0, 0x18e);
            base.pnlBottom.Name = "pnlBottom";
            base.pnlBottom.Size = new Size(450, 40);
            base.picReport.BackColor = Color.White;
            base.picReport.Name = "picReport";
            base.picReport.Size = new Size(0x194, 0x174);
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(450, 0x1b6);
            base.Name = "frmCreditReport";
            this.Text = "Credit Report";
            base.ResumeLayout(false);
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public string Name;
            public string SSN;
            public ArrayList Accounts;
            public DateTime Now;
        }
    }
}

