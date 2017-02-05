namespace KMI.Sim
{
    using KMI.Utility;
    using System;
    using System.Runtime.CompilerServices;

    public class ClientEventHandlers : MarshalByRefObject
    {
        public static ClientEventHandlers Instance = new ClientEventHandlers();

        private ClientEventHandlers()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void ModalMessageHandler(ModalMessage message)
        {
            if (((message.To == S.I.ThisPlayerName) || (message.To == S.R.GetString("All Players"))) && !S.MF.OptOutModalMessageHook(message))
            {
                S.MF.Invoke(new AddModalMessageDelegate(S.MF.ShowModalMessage), new object[] { message });
            }
        }

        public void PlayerMessageHandler(PlayerMessage message)
        {
            if ((((message.To == S.I.ThisPlayerName) && ((S.I.MultiplayerRole == null) || S.I.MultiplayerRole.ReceivesMessages)) || ((message.To == S.R.GetString("All Players")) || S.I.Host)) || S.MF.DesignerMode)
            {
                S.MF.Invoke(new AddPlayerMessageDelegate(S.MF.AddPlayerMessage), new object[] { message });
            }
        }

        public void PlaySoundHandler(string fileName, long entityID, string viewName)
        {
            if ((S.MF.SoundOn && ((S.MF.CurrentEntityID == entityID) || (entityID == -1L))) && (S.MF.CurrentViewName == viewName))
            {
                Sound.PlaySoundFromFile(fileName);
            }
        }

        private delegate void AddModalMessageDelegate(ModalMessage message);

        private delegate void AddPlayerMessageDelegate(PlayerMessage message);
    }
}

