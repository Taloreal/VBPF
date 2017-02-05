namespace KMI.Sim
{
    using System;

    public class S
    {
        public static Simulator I
        {
            get
            {
                return Simulator.Instance;
            }
        }

        public static frmMainBase MF
        {
            get
            {
                return frmMainBase.Instance;
            }
        }

        public static Resource R
        {
            get
            {
                return Simulator.Instance.Resource;
            }
        }

        public static SimStateAdapter SA
        {
            get
            {
                return Simulator.Instance.SimStateAdapter;
            }
        }

        public static SimSettings SS
        {
            get
            {
                return Simulator.Instance.SimState.SimSettings;
            }
        }

        public static SimState ST
        {
            get
            {
                return Simulator.Instance.SimState;
            }
        }
    }
}

