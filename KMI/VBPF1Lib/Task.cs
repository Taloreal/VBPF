namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class Task
    {
        public DateTime ArrivedToday = DateTime.MinValue;
        protected long buildingID;
        public ArrayList DatesAbsent = new ArrayList();
        public ArrayList DatesLate = new ArrayList();
        public int DayLastStarted = -1;
        public DateTime EndDate = DateTime.MinValue;
        public int EndPeriod;
        public long ID;
        public DateTime OneTimeDay = DateTime.MinValue;
        public VBPFPerson Owner;
        public DateTime StartDate;
        public int StartPeriod;
        public bool Weekend;

        public Task()
        {
            if (!S.I.Client)
            {
                this.ID = A.ST.GetNextID();
            }
        }

        public string BadAttendance()
        {
            if (A.SS.FireForAbsencesLateness)
            {
                string str;
                TimeSpan span;
                int num = 0;
                foreach (DateTime time in this.DatesAbsent)
                {
                    span = (TimeSpan) (A.ST.Now - time);
                    if (span.Days < 30)
                    {
                        num++;
                    }
                }
                if (num > 4)
                {
                    str = A.R.GetString("been fired from your job");
                    if (this is AttendClass)
                    {
                        str = A.R.GetString("flunked out of your class");
                    }
                    return A.R.GetString("You have {0} because you were absent more than {1} times in the last month.", new object[] { str, 4 });
                }
                if (num > 2)
                {
                    this.GetAppEntity().Player.SendPeriodicMessage(A.R.GetString("You have been absent a lot recently. You may be fired or flunk out soon!"), "", NotificationColor.Yellow, 30f);
                }
                int num2 = 0;
                foreach (DateTime time in this.DatesLate)
                {
                    span = (TimeSpan) (A.ST.Now - time);
                    if (span.Days < 30)
                    {
                        num2++;
                    }
                }
                if (num2 > 4)
                {
                    str = A.R.GetString("been fired from your job");
                    if (this is AttendClass)
                    {
                        str = A.R.GetString("flunked out of your class");
                    }
                    return A.R.GetString("You have {0} because you were late more than {1} times in the last month.", new object[] { str, 4 });
                }
                if (num2 > 2)
                {
                    this.GetAppEntity().Player.SendPeriodicMessage(A.R.GetString("You have been late a lot recently. You may be fired or flunk out soon!"), "", NotificationColor.Yellow, 30f);
                }
            }
            return null;
        }

        public virtual string CategoryName()
        {
            return null;
        }

        public virtual void CleanUp()
        {
        }

        public virtual string Description()
        {
            return null;
        }

        public virtual bool Do()
        {
            return true;
        }

        public AppEntity GetAppEntity()
        {
            foreach (AppEntity entity in A.ST.Entity.Values)
            {
                if (entity.Person == this.Owner)
                {
                    return entity;
                }
            }
            return null;
        }

        public virtual Color GetColor()
        {
            return Color.White;
        }

        public static string ToTimeString(int period)
        {
            DateTime time = new DateTime(0x7d0, 1, 1);
            return time.AddHours((double) (((float) period) / 2f)).ToShortTimeString();
        }

        public string WeekendString()
        {
            if (this.Weekend)
            {
                return A.R.GetString("Weekend");
            }
            return A.R.GetString("Weekday");
        }

        public AppBuilding Building
        {
            get
            {
                return A.ST.City.BuildingByID(this.buildingID);
            }
            set
            {
                this.buildingID = value.ID;
            }
        }

        public int Duration
        {
            get
            {
                if (this.StartPeriod <= this.EndPeriod)
                {
                    return (this.EndPeriod - this.StartPeriod);
                }
                return ((this.EndPeriod + 0x30) - this.StartPeriod);
            }
        }
    }
}

