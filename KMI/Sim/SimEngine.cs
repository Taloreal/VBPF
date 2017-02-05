namespace KMI.Sim
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public class SimEngine
    {
        private int count = 0;
        protected Thread mainThread;
        private object pauseLock = new object();
        protected bool running;
        protected Simulator simulator = Simulator.Instance;
        protected int skip;
        protected int stepPeriod;
        public bool stopEngine = false;

        public void Draw()
        {
            S.MF.UpdateView();
        }

        private void MainLoop()
        {
            while (!this.stopEngine)
            {
                lock (this.pauseLock)
                {
                    if (!this.running)
                    {
                        Monitor.Wait(this.pauseLock);
                    }
                }
                S.MF.PlayMacroAction();
                int tickCount = Environment.TickCount;
                lock (S.SA)
                {
                    if (!S.I.Client)
                    {
                        this.ProcessTick();
                    }
                }
                int millisecondsTimeout = (tickCount + this.stepPeriod) - Environment.TickCount;
                if (millisecondsTimeout > 0)
                {
                    Thread.Sleep(millisecondsTimeout);
                }
                if ((this.skip <= 0) || ((this.count++ % this.skip) == 0))
                {
                    try { S.MF.Invoke(new NoArgDelegate(this.Draw)); }
                    catch { stopEngine = true; }
                }
            }
            this.mainThread = null;
        }

        public void PauseThread()
        {
            this.running = false;
        }

        public void ProcessTick()
        {
            try
            {
                if ((S.ST.Now >= S.SS.StopDate) && (S.SS.StopDate != DateTime.MinValue))
                {
                    Player player = (Player) S.ST.Player[S.I.ThisPlayerName];
                    if (S.I.Demo)
                    {
                        S.MF.DirtySimState = false;
                        string message = S.R.GetString("Simulations in this demo version are limited to {0} simulated days. In the classroom version, simulations can run indefinitely.", new object[] { S.I.DemoDuration });
                        this.running = false;
                        player.SendModalMessage(new GameOverMessage(S.I.ThisPlayerName, message));
                    }
                    else if (S.SS.StudentOrg > 0)
                    {
                        this.running = false;
                        player.SendModalMessage(new StopDateReachedMessage());
                    }
                } //TAG: RUN TO OVER
                else if (S.ST.Now >= S.ST.RunToDate)
                {
                    this.running = false;
                    S.ST.RunToDate = DateTime.MaxValue;
                    ((Player) S.ST.Player[S.I.ThisPlayerName]).SendModalMessage(new RunToDateReachedMessage());
                }
                else
                {
                    SimState simState = Simulator.Instance.SimState;
                    simState.Step();
                    simState.UpdateEventQueue();
                    foreach (Simulator.TimePeriod period in Simulator.FiringOrder)
                    {
                        if (period == Simulator.TimePeriod.Step)
                        {
                            simState.NewTimePeriod(period);
                        }
                        else if ((period == Simulator.TimePeriod.Hour) && simState.NewHour)
                        {
                            simState.NewTimePeriod(period);
                        }
                        else if ((period == Simulator.TimePeriod.Day) && simState.NewDay)
                        {
                            simState.NewTimePeriod(period);
                        }
                        else if ((period == Simulator.TimePeriod.Week) && simState.NewWeek)
                        {
                            simState.NewTimePeriod(period);
                        }
                        else if ((period == Simulator.TimePeriod.Year) && simState.NewYear)
                        {
                            simState.NewTimePeriod(period);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception);
            }
        }

        public void ResumeThread()
        {
            lock (this.pauseLock)
            {
                if (this.mainThread == null)
                {
                    this.mainThread = new Thread(new ThreadStart(this.MainLoop));
                    this.mainThread.IsBackground = true;
                    this.mainThread.Priority = ThreadPriority.Lowest;
                    this.stopEngine = false;
                    this.running = true;
                    this.mainThread.Start();
                }
                else
                {
                    this.running = true;
                    Monitor.Pulse(this.pauseLock);
                }
            }
        }

        public bool Running
        {
            get
            {
                return this.running;
            }
        }

        public int Skip
        {
            set
            {
                this.skip = value;
            }
        }

        public int StepPeriod
        {
            set
            {
                this.stepPeriod = value;
            }
        }

        public delegate void NoArgDelegate();
    }
}

