namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class Relax : Task
    {
        private States State = States.Init;

        public override string CategoryName()
        {
            return A.R.GetString("Relax");
        }

        public override void CleanUp()
        {
            base.CleanUp();
            base.Owner.Location = (PointF) base.Building.Map.getNode("Couch").Location;
            base.Owner.Pose = "StandSW";
            this.State = States.Init;
        }

        public override bool Do()
        {
            switch (this.State)
            {
                case States.Init:
                    base.Owner.Pose = "Walk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Couch").ToPoints();
                    this.State = States.ToCouch;
                    goto Label_016D;

                case States.ToCouch:
                {
                    if (!base.Owner.Move())
                    {
                        goto Label_016D;
                    }
                    PointF location = (PointF) base.Building.Map.getNode("Couch").Location;
                    if (!base.GetAppEntity().Has("Couch"))
                    {
                        base.Owner.Pose = "StandSW";
                        break;
                    }
                    base.Owner.Location = new PointF(location.X + 12f, location.Y - 6f);
                    base.Owner.Pose = "SitSW";
                    break;
                }
                case States.Relax:
                    if (A.ST.Period != base.EndPeriod)
                    {
                        goto Label_016D;
                    }
                    base.Owner.Location = (PointF) base.Building.Map.getNode("Couch").Location;
                    base.Owner.Pose = "StandSW";
                    return true;

                default:
                    goto Label_016D;
            }
            this.State = States.Relax;
        Label_016D:
            return false;
        }

        public override Color GetColor()
        {
            return Color.LightBlue;
        }

        private enum States
        {
            Init,
            ToCouch,
            Relax
        }
    }
}

