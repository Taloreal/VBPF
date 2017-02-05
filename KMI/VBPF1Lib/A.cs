namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;

    public class A
    {
        public static Simulator I
        {
            get
            {
                return Simulator.Instance;
            }
        }

        public static frmMain MF
        {
            get
            {
                return (frmMain) frmMainBase.Instance;
            }
        }

        public static Resource R
        {
            get
            {
                return Simulator.Instance.Resource;
            }
        }

        public static AppStateAdapter SA
        {
            get
            {
                return (AppStateAdapter) Simulator.Instance.SimStateAdapter;
            }
        }

        public static AppSimSettings SS
        {
            get
            {
                return (AppSimSettings) Simulator.Instance.SimState.SimSettings;
            }
        }

        public static AppSimState ST
        {
            get
            {
                return (AppSimState) Simulator.Instance.SimState;
            }
        }
    }
}

