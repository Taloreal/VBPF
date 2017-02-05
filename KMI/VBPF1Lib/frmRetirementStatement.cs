namespace KMI.VBPF1Lib
{
    using System;
    using System.Collections;

    public class frmRetirementStatement : frmBankStatement
    {
        public frmRetirementStatement()
        {
            this.Text = A.R.GetString("Retirement Statements");
        }

        protected override SortedList GetAccounts()
        {
            return A.SA.GetInvestmentAccounts(A.MF.CurrentEntityID, true);
        }
    }
}

