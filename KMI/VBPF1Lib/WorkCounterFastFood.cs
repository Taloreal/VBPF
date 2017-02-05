namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class WorkCounterFastFood : WorkTask
    {
        public int frame;
        public int Register;
        private States State = States.Init;

        public WorkCounterFastFood(int register)
        {
            this.Register = register;
            base.HourlyWage = 7.25f;
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
                    base.Owner.Pose = "FFWalk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Register" + this.Register).ToPoints();
                    this.State = States.MoveToRegister;
                    break;

                case States.MoveToRegister:
                    if (base.Owner.Move())
                    {
                        base.Owner.Pose = "FFStandSW";
                        this.State = States.WaitForCust;
                    }
                    break;

                case States.WaitForCust:
                    if (A.ST.Period != base.EndPeriod)
                    {
                        if (((FastFoodStore) base.Building).CustomerReadyAtRegister(this.Register))
                        {
                            base.Owner.Pose = "FFTakeOrder";
                            this.State = States.TakeOrder;
                            this.frame = 0;
                        }
                        break;
                    }
                    this.State = States.Exit;
                    base.Owner.Pose = "FFWalk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "EntryPoint").ToPoints();
                    break;

                case States.TakeOrder:
                    if (++this.frame >= (0x4c / (A.ST.SimulatedTimePerStep / 0x4e20)))
                    {
                        base.Owner.Pose = "FFStandSW";
                        this.State = States.WaitForCust;
                        ((FastFoodStore) base.Building).OrderIn[this.Register] = true;
                    }
                    break;

                case States.Exit:
                    base.Owner.Location = (PointF) base.Building.Map.getNode("EntryPoint").Location;
                    base.Owner.Pose = "StandNW";
                    return true;
            }
            return false;
        }

        public override string Name()
        {
            return "Cashier";
        }

        public override string ResumeDescription()
        {
            return A.R.GetString("Responsible for ringing up customer orders and handling customer payments. Maintained high customer service levels.");
        }

        private enum States
        {
            Init,
            MoveToRegister,
            WaitForCust,
            TakeOrder,
            Exit
        }
    }
}

