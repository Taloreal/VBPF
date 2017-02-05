namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;
    using System.Collections;

    [Serializable]
    public class CheckingAccount : BankAccount
    {
        public ArrayList Checks = new ArrayList();
        public ArrayList ChecksInTheMail = new ArrayList();
        protected int nextCheckNumber = 100;
        public ArrayList RecurringPayments = new ArrayList();
        public ArrayList RegisterEntries = new ArrayList();

        public CheckingAccount(float maintenanceFee, float minimumBalance)
        {
            base.maintenanceFee = maintenanceFee;
            base.minimumBalance = minimumBalance;
        }

        public ArrayList BuildRegisterFromTransactions()
        {
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList(base.Transactions);
            foreach (KMI.VBPF1Lib.Check check in this.ChecksInTheMail)
            {
                int index = 0;
                while ((index < list2.Count) && (check.Date > ((Transaction) list2[index]).Date))
                {
                    index++;
                }
                list2.Insert(index, new Transaction(check.Amount, Transaction.TranType.Debit, A.R.GetString("Check # {0} to {1}", new object[] { check.Number, check.Payee })));
            }
            int num2 = Math.Min(list2.Count, 40);
            float num3 = 0f;
            if (list2.Count > 40)
            {
                num3 = base.BalanceThru((Transaction) list2[list2.Count - 0x29]);
            }
            for (int i = list2.Count - num2; i < list2.Count; i++)
            {
                Transaction transaction = (Transaction) list2[i];
                CheckRegisterEntry entry = new CheckRegisterEntry("", transaction.Date.ToString("M/d/yy"), transaction.Description, "", "", "", "", "");
                float amount = 0f;
                if (transaction.Type == Transaction.TranType.Debit)
                {
                    entry.Payment = transaction.Amount.ToString("N2");
                    entry.Balance1 = "-" + entry.Payment;
                    amount = -transaction.Amount;
                }
                else
                {
                    entry.Deposit = transaction.Amount.ToString("N2");
                    entry.Balance1 = "+" + entry.Deposit;
                    amount = transaction.Amount;
                }
                if ((i > 0) || (list2.Count <= 40))
                {
                    num3 += amount;
                }
                entry.Balance2 = num3.ToString("N2");
                if (transaction.Description.StartsWith("Check #"))
                {
                    entry.Number = transaction.Description.Split(new char[] { ' ' })[2];
                    entry.Description1 = transaction.Description.Substring(transaction.Description.IndexOf(" to ") + 4);
                }
                list.Add(entry);
            }
            return list;
        }

        public override string Description()
        {
            return A.R.GetString("Checking account offering free checking. Monthly fee of {0} waived if minimum daily balance greater than {1}.", new object[] { Utilities.FC(base.maintenanceFee, A.I.CurrencyConversion), Utilities.FC(base.minimumBalance, A.I.CurrencyConversion) });
        }

        public override string AccountTypeFriendlyName
        {
            get
            {
                return A.R.GetString("Checking");
            }
        }

        public int NextCheckNumber
        {
            get
            {
                return this.nextCheckNumber;
            }
            set
            {
                this.nextCheckNumber = value;
            }
        }
    }
}

