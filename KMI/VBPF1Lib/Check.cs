﻿namespace KMI.VBPF1Lib
{
    using System;

    [Serializable]
    public class Check
    {
        public long AccountNumber;
        public float Amount;
        public BankAccount ApplyToAccount;
        protected bool cleared;
        public DateTime Date = A.ST.Now;
        public long ID = A.ST.GetNextID();
        public string Memo;
        public int Number;
        public string Payee;
        public string Payor;
        public string Signature;

        public Check(long accountNumber)
        {
            this.AccountNumber = accountNumber;
        }
    }
}

