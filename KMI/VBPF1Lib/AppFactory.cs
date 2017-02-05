namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Reflection;
    using System.Resources;

    public class AppFactory : SimFactory
    {
        public override Entity CreateEntity(Player player, string entityName)
        {
            return new AppEntity(player, entityName);
        }

        public override SortedList CreateImageTable()
        {
            int num;
            string str2;
            Bitmap bitmap2;
            Bitmap bitmap3;
            SortedList table = new SortedList();
            Type typeFromAssembly = typeof(frmMain);
            table.Add("HRLogo", base.CBmp(typeFromAssembly, "Images.HRLogo.png"));
            table.Add("StatusBusTokens", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.BusTokens.png"));
            table.Add("StatusGas", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Gas.png"));
            table.Add("StatusFood", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Food.png"));
            table.Add("StatusHealth", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Health.png"));
            table.Add("PointerHome", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Home.png"));
            table.Add("PointerHomeGray", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.HomeGray.png"));
            table.Add("PointerSchool", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.School.png"));
            table.Add("PointerSchoolGray", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.SchoolGray.png"));
            table.Add("PointerWork", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Work.png"));
            table.Add("PointerWorkGray", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.WorkGray.png"));
            table.Add("PointerPerson", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Person.png"));
            table.Add("PointerPersonGray", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.PersonGray.png"));
            table.Add("PointerCondo", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Condo.png"));
            table.Add("PointerCondoGray", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.CondoGray.png"));
            table.Add("City", base.CBmp(typeFromAssembly, "Images.CityView.City.png"));
            table.Add("CarNESW", base.CBmp(typeFromAssembly, "Images.CityView.CarNESW.gif"));
            table.Add("CarNWSE", base.CBmp(typeFromAssembly, "Images.CityView.CarNWSE.gif"));
            for (num = 0; num < AppConstants.BuildingTypeCount; num++)
            {
                Bitmap bitmap = base.CBmp(typeFromAssembly, "Images.CityView.Building" + num + ".png");
                table.Add("Building" + num, bitmap);
            }
            table.Add("Arrow", base.CBmp(typeFromAssembly, "Images.Arrow.png"));
            base.LoadWithCompassPoints(table, typeFromAssembly, "Images.CityView.Bus", "png");
            table.Add("Walker", base.CBmp(typeFromAssembly, "Images.CityView.Walker.png"));
            for (num = 0; num < 3; num++)
            {
                table.Add("GasCan" + num, base.CBmp(typeFromAssembly, "Images.CityView.GasCan" + num + ".png"));
            }
            table.Add("MotorOil", base.CBmp(typeFromAssembly, "Images.CityView.MotorOil.png"));
            table.Add("CarOnLift", base.CBmp(typeFromAssembly, "Images.CityView.CarOnLift.png"));
            table.Add("CityNavIcon", base.CBmp(typeFromAssembly, "Images.CityNavIcon.png"));
            for (num = 0; num < 6; num++)
            {
                table.Add("Car" + num, base.CBmp(typeFromAssembly, "Images.CityView.Car" + num + ".png"));
                table.Add("StatusCar" + num + "OK", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Car" + num + "OK.png"));
                table.Add("StatusCar" + num + "Broken", base.CBmp(typeFromAssembly, "Images.StatusIconsAndPointers.Car" + num + "Broken.png"));
            }
            table.Add("HomeBack", base.CBmp(typeFromAssembly, "Images.HomeView.ApartmentFloor2.png"));
            for (num = 0; num < 3; num++)
            {
                table.Add("Bed" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Bed" + num + ".png"));
            }
            for (num = 0; num < 4; num++)
            {
                table.Add("Couch" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Couch" + num + ".png"));
                table.Add("CoffeeTable" + num, base.CBmp(typeFromAssembly, "Images.HomeView.CoffeeTable" + num + ".png"));
                table.Add("Chair" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Chair" + num + ".png"));
                table.Add("ChairBack" + num, base.CBmp(typeFromAssembly, "Images.HomeView.ChairBack" + num + ".png"));
            }
            for (num = 0; num < 5; num++)
            {
                table.Add("Carpet" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Carpet" + num + ".png"));
            }
            table.Add("ApartmentKitchenInteriorWall", base.CBmp(typeFromAssembly, "Images.HomeView.ApartmentKitchenInteriorWall.png"));
            table.Add("InteriorWall1", base.CBmp(typeFromAssembly, "Images.HomeView.InteriorWall1.png"));
            table.Add("InteriorWall2", base.CBmp(typeFromAssembly, "Images.HomeView.InteriorWall2.png"));
            table.Add("Computer", base.CBmp(typeFromAssembly, "Images.HomeView.Computer.png"));
            table.Add("KitchenBar3", base.CBmp(typeFromAssembly, "Images.HomeView.KitchenBar3.png"));
            table.Add("KitchenBar", base.CBmp(typeFromAssembly, "Images.HomeView.KitchenBar.png"));
            table.Add("Lamp2", base.CBmp(typeFromAssembly, "Images.HomeView.Lamp2.png"));
            table.Add("Lamp", base.CBmp(typeFromAssembly, "Images.HomeView.Lamp.png"));
            for (num = 0; num < 2; num++)
            {
                table.Add("TVBack" + num, base.CBmp(typeFromAssembly, "Images.HomeView.TVBack" + num + ".png"));
                table.Add("TVFront" + num, base.CBmp(typeFromAssembly, "Images.HomeView.TVFront" + num + ".png"));
            }
            table.Add("WallCabinet", base.CBmp(typeFromAssembly, "Images.HomeView.WallCabinet.png"));
            table.Add("FloorCabinet", base.CBmp(typeFromAssembly, "Images.HomeView.FloorCabinet.png"));
            table.Add("TreadMill", base.CBmp(typeFromAssembly, "Images.HomeView.TreadMill.png"));
            table.Add("Chair", base.CBmp(typeFromAssembly, "Images.HomeView.Chair.png"));
            table.Add("Decor", base.CBmp(typeFromAssembly, "Images.HomeView.Decor.png"));
            table.Add("EndTable2", base.CBmp(typeFromAssembly, "Images.HomeView.EndTable2.png"));
            table.Add("BuiltInDesk", base.CBmp(typeFromAssembly, "Images.HomeView.BuiltInDesk.png"));
            table.Add("Oven", base.CBmp(typeFromAssembly, "Images.HomeView.Oven.png"));
            table.Add("Plant", base.CBmp(typeFromAssembly, "Images.HomeView.Plant.png"));
            table.Add("WallSconce", base.CBmp(typeFromAssembly, "Images.HomeView.WallSconce.png"));
            table.Add("Refrigerator", base.CBmp(typeFromAssembly, "Images.HomeView.Refrigerator.png"));
            table.Add("Paper", base.CBmp(typeFromAssembly, "Images.HomeView.Paper.png"));
            table.Add("Money", base.CBmp(typeFromAssembly, "Images.HomeView.Money.png"));
            table.Add("Check", base.CBmp(typeFromAssembly, "Images.HomeView.Check.png"));
            for (num = 0; num < 3; num++)
            {
                table.Add("BagOfFood" + num, base.CBmp(typeFromAssembly, "Images.HomeView.BagOfFood" + num + ".png"));
            }
            for (num = 0; num < 5; num++)
            {
                table.Add("Platter" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Platter" + num + ".png"));
                table.Add("Platter" + num + "Small", base.CBmp(typeFromAssembly, "Images.HomeView.Platter" + num + "Small.png"));
            }
            table.Add("PlateOfFood", base.CBmp(typeFromAssembly, "Images.HomeView.PlateOfFood.png"));
            table.Add("IceBag", base.CBmp(typeFromAssembly, "Images.HomeView.IceBag.png"));
            table.Add("EndTable", base.CBmp(typeFromAssembly, "Images.HomeView.EndTableSE.png"));
            table.Add("BusToken", base.CBmp(typeFromAssembly, "Images.HomeView.BusToken.png"));
            table.Add("Diploma", base.CBmp(typeFromAssembly, "Images.HomeView.Diploma.png"));
            for (num = 0; num < 3; num++)
            {
                table.Add("BusTokens" + num, base.CBmp(typeFromAssembly, "Images.HomeView.BusTokens" + num + ".png"));
            }
            for (num = 0; num < 5; num++)
            {
                table.Add("DDR" + num, base.CBmp(typeFromAssembly, "Images.HomeView.DDR" + num + ".png"));
            }
            for (num = 0; num < 7; num++)
            {
                table.Add("Art" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Art" + num + ".png"));
            }
            num = 0;
            while (num < 3)
            {
                table.Add("Stereo" + num, base.CBmp(typeFromAssembly, "Images.HomeView.Stereo" + num + ".png"));
                num++;
            }
            table.Add("WorkBack", base.CBmp(typeFromAssembly, "Images.WorkView0.FastFoodFloor.png"));
            table.Add("BagOfFood", base.CBmp(typeFromAssembly, "Images.WorkView0.BagOfFood.png"));
            table.Add("CounterFastFood", base.CBmp(typeFromAssembly, "Images.WorkView0.CounterFastFood.png"));
            table.Add("FoodContainer1", base.CBmp(typeFromAssembly, "Images.WorkView0.FoodContainer1.png"));
            table.Add("FoodContainer0", base.CBmp(typeFromAssembly, "Images.WorkView0.FoodContainer0.png"));
            table.Add("FoodWall", base.CBmp(typeFromAssembly, "Images.WorkView0.FoodWall.png"));
            table.Add("FoodWallTop", base.CBmp(typeFromAssembly, "Images.WorkView0.FoodWallTop.png"));
            table.Add("SodaMachine", base.CBmp(typeFromAssembly, "Images.WorkView0.SodaMachine.png"));
            table.Add("TreeFastFood", base.CBmp(typeFromAssembly, "Images.WorkView0.TreeFastFood.png"));
            table.Add("RightGlass", base.CBmp(typeFromAssembly, "Images.WorkView0.RightGlass.png"));
            table.Add("LeftGlass", base.CBmp(typeFromAssembly, "Images.WorkView0.LeftGlass.png"));
            table.Add("Bar", base.CBmp(typeFromAssembly, "Images.WorkView0.Bar.png"));
            table.Add("PlantsFrontLeft", base.CBmp(typeFromAssembly, "Images.WorkView0.PlantsFrontLeft.png"));
            table.Add("PlantsFrontRight", base.CBmp(typeFromAssembly, "Images.WorkView0.PlantsFrontRight.png"));
            table.Add("OfficeBack", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeBack.png"));
            table.Add("SWWall", base.CBmp(typeFromAssembly, "Images.WorkView1.SWWall.png"));
            table.Add("SEWall", base.CBmp(typeFromAssembly, "Images.WorkView1.SEWall.png"));
            table.Add("OfficeCouch", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeCouch.png"));
            table.Add("OfficeManagerDesk", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeManagerDesk.png"));
            table.Add("OfficeManagerPainting", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeManagerPainting.png"));
            table.Add("OfficeManagerPlant", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeManagerPlant.png"));
            table.Add("OfficePlant", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficePlant.png"));
            table.Add("OfficePrinter", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficePrinter.png"));
            table.Add("OfficeSupervisorBookshelf", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeSupervisorBookshelf.png"));
            table.Add("OfficeSupervisorDesk", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeSupervisorDesk.png"));
            table.Add("OfficeWorkerDesk", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeWorkerDesk.png"));
            table.Add("OfficeWorkerChair", base.CBmp(typeFromAssembly, "Images.WorkView1.OfficeWorkerChair.png"));
            table.Add("ClassBack", base.CBmp(typeFromAssembly, "Images.ClassView.SchoolFloor.png"));
            table.Add("TeacherDesk", base.CBmp(typeFromAssembly, "Images.ClassView.TeacherDesk.png"));
            table.Add("SchoolChairBack", base.CBmp(typeFromAssembly, "Images.ClassView.SchoolChairBack.png"));
            table.Add("SchoolChairBottom", base.CBmp(typeFromAssembly, "Images.ClassView.SchoolChairBottom.png"));
            table.Add("Table", base.CBmp(typeFromAssembly, "Images.ClassView.Table.png"));
            table.Add("1040EZ", base.CBmp(typeFromAssembly, "Images.1040EZ.png"));
            foreach (string str in new string[] { "Female", "Male" })
            {
                num = 0;
                while (num < 0x1c)
                {
                    str2 = num.ToString().PadLeft(4, '0');
                    table.Add(str + "TeacherBoardSW" + str2, base.CBmp(typeFromAssembly, "Images.ClassView." + str + "TeacherBoardSW" + str2 + ".png"));
                    num++;
                }
            }
            foreach (string str in new string[] { "Female", "Male" })
            {
                num = 0;
                while (num < 0x13)
                {
                    str2 = num.ToString().PadLeft(3, '0');
                    bitmap2 = new Bitmap(typeFromAssembly, "Images.People." + str + "FFTakeOrderSW" + str2 + ".gif");
                    this.PalettizeAndInsert(table, bitmap2, str, "FFTakeOrder", "SW", str2);
                    num++;
                }
                foreach (string str3 in new string[] { "NW", "SW" })
                {
                    num = 0;
                    while (num < 9)
                    {
                        str2 = num.ToString().PadLeft(3, '0');
                        bitmap2 = new Bitmap(typeFromAssembly, "Images.People." + str + "FastFoodWalk" + str3 + str2 + ".gif");
                        this.PalettizeFlipAndInsert(table, bitmap2, str, "FFWalk", str3, str2);
                        num += 2;
                    }
                    bitmap3 = new Bitmap(typeFromAssembly, "Images.People." + str + "FastFoodStand" + str3 + ".gif");
                    this.PalettizeFlipAndInsert(table, bitmap3, str, "FFStand", str3, "");
                }
            }
            foreach (string str in new string[] { "Female", "Male" })
            {
                for (num = 0; num < 9; num++)
                {
                    bitmap2 = new Bitmap(typeFromAssembly, string.Concat(new object[] { "Images.WorkView1.Type.", str, "SitTypeNW00", num, ".gif" }));
                    this.PalettizeAndInsert(table, bitmap2, str, "SitType", "NW", "00" + num.ToString());
                }
            }
            foreach (string str4 in new string[] { "Stand", "Sit", "Sleep" })
            {
                foreach (string str in new string[] { "Female", "Male" })
                {
                    foreach (string str3 in new string[] { "NW", "SW" })
                    {
                        bitmap3 = new Bitmap(typeFromAssembly, "Images.People." + str + str4 + str3 + ".gif");
                        this.PalettizeFlipAndInsert(table, bitmap3, str, str4, str3, "");
                    }
                }
            }
            num = 0;
            while (num < 0x12)
            {
                if (num < 6)
                {
                    bitmap3 = new Bitmap(typeFromAssembly, "Images.People.Palette" + num + ".gif");
                }
                else if (num < 12)
                {
                    bitmap3 = new Bitmap(typeFromAssembly, "Images.People.UserPeople.Female" + (num - 6) + ".gif");
                }
                else
                {
                    bitmap3 = new Bitmap(typeFromAssembly, "Images.People.UserPeople.Male" + (num - 12) + ".gif");
                }
                ColorPalette palette = bitmap3.Palette;
                for (int i = 240; i < 0x100; i++)
                {
                    palette.Entries[i] = Color.FromArgb(0, palette.Entries[i]);
                }
                bitmap3.Palette = palette;
                table.Add("Palette" + num, bitmap3);
                num++;
            }
            foreach (string str4 in new string[] { "Walk", "CarryFood", "FFCarryFood" })
            {
                foreach (string str in new string[] { "Female", "Male" })
                {
                    foreach (string str3 in new string[] { "NW", "SW" })
                    {
                        num = 0;
                        while (num < 9)
                        {
                            bitmap2 = new Bitmap(typeFromAssembly, string.Concat(new object[] { "Images.People.", str, str4, str3, "00", num, ".gif" }));
                            this.PalettizeFlipAndInsert(table, bitmap2, str, str4, str3, "00" + num);
                            num += 2;
                        }
                    }
                }
            }
            foreach (string str in new string[] { "Female", "Male" })
            {
                num = 0;
                while (num < 9)
                {
                    bitmap2 = new Bitmap(typeFromAssembly, string.Concat(new object[] { "Images.People.", str, "JumpingJacksSW00", num, ".gif" }));
                    this.PalettizeAndInsert(table, bitmap2, str, "JumpingJacks", "SW", "00" + num);
                    num++;
                }
            }
            foreach (string str in new string[] { "Female", "Male" })
            {
                num = 0;
                while (num < 0x1b)
                {
                    bitmap2 = new Bitmap(typeFromAssembly, "Images.People." + str + "DanceSW" + num.ToString().PadLeft(3, '0') + ".gif");
                    this.PalettizeFlipAndInsert(table, bitmap2, str, "Dance", "SW", num.ToString().PadLeft(3, '0'));
                    num++;
                }
            }
            foreach (string str in new string[] { "Female", "Male" })
            {
                num = 0;
                while (num < 10)
                {
                    bitmap2 = new Bitmap(typeFromAssembly, string.Concat(new object[] { "Images.People.", str, "EatSW00", num, ".gif" }));
                    this.PalettizeFlipAndInsert(table, bitmap2, str, "Eat", "SW", "00" + num);
                    num++;
                }
            }
            AppConstants.CarryAnchorPoints = new Hashtable();
            foreach (string str4 in new string[] { "CarryFood", "FFCarryFood" })
            {
                foreach (string str in new string[] { "Female", "Male" })
                {
                    foreach (string str3 in new string[] { "NW", "SW", "NE", "SE" })
                    {
                        for (num = 0; num < 9; num += 2)
                        {
                            string key = string.Concat(new object[] { str, str4, str3, "00", num });
                            bitmap2 = (Bitmap) table[key];
                            for (int j = 0; j < bitmap2.Width; j++)
                            {
                                for (int k = 0; k < bitmap2.Height; k++)
                                {
                                    Color pixel = bitmap2.GetPixel(j, k);
                                    if (((pixel.R == 0) && (pixel.G == 0)) && (pixel.B == 0))
                                    {
                                        AppConstants.CarryAnchorPoints.Add(key, new Point(j - (bitmap2.Width / 2), (k - bitmap2.Height) + 15));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            table.Add("LogoHSN Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoHSN.png"));
            table.Add("LogoOlympic Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoOlympic.png"));
            table.Add("LogoHerald Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoHerald.png"));
            table.Add("CCardHSN Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.CCardHSN.png"));
            table.Add("CCardOlympic Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.CCardOlympic.png"));
            table.Add("CCardHerald Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.CCardHerald.png"));
            table.Add("DCardHSN Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.DCardHSN.png"));
            table.Add("DCardOlympic Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.DCardOlympic.png"));
            table.Add("DCardHerald Bank", base.CBmp(typeFromAssembly, "Images.LogosAndCards.DCardHerald.png"));
            table.Add("LogoTaranti Auto Loan", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoAuto.png"));
            table.Add("LogoTaranti Auto Lease", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoAuto.png"));
            table.Add("LogoNRG Electric", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoElectric.png"));
            table.Add("LogoS&W Insurance", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoInsurance.png"));
            table.Add("LogoInternet Connect", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoInternet.png"));
            table.Add("LogoVincent Medical", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoMedical.png"));
            table.Add("LogoCity Property Mgt", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoProperty.png"));
            table.Add("LogoQuest Student Loans", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoStudentLoan.png"));
            table.Add("LogoFiduciary Investments", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoFiduciary.png"));
            table.Add("LogoCentury Mortgage", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoCentury.png"));
            table.Add("LogoIRS", base.CBmp(typeFromAssembly, "Images.LogosAndCards.LogoIRS.jpg"));
            return table;
        }

        public override Resource CreateResource()
        {
            ResourceManager manager = new ResourceManager("KMI.Sim.Sim", Assembly.GetAssembly(typeof(SimFactory)));
            ResourceManager manager2 = new ResourceManager("KMI.VBPF1Lib.App", Assembly.GetAssembly(typeof(AppFactory)));
            return new Resource(new ResourceManager[] { manager, manager2 });
        }

        public override SimSettings CreateSimSettings()
        {
            return new AppSimSettings();
        }

        public override SimState CreateSimState(SimSettings simSettings, bool multiplayer)
        {
            return new AppSimState(simSettings, multiplayer);
        }

        public override SimStateAdapter CreateSimStateAdapter()
        {
            return new AppStateAdapter();
        }

        public override View[] CreateViews()
        {
            View[] viewArray = new View[2];
            viewArray[0] = new CityView("Outside", S.R.GetImage("City"));
            viewArray[0].ClearDrawSelectedOnMouseMove = true;
            viewArray[1] = new InsideView("Home", S.R.GetImage("HomeBack"));
            return viewArray;
        }

        public void PalettizeAndInsert(SortedList table, Bitmap b, string gender, string pose, string orient, string frame)
        {
            ColorPalette palette = b.Palette;
            for (int i = 240; i < 0x100; i++)
            {
                palette.Entries[i] = Color.FromArgb(0, palette.Entries[i]);
            }
            b.Palette = palette;
            table.Add(gender + pose + orient + frame, b);
        }

        public void PalettizeFlipAndInsert(SortedList table, Bitmap b, string gender, string pose, string orient, string frame)
        {
            Bitmap bitmap = (Bitmap) b.Clone();
            ColorPalette palette = b.Palette;
            for (int i = 240; i < 0x100; i++)
            {
                palette.Entries[i] = Color.FromArgb(0, palette.Entries[i]);
            }
            b.Palette = palette;
            table.Add(gender + pose + orient + frame, b);
            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
            table.Add(gender + pose + orient.Substring(0, 1) + "E" + frame, bitmap);
        }
    }
}

