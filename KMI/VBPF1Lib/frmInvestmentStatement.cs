namespace KMI.VBPF1Lib
{
    using System;
    using System.Collections;

    public class frmInvestmentStatement : frmBankStatement
    {
        public frmInvestmentStatement()
        {
            this.Text = A.R.GetString("Investment Statements");
        }

        protected override SortedList GetAccounts()
        {
            return A.SA.GetInvestmentAccounts(A.MF.CurrentEntityID, false);
        }
    }
}

