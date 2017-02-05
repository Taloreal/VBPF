namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Drawing;

    [Serializable]
    public class AttendClass : Task
    {
        private int chair;
        public KMI.VBPF1Lib.Course Course;
        public int daysInCourse = 0;
        public States State = States.Init;
        public string ClassEndDate = "";

        public override string CategoryName()
        {
            if (ClassEndDate == "" || ClassEndDate == null)
                ClassEndDate = FindEndDate();
            //return A.R.GetString("Education\r\n" + daysInCourse.ToString() + " of " + Course.Days.ToString() + " days completed.");
            return A.R.GetString(this.Course.Name + " Ends: \r\n" + ClassEndDate);
        }

        public string FindEndDate()
        {
            DateTime Start = A.ST.Now;
            for (int DaysLeft = Course.Days - daysInCourse; DaysLeft != 0; Start = Start.AddDays(1.0))
                if (Weekday(Start) == !Weekend)
                    DaysLeft--;
            return Start.Month + "/" + Start.Day + "/" + Start.Year.ToString().Substring(2);
        }

        private bool Weekday(DateTime ToTest)
        {
            return !(ToTest.DayOfWeek == DayOfWeek.Sunday || ToTest.DayOfWeek == DayOfWeek.Saturday);
        }

        public override void CleanUp()
        {
            base.CleanUp();
            this.State = States.Init;
        }

        public override bool Do()
        {
            switch (this.State)
            {
                case States.Init:
                    if (!base.Building.Persons.Contains(base.Owner))
                    {
                        base.Building.Persons.Add(base.Owner);
                    }
                    this.chair = this.Course.Students.IndexOf(base.Owner);
                    base.Owner.Location = (PointF) base.Building.Map.getNode("EntryPoint").Location;
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Chair" + this.chair).ToPoints();
                    this.State = States.ToChair;
                    goto Label_0331;

                case States.ToChair:
                    if (base.Owner.Move())
                    {
                        this.State = States.AtChair;
                        base.Owner.Pose = "SitNE";
                    }
                    goto Label_0331;

                case States.AtChair:
                    if (A.ST.Period == base.EndPeriod)
                    {
                        base.Owner.Path = base.Building.Map.findPath("Chair" + this.chair, "EntryPoint").ToPoints();
                        base.Owner.Pose = "Walk";
                        this.State = States.FromChair;
                        this.daysInCourse++;
                    }
                    goto Label_0331;

                case States.FromChair:
                {
                    base.Owner.Location = (PointF) base.Building.Map.getNode("EntryPoint").Location;
                    if (!base.Owner.Drone)
                    {
                        if (this.daysInCourse >= this.Course.Days)
                        {
                            this.Course.Students.Remove(base.Owner);
                            AppEntity appEntity = base.GetAppEntity();
                            A.SA.DeleteTask(appEntity.ID, base.ID);
                            appEntity.AcademicTaskHistory.Add(base.ID, this);
                            appEntity.Player.SendMessage(A.R.GetString("Congratulations! You completed your course: {0}. A diploma will now appear on your wall.", new object[] { this.Course.Name }), "", NotificationColor.Green);
                        }
                        break;
                    }
                    DateTime time = A.ST.Now.AddDays(1.0);
                    base.Owner.WakeupTime = new DateTime(time.Year, time.Month, time.Day).AddHours((((float) base.StartPeriod) / 2f) - (0.20000000298023224 + (0.10000000149011612 * A.ST.Random.NextDouble())));
                    base.Building.Persons.Remove(base.Owner);
                    this.State = States.Init;
                    break;
                }
                default:
                    goto Label_0331;
            }
            return true;
        Label_0331:
            return false;
        }

        public override Color GetColor()
        {
            return Color.LightCoral;
        }

        public enum States
        {
            Init,
            ToChair,
            AtChair,
            FromChair
        }
    }
}

