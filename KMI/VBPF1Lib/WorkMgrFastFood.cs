﻿namespace KMI.VBPF1Lib
{
    using System;
    using System.Drawing;

    [Serializable]
    public class WorkMgrFastFood : WorkTask
    {
        public int Register;
        private States State = States.Init;

        public WorkMgrFastFood(int register)
        {
            this.Register = register;
            base.WorkExperienceRequired.Add("Cashier", 1f);
            base.AcademicExperienceRequired.Add("Food Service Mgt I");
            base.HourlyWage = 9.5f;
        }

        //public override string CategoryName()
        //{
        //    //return A.R.GetString("Work\r\n" + this.Name());
        //    return base.CategoryName();
        //}

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
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Serve" + this.Register).ToPoints();
                    this.State = States.MoveToServe;
                    break;

                case States.MoveToServe:
                    if (base.Owner.Move())
                    {
                        if (base.Owner.Pose == "FFCarryFood")
                        {
                            ((FastFoodStore) base.Building).OrderUp[this.Register] = true;
                        }
                        base.Owner.Pose = "FFStandSW";
                        this.State = States.WaitForOrder;
                    }
                    break;

                case States.WaitForOrder:
                    if (A.ST.Period != base.EndPeriod)
                    {
                        if (((FastFoodStore) base.Building).OrderIn[this.Register])
                        {
                            ((FastFoodStore) base.Building).OrderIn[this.Register] = false;
                            base.Owner.Pose = "FFWalk";
                            base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "ServeBack" + this.Register).ToPoints();
                            this.State = States.GetOrder;
                        }
                        break;
                    }
                    this.State = States.Exit;
                    base.Owner.Pose = "FFWalk";
                    base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "EntryPoint").ToPoints();
                    break;

                case States.GetOrder:
                    if (base.Owner.Move())
                    {
                        base.Owner.Pose = "FFCarryFood";
                        base.Owner.Path = base.Building.Map.findPath(base.Owner.Location, "Serve" + this.Register).ToPoints();
                        this.State = States.MoveToServe;
                        ((FastFoodStore) base.Building).TakeItem(this.Register);
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
            return "Shift Manager";
        }

        public override string ResumeDescription()
        {
            return A.R.GetString("Supervised counter personnel ensuring consistently high customer satisfaction levels.");
        }

        private enum States
        {
            Init,
            MoveToServe,
            WaitForOrder,
            GetOrder,
            Exit
        }
    }
}

