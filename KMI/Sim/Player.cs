namespace KMI.Sim
{
    using System;
    using System.Collections;
    using System.Windows.Forms;

    [Serializable]
    public class Player
    {
        protected string playerName;
        protected KMI.Sim.PlayerType playerType;
        protected ArrayList surveys = new ArrayList();

        public Player(string playerName, KMI.Sim.PlayerType playerType)
        {
            this.playerName = playerName;
            this.playerType = playerType;
        }

        public void SendMessage(string message, string from, NotificationColor notificationColor)
        {
            if (!S.I.BlockMessage(message) && ((this.playerType != KMI.Sim.PlayerType.AI) || S.MF.DesignerMode))
            {
                S.SA.FirePlayerMessageEvent(new PlayerMessage(this.PlayerName, message, from, S.ST.Now, notificationColor));
            }
        }

        public void SendModalMessage(ModalMessage modalMessage)
        {
            if (this.playerType != KMI.Sim.PlayerType.AI)
            {
                S.SA.FireModalMessageEvent(modalMessage);
            }
        }

        public void SendModalMessage(string message, string title, MessageBoxIcon icon)
        {
            if (this.playerType != KMI.Sim.PlayerType.AI)
            {
                S.SA.FireModalMessageEvent(new ModalMessage(this.PlayerName, message, title, icon));
            }
        }

        public void SendPeriodicMessage(string message, string from, NotificationColor notificationColor, float daysBetweenMessages)
        {
            string key = message + this.PlayerName + from;
            if (S.I.PeriodicMessageTable.ContainsKey(key))
            {
                DateTime time = (DateTime) S.I.PeriodicMessageTable[key];
                TimeSpan span = (TimeSpan) (S.ST.Now - time);
                if ((span.TotalSeconds / 86400.0) < daysBetweenMessages)
                {
                    return;
                }
                S.I.PeriodicMessageTable[key] = S.ST.Now;
            }
            else
            {
                S.I.PeriodicMessageTable.Add(key, S.ST.Now);
            }
            this.SendMessage(message, from, notificationColor);
        }

        public string PlayerName
        {
            get
            {
                return this.playerName;
            }
        }

        public KMI.Sim.PlayerType PlayerType
        {
            get
            {
                return this.playerType;
            }
        }

        public ArrayList Surveys
        {
            get
            {
                return this.surveys;
            }
        }
    }
}

