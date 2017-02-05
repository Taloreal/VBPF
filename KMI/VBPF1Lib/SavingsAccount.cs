namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;

    [Serializable]
    public class SavingsAccount : BankAccount
    {
        public SavingsAccount(float maintenanceFee, float interestRate, float minimumBalance)
        {
            base.maintenanceFee = maintenanceFee;
            base.interestRate = interestRate;
            base.minimumBalance = minimumBalance;
        }

        public override string Description()
        {
            return A.R.GetString("Savings account offering {0} APR interest. Monthly fee of {1} waived if minimum daily balance greater than {2}.", new object[] { Utilities.FP(base.interestRate, 1), Utilities.FC(base.maintenanceFee, A.I.CurrencyConversion), Utilities.FC(base.minimumBalance, A.I.CurrencyConversion) });
        }

        public float Interest(int year)
        {
            float num = 0f;
            foreach (Transaction transaction in base.Transactions)
            {
                if ((transaction.Date.Year == year) && (transaction.Description == A.R.GetString("Interest earned")))
                {
                    num += transaction.Amount;
                }
            }
            return num;
        }

        public override string AccountTypeFriendlyName
        {
            get
            {
                return A.R.GetString("Savings");
            }
        }
    }
}

