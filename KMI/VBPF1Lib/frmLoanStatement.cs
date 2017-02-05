namespace KMI.VBPF1Lib
{
    using System;
    using System.Collections;

    public class frmLoanStatement : frmBankStatement
    {
        public frmLoanStatement()
        {
            this.Text = A.R.GetString("Loan Statements");
        }

        protected override SortedList GetAccounts()
        {
            return A.SA.GetInstallmentLoans(A.MF.CurrentEntityID);
        }
    }
}

