namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class WorkTask : Task
    {
        public ArrayList AcademicExperienceRequired = new ArrayList();
        public float AdditionalWitholding = 0f;
        public int Allowances = 1;
        public float BonusPotential = 0f;
        public int CreditScoreRequired = 0;
        public BankAccount DirectDepositAccount = null;
        public bool ExemptFromWitholding = false;
        public InsurancePolicy HealthInsurance;
        public float HourlyWage = 7.5f;
        public float HoursThisWeek;
        public ArrayList PayStubs = new ArrayList();
        public float[] R401KAllocations = new float[A.ST.MutualFunds.Count];
        public float R401KMatch = -1f;
        public float R401KPercentWitheld;
        public Hashtable WorkExperienceRequired = new Hashtable();

        public override string CategoryName()
        {
            return A.R.GetString("(" + HourlyWage.ToString() + "/hr) Work\r\n" + this.Name());
        }

        public override string Description()
        {
            DateTime time = new DateTime(0x7d0, 1, 1);
            string s = "";
            foreach (string str2 in this.WorkExperienceRequired.Keys)
            {
                float num = (float) this.WorkExperienceRequired[str2];
                s = s + A.R.GetString("{0} year(s) as a {1}, ", new object[] { num, str2 });
            }
            if (s == "")
            {
                s = "None";
            }
            else
            {
                s = Utilities.FormatCommaSeries(s);
            }
            s = s.Replace("|", A.R.GetString(" or "));
            string str3 = "";
            foreach (string str2 in this.AcademicExperienceRequired)
            {
                str3 = str3 + A.R.GetString("{0}, ", new object[] { str2 });
            }
            if (str3 == "")
            {
                str3 = "None";
            }
            else
            {
                str3 = Utilities.FormatCommaSeries(str3);
            }
            str3 = str3.Replace("|", A.R.GetString(" or "));
            string str4 = "";
            if (this.HealthInsurance != null)
            {
                str4 = str4 + A.R.GetString("Healthcare Insurance");
            }
            if (this.R401KMatch > -1f)
            {
                if (str4.Length > 0)
                {
                    str4 = str4 + "; ";
                }
                str4 = str4 + A.R.GetString("401K Plan with {0} company match", new object[] { Utilities.FP(this.R401KMatch) });
            }
            if (str4 == "")
            {
                str4 = A.R.GetString("None");
            }
            string str5 = A.R.GetString("Mon. - Fri.");
            if (base.Weekend)
            {
                str5 = A.R.GetString("Sat. & Sun.");
            }
            string str6 = A.R.GetString("{0}|Hours: {1} to {2}|Days: {3}|Hourly Pay: {4}|Experience Req'd: {5}|Courses Req'd: {6}|Benefits: {7}", new object[] { this.Name().ToUpper(), Task.ToTimeString(base.StartPeriod), Task.ToTimeString(base.EndPeriod), str5, Utilities.FC(this.HourlyWage, 2, A.I.CurrencyConversion), s, str3, str4 });
            if (this.BonusPotential > 0f)
            {
                str6 = str6.Replace("|Experience", A.R.GetString("|Qtly Bonus: up to {0}|Experience", new object[] { Utilities.FP(this.BonusPotential) }));
            }
            return str6.Replace("|", Environment.NewLine);
        }

        public virtual void EvaluateApplicant(AppEntity e, Offering o, JobApplication jobApp)
        {
            bool flag;
            TimeSpan span;
            object obj2 = e.FiredFrom[o.Building.ID + this.Name()];
            if ((obj2 != null) && ((span = (TimeSpan) (A.ST.Now - ((DateTime) obj2))).Days < 180))
            {
                throw new SimApplicationException(A.R.GetString("You were recently fired from that job. You must wait at least {0} days before you can be rehired.", new object[] { 180 }));
            }
            obj2 = e.Quit[o.Building.ID + this.Name()];
            if ((obj2 != null) && ((span = (TimeSpan) (A.ST.Now - ((DateTime) obj2))).Days < 90))
            {
                throw new SimApplicationException(A.R.GetString("You recently quit that job. You must wait at least {0} days before you can be rehired.", new object[] { 90 }));
            }
            if (jobApp.Name.ToUpper() != e.Name.ToUpper())
            {
                throw new SimApplicationException(A.R.GetString("You got your name wrong on the application. Your application has been rejected."));
            }
            if (this is WorkTravellingSalesman)
            {
                if ((e.Car == null) && jobApp.Car)
                {
                    throw new SimApplicationException(A.R.GetString("A check of motor vehicle registrations shows you don't have a car. Your application has been rejected for lying."));
                }
                if (!jobApp.Car)
                {
                    throw new SimApplicationException(A.R.GetString("This job requires a car and your application indicates you don't have one. Your application has been rejected."));
                }
            }
            foreach (string str in jobApp.ReportedClassNames)
            {
                flag = false;
                foreach (AttendClass class2 in e.AcademicTaskHistory.Values)
                {
                    if (class2.Course.Name == str)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    throw new SimApplicationException(A.R.GetString("You reported completing the class: {0}. But reference checking revealed you did not. Your application has been rejected for lying.", new object[] { str }));
                }
            }
            if (jobApp.ReportedClassNames.Contains("Bachelors Degree"))
            {
                jobApp.ReportedClassNames.Add("Associates Degree");
            }
            int num = 0;
            foreach (string str2 in jobApp.ReportedJobNamesAndMonths.Keys)
            {
                int num2 = (int) jobApp.ReportedJobNamesAndMonths[str2];
                int num3 = (int) Math.Floor((double) (e.YearsExperience(str2) * 12f));
                num += num2;
                if (num2 > num3)
                {
                    throw new SimApplicationException(A.R.GetString("You reported {0} months of experience as a {1}. But reference checking revealed you have {2} months experience at that job. Your application has been rejected for lying.", new object[] { num2, str2, num3.ToString("N0") }));
                }
            }
            jobApp.ReportedJobNamesAndMonths.Add("worker of any kind", num);
            foreach (string str3 in this.AcademicExperienceRequired)
            {
                flag = false;
                string[] strArray = str3.Split(new char[] { '|' });
                foreach (string str4 in strArray)
                {
                    foreach (string str5 in jobApp.ReportedClassNames)
                    {
                        if (str5.IndexOf(str4) > -1)
                        {
                            flag = true;
                        }
                    }
                }
                if (!flag)
                {
                    throw new SimApplicationException(A.R.GetString("You did not get the job, because your application showed that you did not have enough education. This job requires the course: {0}.", new object[] { A.R.GetString(str3.Replace("|", " or ")) }));
                }
            }
            foreach (string str6 in this.WorkExperienceRequired.Keys)
            {
                float num4 = (float) this.WorkExperienceRequired[str6];
                int num5 = 0;
                string[] strArray2 = str6.Split(new char[] { '|' });
                foreach (string str7 in strArray2)
                {
                    if (jobApp.ReportedJobNamesAndMonths.ContainsKey(str7))
                    {
                        num5 += (int) jobApp.ReportedJobNamesAndMonths[str7];
                    }
                }
                if ((num4 * 12f) > num5)
                {
                    object[] args = new object[] { (num4 * 12f).ToString("N0"), str6.Replace("|", " or "), num5 };
                    throw new SimApplicationException(A.R.GetString("You did not get the job, because you did not have enough experience. This job requires {0} months of experience as a {1}, and your application lists only {2} months of experience.", args));
                }
            }
            if (e.CreditScore() < this.CreditScoreRequired)
            {
                throw new SimApplicationException(A.R.GetString("You did not get the job, because your credit score was below {0}.", new object[] { e.CreditScore() }));
            }
        }

        public override Color GetColor()
        {
            return Color.Green;
        }

        public float GetValueYTD(string lineItem, DateTime date)
        {
            float num = 0f;
            foreach (PayStub stub in this.PayStubsYTD(date))
            {
                num += stub.GetValue(lineItem);
            }
            return num;
        }

        public virtual string Name()
        {
            return "Work Task";
        }

        public ArrayList PayStubsYTD(DateTime date)
        {
            ArrayList list = new ArrayList();
            foreach (PayStub stub in this.PayStubs)
            {
                if ((stub.WeekEnding.Year == date.Year) && (stub.WeekEnding <= date))
                {
                    list.Add(stub);
                }
            }
            return list;
        }

        public virtual string ResumeDescription()
        {
            return "Description of task";
        }
    }
}

