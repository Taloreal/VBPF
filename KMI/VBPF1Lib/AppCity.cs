﻿namespace KMI.VBPF1Lib
{
    using KMI.Biz.City;
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Drawing;

    [Serializable]
    public class AppCity : KMI.Biz.City.City
    {
        protected Hashtable buildingByIDcache;
        public static int[] BuildingHeights;
        public ArrayList Buses;
        public ArrayList Cars;
        public int downtownAve;
        public int downtownStreet;
        public ArrayList Pedestrians;

        public AppCity()
        {
            CityBlock block;
            int BuildingInd;
            AppBuilding randomBuilding;
            int lowerBound0;
            int lowerBound1;
            int[] ShiftLengths = { 6, 16, 20, 24 }; //0 = 3 hours, 1 = 8 hours, 2 = 10 hours, 3 = 12 hours
            this.buildingByIDcache = new Hashtable();
            this.Cars = new ArrayList();
            this.Buses = new ArrayList();
            this.Pedestrians = new ArrayList();
            this.downtownAve = A.ST.Random.Next(0, 3);
            this.downtownStreet = A.ST.Random.Next(0, 6);
            int downtownAve = this.downtownAve;
            while (downtownAve == this.downtownAve) 
                downtownAve = A.ST.Random.Next(0, 3); 
            int SimSeed = A.ST.Random.Next(0, 5); 
            float numPlayers = 1f;
            if (A.ST.Multiplayer) 
                numPlayers = 1f + (((float) A.SS.ExpectedMultiplayerPlayers) / 15f); 
            double BuildingDist = 0.0;
            //Pending deletion 5/17/2016 06:05
            //if (A.ST.Multiplayer) 
            //    num9 /= (double) A.SS.ExpectedMultiplayerPlayers; 
            CityBlock[,] blocks = base.blocks;
            int UpperBound0 = blocks.GetUpperBound(0);
            int UpperBound1 = blocks.GetUpperBound(1);
            for (lowerBound0 = blocks.GetLowerBound(0); lowerBound0 <= UpperBound0; lowerBound0++) {
                lowerBound1 = blocks.GetLowerBound(1);
                while (lowerBound1 <= UpperBound1) {
                    block = blocks[lowerBound0, lowerBound1];
                    BuildingInd = 0;
                    while (BuildingInd < block.NumLots) {
                        if (!A.ST.Multiplayer)  //"If Multiplayer use every building, otherwise use BuildingDist to generate grass"
                            BuildingDist = (Math.Pow((double) ((block.Avenue - this.downtownAve) * 2), 2.0) + Math.Pow((double) (block.Street - this.downtownStreet), 2.0)) / 200.0;
                        if ((block[BuildingInd] == null) && (A.ST.Random.NextDouble() > BuildingDist)) //NULL = grass.
                            block[BuildingInd] = new AppBuilding(block, BuildingInd, KMI.Biz.City.City.BuildingTypes[0]); 
                        BuildingInd++;
                    }
                    lowerBound1++;
                }
            }
            for (BuildingInd = 0; BuildingInd < KMI.Biz.City.City.NUM_STREETS; BuildingInd++) 
                base[this.downtownAve, BuildingInd, 2] = new AppBuilding(base[this.downtownAve, BuildingInd], 2, KMI.Biz.City.City.BuildingTypes[6]); 
            for (int i = SimSeed; i < (SimSeed + 3); i++) 
                for (int j = 0; j < KMI.Biz.City.City.LOTS_PER_BLOCK[0]; j++) 
                    base[downtownAve, i, j] = new Classroom(base[downtownAve, i], j, KMI.Biz.City.City.BuildingTypes[5]); 
            this.CreateCourses(16, 32, false);
            this.CreateCourses(36, 42, false);
            this.CreateCourses(16, 32, true);
            this.AddCourse(A.R.GetString("Medical Degree"), A.R.GetString("Four-year medical school program."), 119975f, 0x2080, 16, 32, false, A.R.GetString("Bachelors Degree"));
            for (BuildingInd = 0; BuildingInd < (7 * numPlayers); BuildingInd++) {
                randomBuilding = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, true, 2.5f, 30);
                base[randomBuilding.Avenue, randomBuilding.Street, randomBuilding.Lot] = new Office(randomBuilding.Block, randomBuilding.Lot, KMI.Biz.City.City.BuildingTypes[3]);
            }
            GenJobs(numPlayers);
            GenDwellings();
            CompileShops();
            AddSpecialBuildings();
            this.CreateBanks();
            blocks = base.blocks;
            UpperBound0 = blocks.GetUpperBound(0);
            UpperBound1 = blocks.GetUpperBound(1);
            for (lowerBound0 = blocks.GetLowerBound(0); lowerBound0 <= UpperBound0; lowerBound0++) {
                for (lowerBound1 = blocks.GetLowerBound(1); lowerBound1 <= UpperBound1; lowerBound1++) {
                    block = blocks[lowerBound0, lowerBound1];
                    for (BuildingInd = 0; BuildingInd < block.NumLots; BuildingInd++)
                        if (block[BuildingInd] != null)
                            this.buildingByIDcache.Add(((AppBuilding) block[BuildingInd]).ID, block[BuildingInd]);
                }
            }
            AddBuses();
        }

        public void AddSpecialBuildings()
        {
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[9]; //Car dealer
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[11]; //Insurance
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[13]; //Supermarket
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[14]; //Internet
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[15]; //Auto Garage
            base.GetRandomBuilding(0).BuildingType = KMI.Biz.City.City.BuildingTypes[16]; //Investment
        }

        public void GenJobs(float numPlayers)
        {
            int BuildingInd = 0;
            for (BuildingInd = 0; BuildingInd < (2f * numPlayers); BuildingInd++) {
                this.AddOffice(16, 32, true, false, false);
                this.AddOffice(16, 32, false, true, false);
                this.AddOffice(16, 32, false, false, true);
            }
            this.AddOffice(36, 42, true, false, false);
            this.AddOffice(36, 42, true, false, false);
            this.AddOffice(36, 42, false, true, false);
            this.AddOffice(16, 42, false, false, true);
            
            for (BuildingInd = 0; BuildingInd < (4 * numPlayers); BuildingInd++) {
                AppBuilding randomBuilding = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, true, 2.5f, 30);
                base[randomBuilding.Avenue, randomBuilding.Street, randomBuilding.Lot] = new AppBuilding(randomBuilding.Block, randomBuilding.Lot, KMI.Biz.City.City.BuildingTypes[0x12]);
            }
            for (BuildingInd = 0; BuildingInd < (2f * numPlayers); BuildingInd++) {
                this.AddJobInvisible(16, 32, 18, new WorkInternet1(), false);
                this.AddJobInvisible(36, 42, 18, new WorkInternet1(), false);
                this.AddJobInvisible(16, 32, 18, new WorkInternet2(), false);
                this.AddJobInvisible(16, 32, 18, new WorkInternet3(), false);
            }
            for (BuildingInd = 0; BuildingInd < (3 * numPlayers); BuildingInd++) {
                AppBuilding randomBuilding = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, true, 2.5f, 30);
                base[randomBuilding.Avenue, randomBuilding.Street, randomBuilding.Lot] = new AppBuilding(randomBuilding.Block, randomBuilding.Lot, KMI.Biz.City.City.BuildingTypes[19]);
            }
            for (BuildingInd = 0; BuildingInd < (1f * numPlayers); BuildingInd++) {
                this.AddJobInvisible(16, 32, 19, new WorkHospital1(), false);
                this.AddJobInvisible(30, 46, 19, new WorkHospital1(), false);
                this.AddJobInvisible(16, 32, 19, new WorkHospital1(), true);
                this.AddJobInvisible(30, 46, 19, new WorkHospital1(), true);
            }
            for (BuildingInd = 0; BuildingInd < (2f * numPlayers); BuildingInd++) {
                this.AddJobInvisible(16, 44, 19, new WorkHospital2(), false);
                this.AddJobInvisible(16, 32, 19, new WorkHospital3(), false);
            }
            for (BuildingInd = 0; BuildingInd < (4 * numPlayers); BuildingInd++) {
                AppBuilding randomBuilding = (AppBuilding)base.GetRandomBuilding(0);
                randomBuilding.BuildingType = KMI.Biz.City.City.BuildingTypes[4];
                Job job = new Job {
                    Building = randomBuilding,
                    PrototypeTask = new WorkPizzaGuy()
                };
                job.PrototypeTask.StartPeriod = 16;
                job.PrototypeTask.EndPeriod = 32;
                job.PrototypeTask.Weekend = (BuildingInd % 2) == 0;
                randomBuilding.Offerings.Add(job);
                job = new Job
                {
                    Building = randomBuilding,
                    PrototypeTask = new WorkPizzaGuy()
                };
                job.PrototypeTask.StartPeriod = 38;
                job.PrototypeTask.EndPeriod = 44;
                job.PrototypeTask.Weekend = (BuildingInd % 2) == 0;
                randomBuilding.Offerings.Add(job);
            }
            for (BuildingInd = 0; BuildingInd < (2 * numPlayers); BuildingInd++) {
                AppBuilding randomBuilding = (AppBuilding)base.GetRandomBuilding(0);
                randomBuilding.BuildingType = KMI.Biz.City.City.BuildingTypes[17];
                Job job = new Job
                {
                    Building = randomBuilding,
                    PrototypeTask = new WorkDrugRep()
                };
                job.PrototypeTask.StartPeriod = 16;
                job.PrototypeTask.EndPeriod = 32;
                randomBuilding.Offerings.Add(job);
            }
            for (BuildingInd = 0; BuildingInd < 2; BuildingInd++) {
                this.AddFastFood(16, 32, true, false, false);
                this.AddFastFood(36, 42, true, false, false);
                this.AddFastFood(16, 32, false, true, false);
                this.AddFastFood(16, 32, false, false, true);
            }
            this.AddFastFood(36, 42, false, true, false);
        }

        public void GenDwellings()
        {
            int PlayerOffset = 0;
            if (A.ST.Multiplayer)
                PlayerOffset = Math.Max(0, A.SS.ExpectedMultiplayerPlayers - 2);
            for (int BuildingInd = 0; BuildingInd < (12 + PlayerOffset); BuildingInd++) {
                AppBuilding randomBuilding = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, false, 4f, 30);
                if (BuildingInd < 1)
                    randomBuilding = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, true, 2f, 30);
                randomBuilding = new Dwelling(randomBuilding.Block, randomBuilding.Lot, KMI.Biz.City.City.BuildingTypes[1]);
                base[randomBuilding.Avenue, randomBuilding.Street, randomBuilding.Lot] = randomBuilding;
                Offering offering = new DwellingOffer();
                float RentDistOffset = this.Distance(this.downtownAve, this.downtownStreet, randomBuilding.Avenue, randomBuilding.Street);
                randomBuilding.Rent = (int)Math.Max((float)200f, (float)(1200f * (1f - (RentDistOffset / 7f))));
                offering.Building = randomBuilding;
                randomBuilding.Offerings.Add(offering);
            }
        }

        public void CompileShops()
        {
            for (int i = 0; i != 3; i++) {
                AppBuilding building2 = (AppBuilding)base.GetRandomBuilding(0);
                building2.BuildingType = KMI.Biz.City.City.BuildingTypes[12];
                building2.Prices = new float[A.ST.PurchasableItems.Count];
                for (int k = 0; k < building2.Prices.Length; k++)
                    building2.Prices[k] = (float)((A.ST.Random.NextDouble() * 0.2) + 0.9);
                building2.SaleDiscounts = new float[A.ST.PurchasableItems.Count];
            }
        }

        public void AddBuses()
        {
            Bus bus = new Bus(this.downtownAve, 0, true, 1);
            this.Buses.Add(bus);
            bus = new Bus(this.downtownAve, 2, true, -1);
            this.Buses.Add(bus);
            bus = new Bus(this.downtownAve, 3, true, 1);
            this.Buses.Add(bus);
            bus = new Bus(this.downtownAve, 4, true, -1);
            this.Buses.Add(bus);
            bus = new Bus(this.downtownAve, 5, true, 1);
            this.Buses.Add(bus);
            bus = new Bus(this.downtownAve, 7, true, -1);
            this.Buses.Add(bus);
        }

        public void Add401Ks(float likelihoodPerBuilding)
        {
            ArrayList buildings = this.GetBuildings();
            foreach (AppBuilding building in buildings)
            {
                if (((building.Offerings.Count > 0) && (building.Offerings[0] is Job)) && (A.ST.Random.NextDouble() < likelihoodPerBuilding))
                {
                    bool flag = true;
                    foreach (Offering offering in building.Offerings)
                    {
                        if (offering.Taken)
                        {
                            flag = false;
                        }
                    }
                    float num = (1 + A.ST.Random.Next(3)) * 0.01f;
                    if (flag)
                    {
                        foreach (Offering offering in building.Offerings)
                        {
                            ((WorkTask) offering.PrototypeTask).R401KMatch = num;
                        }
                        foreach (Player player in A.ST.Player.Values)
                        {
                            player.SendPeriodicMessage(A.R.GetString("Some jobs in the city have added 401K retirement plans."), "", NotificationColor.Green, 5f);
                        }
                    }
                }
            }
        }

        public void AddCourse(string name, string resumeDescription, float cost, int hours, int startPeriod, int endPeriod, bool weekend, string prerequisite)
        {
            int num = 150;
            int SimSeed = 0;
            AppBuilding b = null;
        Label_001A:;
            if (((b == null) || (b.Offerings.Count > 1)) && (SimSeed++ < num))
            {
                b = (AppBuilding) base.GetRandomBuilding(5);
                goto Label_001A;
            }
            int num3 = (endPeriod - startPeriod) / 2;
            if (b != null)
            {
                Course course = new Course {
                    Building = b,
                    PrototypeTask = new AttendClass()
                };
                course.PrototypeTask.StartPeriod = startPeriod;
                course.PrototypeTask.EndPeriod = endPeriod;
                course.Name = name;
                course.ResumeDescription = resumeDescription;
                course.Cost = cost;
                course.Days = Math.Min(hours / num3, 0x618);
                if (weekend)
                {
                    course.Days = Math.Min(course.Days, 0x270);
                }
                course.PrototypeTask.Weekend = weekend;
                course.Prerequisite = prerequisite;
                b.Offerings.Add(course);
                int num4 = 6;
                if (A.ST.Multiplayer)
                {
                    num4 -= 2;
                }
                for (int i = 0; i < num4; i++)
                {
                    VBPFPerson person = this.AddGenericPerson(course, b);
                    ((AttendClass) person.Task).Course = course;
                    course.Students.Add(person);
                }
            }
        }

        public void AddFastFood(int startPeriod, int endPeriod, bool cashierOpening, bool shiftOpening, bool mgrOpening)
        {
            Building building = this.GetRandomBuilding(this.downtownAve, this.downtownStreet, true, 3.1f, 30);
            int busyness = 2;
            FastFoodStore store = new FastFoodStore(building.Block, building.Lot, KMI.Biz.City.City.BuildingTypes[2], busyness);
            building = store;
            base[building.Avenue, building.Street, building.Lot] = building;
            store.AddGenericWorker(new WorkCounterFastFood(0));
            store.AddGenericWorker(new WorkMgrFastFood(0));
            for (int i = 0; i < 2; i++)
            {
                WorkTask task = null;
                if (shiftOpening)
                {
                    store.AddGenericWorker(new WorkCounterFastFood(1));
                    task = new WorkMgrFastFood(1);
                }
                else if (cashierOpening)
                {
                    store.AddGenericWorker(new WorkMgrFastFood(1));
                    task = new WorkCounterFastFood(1);
                }
                else if (mgrOpening)
                {
                    store.AddGenericWorker(new WorkCounterFastFood(1));
                    task = new WorkStoreMgrFastFood(1);
                }
                task.Weekend = i == 1;
                if (A.SS.HealthInsuranceForFastFoodJobs)
                {
                    task.HealthInsurance = new InsurancePolicy(25f);
                }
                store.Offerings.Add(new Job(store, task, startPeriod, endPeriod));
            }
        }

        protected VBPFPerson AddGenericPerson(Offering o, AppBuilding b)
        {
            VBPFPerson activeObject = new VBPFPerson();
            Task task = o.CreateTask();
            task.Building = b;
            task.Owner = activeObject;
            activeObject.Task = task;
            A.I.Subscribe(activeObject, A.ST.Now.AddHours((((float) task.StartPeriod) / 2f) - (0.20000000298023224 + (0.10000000149011612 * A.ST.Random.NextDouble()))));
            return activeObject;
        }

        public void AddHealthInsurance(float likelihoodPerBuilding)
        {
            ArrayList buildings = this.GetBuildings();
            foreach (AppBuilding building in buildings)
            {
                if (((building.Offerings.Count > 0) && (building.Offerings[0] is Job)) && (A.ST.Random.NextDouble() < likelihoodPerBuilding))
                {
                    bool flag = true;
                    foreach (Offering offering in building.Offerings)
                    {
                        if (offering.Taken)
                        {
                            flag = false;
                        }
                    }
                    float copay = new float[] { 10f, 25f, 50f }[A.ST.Random.Next(3)];
                    if (flag)
                    {
                        foreach (Offering offering in building.Offerings)
                        {
                            ((WorkTask) offering.PrototypeTask).HealthInsurance = new InsurancePolicy(copay);
                        }
                        foreach (Player player in A.ST.Player.Values)
                        {
                            player.SendPeriodicMessage(A.R.GetString("Some jobs in the city have added health insurance coverage."), "", NotificationColor.Green, 5f);
                        }
                    }
                }
            }
        }

        public void AddJobInvisible(int startPeriod, int endPeriod, int buildingTypeIndex, WorkTask prototypeTask, bool weekend)
        {
            int num = 0;
            int SimSeed = 50;
            while (num++ < SimSeed)
            {
                AppBuilding randomBuilding = (AppBuilding) base.GetRandomBuilding(buildingTypeIndex);
                if ((randomBuilding != null) && (randomBuilding.Offerings.Count < ((num + 10) / 10)))
                {
                    Job job = new Job {
                        Building = randomBuilding,
                        PrototypeTask = prototypeTask
                    };
                    job.PrototypeTask.StartPeriod = startPeriod;
                    job.PrototypeTask.EndPeriod = endPeriod;
                    job.PrototypeTask.Weekend = weekend;
                    randomBuilding.Offerings.Add(job);
                    break;
                }
            }
        }

        public void AddOffice(int startPeriod, int endPeriod, bool deskOpening, bool supervisorOpening, bool mgrOpening)
        {
            int num = 0;
            int SimSeed = 50;
            while (num++ < SimSeed)
            {
                AppBuilding randomBuilding = (AppBuilding) base.GetRandomBuilding(3);
                if ((randomBuilding != null) && (randomBuilding.Offerings.Count < ((num + 10) / 10)))
                {
                    VBPFPerson person;
                    Job job = new Job {
                        Building = randomBuilding,
                        PrototypeTask = new WorkOfficeDesk()
                    };
                    job.PrototypeTask.StartPeriod = startPeriod;
                    job.PrototypeTask.EndPeriod = endPeriod;
                    ((WorkOfficeDesk) job.PrototypeTask).chair = 3;
                    int num3 = 3;
                    if (deskOpening)
                    {
                        randomBuilding.Offerings.Add(job);
                    }
                    else
                    {
                        num3 = 4;
                    }
                    for (int i = 0; i < num3; i++)
                    {
                        ((WorkOfficeDesk) this.AddGenericPerson(job, randomBuilding).Task).chair = i;
                    }
                    job = new Job {
                        Building = randomBuilding,
                        PrototypeTask = new WorkOfficeSup()
                    };
                    job.PrototypeTask.StartPeriod = startPeriod;
                    job.PrototypeTask.EndPeriod = endPeriod;
                    ((WorkOfficeSup) job.PrototypeTask).chair = 4;
                    if (supervisorOpening)
                    {
                        randomBuilding.Offerings.Add(job);
                    }
                    else
                    {
                        person = this.AddGenericPerson(job, randomBuilding);
                    }
                    job = new Job {
                        Building = randomBuilding,
                        PrototypeTask = new WorkOfficeMgr()
                    };
                    job.PrototypeTask.StartPeriod = startPeriod;
                    job.PrototypeTask.EndPeriod = endPeriod;
                    ((WorkOfficeMgr) job.PrototypeTask).chair = 5;
                    if (mgrOpening)
                    {
                        randomBuilding.Offerings.Add(job);
                    }
                    else
                    {
                        person = this.AddGenericPerson(job, randomBuilding);
                    }
                    break;
                }
            }
        }

        public AppBuilding BuildingByID(long ID)
        {
            return (AppBuilding) this.buildingByIDcache[ID];
        }

        public Bus BusAt(int street, int direction)
        {
            foreach (Bus bus in this.Buses)
            {
                if ((bus.Location.Y == street) && (Math.Sign(bus.DY) == direction))
                {
                    return bus;
                }
            }
            return null;
        }

        protected void CreateBanks()
        {
            int index = 0;
            float[] numArray = this.GenerateSpreadRandoms(3, 0.25f, 50, A.ST.Random);
            float[] numArray2 = this.GenerateSpreadRandoms(3, 0.25f, 50, A.ST.Random);
            float[] numArray3 = this.GenerateSpreadRandoms(3, 0.25f, 50, A.ST.Random);
            foreach (string str in new string[] { "HSN Bank", "Herald Bank", "Olympic Bank" })
            {
                AppBuilding randomBuilding = (AppBuilding) base.GetRandomBuilding(0);
                randomBuilding.BuildingType = KMI.Biz.City.City.BuildingTypes[8];
                BankAccount account = new CheckingAccount(Utilities.RoundUpToPowerOfTen(50f * numArray[index], 1), Utilities.RoundUpToPowerOfTen(1000f * numArray[index], 2)) {
                    Building = randomBuilding,
                    BankName = A.R.GetString(str)
                };
                randomBuilding.Offerings.Add(account);
                account = new SavingsAccount(Utilities.RoundUpToPowerOfTen(50f * numArray2[index], 1), (float) Math.Round((double) ((0.025 * (1f - numArray2[index])) + 0.005), 3), Utilities.RoundUpToPowerOfTen(750f * numArray2[index], 2)) {
                    Building = randomBuilding,
                    BankName = A.R.GetString(str)
                };
                randomBuilding.Offerings.Add(account);
                account = new CreditCardAccount(Utilities.RoundUpToPowerOfTen(4000f * numArray3[index], 1), (float) Math.Round((double) (0.02 + (0.12 * (1f - numArray3[index]))), 3), Utilities.RoundUpToPowerOfTen(20f + (80f * numArray3[index]), 1)) {
                    Building = randomBuilding,
                    BankName = A.R.GetString(str)
                };
                randomBuilding.Offerings.Add(account);
                index++;
            }
        }

        public void CreateCourses(int startPeriod, int endPeriod, bool weekend)
        {
            this.AddCourse(A.R.GetString("Food Service Mgt I"), A.R.GetString("Topics included food preparation techniques, food safety procedures, and supervision techniques."), 975f, 520, startPeriod, endPeriod, weekend, null);
            this.AddCourse(A.R.GetString("Intro to Data Entry"), A.R.GetString("Course covering data entry formats and tools as well as data security procedures."), 2975f, 1040, startPeriod, endPeriod, weekend, null);
            this.AddCourse(A.R.GetString("IT Management"), A.R.GetString("Studied personnel supervision, planning and control, and It budgeting."), 4975f, 2080, startPeriod, endPeriod, weekend, null);
            this.AddCourse(A.R.GetString("Associates Degree"), A.R.GetString("Full associates degree program covering multiple disciplines."), 9975f, 4160, startPeriod, endPeriod, weekend, null);
            this.AddCourse(A.R.GetString("Bachelors Degree"), A.R.GetString("Four-year multidisciplinary educational program."), 20000f, 4160, startPeriod, endPeriod, weekend, "Associates Degree");
            this.AddCourse(A.R.GetString("Web Design"), A.R.GetString("Course providing general instruction in website design."), 2975f, 1040, startPeriod, endPeriod, weekend, null);
            this.AddCourse(A.R.GetString("Nursing Degree"), A.R.GetString("Two-year nursing program."), 13475f, 4160, startPeriod, endPeriod, weekend, null);
        }

        public void DeleteOffering(long ID)
        {
            CityBlock[,] blocks = base.blocks;
            int UpperBound0 = blocks.GetUpperBound(0);
            int num3 = blocks.GetUpperBound(1);
            for (int i = blocks.GetLowerBound(0); i <= UpperBound0; i++)
            {
                for (int j = blocks.GetLowerBound(1); j <= num3; j++)
                {
                    CityBlock block = blocks[i, j];
                    for (int k = 0; k < block.NumLots; k++)
                    {
                        if (block[k] != null)
                        {
                            AppBuilding building = (AppBuilding) block[k];
                            foreach (Offering offering in building.Offerings)
                            {
                                if (offering.ID == ID)
                                {
                                    building.Offerings.Remove(offering);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public float Distance(int ave1, int street1, int ave2, int street2)
        {
            return (float) Math.Pow(Math.Pow((double) (2 * (ave1 - ave2)), 2.0) + Math.Pow((double) (street1 - street2), 2.0), 0.5);
        }

        public AppBuilding FindInsideBuilding(AppEntity e)
        {
            AppBuilding building = null;
            if ((e.Dwelling != null) && e.Dwelling.Persons.Contains(e.Person))
            {
                return e.Dwelling;
            }
            foreach (Task task in e.GetAllTasks())
            {
                if ((task.Building != null) && task.Building.Persons.Contains(e.Person))
                {
                    building = task.Building;
                    break;
                }
            }
            if (building == null)
            {
                foreach (AppBuilding building2 in this.GetBuildings())
                {
                    if ((building2.Persons.Count > 0) && building2.Persons.Contains(e.Person))
                    {
                        return building2;
                    }
                }
            }
            return building;
        }

        public Offering FindOfferingForTask(Task t)
        {
            ArrayList offerings = this.GetOfferings();
            foreach (Offering offering in offerings)
            {
                Task prototypeTask = offering.PrototypeTask;
                if ((((prototypeTask != null) && (t.Building == offering.Building)) && ((t.GetType() == prototypeTask.GetType()) && (t.StartPeriod == prototypeTask.StartPeriod))) && (t.EndPeriod == prototypeTask.EndPeriod))
                {
                    return offering;
                }
            }
            return null;
        }

        protected float[] GenerateSpreadRandoms(int numValues, float targetSeparation, int maxTries, Random random)
        {
            float[] numArray = new float[numValues];
            float num = 0f;
            for (int i = 0; i < numValues; i++)
            {
                for (int j = 0; j < maxTries; j++)
                {
                    bool flag = false;
                    num = (float) random.NextDouble();
                    for (int k = 0; k < i; k++)
                    {
                        if (Math.Abs((float) (num - numArray[k])) < targetSeparation)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        break;
                    }
                }
                numArray[i] = num;
            }
            return numArray;
        }

        public ArrayList GetBuildings()
        {
            ArrayList list = new ArrayList();
            CityBlock[,] blocks = base.blocks;
            int UpperBound0 = blocks.GetUpperBound(0);
            int num3 = blocks.GetUpperBound(1);
            for (int i = blocks.GetLowerBound(0); i <= UpperBound0; i++)
            {
                for (int j = blocks.GetLowerBound(1); j <= num3; j++)
                {
                    CityBlock block = blocks[i, j];
                    for (int k = 0; k < block.NumLots; k++)
                    {
                        if (block[k] != null)
                        {
                            list.Add(block[k]);
                        }
                    }
                }
            }
            return list;
        }

        public Offering GetOffering(long ID)
        {
            ArrayList offerings = this.GetOfferings();
            foreach (Offering offering in offerings)
            {
                if (offering.ID == ID)
                {
                    return offering;
                }
            }
            return null;
        }

        public ArrayList GetOfferings()
        {
            ArrayList list = new ArrayList();
            CityBlock[,] blocks = base.blocks;
            int UpperBound0 = blocks.GetUpperBound(0);
            int num3 = blocks.GetUpperBound(1);
            for (int i = blocks.GetLowerBound(0); i <= UpperBound0; i++)
            {
                for (int j = blocks.GetLowerBound(1); j <= num3; j++)
                {
                    CityBlock block = blocks[i, j];
                    for (int k = 0; k < block.NumLots; k++)
                    {
                        if (block[k] != null)
                        {
                            AppBuilding building = (AppBuilding) block[k];
                            foreach (Offering offering in building.Offerings)
                            {
                                list.Add(offering);
                            }
                        }
                    }
                }
            }
            return list;
        }

        public ArrayList GetOfferings(Type type)
        {
            ArrayList list = new ArrayList();
            CityBlock[,] blocks = base.blocks;
            int UpperBound0 = blocks.GetUpperBound(0);
            int num3 = blocks.GetUpperBound(1);
            for (int i = blocks.GetLowerBound(0); i <= UpperBound0; i++)
            {
                for (int j = blocks.GetLowerBound(1); j <= num3; j++)
                {
                    CityBlock block = blocks[i, j];
                    for (int k = 0; k < block.NumLots; k++)
                    {
                        if (block[k] != null)
                        {
                            AppBuilding building = (AppBuilding) block[k];
                            foreach (Offering offering in building.Offerings)
                            {
                                if (offering.GetType() == type)
                                {
                                    list.Add(offering);
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        public AppBuilding GetRandomBuilding(int avenue, int street, bool closeTo, float distance, int tries)
        {
            AppBuilding randomBuilding = null;
            while (tries-- > 0)
            {
                randomBuilding = (AppBuilding) base.GetRandomBuilding(0);
                float num = this.Distance(randomBuilding.Avenue, randomBuilding.Street, avenue, street);
                if ((closeTo && (num < distance)) || (!closeTo && (num > distance)))
                {
                    return randomBuilding;
                }
            }
            return (AppBuilding) base.GetRandomBuilding(0);
        }

        public float LikelihoodOfCrime(Building bldg)
        {
            float maxValue = float.MaxValue;
            ArrayList buildings = this.GetBuildings();
            foreach (Building building in buildings)
            {
                if (building.BuildingType.Index == 10)
                {
                    PointF tf = KMI.Biz.City.City.Transform((float) building.Avenue, (float) building.Street, (float) building.Lot);
                    PointF tf2 = KMI.Biz.City.City.Transform((float) bldg.Avenue, (float) bldg.Street, (float) bldg.Lot);
                    maxValue = Math.Min(maxValue, Utilities.DistanceBetweenIsometric(tf, tf2));
                }
            }
            if (maxValue > 500f)
            {
                return 0f;
            }
            return ((1f - (maxValue / 500f)) * 0.01f);
        }

        public void RaiseSomeWages(float likelihoodPerJob)
        {
            ArrayList offerings = this.GetOfferings(typeof(Job));
            foreach (Offering offering in offerings)
            {
                if (!offering.Taken && (A.ST.Random.NextDouble() < likelihoodPerJob))
                {
                    WorkTask prototypeTask = (WorkTask) offering.PrototypeTask;
                    prototypeTask.HourlyWage *= 1.15f;
                    foreach (Player player in A.ST.Player.Values)
                    {
                        player.SendPeriodicMessage(A.R.GetString("The rate of pay has increased for some jobs in the city."), "", NotificationColor.Green, 5f);
                    }
                }
            }
        }

        public void SetupCondoOfferings()
        {
            AppBuilding current;
            ArrayList list = new ArrayList();
            IEnumerator enumerator = this.GetBuildings().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (AppBuilding) enumerator.Current;
                    if (!(!(current is Dwelling) || ((Offering) current.Offerings[0]).Taken))
                    {
                        list.Add(current);
                    }
                }
            Utilities.Shuffle(list, A.ST.Random);
            for (int i = 0; i < Math.Max(6, list.Count / 3); i++)
            {
                current = (AppBuilding) list[i];
                current.Offerings.Clear();
                DwellingOffer offer = new DwellingOffer {
                    Condo = true,
                    Building = current
                };
                current.Offerings.Add(offer);
            }
        }
    }
}

