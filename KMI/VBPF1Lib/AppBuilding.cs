namespace KMI.VBPF1Lib
{
    using KMI.Biz.City;
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class AppBuilding : Building
    {
        private static string cr = Environment.NewLine;
        public PointF EntryPoint;
        public long ID;
        public MapV2 Map;
        public ArrayList Offerings;
        protected string ownerName;
        public ArrayList Persons;
        public float[] Prices;
        public float[] SaleDiscounts;

        public AppBuilding(CityBlock block, int lotIndex, BuildingType type) : base(block, lotIndex, type)
        {
            this.Offerings = new ArrayList();
            this.Persons = new ArrayList();
            this.ID = A.ST.GetNextID();
            this.ownerName = Utilities.GetRandomFirstName(A.ST.Random);
        }

        public virtual string GetBackgroundImage()
        {
            return "Not Implemented";
        }

        public override ArrayList GetDrawables(int aveRegion, int streetRegion)
        {
            ArrayList list = new ArrayList();
            string clickString = "";
            foreach (Offering offering in this.Offerings)
            {
                if (!offering.Taken)
                {
                    clickString = clickString + offering.Description() + cr + cr;
                }
            }
            if (clickString != "")
            {
                clickString = A.R.GetString("Available {0}:", new object[] { ((Offering) this.Offerings[0]).ThingName() }) + cr + cr + clickString;
            }
            if (base.BuildingType.Index == 6)
            {
                clickString = A.R.GetString("Bus Stop: Buy tokens here!");
            }
            if (base.BuildingType.Index == 8)
            {
                clickString = ((BankAccount) this.Offerings[0]).BankName + " -- " + clickString;
            }
            if (base.BuildingType.Index == 12)
            {
                clickString = A.R.GetString("{0}: Offering furniture, computers, and more!", new object[] { this.OwnerName });
            }
            if (base.BuildingType.Index == 9)
            {
                clickString = A.R.GetString("Taranti Auto & Loan: New and Used Cars for Less!");
            }
            if (base.BuildingType.Index == 11)
            {
                clickString = A.R.GetString("Steiner & Wilson: Insuring Your Happiness");
            }
            if (base.BuildingType.Index == 13)
            {
                clickString = A.R.GetString("Supermarket for all your food needs!");
            }
            if (base.BuildingType.Index == 14)
            {
                clickString = A.R.GetString("InternetConnect for super fast Internet access!");
            }
            if (base.BuildingType.Index == 15)
            {
                clickString = A.R.GetString("Auto Garage: gas, preventive maintenance, repairs and more!");
            }
            if (base.BuildingType.Index == 16)
            {
                clickString = A.R.GetString("Fiduciary Investments: Make Your Money Grow!");
            }
            Point location = Point.Round(KMI.Biz.City.City.Transform2((float) base.Avenue, (float) base.Street, (float) base.lotIndex, aveRegion, streetRegion));
            long ownerID = -1L;
            bool isOwnersDwelling = false;
            if (base.Owner != null)
            {
                ownerID = base.Owner.ID;
                isOwnersDwelling = ((AppEntity) base.Owner).Dwelling == this;
            }
            AppBuildingDrawable drawable = new AppBuildingDrawable(location, "Building" + base.BuildingType.Index, base.BuildingType, base.Avenue, base.Street, base.Lot, ownerID, (ArrayList) this.Offerings.Clone(), clickString, isOwnersDwelling) {
                BuildingID = this.ID
            };
            list.Add(drawable);
            return list;
        }

        public virtual ArrayList GetInsideDrawables()
        {
            return new ArrayList();
        }

        public string Description
        {
            get
            {
                return base.BuildingType.Name;
            }
        }

        public string OwnerName
        {
            get
            {
                return (this.ownerName + "'s " + this.Description);
            }
        }
    }
}

