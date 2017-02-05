namespace KMI.Biz
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Runtime.CompilerServices;

    public class BizStateAdapter : SimStateAdapter
    {
        public virtual CommentLog GetComments(long entityID)
        {
            throw new NotImplementedException("GetComments not overriden in BizStateAdapter Subclass.");
        }

        public virtual ConsultantReport GetConsultantReport(long entityID)
        {
            throw new NotImplementedException("GetConsultantReport not overriden in BizStateAdapter Subclass.");
        }

        public virtual ConsultantReport[] GetGrades(long entityID)
        {
            throw new NotImplementedException("GetGrades not overriden in BizStateAdapter Subclass.");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public object[,] GetMarketShare()
        {
            int num2;
            base.LogMethodCall(new object[0]);
            SimState sT = S.ST;
            if (S.ST.Entity.Count == 0)
            {
                return null;
            }
            int num = Math.Max(0, sT.CurrentWeek - GeneralLedger.WeeksOfFinancialHistory);
            float[] numArray = new float[sT.CurrentWeek - num];
            object[,] objArray = new object[sT.Entity.Count + 1, (sT.CurrentWeek - num) + 1];
            foreach (Entity entity in sT.Entity.Values)
            {
                num2 = num;
                while (num2 < sT.CurrentWeek)
                {
                    numArray[num2 - num] += entity.GL.AccountBalance("Revenue", num2);
                    num2++;
                }
            }
            int num3 = 0;
            foreach (Entity entity in sT.Entity.Values)
            {
                num3++;
                for (num2 = num; num2 < sT.CurrentWeek; num2++)
                {
                    if (!(numArray[num2 - num] == 0f))
                    {
                        objArray[num3, (num2 - num) + 1] = entity.GL.AccountBalance("Revenue", num2) / numArray[num2 - num];
                    }
                    else
                    {
                        objArray[num3, (num2 - num) + 1] = 0f;
                    }
                    objArray[0, (num2 - num) + 1] = entity.GL.EndingDateOfPeriod(num2, GeneralLedger.Frequency.Weekly);
                }
                objArray[num3, 0] = entity.Name + " - " + Utilities.FP((float) objArray[num3, objArray.GetUpperBound(1)]);
            }
            return objArray;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual frmTransferCash.Input getTransferCash(string playerName)
        {
            base.LogMethodCall(new object[] { playerName });
            frmTransferCash.Input input = new frmTransferCash.Input();
            Entity[] playersEntities = S.ST.GetPlayersEntities(playerName);
            input.OwnedEntities = new string[playersEntities.Length];
            input.CashBalances = new float[playersEntities.Length];
            for (int i = 0; i < playersEntities.Length; i++)
            {
                input.OwnedEntities[i] = playersEntities[i].Name;
                input.CashBalances[i] = playersEntities[i].GL.AccountBalance("Cash");
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmVitalSigns.Input getVitalSigns(long entityID)
        {
            Entity entity = SimStateAdapter.CheckEntityExists(entityID);
            frmVitalSigns.Input input = new frmVitalSigns.Input();
            float num = 0f;
            int num2 = 0;
            foreach (Entity entity2 in S.ST.Entity.Values)
            {
                if (entity2.Player == entity.Player)
                {
                    num += entity2.Journal.NumericDataSeriesLastEntry("Cumulative Profit");
                    num2++;
                }
            }
            foreach (Entity entity2 in S.ST.RetiredEntity.Values)
            {
                if (entity2.Player == entity.Player)
                {
                    num += entity2.Journal.NumericDataSeriesLastEntry("Cumulative Profit");
                    num2++;
                }
            }
            input.CumProfit = num;
            input.MultipleEntities = num2 > 1;
            int num3 = Math.Min(S.ST.CurrentWeek, 8);
            input.Profit = new float[num3];
            input.Sales = new float[num3];
            input.Customers = new int[num3];
            int index = 0;
            for (int i = S.ST.CurrentWeek - num3; i < S.ST.CurrentWeek; i++)
            {
                input.Profit[index] = entity.GL.AccountBalance("Profit", i);
                input.Sales[index] = entity.GL.AccountBalance("Revenue", i);
                input.Customers[index] = (int) entity.GL.AccountBalance("Customers", i);
                index++;
            }
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void setTransferCash(long fromEntityID, long toEntityID, float amount)
        {
            Entity entity = SimStateAdapter.CheckEntityExists(fromEntityID);
            Entity entity2 = SimStateAdapter.CheckEntityExists(toEntityID);
            if (amount > entity.GL.AccountBalance("Cash"))
            {
                throw new SimApplicationException(S.R.GetString("The amount you are trying to transfer is greater than the cash now at the store you are transferring from.  The cash balance of that store is {0}. Try transferring much less or that store will run out of cash.", new object[] { Utilities.FC(entity.GL.AccountBalance("Cash"), S.I.CurrencyConversion) }));
            }
            entity.GL.Post("Cash", -amount, "Paid-in Capital");
            entity2.GL.Post("Cash", amount, "Paid-in Capital");
            entity.Journal.AddEntry("Transferred " + Utilities.FC(amount, S.I.CurrencyConversion) + " from " + entity.Name + " to " + entity2.Name + ".");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void TransferCashFrom(string fromEntity, float amount)
        {
            Entity entityByName = S.ST.GetEntityByName(fromEntity);
            if (entityByName == null)
            {
                throw new SimApplicationException(S.R.GetString("Can't transfer cash from {0}.", new object[] { fromEntity }));
            }
            entityByName.GL.Post("Cash", -amount, "Paid-in Capital");
        }
    }
}

