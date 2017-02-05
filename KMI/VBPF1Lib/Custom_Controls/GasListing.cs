using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using KMI.Utility;
using KMI.VBPF1Lib;

namespace KMI.VBPF1Lib.Custom_Controls
{
    public partial class GasListing : UserControl
    {
        public int ToBuy = 1;
        public GasListing() {
            InitializeComponent();
            Settings.GetValue<int>("GasCount", out ToBuy);
            Amount.Text = ToBuy.ToString();
            labImage.Image = A.R.GetImage("GasCan2");
        }

        private void Amount_TextChanged(object sender, EventArgs e) {
            try {
                ToBuy = Convert.ToInt32(Amount.Text);
                Settings.SetValue<int>("GasCount", ToBuy);
                chkBuy.Checked = true;
            }
            catch { Amount.Text = ToBuy.ToString(); return; }
        }
    }
}
