namespace KMI.VBPF1Lib
{
    using KMI.Biz;
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public class AppStateAdapter : BizStateAdapter
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        public long AddEntity(string playerName, string entityName, Person.GenderType gender, int paletteIndex)
        {
            AppEntity activeObject = (AppEntity) base.TryAddEntity(playerName, entityName);
            activeObject.Person = new VBPFPerson(gender, Person.RaceType.Hispanic, "", entityName);
            activeObject.Person.PaletteIndex = paletteIndex;
            A.I.Subscribe(activeObject);
            return activeObject.ID;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public AppBuildingDrawable.AddOfferingInfo AddOffering(long entityID, long offeringID, JobApplication jobApp)
        {
            Task task;
            AppEntity e = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            AppBuildingDrawable.AddOfferingInfo info = new AppBuildingDrawable.AddOfferingInfo();
            Offering o = A.ST.City.GetOffering(offeringID);
            if (o.Taken)
            {
                throw new SimApplicationException(A.R.GetString("I'm sorry. That offering has been taken."));
            }
            if (o.GetType() == typeof(DwellingOffer))
            {
                bool flag = e.Dwelling == null;
                this.MoveTo(entityID, o.ID);
                if (flag)
                {
                    Dwelling building = (Dwelling) o.Building;
                    e.Dwelling.Persons.Add(e.Person);
                    e.Person.Location = e.Dwelling.EntryPoint;
                    task = new Sleep {
                        StartPeriod = 0,
                        EndPeriod = 12
                    };
                    this.AddTask(e.ID, task);
                    Task task2 = new Sleep {
                        StartPeriod = 0,
                        EndPeriod = 12,
                        Weekend = true
                    };
                    this.AddTask(e.ID, task2);
                    e.Journal.Entries.Clear();
                    if (A.SS.AutoPayRentElectric)
                    {
                        this.SetBankAccount(e.ID, (BankAccount) A.ST.City.GetOfferings(typeof(CheckingAccount))[0], A.SS.InitialCash / 2f, -1L);
                        RecurringPayment payment = new RecurringPayment {
                            Amount = building.Rent,
                            Day = 2,
                            PayeeAccountNumber = ((BankAccount) e.MerchantAccounts["City Property Mgt"]).AccountNumber,
                            PayeeName = ((BankAccount) e.MerchantAccounts["City Property Mgt"]).BankName
                        };
                        ((CheckingAccount) e.BankAccounts.GetByIndex(0)).RecurringPayments.Add(payment);
                        payment = new RecurringPayment {
                            Amount = A.ST.ElectricityCost,
                            Day = 2,
                            PayeeAccountNumber = ((BankAccount) e.MerchantAccounts["NRG Electric"]).AccountNumber,
                            PayeeName = ((BankAccount) e.MerchantAccounts["NRG Electric"]).BankName
                        };
                        ((CheckingAccount) e.BankAccounts.GetByIndex(0)).RecurringPayments.Add(payment);
                    }
                }
            }
            else
            {
                task = o.CreateTask();
                if (o is Job)
                {
                    ((WorkTask) task).EvaluateApplicant(e, o, jobApp);
                }
                task.Building = o.Building;
                task.Owner = e.Person;
                DailyRoutine routine = e.GetDailyRoutine(task.Weekend).MakeCopy();
                e.CheckValiditySynchTravel(e.ModeOfTransportation, routine, task, true);
                e.SetDailyRoutine(task.Weekend, routine);
                if (task is WorkTask)
                {
                    e.WorkTaskHistory.Add(task.ID, task);
                    info.TaskID = task.ID;
                    o.Taken = true;
                }
                if (task is AttendClass)
                {
                    ((AttendClass) task).Course = (Course) o;
                    ((Course) o).Students.Add(task.Owner);
                }
                info.IsFirstTravel = e.ModeOfTransportation < 0;
            }
            e.Journal.AddEntry(o.JournalEntry());
            return info;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddTask(long entityID, Task task)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            DailyRoutine routine = entity.GetDailyRoutine(task.Weekend).MakeCopy();
            task.ID = A.ST.GetNextID();
            task.Owner = entity.Person;
            task.Building = entity.Dwelling;
            entity.CheckValiditySynchTravel(entity.ModeOfTransportation, routine, task, true);
            entity.SetDailyRoutine(task.Weekend, routine);
            if ((((task is Sleep) || (task is Eat)) || (task is Relax)) || (task is Exercise))
            {
                entity.Journal.AddEntry(A.R.GetString("Decided to {0} from {1} to {2} on {3}s.", new object[] { task.CategoryName().ToLower(), Task.ToTimeString(task.StartPeriod), Task.ToTimeString(task.EndPeriod), task.WeekendString().ToLower() }));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void BuyFund(long entityID, string fundName, float amount, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float num = amount;
            if (accountNumber == -1L)
            {
                if (entity.Cash < amount)
                {
                    throw new SimApplicationException(A.R.GetString("You do not have enough cash to make this purchase."));
                }
                entity.Cash -= amount;
            }
            else
            {
                BankAccount account = (BankAccount) entity.BankAccounts[accountNumber];
                if (account.EndingBalance() < amount)
                {
                    throw new SimApplicationException(A.R.GetString("You do not have enough money in the account to make this purchase."));
                }
                account.Transactions.Add(new Transaction(amount, Transaction.TranType.Debit, A.R.GetString("Share purchase")));
            }
            Fund fund = A.ST.GetFund(fundName);
            InvestmentAccount investmentAccount = entity.GetInvestmentAccount(fundName, false);
            if (investmentAccount == null)
            {
                investmentAccount = new InvestmentAccount(fund) {
                    BankName = A.R.GetString("Fiduciary Investments")
                };
                entity.InvestmentAccounts.Add(investmentAccount.AccountNumber, investmentAccount);
            }
            amount -= fund.FrontEndLoad * amount;
            investmentAccount.Transactions.Add(new Transaction(amount / fund.Price, Transaction.TranType.Credit, A.R.GetString("Share Purchase"), A.ST.Now.AddDays(-1.0)));
            investmentAccount.CostBasis += amount;
            entity.Journal.AddEntry(A.R.GetString("Invested {0} in {1}.", new object[] { Utilities.FC(num, A.I.CurrencyConversion), fund.Name }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CashCheck(long entityID, KMI.VBPF1Lib.Check check, float fee)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.Cash += check.Amount * (1f - fee);
            entity.Checks.Remove(check.ID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CloseAccount(long entityID, BankAccount account)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.CloseAccount(account);
            entity.Journal.AddEntry(A.R.GetString("Closed account: {0}.", new object[] { account.ToString() }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Bill CreateBill(string from, string description, float amount, BankAccount account)
        {
            return new Bill(from, description, amount, account);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteOffering(long ID)
        {
            A.ST.City.DeleteOffering(ID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteTask(long entityID, long taskID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            Task taskByID = entity.GetTaskByID(taskID);
            entity.RemoveTask(taskByID);
            entity.CheckValiditySynchTravel(entity.ModeOfTransportation, entity.GetDailyRoutine(taskByID.Weekend), null, false);
            string str = taskByID.CategoryName();
            if (taskByID is AttendClass)
            {
                str = "Attend Class";
            }
            entity.Journal.AddEntry(A.R.GetString("Decided not to {0} from {1} to {2} on {3}s.", new object[] { taskByID.CategoryName().ToLower(), Task.ToTimeString(taskByID.StartPeriod), Task.ToTimeString(taskByID.EndPeriod), taskByID.WeekendString().ToLower() }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteTask(long entityID, long workTaskID, bool fired, bool quit)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.GetTaskByID(workTaskID) is WorkTask)
            {
                WorkTask taskByID = (WorkTask) entity.GetTaskByID(workTaskID);
                if (fired)
                {
                    entity.FiredFrom.Remove(taskByID.Building.ID.ToString() + taskByID.Name());
                    entity.FiredFrom.Add(taskByID.Building.ID.ToString() + taskByID.Name(), A.ST.Now);
                }
                if (quit)
                {
                    entity.Quit.Remove(taskByID.Building.ID.ToString() + taskByID.Name());
                    entity.Quit.Add(taskByID.Building.ID.ToString() + taskByID.Name(), A.ST.Now);
                }
            }
            this.DeleteTask(entityID, workTaskID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DepositCheck(long entityID, KMI.VBPF1Lib.Check check, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = (BankAccount) entity.BankAccounts[accountNumber];
            account.Transactions.Add(new Transaction(check.Amount, Transaction.TranType.Credit, check.Memo));
            entity.Checks.Remove(check.ID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EditTask(long entityID, long taskID, int startPeriod, int endPeriod)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            Task taskByID = entity.GetTaskByID(taskID);
            int num = taskByID.StartPeriod;
            int num2 = taskByID.EndPeriod;
            taskByID.StartPeriod = startPeriod;
            taskByID.EndPeriod = endPeriod;
            DailyRoutine routine = entity.GetDailyRoutine(taskByID.Weekend).MakeCopy();
            try
            {
                entity.CheckValiditySynchTravel(entity.ModeOfTransportation, routine, taskByID, false);
            }
            catch (SimApplicationException exception)
            {
                taskByID.StartPeriod = num;
                taskByID.EndPeriod = num2;
                throw exception;
            }
            entity.SetDailyRoutine(taskByID.Weekend, routine);
            taskByID.DayLastStarted = -1;
            entity.Journal.AddEntry(A.R.GetString("Changed time to {0} to {1} until {2} on {3}s.", new object[] { taskByID.CategoryName().ToLower(), Task.ToTimeString(taskByID.StartPeriod), Task.ToTimeString(taskByID.EndPeriod), taskByID.WeekendString().ToLower() }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public AppBuildingDrawable.AddOfferingInfo Enroll(long entityID, long courseID, InstallmentLoan loan, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            Course offering = (Course) A.ST.City.GetOffering(courseID);
            if (offering.Prerequisite != null)
            {
                bool flag = false;
                foreach (AttendClass class2 in entity.AcademicTaskHistory.Values)
                {
                    if (class2.Course.Name == offering.Prerequisite)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    throw new SimApplicationException(A.R.GetString("We're sorry. You are required to complete the course {0} before taking this course.", new object[] { offering.Prerequisite }));
                }
            }
            if (offering.Students.Count >= 8)
            {
                throw new SimApplicationException(A.R.GetString("We're sorry. There is no more room left in that course. Please try back in a few months."));
            }
            BankAccount account = null;
            float amount = offering.Cost - loan.OriginalBalance;
            if (accountNumber > -1L)
            {
                account = (BankAccount) entity.BankAccounts[accountNumber];
                if (amount > account.EndingBalance())
                {
                    throw new SimApplicationException("You do not have enough money in that checking account to pay the balance. Choose another account or get a larger loan.");
                }
            }
            this.AddOffering(entityID, courseID, null);
            if (loan.OriginalBalance > 0f)
            {
                loan.Transactions.Add(new Transaction(loan.OriginalBalance, Transaction.TranType.Credit, "Original Balance"));
                entity.StudentLoans.Add(loan.AccountNumber, loan);
            }
            if (accountNumber > -1L)
            {
                account.Transactions.Add(new Transaction(amount, Transaction.TranType.Debit, "Payment for course"));
            }
            return new AppBuildingDrawable.AddOfferingInfo { IsFirstTravel = entity.ModeOfTransportation < 0 };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frm401K.Input Get401K(long entityID, long taskID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frm401K.Input input = new frm401K.Input();
            WorkTask taskByID = (WorkTask) entity.GetTaskByID(taskID);
            input.Allocations = taskByID.R401KAllocations;
            input.CompanyMatch = taskByID.R401KMatch;
            input.PercentWitheld = taskByID.R401KPercentWitheld;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetAllOneTimeEvents(long entityID)
        {
            return A.ST.OneTimeEvents;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmW4.Input GetAllowances(long entityID, long taskID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmW4.Input input = new frmW4.Input();
            WorkTask taskByID = (WorkTask) entity.GetTaskByID(taskID);
            input.Allowances = taskByID.Allowances;
            input.Additional = taskByID.AdditionalWitholding;
            input.DisabledForCompetition = A.SS.DisableFW4;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Car GetAutoInsurance(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car == null)
            {
                throw new SimApplicationException(A.R.GetString("You do not need auto insurance because you don't have a car."));
            }
            return (Car) Utilities.DeepCopyBySerialization(entity.Car);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BankAccount GetBankAccount(long entityID, string bankName, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return (BankAccount) entity.BankAccounts[bankName + accountNumber];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetBankAccounts(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.BankAccounts;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetBills(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return (SortedList) Utilities.DeepCopyBySerialization(entity.Bills);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetCarPrice(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car == null)
            {
                throw new SimApplicationException(A.R.GetString("You cannot sell a car because you do not own one."));
            }
            if (entity.Car.LeaseCost > 0f)
            {
                return A.R.GetString("Your car is leased, so you cannot sell it. If you would like to terminate your lease, you will be charged a penalty of 3 monthly payments. Do you want to terminate the lease?");
            }
            float amount = entity.Car.ComputeResalePrice(A.ST.Now);
            float num2 = 0f;
            if (entity.Car.Loan != null)
            {
                num2 = entity.Car.Loan.EndingBalance();
            }
            string str = A.R.GetString("Tommy Taranti will give you {0} for the car. You have a balance of {1} on your loan, ", new object[] { Utilities.FC(amount, 2, A.I.CurrencyConversion), Utilities.FC(num2, 2, A.I.CurrencyConversion) });
            if ((amount - num2) < 0f)
            {
                throw new SimApplicationException(A.R.GetString(str + "so you would still owe {0} on the loan. You must payoff that much of the loan or more before you can sell the car.", new object[] { Utilities.FC(amount - num2, 2, A.I.CurrencyConversion) }));
            }
            return (str + A.R.GetString("so you'll receive a check for {0}. Proceed with the sale?", new object[] { Utilities.FC(amount - num2, 2, A.I.CurrencyConversion) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetCarShop(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car != null)
            {
                throw new SimApplicationException(A.R.GetString("In this sim, you can only have one car at a time. If you would like a different car, please sell your current one, then buy another."));
            }
            return (ArrayList) A.ST.PurchasableCars.Clone();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public float GetCash(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.Cash;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetChecks(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.Checks;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetCondoPrice(long entityID, Offering offering)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float amount = offering.Building.Rent * A.ST.RealEstateIndex;
            float num2 = 0.05f * amount;
            float num3 = 0f;
            Mortgage mortgage = entity.GetMortgage(offering.Building);
            if (mortgage != null)
            {
                num3 = mortgage.EndingBalance();
            }
            string str = A.R.GetString("The highest offer for your condo was {0}. You will pay a real estate commission of {1} on the sale. You have a balance of {2} on your mortgage, ", new object[] { Utilities.FC(amount, 2, A.I.CurrencyConversion), Utilities.FP(0.05f), Utilities.FC(num3, 2, A.I.CurrencyConversion) });
            if (((amount - num2) - num3) < 0f)
            {
                throw new SimApplicationException(A.R.GetString(str + "so you would still owe {0} on the loan. You must payoff that much of the loan or more before you can sell the condo.", new object[] { Utilities.FC(-((amount - num2) - num3), 2, A.I.CurrencyConversion) }));
            }
            return (str + A.R.GetString("so you'll receive a check for {0}. Proceed with the sale?", new object[] { Utilities.FC((amount - num2) - num3, 2, A.I.CurrencyConversion) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetCreditCardAccounts(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.CreditCardAccounts;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public CreditCardAccount GetCreditCardOffer(long entityID, CreditCardAccount proto)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.CreditScore() < 0)
            {
                throw new SimApplicationException(A.R.GetString("You were denied a credit card. Your credit score was too low. Try paying your bills in full and on time to raise your credit score above {0}, then reapply.", new object[] { 0 }));
            }
            foreach (CreditCardAccount account in entity.CreditCardAccounts.Values)
            {
                if (account.BankName == proto.BankName)
                {
                    throw new SimApplicationException(A.R.GetString("You already have a credit card from that bank."));
                }
            }
            float num = entity.CreditScoreNormalized();
            CreditCardAccount account2 = new CreditCardAccount(proto.CreditLimit * (1f + num), A.ST.PrimeRate() + (proto.InterestRate * (2f - num)), proto.LatePaymentFee) {
                BankName = proto.BankName
            };
            account2.GenerateNewAccountNumber();
            account2.OwnerName = entity.Name;
            return account2;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmCreditReport.Input GetCreditReport(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmCreditReport.Input input = new frmCreditReport.Input {
                Name = entity.Name,
                SSN = A.R.GetString("XXX-XX-{0}", new object[] { entity.Person.ID.ToString().PadLeft(4, '0') }),
                Accounts = new ArrayList(entity.CreditCardAccounts.Values)
            };
            ArrayList list = new ArrayList();
            list.AddRange(entity.ClosedCreditCardAccounts.Values);
            list.AddRange(this.GetInstallmentLoans(entity.ID).Values);
            list.AddRange(entity.ClosedInstallmentLoans.Values);
            list.AddRange(entity.MerchantAccounts.Values);
            input.Now = A.ST.Now;
            foreach (BankAccount account in list)
            {
                if (account.DateClosed > A.ST.Now.AddMonths(-36))
                {
                    input.Accounts.Add(account);
                }
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmCreditScore.Input GetCreditScore(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmCreditScore.Input input = new frmCreditScore.Input();
            ArrayList reasons = new ArrayList();
            input.Score = entity.CreditScore(reasons);
            input.Reasons = reasons;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DailyRoutine[] GetDailyRoutines(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return new DailyRoutine[] { entity.GetDailyRoutine(false), entity.GetDailyRoutine(true) };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BankAccount GetDirectDepositAccount(long entityID, long taskID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            foreach (Task task in entity.GetAllTasks())
            {
                if ((task.ID == taskID) && (task is WorkTask))
                {
                    return ((WorkTask) task).DirectDepositAccount;
                }
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BankAccount GetFedTaxAccount(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return (BankAccount) Utilities.DeepCopyBySerialization(entity.MerchantAccounts["IRS"]);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetFunds()
        {
            return A.ST.MutualFunds;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetGoodCreditCardAccounts(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            SortedList list = new SortedList();
            foreach (CreditCardAccount account in entity.CreditCardAccounts.Values)
            {
                BankAccount.Status status = account.PastDueStatus(A.ST.Year, A.ST.Month);
                if ((status != BankAccount.Status.NewlyCancelled) && (status != BankAccount.Status.Cancelled))
                {
                    list.Add(account.AccountNumber, account);
                }
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public float[] GetHealth(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float[] numArray = new float[A.SS.HealthFactorsToConsider];
            for (int i = 0; i < A.SS.HealthFactorsToConsider; i++)
            {
                numArray[i] = entity.HealthFactorAvg(i);
            }
            return numArray;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public InsurancePolicy GetHealthInsurance(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return (InsurancePolicy) Utilities.DeepCopyBySerialization(entity.HealthInsurance);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmHomeOwnersInsurance.Input GetHomeOwnersInsurance(Offering offering)
        {
            frmHomeOwnersInsurance.Input input = new frmHomeOwnersInsurance.Input();
            Dwelling building = (Dwelling) offering.Building;
            input.Policy = (InsurancePolicy) Utilities.DeepCopyBySerialization(building.Insurance);
            input.Value = building.Rent * A.ST.RealEstateIndex;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetInstallmentLoans(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.GetInstallmentLoans();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetInvestmentAccounts(long entityID, bool retirement)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (retirement)
            {
                return entity.RetirementAccounts;
            }
            return entity.InvestmentAccounts;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetLastYear()
        {
            return (A.ST.Year - 1);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Mortgage[] GetMortgage(long entityID, Offering offering)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float interestRate = (A.ST.PrimeRate() + 0.03f) + ((1f - entity.CreditScoreNormalized()) * 0.08f);
            if (entity.CreditScore() < 610)
            {
                interestRate = 0f;
            }
            float num2 = offering.Building.Rent * A.ST.RealEstateIndex;
            Mortgage[] mortgageArray = new Mortgage[2];
            mortgageArray[0] = new Mortgage(A.ST.Now, num2 - 0f, interestRate, 360);
            mortgageArray[0].BankName = "Century Mortgage";
            mortgageArray[1] = new Mortgage(A.ST.Now, num2 - 0f, interestRate, 180);
            mortgageArray[1].BankName = "Century Mortgage";
            return mortgageArray;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetMovingMessage(long entityID, DwellingOffer offering)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Dwelling == null)
            {
                return null;
            }
            string str = "";
            if (!(((DwellingOffer) entity.Dwelling.Offerings[0]).Condo || (entity.Dwelling.MonthsLeftOnLease <= 0)))
            {
                str = str + A.R.GetString("You have {0} months left on your lease and will be billed for all of them upon moving. ", new object[] { entity.Dwelling.MonthsLeftOnLease });
            }
            return (str + A.R.GetString("Are you sure you want to move?"));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Hashtable GetNamesAndIds()
        {
            Hashtable hashtable = new Hashtable();
            foreach (Entity entity in A.ST.Entity.Values)
            {
                hashtable.Add(entity.Name, entity.ID);
            }
            return hashtable;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TaxReturn GetNewAccountantsReport(long entityID, int year)
        {
            AppEntity e = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            AccountantsReport report = new AccountantsReport(year);
            report.PrepareTaxes(year, e, true);
            return report;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public F1040EZ GetNewF1040EZ(long entityID, int year)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            F1040EZ fez = new F1040EZ(year);
            fez.Lines[0] = entity.Name;
            fez.Lines[1] = entity.ID.ToString().PadLeft(4, '0');
            fez.Lines[2] = "123 Any Street";
            fez.Lines[3] = "Springfield, USA";
            fez.Lines[4] = entity.Name;
            fez.Lines[5] = A.ST.Now.ToShortDateString();
            return fez;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetOfferings()
        {
            return A.ST.City.GetOfferings();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetOfferings(int ave, int street, int lot)
        {
            AppBuilding building = (AppBuilding) A.ST.City[ave, street, lot];
            if (building != null)
            {
                return building.Offerings;
            }
            return new ArrayList();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetOneTimeEventsAttending(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.OneTimeEvents;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SortedList GetOneTimeEventsInvitedTo(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            SortedList list = new SortedList();
            foreach (OneTimeEvent event2 in A.ST.OneTimeEvents.Values)
            {
                if ((event2.rnd < entity.PartyHostingScore()) || (event2.Building == entity.Dwelling))
                {
                    list.Add(event2.Key, event2);
                }
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetPayees(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (!entity.Has("Computer"))
            {
                throw new SimApplicationException(A.R.GetString("You need a computer to do online banking. Buy one at the store."));
            }
            if (!entity.Internet)
            {
                throw new SimApplicationException(A.R.GetString("You need to subscribe to Internet access to use online banking."));
            }
            if (entity.ElectricityOff)
            {
                throw new SimApplicationException(A.R.GetString("Your electricity is turned off. You cannot use your computer to do online banking."));
            }
            return entity.Payees;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public InstallmentLoan GetPayForCar(long entityID, float price)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float interestRate = (A.ST.PrimeRate() + 0.03f) + ((1f - entity.CreditScoreNormalized()) * 0.04f);
            if (entity.CreditScore() < 510)
            {
                interestRate = 0f;
            }
            return new InstallmentLoan(A.ST.Now, price - 0.1f, interestRate, 0x24);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmPersonalBalanceSheet.Input GetPersonalBalanceSheet(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmPersonalBalanceSheet.Input objectToCopy = new frmPersonalBalanceSheet.Input {
                now = A.ST.Now,
                cash = entity.Cash,
                bankAccounts = entity.BankAccounts,
                investmentAccounts = entity.InvestmentAccounts,
                retirementAccounts = entity.RetirementAccounts,
                realEstateValue = entity.ComputeRealEstateValue(),
                creditCardAccounts = entity.CreditCardAccounts,
                merchantAccounts = entity.MerchantAccounts,
                installmentLoans = this.GetInstallmentLoans(entityID)
            };
            if ((entity.Car != null) && (entity.Car.LeaseCost == 0f))
            {
                objectToCopy.carValue = entity.Car.ComputeResalePrice(A.ST.Now);
            }
            objectToCopy.includeOtherLiabilities = A.SS.IncludeOtherLiabilities;
            return (frmPersonalBalanceSheet.Input) Utilities.DeepCopyBySerialization(objectToCopy);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public float GetPrimeRate()
        {
            return A.ST.PrimeRate();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetRegisterEntries(long entityID, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return ((CheckingAccount) entity.BankAccounts[accountNumber]).RegisterEntries;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public InsurancePolicy GetRentersInsurance(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (((DwellingOffer) entity.Dwelling.Offerings[0]).Condo && (entity.RentersInsurance.Deductible == -1f))
            {
                throw new SimApplicationException("You live in a condo. You need homeowner's insurance, not renter's insurance.");
            }
            return (InsurancePolicy) Utilities.DeepCopyBySerialization(entity.RentersInsurance);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmResume.Input GetResume(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return new frmResume.Input { Name = entity.Name, AcademicTaskHistory = entity.AcademicTaskHistory, WorkTaskHistory = entity.WorkTaskHistory };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public override frmScoreboard.Input getScoreboard(bool showAIOwnedEntities)
        {
            frmScoreboard.Input input = base.getScoreboard(showAIOwnedEntities);
            input.ScoreFriendlyName = A.R.GetString("Net Worth");
            ArrayList list = new ArrayList(input.EntityNames);
            ArrayList list2 = new ArrayList(input.Scores);
            foreach (Entity entity in A.ST.RetiredEntity.Values)
            {
                int index = list.IndexOf(entity.Name);
                if (index > -1)
                {
                    list.RemoveAt(index);
                    list2.RemoveAt(index);
                }
            }
            input.EntityNames = (string[]) list.ToArray(typeof(string));
            input.Scores = (ArrayList[]) list2.ToArray(typeof(ArrayList));
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetShop(long entityID, long buildingID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            AppBuilding building = A.ST.City.BuildingByID(buildingID);
            ArrayList list = (ArrayList) Utilities.DeepCopyBySerialization(A.ST.PurchasableItems);
            ArrayList list2 = new ArrayList();
            for (int i = 0; i < list.Count; i++)
            {
                bool flag = false;
                PurchasableItem item = (PurchasableItem) list[i];
                foreach (PurchasableItem item2 in entity.PurchasedItems)
                {
                    if (item2.Name == item.Name)
                    {
                        flag = true;
                    }
                }
                if (!flag || (A.SS.BreakInDate > DateTime.MinValue))
                {
                    bool flag2 = false;
                    if ((A.SS.LevelManagementOn && !A.SS.OnlineBankingEnabledForOwner) && (item.Name == "Computer"))
                    {
                        flag2 = true;
                    }
                    if ((A.SS.LevelManagementOn && (A.SS.HealthFactorsToConsider < 5)) && (item.Name == "DDR"))
                    {
                        flag2 = true;
                    }
                    if (!flag2)
                    {
                        item.Price *= building.Prices[i];
                        item.saleDiscount = building.SaleDiscounts[i];
                        list2.Add(item);
                    }
                }
            }
            return list2;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetShopAutoRepair(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car == null)
            {
                throw new SimApplicationException(A.R.GetString("In this sim, you can only buy gas or car repairs if you have a car. Purchase or lease a car first."));
            }
            return new ArrayList(A.ST.PurchasableAutoSupplies);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetShopBusTokens(long entityID)
        {
            return new ArrayList(A.ST.PurchasableBusTokens);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetShopFood(long entityID)
        {
            if (A.SS.HealthFactorsToConsider > 4)
            {
                return new ArrayList(A.ST.PurchasableFood);
            }
            ArrayList list = new ArrayList();
            for (int i = 0; i < 3; i++)
            {
                list.Add(A.ST.PurchasableFood[i]);
            }
            return list;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmSnapshot.Input GetSnapshot(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmSnapshot.Input input = new frmSnapshot.Input {
                food = entity.Food.Count,
                health = entity.Health,
                gas = -1f,
                busTokens = -1
            };
            if (entity.Car != null)
            {
                input.gas = entity.Car.Gas;
                input.carImageName = "Status" + ((PurchasableItem) A.ST.PurchasableCars[entity.Car.CarIndex()]).ImageName;
                input.carBroken = entity.Car.Broken;
            }
            if (entity.ModeOfTransportation == 1)
            {
                input.busTokens = entity.BusTokens;
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public InstallmentLoan GetStudentLoan(long entityID, Course course)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float interestRate = (A.ST.PrimeRate() + 0.01f) + ((1f - entity.CreditScoreNormalized()) * 0.05f);
            if (entity.CreditScore() < 0)
            {
                interestRate = 0f;
            }
            InstallmentLoan loan = new InstallmentLoan(A.ST.Now, course.Cost, interestRate, 60);
            int num2 = 5;
            if (course.PrototypeTask.Weekend)
            {
                num2 = 2;
            }
            loan.BeginBilling = A.ST.Now.AddDays((double) (7 * (course.Days / num2)));
            loan.BeginBilling = new DateTime(loan.BeginBilling.Year, loan.BeginBilling.Month, 1);
            loan.BankName = "Quest Student Loans";
            return loan;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList GetTaxes(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return (ArrayList) entity.TaxReturns.Clone();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmPayStubs.Input GetTaxInfo(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            frmPayStubs.Input input = new frmPayStubs.Input {
                PayStubs = new ArrayList()
            };
            ArrayList list = new ArrayList();
            foreach (WorkTask task in entity.WorkTaskHistory.Values)
            {
                list.AddRange(task.PayStubs);
            }
            foreach (PayStub stub in list)
            {
                int index = 0;
                while ((index < input.PayStubs.Count) && (stub.WeekEnding > ((PayStub) input.PayStubs[index]).WeekEnding))
                {
                    index++;
                }
                input.PayStubs.Insert(index, stub);
            }
            input.FW2s = new ArrayList(entity.FW2s.Values);
            input.F1099s = new ArrayList(entity.F1099s.Values);
            input.BeginYear = A.SS.StartDate.Year;
            input.EndYear = A.ST.Year;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetTransportation(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            return entity.ModeOfTransportation;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool HasEntity(string playerName)
        {
            return ((A.ST.GetPlayersEntities(playerName).Length > 0) || (A.ST.GetPlayersRetiredEntities(playerName).Length > 0));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool HasHealthInsuranceThruWork(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            foreach (Task task in entity.GetAllTasks())
            {
                if ((task is WorkTask) && (((WorkTask) task).HealthInsurance != null))
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InternetSubscribe(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Internet)
            {
                throw new SimApplicationException(A.R.GetString("You already have a subscription."));
            }
            entity.Internet = true;
            if (!entity.Reserved.ContainsKey("WantsInternet"))
            {
                entity.Reserved.Add("WantsInternet", "");
            }
            entity.Journal.AddEntry(A.R.GetString("Subscribed to Internet access."));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InternetUnSubscribe(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (!entity.Internet)
            {
                throw new SimApplicationException(A.R.GetString("You are already unsubscribed."));
            }
            entity.Internet = false;
            entity.Reserved.Remove("WantsInternet");
            entity.Journal.AddEntry(A.R.GetString("Dropped Internet access."));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LeaseCar(long entityID, float cost, string carName)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            int num = 0;
            foreach (PurchasableItem item in A.ST.PurchasableCars)
            {
                if (item.Name == carName)
                {
                    Car car = new Car {
                        OriginalPrice = item.Price,
                        Purchased = A.ST.Now,
                        LeaseCost = cost
                    };
                    entity.Car = car;
                    break;
                }
                num++;
            }
            A.ST.City.Cars.Add(entity.Car);
            entity.Car.SetLocation(entity.Dwelling);
            A.ST.Reserved.Add(entity.Car, num);
            entity.Journal.AddEntry(A.R.GetString("Leased a car for {0} per month.", new object[] { Utilities.FC(entity.Car.LeaseCost, A.I.CurrencyConversion) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void MoveTo(long entityID, long offeringID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            DwellingOffer offering = (DwellingOffer) A.ST.City.GetOffering(offeringID);
            if (entity.Dwelling != null)
            {
                if (entity.Dwelling.MonthsLeftOnLease > 0)
                {
                    entity.AddBill(new Bill("City Property Mgt", "Lease termination penalty", (float) (entity.Dwelling.MonthsLeftOnLease * entity.Dwelling.Rent), (BankAccount) entity.MerchantAccounts["City Property Mgt"]));
                }
                foreach (Task task in entity.GetAllTasks())
                {
                    if (task.Building == entity.Dwelling)
                    {
                        task.Building = (Dwelling) offering.Building;
                    }
                }
                if (entity.Dwelling.Persons.Contains(entity.Person))
                {
                    entity.Dwelling.Persons.Remove(entity.Person);
                    offering.Building.Persons.Add(entity.Person);
                }
                if (!((DwellingOffer) entity.Dwelling.Offerings[0]).Condo)
                {
                    ArrayList offerings = A.ST.City.GetOfferings(typeof(DwellingOffer));
                    foreach (DwellingOffer offer2 in offerings)
                    {
                        if (offer2.Building == entity.Dwelling)
                        {
                            offer2.Taken = false;
                        }
                    }
                    entity.Dwelling.Owner = null;
                }
                entity.CheckValiditySynchTravel(entity.ModeOfTransportation, entity.GetDailyRoutine(false), null, false);
                entity.CheckValiditySynchTravel(entity.ModeOfTransportation, entity.GetDailyRoutine(true), null, false);
            }
            entity.Dwelling = (Dwelling) offering.Building;
            entity.Dwelling.Owner = entity;
            offering.Taken = true;
            if (!offering.Condo)
            {
                entity.Dwelling.MonthsLeftOnLease = 12;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DateTime Now()
        {
            return A.ST.Now;
        }

        #region Payment methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PayByCard(long entityID, Bill bill, float amountToPay, long accountNumber, bool credit)
        {
            Transaction.TranType debit;
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = null;
            if (credit)
            {
                debit = Transaction.TranType.Credit;
                account = (CreditCardAccount) entity.CreditCardAccounts[accountNumber];
                if ((account.EndingBalance() + amountToPay) > ((CreditCardAccount) account).CreditLimit)
                {
                    throw new SimApplicationException(A.R.GetString("Credit card transaction rejected. That charge would exceed your credit limit."));
                }
                if ((bill.Account != null) && (bill.Account.AccountNumber == accountNumber))
                {
                    throw new SimApplicationException(A.R.GetString("Sorry. You cannot pay a credit card with the same credit card. Please choose another method of payment."));
                }
            }
            else
            {
                debit = Transaction.TranType.Debit;
                account = (BankAccount) entity.BankAccounts[accountNumber];
                if (account.EndingBalance() < bill.Amount)
                {
                    throw new SimApplicationException(A.R.GetString("Debit card transaction rejected. You do not have enough money in the account."));
                }
            }
            account.Transactions.Add(new Transaction(bill.Amount, debit, bill.From));
            long num = -1L;
            if (bill.Account != null)
            {
                num = bill.Account.AccountNumber;
            }
            entity.ApplyPaymentToAccount(num, bill.Amount);
            entity.Bills.Remove(bill.ID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PayByCash(long entityID, Bill bill, float amountToPay)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Cash < amountToPay)
            {
                throw new SimApplicationException(A.R.GetString("You do not have enough cash to pay by cash."));
            }
            if (bill.Account != null)
            {
                throw new SimApplicationException(A.R.GetString("You should not send cash in the mail. Please choose another method of payment."));
            }
            entity.Cash -= amountToPay;
            entity.Bills.Remove(bill.ID);
            long accountNumber = -1L;
            if (bill.Account != null)
            {
                accountNumber = bill.Account.AccountNumber;
            }
            entity.ApplyPaymentToAccount(accountNumber, amountToPay);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PayByCheck(long entityID, long accountNumber, Bill bill, KMI.VBPF1Lib.Check check)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            CheckingAccount account = (CheckingAccount) entity.BankAccounts[accountNumber];
            account.NextCheckNumber++;
            check.ApplyToAccount = bill.Account;
            entity.Bills.Remove(bill.ID);
            account.ChecksInTheMail.Add(check);
        }
        #endregion

        #region Purchase
        #region Purchasing Basics
        public override void ProvideCash(long entityID, float amount)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.Cash += amount;
        }

        public void PurchaseFood(AppEntity entity, int amount, out string str)
        {
            str = amount + " bags of food, ";
            entity.AddFood(amount);
        }

        public void PurchaseGas(AppEntity entity, int amount, out string str)
        {
            str = amount + " gals of gas, ";
            entity.Car.Gas += amount;
        }

        public void PurchaseTokens(AppEntity entity, int amount, out string str)
        {
            str = amount + " tokens, ";
            entity.BusTokens += amount;
        }

        public void PurchaseBasics(AppEntity entity, ArrayList shoppingList, out string str)
        {
            string temp = "";
            if (((string)shoppingList[0]).Contains(" bags of food"))
            {
                PurchaseFood(entity, Convert.ToInt32(((string)shoppingList[0]).Replace(" bags of food", "")), out temp);
                shoppingList.RemoveAt(0);
            }
            else if (((string)shoppingList[0]).Contains(" gals of gas"))
            {
                PurchaseGas(entity, Convert.ToInt32(((string)shoppingList[0]).Replace(" gals of gas", "")), out temp);
                shoppingList.RemoveAt(0);
            }
            else if (((string)shoppingList[0]).Contains(" tokens"))
            {
                PurchaseTokens(entity, Convert.ToInt32(((string)shoppingList[0]).Replace(" tokens", "")), out temp);
                shoppingList.RemoveAt(0);
            }
            str = temp;
        }
        #endregion

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Purchase(long entityID, ArrayList shoppingList)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            string str = "";
            PurchaseBasics(entity, shoppingList, out str);
            foreach (string str2 in shoppingList)
            {
                foreach (PurchasableItem item in A.ST.PurchasableItems)
                {
                    if (str2 == item.Name)
                    {
                        this.PurgeSimilarItem(item.Name, entity.PurchasedItems);
                        entity.PurchasedItems.Add(item);
                        str = str + item.FriendlyName + ", ";
                    }
                }
                int num = 0;
                while (num < A.ST.PurchasableFood.Count)
                {
                    if (str2 == ((PurchasableItem) A.ST.PurchasableFood[num]).Name)
                    {
                        if (num < 3)
                        {
                            entity.AddFood(14 * (num + 1));
                        }
                        else
                        {
                            entity.PartyFood.Add(A.ST.PurchasableFood[num]);
                        }
                        str = str + ((PurchasableItem) A.ST.PurchasableFood[num]).FriendlyName + ", ";
                    }
                    num++;
                }
                num = 0;
                while (num < A.ST.PurchasableBusTokens.Count)
                {
                    if (str2 == ((PurchasableItem) A.ST.PurchasableBusTokens[num]).Name)
                    {
                        entity.BusTokens += 20 + (num * 40);
                        str = str + ((PurchasableItem) A.ST.PurchasableBusTokens[num]).FriendlyName + ", ";
                    }
                    num++;
                }
                if (entity.Car != null)
                {
                    for (num = 0; num < A.ST.PurchasableAutoSupplies.Count; num++)
                    {
                        if (str2 == ((PurchasableItem) A.ST.PurchasableAutoSupplies[num]).Name)
                        {
                            if (num < 3)
                            {
                                entity.Car.Gas += (num + 1) * 10;
                            }
                            else if (num == 3)
                            {
                                entity.Car.LastTuneup = A.ST.Now;
                            }
                            else
                            {
                                entity.Car.Broken = false;
                                entity.Car.LastTuneup = A.ST.Now;
                            }
                            str = str + ((PurchasableItem) A.ST.PurchasableAutoSupplies[num]).FriendlyName + ", ";
                        }
                    }
                }
            }
            entity.Journal.AddEntry(A.R.GetString("Purchased {0}.", new object[] { Utilities.FormatCommaSeries(str.ToLower()) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PurchaseCar(long entityID, InstallmentLoan loan, string carName, long downPaymentAccountNumber, float downPayment)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = (BankAccount) entity.BankAccounts[downPaymentAccountNumber];
            if (account.EndingBalance() < downPayment)
            {
                throw new SimApplicationException("You don't have sufficient funds in that account to make the down payment. Please try again.");
            }
            int num = 0;
            foreach (PurchasableItem item in A.ST.PurchasableCars)
            {
                if (item.Name == carName)
                {
                    Car car = new Car {
                        OriginalPrice = item.Price,
                        Purchased = A.ST.Now
                    };
                    entity.Car = car;
                    account.Transactions.Add(new Transaction(downPayment, Transaction.TranType.Debit, "Car Downpayment"));
                    loan.Transactions.Add(new Transaction(loan.OriginalBalance, Transaction.TranType.Credit, "Original Balance"));
                    loan.BankName = "Taranti Auto Loan";
                    loan.BeginBilling = A.ST.Now;
                    entity.Car.Loan = loan;
                    break;
                }
                num++;
            }
            A.ST.City.Cars.Add(entity.Car);
            entity.Car.SetLocation(entity.Dwelling);
            A.ST.Reserved.Add(entity.Car, num);
            entity.Journal.AddEntry(A.R.GetString("Bought a car for {0}.", new object[] { Utilities.FC(entity.Car.OriginalPrice, A.I.CurrencyConversion) }));
        }
        #endregion

        protected void PurgeSimilarItem(string name, ArrayList purchasedItems)
        {
            ArrayList list = new ArrayList(purchasedItems);
            foreach (PurchasableItem item in list)
            {
                if (!(!name.StartsWith(item.Name.Substring(0, item.Name.Length - 1)) || name.StartsWith("Art")))
                {
                    purchasedItems.Remove(item);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SchedulePayments(long entityID, long accountNumber, Hashtable payments)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            Transaction.TranType debit = Transaction.TranType.Debit;
            BankAccount account = (BankAccount) entity.BankAccounts[accountNumber];
            foreach (BankAccount account2 in payments.Keys)
            {
                float amount = (float) payments[account2];
                if (account.EndingBalance() < amount)
                {
                    throw new SimApplicationException(A.R.GetString("Some online payments were rejected. You do not have enough money in the account."));
                }
                account.Transactions.Add(new Transaction(amount, debit, account2.BankName));
                entity.ApplyPaymentToAccount(account2.AccountNumber, amount);
                entity.RemoveBillIfExactMatch(account2.AccountNumber, amount);
            }
            entity.Journal.AddEntry(A.R.GetString("Bill payments made via online banking."));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SellCar(long entityID)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car != null)
            {
                if (entity.Car.LeaseCost > 0f)
                {
                    entity.AddBill(new Bill("Taranti Auto Lease", "Lease termination penalty", entity.Car.LeaseCost * 3f, (BankAccount) entity.MerchantAccounts["Taranti Auto Lease"]));
                }
                else
                {
                    float num = entity.Car.ComputeResalePrice(A.ST.Now);
                    float num2 = 0f;
                    if (entity.Car.Loan != null)
                    {
                        num2 = entity.Car.Loan.EndingBalance();
                    }
                    KMI.VBPF1Lib.Check c = new KMI.VBPF1Lib.Check(-1L) {
                        Amount = num - num2,
                        Payee = entity.Name,
                        Date = A.ST.Now,
                        Payor = "Taranti Auto & Loan",
                        Number = (int) A.ST.GetNextID(),
                        Memo = A.R.GetString("Sale of car"),
                        Signature = A.R.GetString("Tommy T. Taranti")
                    };
                    entity.AddCheck(c);
                }
                if (entity.Car.Loan != null)
                {
                    entity.CloseAccount(entity.Car.Loan);
                }
                A.ST.City.Cars.Remove(entity.Car);
                entity.Car = null;
                entity.Journal.AddEntry(A.R.GetString("Got rid of car."));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SellCondo(long entityID, Offering offering)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            float amount = offering.Building.Rent * A.ST.RealEstateIndex;
            float num2 = 0f;
            Mortgage a = entity.GetMortgage(offering.Building);
            if (a != null)
            {
                num2 = a.EndingBalance();
            }
            float num3 = amount * 0.05f;
            KMI.VBPF1Lib.Check c = new KMI.VBPF1Lib.Check(-1L) {
                Amount = (amount - num3) - num2,
                Payee = entity.Name,
                Date = A.ST.Now,
                Payor = "Ward and June Cleaver",
                Number = (int) A.ST.GetNextID(),
                Memo = A.R.GetString("Sale of condo"),
                Signature = A.R.GetString("Ward Cleaver")
            };
            entity.AddCheck(c);
            if (a != null)
            {
                entity.CloseAccount(a);
            }
            ArrayList offerings = A.ST.City.GetOfferings(typeof(DwellingOffer));
            foreach (DwellingOffer offer in offerings)
            {
                if (offer.ID == offering.ID)
                {
                    offer.Taken = false;
                    offer.Building.Owner = null;
                }
            }
            entity.Journal.AddEntry(A.R.GetString("Sold condo for {0}.", new object[] { Utilities.FC(amount, A.I.CurrencyConversion) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SellFund(long entityID, string fundName, float amount, long accountNumber, bool retirement)
        {
            if (amount != 0f)
            {
                AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
                InvestmentAccount investmentAccount = entity.GetInvestmentAccount(fundName, false);
                Fund fund = A.ST.GetFund(fundName);
                if (investmentAccount == null)
                {
                    throw new SimApplicationException("You haven't bought any shares of that fund yet.");
                }
                amount = Math.Min(amount, investmentAccount.EndingBalance() * fund.Price);
                float num = amount / fund.Price;
                if (num == 0f)
                {
                    throw new SimApplicationException("You don't own any shares of that fund yet.");
                }
                int num2 = investmentAccount.ShareAge();
                float num3 = 0f;
                if (num2 < 0x721)
                {
                    num3 = (fund.BackEndLoad * num) * fund.Price;
                    if (num3 > 0f)
                    {
                        entity.Player.SendMessage(A.R.GetString("You were charged a back end load or redemption fee of {0} for selling shares held less than {1} years.", new object[] { Utilities.FC(num3, 2, A.I.CurrencyConversion), 5 }), "", NotificationColor.Yellow);
                    }
                }
                float num4 = 0f;
                if ((A.ST.Now < new DateTime(0x803, 6, 30)) && retirement)
                {
                    num4 = 0.1f * amount;
                }
                if (accountNumber == -1L)
                {
                    entity.Cash += (amount - num3) - num4;
                }
                else
                {
                    BankAccount account2 = (BankAccount) entity.BankAccounts[accountNumber];
                    account2.Transactions.Add(new Transaction((amount - num3) - num4, Transaction.TranType.Credit, A.R.GetString("Share redemption")));
                }
                float num5 = investmentAccount.CostBasis / investmentAccount.EndingBalance();
                float num6 = amount - (num * num5);
                investmentAccount.CostBasis -= amount;
                int year = A.ST.Now.Year;
                Hashtable sTCapitalGains = entity.STCapitalGains;
                if (!((num2 <= 0x16c) || investmentAccount.Retirement))
                {
                    sTCapitalGains = entity.LTCapitalGains;
                }
                if (sTCapitalGains.ContainsKey(year))
                {
                    sTCapitalGains[year] = ((float) sTCapitalGains[year]) + num6;
                }
                else
                {
                    sTCapitalGains.Add(year, num6);
                }
                Transaction transaction = new Transaction(num, Transaction.TranType.Debit, A.R.GetString("Share Redemption"));
                if (transaction.Amount > investmentAccount.Value)
                {
                    transaction.Amount = investmentAccount.Value;
                }
                investmentAccount.Transactions.Add(transaction);
                entity.Journal.AddEntry(A.R.GetString("Sold {0} shares of {1}. Net proceeds were {2}.", new object[] { num, fund.Name, Utilities.FC((amount - num3) - num4, A.I.CurrencyConversion) }));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Set401K(long entityID, long taskID, float percentWitheld, float[] allocations)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            WorkTask taskByID = (WorkTask) entity.GetTaskByID(taskID);
            taskByID.R401KAllocations = allocations;
            taskByID.R401KPercentWitheld = percentWitheld;
            entity.Journal.AddEntry(A.R.GetString("Changed 401K investment allocations."));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetAllowances(long entityID, long taskID, bool exempt, int allowances, float additional)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            WorkTask taskByID = (WorkTask) entity.GetTaskByID(taskID);
            taskByID.ExemptFromWitholding = exempt;
            taskByID.Allowances = allowances;
            taskByID.AdditionalWitholding = additional;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetAutoInsurance(long entityID, InsurancePolicy policy)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (entity.Car != null)
            {
                entity.Car.Insurance = policy;
                entity.Journal.AddEntry(A.R.GetString("Purchased auto insurance policy for {0} per month.", new object[] { Utilities.FC(policy.MonthlyPremium, 2, S.I.CurrencyConversion) }));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetBankAccount(long entityID, BankAccount account, float deposit, long transferFrom)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (transferFrom == -1L)
            {
                if (deposit > (entity.Cash + 0.01))
                {
                    throw new SimApplicationException("You do not have that much cash. Reduce your initial deposit level and try again.");
                }
                entity.Cash -= deposit;
            }
            else
            {
                if (deposit > ((BankAccount) entity.BankAccounts[transferFrom]).EndingBalance())
                {
                    throw new SimApplicationException("You do not have that much in the account you are transferring from. Reduce your initial deposit level and try again.");
                }
                ((BankAccount) entity.BankAccounts[transferFrom]).Transactions.Add(new Transaction(deposit, Transaction.TranType.Debit, A.R.GetString("Transfer")));
            }
            account = (BankAccount) Utilities.DeepCopyBySerialization(account);
            account.GenerateNewAccountNumber();
            account.Transactions.Add(new Transaction(deposit, Transaction.TranType.Credit, A.R.GetString("Initial deposit")));
            account.OwnerName = entity.Name;
            entity.BankAccounts.Add(account.AccountNumber, account);
            entity.Journal.AddEntry(A.R.GetString("Opened new account: {0} with initial deposit of {1}.", new object[] { account.ToString(), Utilities.FC(deposit, S.I.CurrencyConversion) }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetCreditCardAccount(long entityID, BankAccount account)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.CreditCardAccounts.Add(account.AccountNumber, account);
            entity.Journal.AddEntry(A.R.GetString("Got new credit card from {0}.", new object[] { account.BankName }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetDepositWithdrawCash(long entityID, bool withdraw, float amount, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = (BankAccount) entity.BankAccounts[accountNumber];
            if (withdraw)
            {
                if (account.EndingBalance() < amount)
                {
                    throw new SimApplicationException("You do not have that much in the account. Please try again.");
                }
                entity.Cash += amount;
                account.Transactions.Add(new Transaction(amount, Transaction.TranType.Debit, "Cash Withdrawal"));
            }
            else
            {
                if (entity.Cash < amount)
                {
                    throw new SimApplicationException("You do not have enough cash to make that deposit. Please try again.");
                }
                entity.Cash -= amount;
                account.Transactions.Add(new Transaction(amount, Transaction.TranType.Credit, "Cash Deposit"));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetDirectDepositAccount(long entityID, long taskID, long accountNumber)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = null;
            if (accountNumber > -1L)
            {
                account = (BankAccount) entity.BankAccounts[accountNumber];
            }
            foreach (Task task in entity.GetAllTasks())
            {
                if ((task.ID == taskID) && (task is WorkTask))
                {
                    ((WorkTask) task).DirectDepositAccount = account;
                }
            }
            if (account != null)
            {
                entity.Journal.AddEntry(A.R.GetString("Set up direct deposit to {0}.", new object[] { account.ToString() }));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetHealthInsurance(long entityID, InsurancePolicy policy)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.HealthInsurance = policy;
            if (policy.Deductible > -1f)
            {
                entity.Journal.AddEntry(A.R.GetString("Purchased health insurance policy for {0} per month.", new object[] { Utilities.FC(policy.MonthlyPremium, 2, S.I.CurrencyConversion) }));
            }
            else
            {
                entity.Journal.AddEntry(A.R.GetString("Decided not to carry health insurance."));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetHomeOwnersInsurance(Offering offering, InsurancePolicy policy)
        {
            ((Dwelling) offering.Building).Insurance = policy;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetMortgage(long entityID, Mortgage loan, long offeringID, long accountNumber, float closingCosts)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if (loan.Payment > 3f)
            {
                if ((loan.Payment / Math.Max(1f, entity.GrossIncomeLast12Months)) > 0.28)
                {
                    throw new SimApplicationException("Your mortgage payment would be more than 28% of gross income. Your mortgage application has been rejected.");
                }
                if (((loan.Payment + entity.DebtService) / Math.Max(1f, entity.GrossIncomeLast12Months)) > 0.36)
                {
                    throw new SimApplicationException("Your mortgage payment plus other debt payments would be more than 36% of gross income. Your mortgage application has been rejected.");
                }
            }
            BankAccount account = (BankAccount) entity.BankAccounts[accountNumber];
            if (account.EndingBalance() < closingCosts)
            {
                throw new SimApplicationException("You don't have sufficient funds in that account to cover the cash required at closing. Please try again.");
            }
            account.Transactions.Add(new Transaction(closingCosts, Transaction.TranType.Debit, "Condo Closing"));
            Offering offering = A.ST.City.GetOffering(offeringID);
            offering.Taken = true;
            offering.Building.Owner = entity;
            if (loan.OriginalBalance > 1f)
            {
                loan.Transactions.Add(new Transaction(loan.OriginalBalance, Transaction.TranType.Credit, "Original Balance"));
                loan.Building = offering.Building;
                entity.Mortgages.Add(loan.AccountNumber, loan);
            }
            entity.Journal.AddEntry(offering.JournalEntry());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetOneTimeEvents(long entityID, ArrayList eventIDs)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.OneTimeEvents.Clear();
            foreach (long num in eventIDs)
            {
                OneTimeEvent oneTimeEventByID = (OneTimeEvent) A.ST.GetOneTimeEventByID(num);
                if (oneTimeEventByID != null)
                {
                    oneTimeEventByID = (OneTimeEvent) Utilities.DeepCopyBySerialization(oneTimeEventByID);
                    entity.OneTimeEvents.Add(oneTimeEventByID.Key, oneTimeEventByID);
                }
            }
            entity.CheckValiditySynchTravel(entity.ModeOfTransportation, entity.GetDailyRoutine(), null, false);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetParty(long entityID, DateTime date, int startPeriod, int endPeriod)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            Dance dance = new Dance {
                OneTimeDay = date
            };
            DateTime key = dance.Key;
            dance.StartPeriod = startPeriod;
            dance.EndPeriod = endPeriod;
            dance.Building = entity.Dwelling;
            dance.HostID = entity.ID;
            A.ST.OneTimeEvents.Add(dance.Key, dance);
            entity.OneTimeEvents.Add(dance.Key, dance);
            entity.CheckValiditySynchTravel(entity.ModeOfTransportation, entity.GetDailyRoutine(), null, false);
            entity.Journal.AddEntry(A.R.GetString("Decided to have a party from {0} to {1} on {2}.", new object[] { Task.ToTimeString(dance.StartPeriod), Task.ToTimeString(dance.EndPeriod), dance.OneTimeDay.ToShortDateString() }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetRecurringPayments(long entityID, long accountNumber, ArrayList payments)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            CheckingAccount account = (CheckingAccount) entity.BankAccounts[accountNumber];
            account.RecurringPayments = payments;
            entity.Journal.AddEntry(A.R.GetString("Recurring payments set via online banking."));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetRegisterEntries(long entityID, string bankName, long accountNumber, ArrayList entries)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            ((CheckingAccount) entity.BankAccounts[accountNumber]).RegisterEntries = entries;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetRentersInsurance(long entityID, InsurancePolicy policy)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.RentersInsurance = policy;
            if (policy.Deductible > -1f)
            {
                entity.Journal.AddEntry(A.R.GetString("Purchased renter's insurance policy for {0} per month.", new object[] { Utilities.FC(policy.MonthlyPremium, 2, S.I.CurrencyConversion) }));
            }
            else
            {
                entity.Journal.AddEntry(A.R.GetString("Decided not to carry renter's insurance."));
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetTaxes(long entityID, TaxReturn taxReturn)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            entity.TaxReturns.Add(taxReturn);
            entity.Journal.AddEntry(A.R.GetString("Filed tax return for {0}.", new object[] { taxReturn.Year }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SetTransportation(long entityID, int index)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            if ((index == 2) && (entity.Car == null))
            {
                throw new SimApplicationException(A.R.GetString("You do not own or lease a car. Please choose another form of transportation."));
            }
            entity.SetModeOfTransportation(index);
            string[] strArray = new string[] { A.R.GetString("walking"), A.R.GetString("bus"), A.R.GetString("car") };
            entity.Journal.AddEntry(A.R.GetString("Changed mode of transportation to {0}.", new object[] { strArray[index] }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int TaxYearDue(long entityID)
        {
            int num = A.ST.Year - 1;
            if (num == (A.SS.StartDate.Year - 1))
            {
                return -1;
            }
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            foreach (TaxReturn return2 in entity.TaxReturns)
            {
                if (return2.Year == num)
                {
                    return -1;
                }
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void TransferFunds(long entityID, long fromAccountNumber, long toAccountNumber, float amount)
        {
            AppEntity entity = (AppEntity) SimStateAdapter.CheckEntityExists(entityID);
            BankAccount account = (BankAccount) entity.BankAccounts[fromAccountNumber];
            BankAccount account2 = (BankAccount) entity.BankAccounts[toAccountNumber];
            if (account.EndingBalance() < amount)
            {
                throw new SimApplicationException("You do not have that much in the account. Please try again.");
            }
            account.Transactions.Add(new Transaction(amount, Transaction.TranType.Debit, A.R.GetString("Transfer out")));
            account2.Transactions.Add(new Transaction(amount, Transaction.TranType.Credit, A.R.GetString("Transfer in")));
            entity.Journal.AddEntry(A.R.GetString("Transferred {0} from {1} to {2}.", new object[] { Utilities.FC(amount, S.I.CurrencyConversion), account.ToString(), account2.ToString() }));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool UseAccountant()
        {
            return (((A.SS.LevelManagementOn && (A.SS.Level > 1)) || (!A.SS.LevelManagementOn && A.SS.ViewPortfolioEnabledForOwner)) || A.SS.AlwaysUseTaxAccountant);
        }
    }
}

