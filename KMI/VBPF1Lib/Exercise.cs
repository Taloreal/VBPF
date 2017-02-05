namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class Exercise : Task
    {
        private States State = States.Init;

        public override string CategoryName()
        {
            return A.R.GetString("Exercise");
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
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "TreadMillWalk").ToPoints();
                    this.State = States.ToTreadMill;
                    break;

                case States.ToTreadMill:
                    if (base.Owner.Move())
                    {
                        if (!base.GetAppEntity().Has("TreadMill"))
                        {
                            PointF location = (PointF) base.Building.Map.getNode("TreadMillWalk").Location;
                            base.Owner.Location = new PointF(location.X, location.Y + 24f);
                            base.Owner.Pose = "JumpingJacksSW";
                        }
                        this.State = States.Exercise;
                    }
                    break;
                case States.Exercise:
                    if (A.ST.Period != base.EndPeriod)
                        break;
                    base.Owner.Location = (PointF) base.Building.Map.getNode("TreadMillWalk").Location;
                    base.Owner.Pose = "StandSW";
                    return true;
            }
            return false;
        }

        public override Color GetColor()
        {
            return Color.Orange;
        }

        private enum States
        {
            Init,
            ToTreadMill,
            Exercise
        }
    }
}

