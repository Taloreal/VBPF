namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class frm401K : Form
    {
        private Button btnCancel;
        private Button btnHelp;
        private Button btnOK;
        private Container components = null;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Input input;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label labMatch;
        private FlowLayoutPanel panFunds;
        private long taskID;
        private Label AllocatedAmount;
        private NumericUpDown updPercent;

        public frm401K(long taskID)
        {
            this.InitializeComponent();
            ArrayList funds = A.SA.GetFunds();
            this.input = A.SA.Get401K(A.MF.CurrentEntityID, taskID);
            this.taskID = taskID;
            this.updPercent.Value = ((decimal) this.input.PercentWitheld) * 100M;
            if (updPercent.Value != 0)
                AllocatedAmount.Text = "Allocated amount : 100%";
            for (int i = 0; i != funds.Count; i++)
            {
                Fund fund = ((Fund)funds[i]);
                AllocationControl control = new AllocationControl { Top = i * Height, Tag = fund };
                control.updPct.Value = ((decimal)this.input.Allocations[i]) * 100M;
                control.updPct.TextChanged += new EventHandler(ValueChanged);
                control.labFundName.Text = fund.Name;
                if ((i % 2) == 1)
                    control.BackColor = Color.LightGray;
                this.panFunds.Controls.Add(control);
            }
        }

        public void ValueChanged(object sender, EventArgs e)
        {
            decimal Count = 0M;
            CountInvestments(out Count);
            AllocatedAmount.Text = "Allocated amount : " + Count.ToString();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("View Retirement Portfolio"));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            decimal Counted = 0M;
            float[] allocations = CountInvestments(out Counted);
            if (this.updPercent.Value > 0M && Counted != 100M)
            {
                string str = "less";
                if (Counted > 100M)
                    str = "more";
                MessageBox.Show(A.R.GetString("Your investment allocation percentages add to {0} than 100%. Please try again.", new object[] { str }), "Try Again");
                return;
            }
            A.SA.Set401K(A.MF.CurrentEntityID, this.taskID, ((float) this.updPercent.Value) / 100f, allocations);
            base.Close();
        }

        public float[] CountInvestments(out decimal Total)
        {
            Total = 0M;
            int num2 = 0;
            float[] allocations = new float[this.panFunds.Controls.Count];
            foreach (AllocationControl control in this.panFunds.Controls)
            {
                Total += control.updPct.Value;
                allocations[num2++] = ((float)control.updPct.Value) / 100f;
            }
            return allocations;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.updPercent = new System.Windows.Forms.NumericUpDown();
            this.labMatch = new System.Windows.Forms.Label();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panFunds = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.AllocatedAmount = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updPercent)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(20, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Percent of Pay to Withhold:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(20, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(136, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Company Match:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.updPercent);
            this.groupBox1.Controls.Add(this.labMatch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(20, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(268, 92);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Contribution Rate";
            // 
            // updPercent
            // 
            this.updPercent.Location = new System.Drawing.Point(172, 24);
            this.updPercent.Name = "updPercent";
            this.updPercent.Size = new System.Drawing.Size(44, 20);
            this.updPercent.TabIndex = 4;
            this.updPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.updPercent.ValueChanged += new System.EventHandler(this.updPercent_ValueChanged);
            // 
            // labMatch
            // 
            this.labMatch.Location = new System.Drawing.Point(168, 56);
            this.labMatch.Name = "labMatch";
            this.labMatch.Size = new System.Drawing.Size(40, 16);
            this.labMatch.TabIndex = 3;
            this.labMatch.Text = "0%";
            this.labMatch.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(212, 300);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(72, 24);
            this.btnHelp.TabIndex = 25;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(122, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 24);
            this.btnCancel.TabIndex = 24;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(24, 300);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 24);
            this.btnOK.TabIndex = 23;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panFunds);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(20, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(268, 156);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Investment Allocation";
            // 
            // panFunds
            // 
            this.panFunds.AutoScroll = true;
            this.panFunds.Location = new System.Drawing.Point(4, 32);
            this.panFunds.Name = "panFunds";
            this.panFunds.Size = new System.Drawing.Size(260, 120);
            this.panFunds.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(52, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "Must add to 100 percent.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // AllocatedAmount
            // 
            this.AllocatedAmount.Location = new System.Drawing.Point(12, 271);
            this.AllocatedAmount.Name = "AllocatedAmount";
            this.AllocatedAmount.Size = new System.Drawing.Size(144, 20);
            this.AllocatedAmount.TabIndex = 2;
            this.AllocatedAmount.Text = "Allocated amount : 0%";
            this.AllocatedAmount.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // frm401K
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(308, 336);
            this.Controls.Add(this.AllocatedAmount);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frm401K";
            this.ShowInTaskbar = false;
            this.Text = "401K Retirement Savings Elections";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.updPercent)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void updPercent_ValueChanged(object sender, EventArgs e)
        {
            this.labMatch.Text = Utilities.FP((float) Math.Min(this.updPercent.Value / 100M, (decimal) this.input.CompanyMatch));
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public float[] Allocations;
            public float PercentWitheld;
            public float CompanyMatch;
        }
    }
}

