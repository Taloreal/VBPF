namespace KMI.Biz
{
    using KMI.Sim;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    [Serializable]
    public class CommentLog : ActiveObject
    {
        private ArrayList cache = new ArrayList();
        private int dayCounter = 0;
        private int daysToKeep;
        private int frequencyInDays = 0;
        private DateTime startDate;

        public CommentLog(int frequencyInDays, int daysToKeep)
        {
            this.frequencyInDays = frequencyInDays;
            this.dayCounter = frequencyInDays;
            this.daysToKeep = daysToKeep;
            this.cache.Add(new Hashtable());
            this.startDate = Simulator.Instance.SimState.Now;
            Simulator.Instance.Subscribe(this, Simulator.TimePeriod.Day);
        }

        public void Comment(string category, string subCategory, string comment)
        {
            if (!S.I.BlockMessage(comment))
            {
                Hashtable hashtable2;
                Hashtable hashtable3;
                Hashtable hashtable = (Hashtable) this.cache[this.cache.Count - 1];
                if (hashtable.Contains(category))
                {
                    hashtable2 = (Hashtable) hashtable[category];
                }
                else
                {
                    hashtable2 = new Hashtable();
                    hashtable.Add(category, hashtable2);
                }
                if (hashtable2.Contains(subCategory))
                {
                    hashtable3 = (Hashtable) hashtable2[subCategory];
                }
                else
                {
                    hashtable3 = new Hashtable();
                    hashtable2.Add(subCategory, hashtable3);
                }
                if (hashtable3.Contains(comment))
                {
                    hashtable3[comment] = ((int) hashtable3[comment]) + 1;
                }
                else
                {
                    hashtable3.Add(comment, 1);
                }
            }
        }

        public override void NewDay()
        {
            if (this.dayCounter >= this.frequencyInDays)
            {
                this.dayCounter = 0;
                if (this.cache.Count > this.daysToKeep)
                {
                    this.cache[(this.cache.Count - this.daysToKeep) - 1] = null;
                }
                this.cache.Add(new Hashtable());
            }
            this.dayCounter++;
        }

        public ArrayList Comments
        {
            get
            {
                return this.cache;
            }
        }

        public int DaysToKeep
        {
            get
            {
                return this.daysToKeep;
            }
        }

        public int FrequencyInDays
        {
            get
            {
                return this.frequencyInDays;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.startDate;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public CommentLog log;
            public int frequencyInDays;
        }
    }
}

