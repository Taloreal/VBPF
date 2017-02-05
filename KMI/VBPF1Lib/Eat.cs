namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Drawing;

    [Serializable]
    public class Eat : Task
    {
        private States State = States.Init;

        public override string CategoryName()
        {
            return A.R.GetString("Eat");
        }

        public override void CleanUp()
        {
            base.CleanUp();
            this.State = States.Init;
        }

        public override bool Do()
        {
            if (A.ST.Period == base.EndPeriod)
            {
                base.Owner.Location = (PointF) base.Building.Map.getNode("Eat").Location;
                base.Owner.Pose = "StandSE";
                return true;
            }
            switch (this.State)
            {
                case States.Init:
                    base.Owner.Pose = "Walk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Eat").ToPoints();
                    this.State = States.ToEat;
                    break;

                case States.ToEat:
                    if (base.Owner.Move())
                    {
                        PointF location = (PointF) base.Building.Map.getNode("Eat").Location;
                        base.Owner.Location = new PointF(location.X - 50f, location.Y + 5f);
                        this.State = States.Eat;
                    }
                    break;

                case States.Eat:
                {
                    AppEntity appEntity = base.GetAppEntity();
                    if (appEntity.Food.Count <= 0)
                    {
                        base.Owner.Pose = "SitSE";
                        appEntity.Player.SendPeriodicMessage(A.R.GetString("You tried to eat, but there's no more food in fridge!"), "", NotificationColor.Yellow, 1f);
                        break;
                    }
                    base.Owner.Pose = "EatSE";
                    break;
                }
            }
            return false;
        }

        public override Color GetColor()
        {
            return Color.LightGreen;
        }

        private enum States
        {
            Init,
            ToEat,
            Eat
        }
    }
}

