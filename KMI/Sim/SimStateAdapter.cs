namespace KMI.Sim
{
    using KMI.Sim.Academics;
    using KMI.Sim.Drawables;
    using KMI.Sim.Surveys;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class SimStateAdapter : MarshalByRefObject
    {
        public const int SOUND_NOT_ENTITY_SPECIFIC = -1;

        public event ModalMessageDelegate ModalMessageEvent;

        public event PlayerMessageDelegate PlayerMessageEvent;

        public event PlaySoundDelegate PlaySoundEvent;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void ChangeEntityOwner(long entityID, string newOwnerName)
        {
            CheckEntityExists(entityID);
            Entity entity = (Entity) this.simState.Entity[entityID];
            foreach (Player player in this.simState.Player.Values)
            {
                if (player.PlayerName == newOwnerName)
                {
                    entity.Player = player;
                    return;
                }
            }
            throw new SimApplicationException("Player name not found.");
        }

        public static Entity CheckEntityExists(long entityID)
        {
            if (S.ST.Entity.Contains(entityID))
            {
                return (Entity) S.ST.Entity[entityID];
            }
            EntityNotFoundException exception = new EntityNotFoundException(S.R.GetString("{0} no longer exists.", new object[] { S.I.EntityName }));
            throw exception;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CloseEntity(long entityID)
        {
            this.LogMethodCall(new object[0]);
            CheckEntityExists(entityID).Retire();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual Survey ConductAndAddSurvey(string playerName, long entityID, ArrayList questions, int numToSurvey, float cost)
        {
            string[] entityNames = new string[S.ST.Entity.Count];
            int num = 0;
            foreach (Entity entity in S.ST.Entity.Values)
            {
                entityNames[num++] = entity.Name;
            }
            Survey survey = S.I.SimFactory.CreateSurvey(entityID, this.simState.Now, entityNames, questions);
            survey.Execute(numToSurvey);
            ArrayList surveys = ((Player) S.ST.Player[playerName]).Surveys;
            surveys.Add(survey);
            if (surveys.Count > Survey.MaxSurveys)
            {
                surveys.RemoveAt(0);
            }
            if ((entityID != -1L) && S.ST.Entity.ContainsKey(entityID))
            {
                Entity entity = (Entity) S.ST.Entity[entityID];
                if (Survey.BillForSurveys)
                {
                    entity.GL.Post("Surveys", cost, "Cash");
                }
                entity.Journal.AddEntry(S.R.GetString("Conducted survey of {0} {1}.", new object[] { Utilities.FU(survey.Responses.Count), Survey.SurveyableObjectName }));
            }
            return survey;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Player CreateClientPlayer(string playerName, string password)
        {
            if ((S.ST.Multiplayer && S.I.UserAdminSettings.PasswordsForMultiplayer) && !((password.Length >= 3) && S.ST.ValidateMultiplayerTeamPassword(playerName, password)))
            {
                throw new frmJoinMultiplayerSession.BadTeamPasswordException();
            }
            return this.CreatePlayer(playerName, PlayerType.Human);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Player CreatePlayer(string playerName, PlayerType playerType)
        {
            this.LogMethodCall(new object[] { playerName, playerType });
            Player player = null;
            Simulator instance = Simulator.Instance;
            foreach (Player player2 in this.simState.Player.Values)
            {
                if (player2.PlayerName.ToUpper() == playerName.ToUpper())
                {
                    player = player2;
                }
            }
            if (player == null)
            {
                player = S.I.SimFactory.CreatePlayer(playerName, playerType);
                this.simState.Player.Add(playerName, player);
                return player;
            }
            if (!this.simState.RoleBasedMultiplayer)
            {
                player.SendMessage(S.R.GetString("Welcome back, {0}.", new object[] { player.PlayerName }), "", NotificationColor.Green);
            }
            return player;
        }

        public void FireModalMessageEvent(ModalMessage message)
        {
            if (S.I.Client)
            {
                throw new Exception("FireModalMessageEvent called from client.");
            }
            if (this.ModalMessageEvent != null)
            {
                foreach (ModalMessageDelegate delegate2 in this.ModalMessageEvent.GetInvocationList())
                {
                    try
                    {
                        delegate2.BeginInvoke(message, new AsyncCallback(this.ModalMessageCallback), delegate2);
                    }
                    catch (SocketException)
                    {
                        this.ModalMessageEvent = (ModalMessageDelegate) Delegate.Remove(this.ModalMessageEvent, delegate2);
                    }
                }
            }
        }

        public void FirePlayerMessageEvent(PlayerMessage message)
        {
            if (S.I.Client)
            {
                throw new Exception("FirePlayerMessageEvent called from client.");
            }
            if (this.PlayerMessageEvent != null)
            {
                foreach (PlayerMessageDelegate delegate2 in this.PlayerMessageEvent.GetInvocationList())
                {
                    try
                    {
                        delegate2.BeginInvoke(message, new AsyncCallback(this.PlayerMessageCallback), delegate2);
                    }
                    catch (SocketException)
                    {
                        this.PlayerMessageEvent = (PlayerMessageDelegate) Delegate.Remove(this.PlayerMessageEvent, delegate2);
                    }
                }
            }
        }

        public void FirePlaySoundEvent(string fileName, long entityID, string viewName)
        {
            if (S.I.Client)
            {
                throw new Exception("FirePlaySoundEvent called from client.");
            }
            if (this.PlaySoundEvent != null)
            {
                foreach (PlaySoundDelegate delegate2 in this.PlaySoundEvent.GetInvocationList())
                {
                    try
                    {
                        delegate2.BeginInvoke(fileName, entityID, viewName, new AsyncCallback(this.PlaySoundCallback), delegate2);
                    }
                    catch (SocketException)
                    {
                        this.PlaySoundEvent = (PlaySoundDelegate) Delegate.Remove(this.PlaySoundEvent, delegate2);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public AcademicGod GetAcademicGod()
        {
            return S.ST.GetAcademicGod();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmActionsJournal.Input getActionsJournal(long entityID)
        {
            Entity current;
            if (entityID != -1L)
            {
                CheckEntityExists(entityID);
            }
            frmActionsJournal.Input input = new frmActionsJournal.Input {
                Journals = new ArrayList()
            };
            if (entityID == -1L)
            {
                IEnumerator enumerator = S.ST.Entity.Values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (Entity) enumerator.Current;
                    input.Journals.Add(current.Journal);
                }
                foreach (Entity entity in S.ST.RetiredEntity.Values)
                {
                    input.Journals.Add(entity.Journal);
                }
            }
            else
            {
                current = (Entity) S.ST.Entity[entityID];
                input.Journals.Add(current.Journal);
            }
            input.StartDate = S.ST.SimSettings.StartDate;
            input.EndDate = S.ST.Now;
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public long GetAnEntityIdForPlayer(string playerName)
        {
            Entity[] playersEntities = S.ST.GetPlayersEntities(playerName);
            if (playersEntities.Length == 0)
            {
                return -1L;
            }
            return playersEntities[0].ID;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int getCurrentWeek()
        {
            this.LogMethodCall(new object[0]);
            return Simulator.Instance.SimState.CurrentWeek;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string GetEntityPlayer(long entityID)
        {
            this.LogMethodCall(new object[] { entityID });
            if (this.simState.Entity.Count == 0)
            {
                return null;
            }
            CheckEntityExists(entityID);
            Entity entity = (Entity) this.simState.Entity[entityID];
            return entity.Player.PlayerName;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public GeneralLedger GetGL(long entityID)
        {
            CheckEntityExists(entityID);
            return ((Entity) this.simState.Entity[entityID]).GL;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int getHumanPlayerCount()
        {
            this.LogMethodCall(new object[0]);
            int num = 0;
            foreach (Entity entity in S.ST.Entity.Values)
            {
                if (entity.Player.PlayerType == PlayerType.Human)
                {
                    num++;
                }
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual float getHumanScore(string seriesName)
        {
            this.LogMethodCall(new object[] { seriesName });
            float num = 0f;
            foreach (Entity entity in S.ST.Entity.Values)
            {
                if (entity.Player.PlayerType == PlayerType.Human)
                {
                    num += entity.Journal.NumericDataSeriesLastEntry(seriesName);
                }
            }
            foreach (Entity entity in S.ST.RetiredEntity.Values)
            {
                if (entity.Player.PlayerType == PlayerType.Human)
                {
                    num += entity.Journal.NumericDataSeriesLastEntry(seriesName);
                }
            }
            return num;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool getMultiplayer()
        {
            this.LogMethodCall(new object[0]);
            return this.simState.Multiplayer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string[] GetOtherOwnedEntities(long entityID)
        {
            Entity entity = CheckEntityExists(entityID);
            ArrayList list = new ArrayList();
            foreach (Entity entity2 in S.ST.Entity.Values)
            {
                if ((entity != entity2) && (entity.Player == entity2.Player))
                {
                    list.Add(entity2.Name);
                }
            }
            return (string[]) list.ToArray(typeof(string));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public byte[] getPdfAssignment()
        {
            this.LogMethodCall(new object[0]);
            return this.simState.SimSettings.PdfAssignment;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public frmRunTo.Input GetRunTo()
        {
            return new frmRunTo.Input { runTo = S.ST.RunToDate, now = S.ST.Now };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual frmScoreboard.Input getScoreboard(bool showAIOwnedEntities)
        {
            this.LogMethodCall(new object[0]);
            frmScoreboard.Input input = new frmScoreboard.Input {
                ScoreFriendlyName = Journal.ScoreSeriesName
            };
            Hashtable hashtable = new Hashtable();
            ArrayList list = new ArrayList(S.ST.Entity.Values);
            foreach (Entity entity in list)
            {
                if (showAIOwnedEntities || !entity.AI)
                {
                    ArrayList list2 = entity.Journal.NumericDataSeries(Journal.ScoreSeriesName);
                    string playerName = entity.Player.PlayerName;
                    if (entity.AI)
                    {
                        playerName = entity.Name;
                    }
                    if (hashtable.ContainsKey(playerName))
                    {
                        ArrayList list3 = (ArrayList) hashtable[playerName];
                        for (int i = 0; i < list2.Count; i++)
                        {
                            if (i < list3.Count)
                            {
                                list3[i] = ((float) list3[i]) + ((float) list2[i]);
                            }
                            else
                            {
                                list3.Add(list2[i]);
                            }
                        }
                    }
                    else
                    {
                        hashtable.Add(playerName, list2.Clone());
                    }
                }
            }
            input.EntityNames = new string[hashtable.Count];
            hashtable.Keys.CopyTo(input.EntityNames, 0);
            input.Scores = new ArrayList[hashtable.Count];
            hashtable.Values.CopyTo(input.Scores, 0);
            return input;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public SimSettings getSimSettings()
        {
            this.LogMethodCall(new object[0]);
            return this.simState.SimSettings;
        }

        public Guid GetSimulatorID()
        {
            return S.I.GUID;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList getSurveys(string playerName)
        {
            return ((Player) S.ST.Player[playerName]).Surveys;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual ViewUpdate GetViewUpdate(string viewName, long entityID, params object[] args)
        {
            Entity entity = null;
            if (!((entityID == -1L) && S.I.SafeViewsForNoEntity.Contains(viewName)))
            {
                entity = CheckEntityExists(entityID);
            }
            ViewUpdate update = new ViewUpdate {
                Drawables = S.I.View(viewName).BuildDrawables(entityID, args),
                Now = S.ST.Now,
                CurrentWeek = S.ST.CurrentWeek
            };
            if (entity != null)
            {
                update.Cash = entity.CriticalResourceBalance();
            }
            update.EntityNames = S.ST.EntityNameTable();
            return update;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool IsUniqueEntityName(string name)
        {
            foreach (Entity entity in S.ST.Entity.Values)
            {
                if (entity.Name == name)
                {
                    return false;
                }
            }
            foreach (Entity entity in S.ST.RetiredEntity.Values)
            {
                if (entity.Name == name)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int Level()
        {
            return S.SS.Level;
        }

        protected void LogMethodCall(params object[] args)
        {
            if (S.MF.DesignerMode && !S.I.Client)
            {
                MethodBase method = new StackFrame(1).GetMethod();
                ParameterInfo[] parameters = method.GetParameters();
                S.MF.SaveMacroAction(new MacroAction(method, args, S.ST.Now));
            }
        }

        public void ModalMessageCallback(IAsyncResult ar)
        {
            ((ModalMessageDelegate) ar.AsyncState).EndInvoke(ar);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Ping()
        {
            return true;
        }

        public void PlayerMessageCallback(IAsyncResult ar)
        {
            ((PlayerMessageDelegate) ar.AsyncState).EndInvoke(ar);
        }

        public void PlaySoundCallback(IAsyncResult ar)
        {
            ((PlaySoundDelegate) ar.AsyncState).EndInvoke(ar);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void ProvideCash(long entityID, float amount)
        {
            CheckEntityExists(entityID).GL.Post("Cash", amount, "Paid-in Capital");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RenameEntity(long entityID, string newName)
        {
            CheckEntityExists(entityID).Name = newName;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool RoleBasedMultiplayer()
        {
            this.LogMethodCall(new object[0]);
            return this.simState.RoleBasedMultiplayer;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendMessage(string fromPlayerName, string toPlayerName, string message)
        {
            ((Player) S.ST.Player[toPlayerName]).SendMessage(message, fromPlayerName, NotificationColor.Blue);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void SetRunTo(DateTime date)
        {
            S.ST.RunToDate = date;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual void SetRunTo(int daysAhead)
        {
            S.ST.RunToDate = new DateTime(S.ST.Now.Year, S.ST.Now.Month, S.ST.Now.Day).AddDays((double) daysAhead);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Entity TryAddEntity(string playerName, string entityName)
        {
            foreach (Entity entity in S.ST.Entity.Values)
            {
                if (entity.Name.ToUpper() == entityName.ToUpper())
                {
                    throw new SimApplicationException(S.R.GetString("That name is already taken. Please try another."));
                }
            }
            foreach (Entity entity in S.ST.RetiredEntity.Values)
            {
                if (entity.Name.ToUpper() == entityName.ToUpper())
                {
                    throw new SimApplicationException(S.R.GetString("A previously existing {0} had that name. Please try another.", new object[] { S.I.EntityName.ToLower() }));
                }
            }
            Entity entity2 = S.I.SimFactory.CreateEntity((Player) S.ST.Player[playerName], entityName);
            S.ST.Entity.Add(entity2.ID, entity2);
            return entity2;
        }

        protected SimState simState
        {
            get
            {
                return Simulator.Instance.SimState;
            }
        }

        [Serializable]
        public class ViewUpdate
        {
            public object AppData;
            public float Cash;
            public int CurrentWeek;
            public Drawable[] Drawables;
            public Hashtable EntityNames;
            public DateTime Now;
        }
    }
}

