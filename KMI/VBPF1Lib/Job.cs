namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;

    [Serializable]
    public class Job : Offering
    {
        public float WeeklyPay;

        public Job()
        {
            this.WeeklyPay = 200f;
        }

        public Job(AppBuilding building, Task task, int startPeriod, int endPeriod)
        {
            this.WeeklyPay = 200f;
            base.Building = building;
            base.PrototypeTask = task;
            base.PrototypeTask.StartPeriod = startPeriod;
            base.PrototypeTask.EndPeriod = endPeriod;
        }

        public override string Description()
        {
            return base.PrototypeTask.Description();
        }

        public override string JournalEntry()
        {
            return A.R.GetString("Got job as a {0}, paying {1} per week.", new object[] { this.ToString(), Utilities.FC(this.WeeklyPay, 2, A.I.CurrencyConversion) });
        }

        public override string ThingName()
        {
            return A.R.GetString("Jobs");
        }

        public override string ToString()
        {
            WorkTask prototypeTask = (WorkTask) base.PrototypeTask;
            return prototypeTask.Name();
        }
    }
}

