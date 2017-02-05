using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KMI.VBPF1Lib
{
    public partial class FrmLoanApplication : Form
    {
        public FrmLoanApplication()
        {
            InitializeComponent();
        }

        private void FrmLoanApplication_Load(object sender, EventArgs e)
        {
            lbScore.Text += A.SA.GetCreditScore(A.MF.CurrentEntityID).Score;
        }
    }
}
