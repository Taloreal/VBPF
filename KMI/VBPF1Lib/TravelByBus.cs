namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class TravelByBus : TravelTask
    {
        public KMI.VBPF1Lib.Bus Bus;
        public AppBuilding BusStop;
        public KMI.VBPF1Lib.Pedestrian Pedestrian = new KMI.VBPF1Lib.Pedestrian();
        private States State = States.Init;

        public override string CategoryName()
        {
            return A.R.GetString("Travel by Bus");
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
                {
                    this.Pedestrian.SetLocation(base.From);
                    AppEntity appEntity = base.GetAppEntity();
                    appEntity.BusTokens--;
                    base.From.Persons.Remove(base.Owner);
                    A.ST.City.Pedestrians.Add(this.Pedestrian);
                    this.Pedestrian.SendTo(A.ST.City.downtownAve, base.From.Street, 3);
                    this.State = States.ToBus;
                    break;
                }
                case States.WaitForBus:
                    this.Bus = A.ST.City.BusAt(base.From.Street, Math.Sign((int) (base.To.Street - base.From.Street)));
                    if (this.Bus != null)
                    {
                        A.ST.City.Pedestrians.Remove(this.Pedestrian);
                        this.State = States.OnBus;
                    }
                    break;

                case States.ToBus:
                    if (this.Pedestrian.NewStep())
                    {
                        this.State = States.WaitForBus;
                    }
                    break;

                case States.OnBus:
                    this.Pedestrian.Location = this.Bus.Location;
                    if (this.Bus.Location.Y == base.To.Street)
                    {
                        A.ST.City.Pedestrians.Add(this.Pedestrian);
                        this.Pedestrian.Location = this.Bus.Location;
                        this.Pedestrian.SendTo(base.To);
                        this.State = States.FromBus;
                    }
                    break;

                case States.FromBus:
                    if (!this.Pedestrian.NewStep())
                    {
                        break;
                    }
                    base.Owner.Pose = "StandSW";
                    base.To.Persons.Add(base.Owner);
                    A.ST.City.Pedestrians.Remove(this.Pedestrian);
                    if (base.To.Map != null)
                    {
                        base.Owner.Location = (PointF) base.To.Map.getNode("EntryPoint").Location;
                    }
                    return true;
            }
            return false;
        }

        public override int EstTimeInSteps(AppBuilding from, AppBuilding to)
        {
            int num = KMI.VBPF1Lib.Pedestrian.EstTimeInSteps(from.Avenue, from.Street, from.Lot, A.ST.City.downtownAve, from.Street, 3) + KMI.VBPF1Lib.Bus.EstTimeInSteps(A.ST.City.downtownAve, from.Street, 3, A.ST.City.downtownAve, to.Street, 3);
            num += KMI.VBPF1Lib.Pedestrian.EstTimeInSteps(A.ST.City.downtownAve, to.Street, 3, to.Avenue, to.Street, to.Lot);
            num += 0x927c0 / A.ST.SimulatedTimePerStep;
            int num2 = new TravelByFoot().EstTimeInSteps(from, to);
            if (num2 < num)
            {
                return num2;
            }
            return num;
        }

        private enum States
        {
            Init,
            ToDoor,
            WaitForBus,
            ToBus,
            OnBus,
            FromBus
        }
    }
}

