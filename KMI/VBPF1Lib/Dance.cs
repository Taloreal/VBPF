namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class Dance : OneTimeEvent
    {
        private int DDRCounter;
        private States State = States.Init;

        public Dance()
        {
            base.rnd = (float) A.ST.Random.NextDouble();
        }

        public override string CategoryName()
        {
            return A.R.GetString("Party");
        }

        public override void CleanUp()
        {
            base.CleanUp();
            this.State = States.Init;
            AppEntity entity = (AppEntity) A.ST.Entity[base.HostID];
            if (entity != null)
            {
                entity.PartyFood.Clear();
                if (entity.DDRLockedBy == base.Owner.ID)
                {
                    entity.DDRLockedBy = -1L;
                }
            }
        }

        public override bool Do()
        {
            AppEntity entity = (AppEntity) A.ST.Entity[base.HostID];
            if (A.ST.Period == base.EndPeriod)
            {
                this.CleanUp();
                if (base.Owner.Drone)
                {
                    base.Building.Persons.Remove(base.Owner);
                    base.Owner.Retire();
                    return false;
                }
                base.Owner.Pose = "StandSW";
                base.Owner.Location = (PointF) base.Building.Map.getNode("Dance").Location;
                return true;
            }
            switch (this.State)
            {
                case States.Init:
                    if (!base.Building.Persons.Contains(base.Owner))
                    {
                        base.Building.Persons.Add(base.Owner);
                    }
                    base.Owner.Pose = "Walk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Dance").ToPoints();
                    if (entity != null)
                    {
                        if (entity.PartyAttendance.Contains(base.Key))
                        {
                            entity.PartyAttendance[base.Key] = ((int) entity.PartyAttendance[base.Key]) + 1;
                        }
                        else
                        {
                            entity.PartyAttendance.Add(base.Key, 1);
                        }
                    }
                    this.State = States.ToDance;
                    break;

                case States.ToDance:
                    if (base.Owner.Move())
                    {
                        PointF location = (PointF) base.Building.Map.getNode("Dance").Location;
                        float num = A.ST.Random.Next(80) - 20;
                        float num2 = A.ST.Random.Next(20) - 10;
                        base.Owner.Location = new PointF((location.X - num) - num2, (location.Y + (num / 2f)) - (num2 / 2f));
                        base.Owner.Pose = "DanceSW";
                        this.State = States.Dance;
                    }
                    break;

                case States.Dance:
                    if ((((entity != null) && (entity.DDRLockedBy == -1L)) && entity.Has("DDR")) && (A.ST.Random.Next(20) == 0))
                    {
                        entity.DDRLockedBy = base.Owner.ID;
                        base.Owner.Pose = "Walk";
                        base.Owner.Path = base.Building.Map.findPath("Dance", "DDR").ToPoints();
                        this.State = States.ToDDR;
                    }
                    break;

                case States.ToDDR:
                    if (base.Owner.Move())
                    {
                        base.Owner.Pose = "DanceSW";
                        base.Owner.Location = new PointF(base.Owner.Location.X, base.Owner.Location.Y + 10f);
                        this.State = States.DDR;
                        this.DDRCounter = 30;
                    }
                    break;

                case States.DDR:
                    if (this.DDRCounter-- < 0)
                    {
                        base.Owner.Path = base.Building.Map.findPath("DDR", "Dance").ToPoints();
                        entity.DDRLockedBy = -1L;
                        base.Owner.Pose = "Walk";
                        base.Owner.Location = new PointF(base.Owner.Location.X, base.Owner.Location.Y - 10f);
                        this.State = States.ToDance;
                    }
                    break;
            }
            return false;
        }

        private enum States
        {
            Init,
            ToDance,
            Dance,
            ToDDR,
            DDR
        }
    }
}

