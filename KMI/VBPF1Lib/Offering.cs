namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;

    [Serializable]
    public abstract class Offering
    {
        protected long buildingID;
        public long ID = A.ST.GetNextID();
        public Task PrototypeTask;
        public bool Taken;

        public Task CreateTask()
        {
            Task task = (Task) Utilities.DeepCopyBySerialization(this.PrototypeTask);
            task.ID = A.ST.GetNextID();
            task.StartDate = A.ST.Now;
            return task;
        }

        public virtual string Description()
        {
            return null;
        }

        public virtual string JournalEntry()
        {
            return "";
        }

        public virtual string ThingName()
        {
            return null;
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
    }
}

