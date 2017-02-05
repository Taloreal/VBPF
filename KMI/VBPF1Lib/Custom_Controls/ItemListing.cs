namespace KMI.VBPF1Lib.Custom_Controls
{
    using KMI.Utility;
    using KMI.VBPF1Lib;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class ItemListing : UserControl
    {
        public CheckBox chkBuy;
        protected Container components;
        protected Label labDescription;
        protected Label label4;
        protected Label labImage;
        public Label labName;
        protected Label labOnSale;
        protected Label labPrice;
        public Panel panel1;
        public Panel panel2;
        public Panel panel3;
        public PurchasableItem purchasableItem;

        public ItemListing()
        {
            this.components = null;
            this.InitializeComponent();
        }

        public ItemListing(PurchasableItem p)
        {
            this.components = null;
            this.InitializeComponent();
            this.labDescription.Text = p.Description;
            this.labImage.Image = A.R.GetImage(p.ImageName);
            this.labName.Text = p.FriendlyName;
            this.labPrice.Text = Utilities.FC(p.Price, A.I.CurrencyConversion);
            if (p.saleDiscount > 0f)
            {
                this.labPrice.ForeColor = Color.Red;
                this.labOnSale.Visible = true;
            }
            this.purchasableItem = p;
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.labImage = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labOnSale = new System.Windows.Forms.Label();
            this.chkBuy = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labPrice = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labName = new System.Windows.Forms.Label();
            this.labDescription = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labImage);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(168, 104);
            this.panel1.TabIndex = 0;
            // 
            // labImage
            // 
            this.labImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labImage.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labImage.Location = new System.Drawing.Point(0, 0);
            this.labImage.Name = "labImage";
            this.labImage.Size = new System.Drawing.Size(168, 104);
            this.labImage.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labOnSale);
            this.panel2.Controls.Add(this.chkBuy);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.labPrice);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(392, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(15);
            this.panel2.Size = new System.Drawing.Size(104, 104);
            this.panel2.TabIndex = 1;
            // 
            // labOnSale
            // 
            this.labOnSale.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labOnSale.ForeColor = System.Drawing.Color.Red;
            this.labOnSale.Location = new System.Drawing.Point(12, 36);
            this.labOnSale.Name = "labOnSale";
            this.labOnSale.Size = new System.Drawing.Size(74, 16);
            this.labOnSale.TabIndex = 3;
            this.labOnSale.Text = "On Sale!";
            this.labOnSale.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.labOnSale.Visible = false;
            // 
            // chkBuy
            // 
            this.chkBuy.Location = new System.Drawing.Point(72, 56);
            this.chkBuy.Name = "chkBuy";
            this.chkBuy.Size = new System.Drawing.Size(16, 20);
            this.chkBuy.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 16);
            this.label4.TabIndex = 1;
            this.label4.Text = "Buy It!";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labPrice
            // 
            this.labPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labPrice.Location = new System.Drawing.Point(4, 15);
            this.labPrice.Name = "labPrice";
            this.labPrice.Size = new System.Drawing.Size(84, 16);
            this.labPrice.TabIndex = 0;
            this.labPrice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labName);
            this.panel3.Controls.Add(this.labDescription);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(168, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(15);
            this.panel3.Size = new System.Drawing.Size(224, 104);
            this.panel3.TabIndex = 2;
            // 
            // labName
            // 
            this.labName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labName.Location = new System.Drawing.Point(8, 4);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(208, 28);
            this.labName.TabIndex = 1;
            // 
            // labDescription
            // 
            this.labDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labDescription.Location = new System.Drawing.Point(8, 36);
            this.labDescription.Name = "labDescription";
            this.labDescription.Size = new System.Drawing.Size(204, 56);
            this.labDescription.TabIndex = 0;
            // 
            // ItemListing
            // 
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "ItemListing";
            this.Size = new System.Drawing.Size(496, 104);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}

