namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmPayBills : Form
    {
        private SortedList bills;
        private Button btnAccept;
        private Button btnBack;
        private Button btnCancel;
        private Button btnHelp;
        private Button btnNext;
        private Container components = null;
        private int currentIndex;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label labNoOffers;
        private Label labPage;
        private Panel panBills;
        private PictureBox picBill;
        protected int TransactionsPerPage = 0x18;
        private NumericUpDown updPage;

        public frmPayBills()
        {
            this.InitializeComponent();
            this.RefreshData();
            this.currentIndex = 0;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                new frmPayBy(this.currentBill).ShowDialog(this);
                A.MF.UpdateView();
                this.RefreshData();
                this.picBill.Refresh();
            }
            catch (Exception exception)
            {
                frmExceptionHandler.Handle(exception, this);
            }
            if (bills.Count == 0)
                this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.currentIndex--;
            this.picBill.Refresh();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(this.Text);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            this.currentIndex++;
            this.picBill.Refresh();
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.picBill = new System.Windows.Forms.PictureBox();
            this.labNoOffers = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panBills = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labPage = new System.Windows.Forms.Label();
            this.updPage = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.picBill)).BeginInit();
            this.panBills.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updPage)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(392, 88);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(96, 24);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Pay";
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(392, 184);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 24);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(392, 232);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(96, 24);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnBack
            // 
            this.btnBack.Location = new System.Drawing.Point(120, 528);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(48, 24);
            this.btnBack.TabIndex = 4;
            this.btnBack.Text = "<<";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(184, 528);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(48, 24);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = ">>";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // picBill
            // 
            this.picBill.BackColor = System.Drawing.Color.White;
            this.picBill.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picBill.Location = new System.Drawing.Point(16, 20);
            this.picBill.Name = "picBill";
            this.picBill.Size = new System.Drawing.Size(337, 484);
            this.picBill.TabIndex = 6;
            this.picBill.TabStop = false;
            this.picBill.Visible = false;
            this.picBill.Paint += new System.Windows.Forms.PaintEventHandler(this.picBill_Paint);
            // 
            // labNoOffers
            // 
            this.labNoOffers.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labNoOffers.ForeColor = System.Drawing.Color.Gray;
            this.labNoOffers.Location = new System.Drawing.Point(24, 136);
            this.labNoOffers.Name = "labNoOffers";
            this.labNoOffers.Size = new System.Drawing.Size(264, 120);
            this.labNoOffers.TabIndex = 7;
            this.labNoOffers.Text = "There are no more bills.";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.White;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(20, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(337, 484);
            this.label2.TabIndex = 8;
            this.label2.Visible = false;
            // 
            // panBills
            // 
            this.panBills.Controls.Add(this.picBill);
            this.panBills.Controls.Add(this.label2);
            this.panBills.Controls.Add(this.label3);
            this.panBills.Controls.Add(this.label4);
            this.panBills.Controls.Add(this.label5);
            this.panBills.Location = new System.Drawing.Point(0, 8);
            this.panBills.Name = "panBills";
            this.panBills.Size = new System.Drawing.Size(368, 504);
            this.panBills.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Location = new System.Drawing.Point(24, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(337, 484);
            this.label3.TabIndex = 9;
            this.label3.Visible = false;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Location = new System.Drawing.Point(28, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(337, 484);
            this.label4.TabIndex = 10;
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(32, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(337, 484);
            this.label5.TabIndex = 11;
            this.label5.Visible = false;
            // 
            // labPage
            // 
            this.labPage.Location = new System.Drawing.Point(392, 24);
            this.labPage.Name = "labPage";
            this.labPage.Size = new System.Drawing.Size(40, 16);
            this.labPage.TabIndex = 10;
            this.labPage.Text = "Page:";
            // 
            // updPage
            // 
            this.updPage.Location = new System.Drawing.Point(392, 40);
            this.updPage.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updPage.Name = "updPage";
            this.updPage.Size = new System.Drawing.Size(40, 20);
            this.updPage.TabIndex = 11;
            this.updPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.updPage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updPage.ValueChanged += new System.EventHandler(this.updPage_ValueChanged);
            // 
            // frmPayBills
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(514, 560);
            this.Controls.Add(this.updPage);
            this.Controls.Add(this.labPage);
            this.Controls.Add(this.panBills);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.labNoOffers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmPayBills";
            this.ShowInTaskbar = false;
            this.Text = "Pay Bills";
            ((System.ComponentModel.ISupportInitialize)(this.picBill)).EndInit();
            this.panBills.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.updPage)).EndInit();
            this.ResumeLayout(false);

        }

        private void picBill_Paint(object sender, PaintEventArgs e)
        {
            if (((this.bills.Count > 0) && (this.currentIndex >= 0)) && (this.currentIndex < this.bills.Count))
            {
                if ((this.currentBill.Account is CreditCardAccount) || (this.currentBill.Account is InstallmentLoan))
                {
                    int count = this.currentBill.Account.TransactionsForMonth(this.currentBill.Date.Year, this.currentBill.Date.Month).Count;
                    int num2 = (int) Math.Max(1.0, Math.Ceiling((double) (((float) count) / ((float) this.TransactionsPerPage))));
                    this.updPage.Value = Math.Min(this.updPage.Value, num2);
                    this.updPage.Maximum = num2;
                    this.updPage.Visible = true;
                    this.labPage.Visible = true;
                    this.currentBill.Account.PrintPage(((int) this.updPage.Value) - 1, e.Graphics, this.currentBill.Date.Year, this.currentBill.Date.Month, num2, this.TransactionsPerPage);
                }
                else
                {
                    this.currentBill.PrintPage(e.Graphics);
                    this.updPage.Visible = false;
                    this.labPage.Visible = false;
                }
            }
            else if (this.bills.Count == 0)
            {
                this.btnAccept.Enabled = false;
            }
            this.btnBack.Enabled = this.currentIndex > 0;
            this.btnNext.Enabled = this.currentIndex < (this.bills.Count - 1);
        }

        protected void RefreshData()
        {
            this.bills = A.SA.GetBills(A.MF.CurrentEntityID);
            this.currentIndex = Math.Min(this.currentIndex, this.bills.Count - 1);
            this.panBills.Visible = this.bills.Count > 0;
            this.btnAccept.Enabled = this.bills.Count > 0;
            for (int i = 0; i < this.panBills.Controls.Count; i++)
            {
                this.panBills.Controls[i].Visible = i < this.bills.Count;
            }
        }

        private void updPage_ValueChanged(object sender, EventArgs e)
        {
            this.picBill.Refresh();
        }

        public Bill currentBill
        {
            get
            {
                return (Bill) this.bills.GetByIndex(this.currentIndex);
            }
        }
    }
}

