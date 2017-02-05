namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;

    [Serializable]
    public class DwellingOffer : Offering
    {
        public bool Condo = false;

        public override string Description()
        {
            if (this.Condo)
            {
                return A.R.GetString("One bedroom condo for sale! Priced to move at {0}.", new object[] { Utilities.FC(base.Building.Rent * A.ST.RealEstateIndex, A.I.CurrencyConversion) });
            }
            return A.R.GetString("One bedroom apartment. {0}/month. One year lease. Month-to-month after one year.", new object[] { Utilities.FC((float) base.Building.Rent, A.I.CurrencyConversion) });
        }

        public override string JournalEntry()
        {
            if (this.Condo)
            {
                return A.R.GetString("Bought condo for {0}.", new object[] { Utilities.FC(base.Building.Rent * A.ST.RealEstateIndex, A.I.CurrencyConversion) });
            }
            return A.R.GetString("Leased apartment for {0} per month.", new object[] { Utilities.FC((float) base.Building.Rent, A.I.CurrencyConversion) });
        }

        public override string ThingName()
        {
            return A.R.GetString("Housing");
        }
    }
}

