namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;
    using System.Windows.Forms;

    [Serializable]
    public class God : ActiveObject
    {
        private int currentPeriod = -1;

        public God()
        {
            A.I.Subscribe(this, Simulator.TimePeriod.Week);
            A.I.Subscribe(this, Simulator.TimePeriod.Day);
            A.I.Subscribe(this, Simulator.TimePeriod.Step);
            A.I.Subscribe(this, Simulator.TimePeriod.Year);
        }

        public void DoInflation()
        {
            double inflationPer = NextInflation(0.95, 1.05);
            PlayerMessage.Broadcast(A.R.GetString("Some prices and rents may have risen due to inflation by " + (inflationPer *100).ToString() + "%!"), "", NotificationColor.Yellow);
            foreach (PurchasableItem item in A.ST.AllPurchasables)
                item.Price = this.Inflate(item.Price, inflationPer);
            //Utilities
            float inflated = this.Inflate(A.ST.ElectricityCost, inflationPer);
            A.ST.ElectricityCost = this.Inflate(A.ST.ElectricityCost, inflationPer);
            inflated = this.Inflate(A.ST.InternetCost, inflationPer);
            A.ST.InternetCost = (inflated <= 55f) ? inflated : 55f;

            foreach (Offering offering in A.ST.City.GetOfferings()) {
                if (offering is DwellingOffer) {
                    DwellingOffer offer = (DwellingOffer) offering;
                    if ((!offer.Taken || offer.Condo) || (((Dwelling)offer.Building).MonthsLeftOnLease <= 0))
                        offering.Building.Rent = (int)this.Inflate((float)offering.Building.Rent, inflationPer);
                }
                if (offering is Course) 
                    ((Course) offering).Cost = this.Inflate(((Course) offering).Cost, inflationPer);
            }
        }

        Random InflationRandomizer = new Random(1048596);
        public double NextInflation(double minp, double maxp)
        {
            int min = (int)(minp * 100);
            int max = (int)(maxp * 100);
            double next = InflationRandomizer.Next(min, max);
            return next / 100.0;
        }

        /// <summary>
        /// Inflates prices based on a fixed inflation rate.
        /// </summary>
        /// <param name="amount">The original amount to inflate.</param>
        /// <returns>The inflated price.</returns>
        public float Inflate(float amount) {
            return (float)Math.Round((double)(amount * (1f + A.SS.InflationRate)), 2);
        }

        /// <summary>
        /// Inflates prices based on a given percentage.
        /// </summary>
        /// <param name="amount">The original amount to inflate.</param>
        /// <param name="percent">The percent to inflate by.</param>
        /// <returns>The inflated price.</returns>
        public float Inflate(float amount, double percent) {
            return (float)Math.Round((double)(amount*percent), 2);
        }

        protected void ManageLevels()
        {
            AppEntity entity = (AppEntity) A.ST.Entity[A.MF.CurrentEntityID];
            if (entity != null)
            {
                float num = entity.CriticalResourceBalance();
                string newLine = Environment.NewLine;
                if ((A.SS.Level == 1) && (num > 25000f))
                {
                    entity.Player.SendModalMessage(A.R.GetString("Congratulations! You have reached Level 2. In this level, some of the jobs in the city will add 401K retirement plans and health benefits! You will also be able to use online banking to simplify your life. You now have one additional health factor to consider. You must maintain an active social life by hosting and attending gatherings with your friends! Good luck."), A.R.GetString("Congratulations!"), MessageBoxIcon.Exclamation);
                    A.SS.Level = 2;
                    A.MF.EnableDisable();
                }
                else if ((A.SS.Level == 2) && (num > 100000f))
                {
                    entity.Player.SendModalMessage(A.R.GetString("Congratulations! You have reached Level 3, the final level. In this level your goal is to continue to build your wealth. You can now purchase condominiums (real estate). You can live in these or just purchase them just for investments."), A.R.GetString("Congratulations!"), MessageBoxIcon.Exclamation);
                    A.SS.Level = 3;
                    A.MF.EnableDisable();
                }
            }
        }

        public override void NewDay()
        {
            AppEntity current;
            AppSimState sT = A.ST;
            sT.DayCount++;
            if ((A.ST.Day == 28) && A.SS.PayBillsEnabledForOwner)
            {
                PlayerMessage.Broadcast(A.R.GetString("It's near the end of the month. Keep an eye out for new bills on your desk."), "", NotificationColor.Yellow);
            }
            SortedList list = new SortedList(A.ST.OneTimeEvents);
            foreach (OneTimeEvent event2 in list.Values)
            {
                if (A.ST.Now > event2.OneTimeDay)
                {
                    A.ST.OneTimeEvents.Remove(event2.Key);
                    IEnumerator enumerator2 = A.ST.Entity.Values.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            current = (AppEntity) enumerator2.Current;
                            current.OneTimeEvents.Remove(event2.Key);
                        }
                }
            }
            foreach (OneTimeEvent event2 in A.ST.OneTimeEvents.Values)
            {
                if (A.ST.Now.AddDays(1.0) > event2.OneTimeDay)
                {
                    current = (AppEntity) A.ST.Entity[event2.HostID];
                    if (current != null)
                    {
                        float num = 600f;
                        float num2 = 0f;
                        foreach (PurchasableItem item in current.PartyFood)
                        {
                            num2 += item.Price;
                        }
                        if (current.Has("DDR"))
                        {
                            num2 += 200f;
                        }
                        if (current.Has("Stereo"))
                        {
                            num2 += (current.ImageIndexOf("Stereo") + 1) * 0x42;
                        }
                        event2.AddAIAttendees((int) (Math.Min((float) 1f, (float) (num2 / num)) * 40f));
                    }
                }
            }
            if (A.SS.AIParties && (A.ST.Random.Next(7) == 0))
            {
                A.ST.AddAIOneTimeEvent();
            }
            foreach (AppEntity entity in A.ST.Entity.Values)
            {
                foreach (Surprise surprise in entity.Surprises)
                {
                    surprise.CheckFireSurprise(entity);
                }
            }
            foreach (AppEntity entity in A.ST.Entity.Values)
            {
                if (A.SS.BreakInDate.ToShortDateString() == A.ST.Now.ToShortDateString())
                {
                    entity.Surprises[3].FireSurprise(entity);
                }
            }
            if (A.SS.LevelManagementOn)
            {
                this.ManageLevels();
            }
            if (((A.SS.Level > 1) && (A.ST.Day == 1)) && ((A.ST.Month % 12) == 8))
            {
                A.ST.City.AddHealthInsurance(0.1f);
            }
            if (((A.SS.Level > 1) && (A.ST.Day == 1)) && ((A.ST.Month % 12) == 4))
            {
                A.ST.City.Add401Ks(0.1f);
            }
            if ((A.SS.LevelManagementOn && (A.ST.Day == 1)) && ((A.ST.Month % 12) == 2))
            {
                A.ST.City.RaiseSomeWages(0.075f);
            }
        }

        public override bool NewStep()
        {
            int period = A.ST.Period;
            if (period != this.currentPeriod)
            {
                foreach (AppEntity entity in A.ST.Entity.Values)
                {
                    entity.NewPeriod();
                }
            }
            this.currentPeriod = period;
            return false;
        }

        public override void NewWeek()
        {
            if (A.SS.Sales && A.SS.ShopForGoodsEnabledForOwner)
            {
                ArrayList buildings = A.ST.City.GetBuildings();
                foreach (AppBuilding building in buildings)
                {
                    if (building.BuildingType.Index == 12)
                    {
                        building.SaleDiscounts = new float[building.SaleDiscounts.Length];
                        int maxValue = 8;
                        if (A.ST.Reserved["SaleEvery"] != null)
                        {
                            maxValue = (int) A.ST.Reserved["SaleEvery"];
                        }
                        if (A.ST.Random.Next(maxValue) == 0)
                        {
                            bool flag = false;
                            for (int i = 0; i < building.SaleDiscounts.Length; i++)
                            {
                                if (A.ST.Random.NextDouble() < 0.25)
                                {
                                    building.SaleDiscounts[i] = (float) ((A.ST.Random.NextDouble() * 0.4) + 0.1);
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                PlayerMessage.Broadcast(A.R.GetString("{0} is having a sale this week!", new object[] { building.OwnerName }), "", NotificationColor.Green);
                            }
                        }
                    }
                }
            }
        }

        public override void NewYear() {
            if ((A.ST.CurrentWeek > 1) && (A.SS.InflationRate > 0f)) 
                this.DoInflation(); 
        }
    }
}

