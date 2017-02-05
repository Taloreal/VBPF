namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class Sleep : Task
    {
        private States State = States.Init;

        public override string CategoryName()
        {
            return A.R.GetString("Sleep");
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
                    base.Owner.Pose = "Walk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Bed").ToPoints();
                    this.State = States.ToBed;
                    break;

                case States.ToBed:
                    if (base.Owner.Move())
                    {
                        PointF location = (PointF) base.Building.Map.getNode("Bed").Location;
                        base.Owner.Location = new PointF(location.X + 62f, location.Y + 52f);
                        base.Owner.Pose = "SleepSW";
                        this.State = States.Sleep;
                    }
                    break;

                case States.Sleep:
                    if (A.ST.Period != base.EndPeriod)
                    {
                        break;
                    }
                    base.Owner.Location = (PointF) base.Building.Map.getNode("Bed").Location;
                    base.Owner.Pose = "StandSW";
                    return true;
            }
            return false;
        }

        public override Color GetColor()
        {
            return Color.Blue;
        }

        private enum States
        {
            Init,
            ToBed,
            Sleep
        }
    }
}

