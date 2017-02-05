namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;

    public delegate TResult FuncTest<T1, T2, TResult>(T1 obj1, T2 obj2);
    [Serializable]
    public class Surprise
    {
        protected string keyName;
        protected DateTime LastFired = DateTime.MinValue;
        protected int level;
        protected float likelihoodPerDay;
        protected float minSpacing;
        protected float netWorth;
        public static bool Enabled = true;
        private static int CanNext = 31;
        public static FuncTest<int, int, bool> CorrectLvl = (x, y) => x >= y;

        public void CheckFireSurprise(AppEntity e)
        {
            if (CanNext != 0 || !Enabled) { CanNext--; return; }
            if (CorrectLvl(A.SS.Level, this.Level))
            if (((A.SS.Level >= this.Level) && (A.SS.Surprises.IndexOf(this.KeyName) > -1)) && (e.CriticalResourceBalance() > this.NetWorth))
            {
                TimeSpan span = (TimeSpan) (A.ST.Now - this.LastFired);
                if ((span.Days > this.MinSpacing) && (A.ST.Random.NextDouble() < this.LikelihoodPerDay))
                {
                    CanNext = 31;
                    this.FireSurprise(e);
                }
            }
        }

        public void FireSurprise(AppEntity e)
        {
            InsurancePolicy bestHealthInsurance;
            KMI.VBPF1Lib.Check check;
            switch (this.KeyName)
            {
                case "Health":
                {
                    int index = A.ST.Random.Next(4);
                    string str = A.R.GetString("staph infection|dislocated shoulder|torn ligament|ruptured appendix").Split(new char[] { '|' })[index];
                    string str2 = str.Split(new char[] { ' ' })[1];
                    float amount = new float[] { 313f, 617f, 1326f, 4276f }[index];
                    e.Player.SendMessage(A.R.GetString("Oh, no! What bad luck! You have suffered from a {0}. Fortunately, you were treated successfully at the hospital. A bill from the hospital will be sent to you shortly!", new object[] { str }), "", NotificationColor.Red);
                    BankAccount account = (BankAccount) e.MerchantAccounts["Vincent Medical"];
                    e.AddBill(new Bill("Vincent Medical", A.R.GetString("Treatment for {0}", new object[] { str2 }), amount, account));
                    bestHealthInsurance = e.GetBestHealthInsurance();
                    if ((bestHealthInsurance == e.HealthInsurance) && e.InsuranceOff)
                    {
                        e.Player.SendMessage("The insurance company did not pay out because your policy was suspended due to lack of payment.", "", NotificationColor.Red);
                    }
                    else if (bestHealthInsurance.Copay > -1f)
                    {
                        account.Transactions.Add(new Transaction(amount - bestHealthInsurance.Copay, Transaction.TranType.Debit, "Insurance payment"));
                    }
                    this.LastFired = A.ST.Now;
                    return;
                }
                case "Car Accident":
                    if ((e.Car != null) && !e.Car.Broken)
                    {
                        e.Player.SendMessage(A.R.GetString("Uh, oh! You've been in a car accident and your transmission is ruined. You can't drive it until you get it repaired. If you have insurance, you will receive a check for the repair cost less your deductible."), "", NotificationColor.Red);
                        e.Car.Broken = true;
                        if (e.Car.Insurance.Deductible > -1f)
                        {
                            if (e.InsuranceOff)
                            {
                                e.Player.SendMessage("The insurance company did not pay out because your policy was suspended due to lack of payment.", "", NotificationColor.Red);
                            }
                            else
                            {
                                check = new KMI.VBPF1Lib.Check(-1L) {
                                    Amount = ((PurchasableItem) A.ST.PurchasableAutoSupplies[4]).Price - e.Car.Insurance.Deductible,
                                    Payee = e.Name,
                                    Date = A.ST.Now,
                                    Payor = "S&&W Insurance",
                                    Number = (int) A.ST.GetNextID(),
                                    Memo = A.R.GetString("Car repairs"),
                                    Signature = A.R.GetString("Samuel S. Steiner")
                                };
                                e.AddCheck(check);
                            }
                        }
                        this.LastFired = A.ST.Now;
                    }
                    return;

                case "Car Breakdown":
                    if (e.Car != null)
                    {
                        e.Player.SendPeriodicMessage(A.R.GetString("Getting a lube for your car every four months or so will help prevent costly breakdowns."), "", NotificationColor.Yellow, 365f);
                        if ((!e.Car.Broken && (e.Car.LeaseCost == 0f)) && (A.ST.Random.NextDouble() < e.Car.LikelihoodOfBreakDown()))
                        {
                            e.Player.SendMessage(A.R.GetString("Oh no! Your car has broken down because you failed to maintain it properly or it's getting old. You can't use it until you get it repaired at the auto garage."), "", NotificationColor.Red);
                            e.Car.Broken = true;
                            this.LastFired = A.ST.Now;
                        }
                    }
                    return;

                case "Robbery":
                {
                    float cash = e.Cash;
                    foreach (PurchasableItem item in e.PurchasedItems)
                    {
                        cash += item.Price;
                    }
                    if (cash > 0f)
                    {
                        e.Player.SendMessage(A.R.GetString("Bad news! Your residence was broken into and everything was taken. If you have insurance, you will receive a check for the cost of items stolen (up to your coverage limit) less your deductible."), "", NotificationColor.Red);
                        e.PurchasedItems.Clear();
                        e.Cash = 0f;
                        bestHealthInsurance = null;
                        if (((DwellingOffer) e.Dwelling.Offerings[0]).Condo)
                        {
                            bestHealthInsurance = e.Dwelling.Insurance;
                        }
                        else
                        {
                            bestHealthInsurance = e.RentersInsurance;
                        }
                        if ((bestHealthInsurance.Deductible > -1f) && (bestHealthInsurance.Limit > 0f))
                        {
                            if (e.InsuranceOff)
                            {
                                e.Player.SendMessage("The insurance company did not pay out because your policy was suspended due to lack of payment.", "", NotificationColor.Red);
                            }
                            else
                            {
                                check = new KMI.VBPF1Lib.Check(-1L) {
                                    Amount = Math.Max((float) (Math.Min(cash, bestHealthInsurance.Limit) - bestHealthInsurance.Deductible), (float) 0f),
                                    Payee = e.Name,
                                    Date = A.ST.Now,
                                    Payor = "S&W Insurance",
                                    Number = (int) A.ST.GetNextID(),
                                    Memo = A.R.GetString("Loss from Theft"),
                                    Signature = A.R.GetString("Samuel S. Steiner")
                                };
                                if (check.Amount > 0f)
                                {
                                    e.AddCheck(check);
                                }
                            }
                        }
                        this.LastFired = A.ST.Now;
                    }
                    return;
                }
                case "Layoff":
                    foreach (Task task in e.GetAllTasks())
                    {
                        if ((task is WorkTask) && !this.LastOfItsKind((WorkTask) task))
                        {
                            e.Player.SendMessage(A.R.GetString("Tough luck. Your employer, {0}, has eliminated your position as a {1}. You have been laid off!", new object[] { task.Building.OwnerName, ((WorkTask) task).Name() }), "", NotificationColor.Red);
                            A.SA.DeleteTask(e.ID, task.ID);
                            A.ST.City.FindOfferingForTask(task).Taken = true;
                            this.LastFired = A.ST.Now;
                            break;
                        }
                    }
                    return;
            }
            throw new Exception("Bad key name in surprises.");
        }

        protected bool LastOfItsKind(WorkTask t)
        {
            Offering offering = A.ST.City.FindOfferingForTask(t);
            ArrayList offerings = A.ST.City.GetOfferings(offering.GetType());
            foreach (Offering offering2 in offerings)
            {
                if (!((((offering == offering2) || (offering2.PrototypeTask.StartPeriod != offering.PrototypeTask.StartPeriod)) || (offering2.PrototypeTask.GetType() != offering.PrototypeTask.GetType())) || offering2.Taken))
                {
                    return false;
                }
            }
            return true;
        }

        public string KeyName
        {
            get
            {
                return this.keyName;
            }
            set
            {
                this.keyName = value;
            }
        }

        public int Level
        {
            get
            {
                return this.level;
            }
            set
            {
                this.level = value;
            }
        }

        public float LikelihoodPerDay
        {
            get
            {
                return this.likelihoodPerDay;
            }
            set
            {
                this.likelihoodPerDay = value;
            }
        }

        public float MinSpacing
        {
            get
            {
                return this.minSpacing;
            }
            set
            {
                this.minSpacing = value;
            }
        }

        public float NetWorth
        {
            get
            {
                return this.netWorth;
            }
            set
            {
                this.netWorth = value;
            }
        }
    }
}

