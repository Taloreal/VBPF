namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;

    [Serializable]
    public class WorkTravellingSalesman : WorkTask
    {
        public KMI.VBPF1Lib.Car Car;
        protected int count = 0;
        public int Mileage = 0;
        protected bool returning = false;
        public States State;
        public int VisitBuildingIndex = 0;

        public override void CleanUp()
        {
            base.CleanUp();
            this.State = States.Init;
            this.returning = false;
        }

        public override bool Do()
        {
            if (A.ST.Period != base.EndPeriod)
            {
                string str;
                switch (this.State)
                {
                    case States.Init:
                        this.Car = base.GetAppEntity().Car;
                        if (((this.Car != null) && !this.Car.Broken) && (this.Car.Gas > 0f))
                        {
                            this.State = States.Wait;
                            goto Label_0227;
                        }
                        if (this.Car != null)
                        {
                            if (this.Car.Broken)
                            {
                                str = A.R.GetString("your car is broken down");
                            }
                            else
                            {
                                str = A.R.GetString("your car is out of gas");
                            }
                            break;
                        }
                        str = A.R.GetString("you don't have a car");
                        break;

                    case States.Drive:
                    {
                        float num = A.ST.SimulatedTimePerStep / 0x4e20;
                        this.Mileage += (int) num;
                        if (this.Car.NewStep())
                        {
                            this.State = States.Wait;
                            this.count = 20;
                        }
                        goto Label_0227;
                    }
                    case States.Wait:
                        this.count -= A.ST.SimulatedTimePerStep / 0x4e20;
                        if (this.count < 0)
                        {
                            AppBuilding bldg = null;
                            if (!this.returning)
                            {
                                bldg = (AppBuilding) A.ST.City.GetRandomBuilding(this.VisitBuildingIndex);
                            }
                            else
                            {
                                bldg = base.Building;
                            }
                            this.Car.SendTo(bldg);
                            this.returning = !this.returning;
                            this.State = States.Drive;
                        }
                        goto Label_0227;

                    default:
                        goto Label_0227;
                }
                base.GetAppEntity().Player.SendMessage(A.R.GetString("Because {0}, you are unable to do your job as a {1} and have been marked absent for the day.", new object[] { str, this.Name().ToLower() }), "", NotificationColor.Yellow);
                base.DatesAbsent.Add(A.ST.Now);
            }
            return true;
        Label_0227:
            return false;
        }

        public enum States
        {
            Init,
            Drive,
            Wait
        }
    }
}

