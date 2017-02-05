namespace KMI.VBPF1Lib
{
    using KMI.Biz.City;
    using KMI.Sim.Drawables;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class Dwelling : AppBuilding
    {
        public InsurancePolicy Insurance;
        public int MonthsLeftOnLease;

        public Dwelling(CityBlock block, int lotIndex, BuildingType type) : base(block, lotIndex, type)
        {
            this.Insurance = new InsurancePolicy(250f, false, 0f);
            base.Map = AppConstants.HomeMap;
            base.EntryPoint = (PointF) base.Map.getNode("EntryPoint").Location;
        }

        public override string GetBackgroundImage()
        {
            return "HomeBack";
        }

        public override ArrayList GetInsideDrawables()
        {
            int num3;
            int num4;
            AppEntity owner = (AppEntity) base.Owner;
            ArrayList list = new ArrayList();
            ArrayList c = new ArrayList();
            ArrayList list3 = new ArrayList();
            ArrayList list4 = new ArrayList();
            bool flag = (owner != null) && (owner.Dwelling == this);
            bool flag2 = false;
            foreach (VBPFPerson person in base.Persons)
            {
                if (person.Pose.EndsWith("EatSE"))
                {
                    flag2 = true;
                }
                if (person.Location.Y < ((-0.5f * (person.Location.X - 135f)) + 360f))
                {
                    c.AddRange(person.GetDrawables());
                }
                else if (person.Location.Y < ((0.5f * (person.Location.X - 365f)) + 242f))
                {
                    list3.AddRange(person.GetDrawables());
                }
                else
                {
                    list4.AddRange(person.GetDrawables());
                }
            }
            c.Sort();
            list3.Sort();
            list4.Sort();
            if (flag)
            {
                foreach (PurchasableItem item in owner.PurchasedItems)
                {
                    if (item.Name.StartsWith("Art"))
                    {
                        list.Add(new Drawable(base.Map.getNode(item.Name).Location, item.ImageName));
                    }
                }
                if (owner.Has("DDR"))
                {
                    int num = 0;
                    if (owner.DDRLockedBy > -1L)
                    {
                        num = A.ST.Random.Next(5);
                    }
                    FlexDrawable drawable = new FlexDrawable(base.Map.getNode("DDR").Location, "DDR" + num) {
                        VerticalAlignment = FlexDrawable.VerticalAlignments.Middle,
                        HorizontalAlignment = FlexDrawable.HorizontalAlignments.Center
                    };
                    list.Add(drawable);
                }
                if (owner.Has("Carpet"))
                {
                    list.Add(new Drawable(base.Map.getNode("Carpet").Location, "Carpet" + owner.ImageIndexOf("Carpet")));
                }
                PointF tf = (PointF) base.Map.getNode("EndTable").Location;
                list.Add(new Drawable(tf, "EndTable"));
                int num2 = 0x19;
                for (num3 = 0; num3 < Math.Min((int) (2 * num2), (int) (owner.BusTokens / 2)); num3++)
                {
                    num4 = num3 / num2;
                    Drawable drawable2 = new Drawable(Point.Round(new PointF((tf.X + 97f) + (num4 * 10), ((tf.Y - ((num3 % num2) * 2)) + 3f) + (num4 * 5))), "BusToken") {
                        ToolTipText = A.R.GetString("{0} Bus Tokens", new object[] { owner.BusTokens })
                    };
                    list.Add(drawable2);
                }
                int num5 = Math.Min(20, 80 / Math.Max(1, owner.PartyFood.Count));
                PointF tf2 = (PointF) base.Map.getNode("Platter0").Location;
                int num6 = 0;
                ArrayList list5 = new ArrayList();
                foreach (PurchasableItem item2 in owner.PartyFood)
                {
                    Drawable drawable3 = new Drawable(new PointF(tf2.X - (num6 * num5), tf2.Y + ((num6++ * num5) / 2)), item2.ImageName + "Small") {
                        ToolTipText = item2.FriendlyName
                    };
                    list.Add(drawable3);
                }
            }
            list.Add(new Drawable(base.Map.getNode("Chair4").Location, "Chair"));
            list.Add(new Drawable(base.Map.getNode("Chair4b").Location, "Chair"));
            list.Add(new Drawable(base.Map.getNode("KitchenBar3").Location, "KitchenBar3"));
            list.AddRange(c);
            list.Add(new Drawable(base.Map.getNode("ApartmentKitchenInteriorWall").Location, "ApartmentKitchenInteriorWall"));
            Point location = base.Map.getNode("KitchenBar").Location;
            list.Add(new Drawable(location, "KitchenBar"));
            if (flag2)
            {
                list.Add(new Drawable(new Point(location.X + 0x36, location.Y + 8), "PlateOfFood"));
            }
            list.Add(new Drawable(base.Map.getNode("WallCabinet").Location, "WallCabinet"));
            Drawable drawable4 = new RefrigeratorDrawable(base.Map.getNode("Refrigerator").Location, "Refrigerator");
            if (owner != null)
            {
                drawable4.ToolTipText = A.R.GetString("Food for {0} meals.", new object[] { ((AppEntity) base.Owner).Food.Count });
            }
            list.Add(drawable4);
            list.Add(new Drawable(base.Map.getNode("Oven").Location, "Oven"));
            list.Add(new Drawable(base.Map.getNode("InteriorWall2").Location, "InteriorWall2"));
            list.Add(new Drawable(base.Map.getNode("BuiltInDesk").Location, "BuiltInDesk"));
            if (flag)
            {
                if (owner.Has("Bed"))
                {
                    list.Add(new Drawable(base.Map.getNode("Bed1").Location, "Bed" + owner.ImageIndexOf("Bed")));
                    list.Add(new Drawable(base.Map.getNode("Lamp").Location, "Lamp"));
                }
                if (owner.Has("TreadMill"))
                {
                    list.Add(new Drawable(base.Map.getNode("TreadMill").Location, "TreadMill"));
                }
                if (owner.Has("Couch"))
                {
                    list.Add(new Drawable(base.Map.getNode("Couch1").Location, "Couch" + owner.ImageIndexOf("Couch")));
                    list.Add(new Drawable(base.Map.getNode("Chair3").Location, "Chair" + owner.ImageIndexOf("Couch")));
                    list.Add(new Drawable(base.Map.getNode("Chair3Back").Location, "ChairBack" + owner.ImageIndexOf("Couch")));
                    list.Add(new Drawable(base.Map.getNode("CofeeTable").Location, "CoffeeTable" + owner.ImageIndexOf("Couch")));
                }
                if (owner.Has("TV"))
                {
                    list.Add(new Drawable(base.Map.getNode("TVBack").Location, "TVBack" + owner.ImageIndexOf("TV")));
                }
                PointF tf3 = (PointF) base.Map.getNode("BuiltInDesk").Location;
                int num7 = 0x19;
                float cash = ((AppEntity) base.Owner).Cash;
                if (cash > 0.01f)
                {
                    for (num3 = 0; num3 < Math.Min((float) (4 * num7), cash / 20f); num3++)
                    {
                        num4 = num3 / num7;
                        Drawable drawable5 = new CashDrawable(Point.Round(new PointF((tf3.X + 16f) + (num4 * 6), ((tf3.Y - ((num3 % num7) * 2)) + 41f) - (num4 * 3))), "Money", " ", cash) {
                            ToolTipText = Utilities.FC(cash, 2, A.I.CurrencyConversion)
                        };
                        list.Add(drawable5);
                    }
                }
                num3 = 0;
                while (num3 < ((AppEntity) base.Owner).Bills.Count)
                {
                    list.Add(new BillDrawable(Point.Round(new PointF(tf3.X + 74f, (tf3.Y - (num3 * 2)) + 12f)), "Paper", " "));
                    num3++;
                }
                for (num3 = 0; num3 < ((AppEntity) base.Owner).Checks.Count; num3++)
                {
                    list.Add(new CheckDrawable(Point.Round(new PointF(tf3.X + 96f, (tf3.Y - (num3 * 2)) + 11f)), "Check", " "));
                }
                if (owner.Has("Computer"))
                {
                    list.Add(new ComputerDrawable(Point.Round(new PointF(tf3.X + 45f, tf3.Y + 5f)), "Computer", " "));
                }
            }
            list.AddRange(list3);
            list.Add(new Drawable(base.Map.getNode("InteriorWall1").Location, "InteriorWall1"));
            if (flag)
            {
                if (owner.Has("Stereo"))
                {
                    list.Add(new Drawable(base.Map.getNode("Stereo").Location, "Stereo" + owner.ImageIndexOf("Stereo")));
                }
                if (owner != null)
                {
                    for (num3 = 0; num3 < Math.Min(8, owner.AcademicTaskHistory.Count); num3++)
                    {
                        PointF tf4 = (PointF) base.Map.getNode("Diploma").Location;
                        int num9 = 0x1c;
                        Drawable drawable6 = new Drawable(new PointF(tf4.X + ((num3 % 4) * num9), (tf4.Y + (((num3 % 4) * num9) / 2)) + ((num3 / 4) * 0x1c)), "Diploma") {
                            ToolTipText = A.R.GetString("Diploma for {0}", new object[] { ((AttendClass) owner.AcademicTaskHistory.GetByIndex(num3)).Course.Name })
                        };
                        list.Add(drawable6);
                    }
                }
            }
            list.AddRange(list4);
            if (flag && (owner.Person.Task is BeSick))
            {
                PointF tf5 = (PointF) base.Map.getNode("Bed").Location;
                list.Add(new Drawable(new PointF(tf5.X + 84f, tf5.Y - 22f), "IceBag"));
            }
            return list;
        }
    }
}

