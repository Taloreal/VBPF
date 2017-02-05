namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;

    [Serializable]
    public class AccountantsReport : TaxReturn
    {
        public int FedWT;
        public int Owed;
        public string Report;
        public int Tax;

        public AccountantsReport(int year) : base(year)
        {
        }

        public void PrepareTaxes(int year, AppEntity e, bool itemizeIfBetter)
        {
            int num = 0x2102;
            float amount = 0f;
            float num3 = 0f;
            int stateWT = 0;
            string newLine = Environment.NewLine;
            this.FedWT = 0;
            this.Report = this.Report + A.R.GetString("Tax Professional's Tax Return for {0}" + newLine + "Tax Year {1}" + newLine + newLine, new object[] { e.Name, year }).ToUpper();
            foreach (FW2 fw in e.FW2s.Values)
            {
                if (fw.Year() == year)
                {
                    amount += fw.Wages;
                    this.FedWT += (int) Math.Ceiling((double) fw.FedWT);
                    stateWT = (int) fw.StateWT;
                }
            }
            this.Report = this.Report + A.R.GetString("  Wages: {0}" + newLine, new object[] { Utilities.FC(amount, A.I.CurrencyConversion) });
            foreach (F1099Int num5 in e.F1099s.Values)
            {
                if (num5.Year() == year)
                {
                    num3 += num5.Interest;
                }
            }
            this.Report = this.Report + A.R.GetString("+ Interest Income: {0}" + newLine, new object[] { Utilities.FC(num3, A.I.CurrencyConversion) });
            float num6 = 0f;
            foreach (InvestmentAccount account in e.InvestmentAccounts.Values)
            {
                foreach (Transaction transaction in account.Transactions)
                {
                    if ((transaction.Date.Year == year) && (transaction.Description == A.R.GetString("Dividend Purchase")))
                    {
                        num6 += transaction.Amount * account.Fund.PriceOn(transaction.Date);
                    }
                }
            }
            this.Report = this.Report + A.R.GetString("+ Ordinary Dividends: {0}" + newLine, new object[] { Utilities.FC(num6, A.I.CurrencyConversion) });
            float num7 = 0f;
            if (e.STCapitalGains.ContainsKey(year))
            {
                num7 = (float) e.STCapitalGains[year];
            }
            this.Report = this.Report + A.R.GetString("+ Short-term Capital Gains: {0}" + newLine, new object[] { Utilities.FC(num7, A.I.CurrencyConversion) });
            int num8 = (int) Math.Round((double) (((Math.Round((double) amount) + Math.Round((double) num3)) + Math.Round((double) num6)) + Math.Round((double) num7)));
            this.Report = this.Report + A.R.GetString("= Total Ordinary Income: {0}" + newLine, new object[] { Utilities.FC((float) num8, A.I.CurrencyConversion) });
            int num9 = num;
            if (itemizeIfBetter)
            {
                int num10 = 0;
                foreach (Mortgage mortgage in e.Mortgages.Values)
                {
                    num10 += mortgage.InterestPaid(year);
                }
                if ((num10 + stateWT) > num)
                {
                    num9 = num10 + stateWT;
                    this.Report = this.Report + A.R.GetString("- Itemized Deduction: {0}" + newLine, new object[] { Utilities.FC((float) num9, A.I.CurrencyConversion) });
                    if (num10 > 0)
                    {
                        this.Report = this.Report + A.R.GetString("    Mortgage Interest: {0}" + newLine, new object[] { Utilities.FC((float) num10, A.I.CurrencyConversion) });
                    }
                    this.Report = this.Report + A.R.GetString("    State Taxes: {0}" + newLine, new object[] { Utilities.FC((float) stateWT, A.I.CurrencyConversion) });
                }
            }
            if (num9 == num)
            {
                this.Report = this.Report + A.R.GetString("- Standard Deduction: {0}" + newLine, new object[] { Utilities.FC((float) num9, A.I.CurrencyConversion) });
            }
            int num11 = Math.Max(0, num8 - num9);
            this.Report = this.Report + A.R.GetString("= Taxable Ordinary Income: {0}" + newLine, new object[] { Utilities.FC((float) num11, A.I.CurrencyConversion) });
            float[] dollarBrackets = new float[] { 0f, 7550f, 30650f, 74200f, 154800f, 336550f };
            this.Tax = (int) Math.Round((double) F1040EZ.ComputeTax((float) num11, dollarBrackets, new float[] { 0.1f, 0.15f, 0.25f, 0.28f, 0.33f, 0.35f }));
            this.Report = this.Report + A.R.GetString("  Tax On Ordinary Income: {0}" + newLine + newLine, new object[] { Utilities.FC((float) this.Tax, A.I.CurrencyConversion) });
            float num12 = 0f;
            if (e.LTCapitalGains.ContainsKey(year))
            {
                num12 = (float) e.LTCapitalGains[year];
            }
            this.Report = this.Report + A.R.GetString("  Long-term Capital Gains: {0}" + newLine, new object[] { Utilities.FC(num12, A.I.CurrencyConversion) });
            float num13 = 0.05f;
            if ((num11 + ((int) num12)) > dollarBrackets[1])
            {
                num13 = 0.15f;
            }
            int num14 = (int) (num13 * ((int) num12));
            this.Report = this.Report + A.R.GetString("x Capital Gains Tax Rate: {0}" + newLine, new object[] { Utilities.FP(num13) });
            this.Report = this.Report + A.R.GetString("= Tax on Long-term Capital Gains: {0}" + newLine + newLine, new object[] { Utilities.FC((float) num14, A.I.CurrencyConversion) });
            this.Tax += num14;
            this.Owed = this.Tax - this.FedWT;
            this.Report = this.Report + A.R.GetString("  Total Tax Liability: {0}" + newLine, new object[] { Utilities.FC((float) this.Tax, A.I.CurrencyConversion) });
            this.Report = this.Report + A.R.GetString("- Total Federal Withholding: {0}" + newLine, new object[] { Utilities.FC((float) this.FedWT, A.I.CurrencyConversion) });
            if (this.Owed > 0)
            {
                this.Report = this.Report + A.R.GetString("= Tax Due: {0}" + newLine, new object[] { Utilities.FC((float) this.Owed, A.I.CurrencyConversion) });
                base.Values[8] = this.Owed;
            }
            else
            {
                this.Report = this.Report + A.R.GetString("= Refund: {0}" + newLine, new object[] { Utilities.FC((float) -this.Owed, A.I.CurrencyConversion) });
                base.Values[7] = -this.Owed;
            }
        }
    }
}

