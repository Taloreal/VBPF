namespace KMI.VBPF1Lib
{
    using KMI.Utility;
    using KMI.VBPF1Lib.Custom_Controls;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmShop : Form
    {
        #region Controls
        private Button btnHelp;
        private Button btnOK;
        private Button button2;
        private bool Car = false;
        private Container components = null;
        private Panel panel2;
        private Panel panListings2;
        private ArrayList PurchasableItems;
        private string sellerName;
        private FlowLayoutPanel panListings;
        #endregion

        #region Variables
        public int ActiveIndex = -1;
        double QuantityPrice = 0.0;
        public List<UserControl> QuantityListings = new List<UserControl> { new FoodListing(), new GasListing(), new BusListing()};
        public List<string> QuantityProducts = new List<string> { " bags of food", " gals of gas", " tokens" };
        public List<string> SellerNames = new List<string> { "SuperMarket, Inc.", "Gas & Repairs, Inc.", "City Bus" };
        public List<string> Ignore = new List<string> { "BagOfFood0", "GasCan0", "BusTokens0" };
        public List<int> PriceCalcs = new List<int> { 14, 10, 20 };
        public List<string> SimilarGroups = new List<string>();
        public List<List<PurchasableItem>> Categorized = new List<List<PurchasableItem>>();
        #endregion

        public frmShop(string sellerName, ArrayList purchasableItems, bool car)
        {
            this.InitializeComponent();
            this.sellerName = sellerName;
            this.Car = car;
            int num = 0;
            List<PurchasableItem> PIs = new List<PurchasableItem>(
                (PurchasableItem[])purchasableItems.ToArray(typeof(PurchasableItem))
            );
            AddTypes(ref PIs, out PurchasableItems);
            int ind = ActiveIgnore();
            if (ind != -1)
                QuantityPrice = GetPrice(ind);
            CreateCategoryListings(ref num);
            foreach (PurchasableItem item in this.PurchasableItems) {
                ItemListing listing = new ItemListing(item) { Top = num * Height };
                if ((num % 2) == 1)
                    listing.BackColor = Color.LightGray;
                this.panListings.Controls.Add(listing);
                num++;
            }
            if (ind != -1)
                AddQuantityListing(ind, PriceCalcs[ind], Ignore[ind].Substring(0, Ignore[ind].Length - 1));
        }

        public int ActiveIgnore()
        {
            int ig = -1;
            for (int i = 0; i != 3; i++)
                if (sellerName == SellerNames[i])
                    ig = i;
            return ig;
        }

        public double GetPrice(int Iggy)
        {
            double price = 0.0;
            foreach (PurchasableItem PI in PurchasableItems)
                if (PI.ImageName == Ignore[Iggy])
                    price = Math.Round(PI.Price / PriceCalcs[Iggy], 2);
            return price;
        }

        public void CreateCategoryListings(ref int num)
        {
            foreach (List<PurchasableItem> LPI in Categorized)
            {
                CategoryListing listing = new CategoryListing(LPI) {
                    Top = num * Height
                };
                if ((num % 2) == 1)
                    listing.BackColor = Color.LightGray;
                this.panListings.Controls.Add(listing);
                num++;
            }
        }

        public void AddTypes(ref List<PurchasableItem> AvailableItems, out ArrayList LeftOvers)
        {
            LeftOvers = new ArrayList();
            while (AvailableItems.Count != 0)
            {
                int Num = 0;
                if (Ignore.Contains(AvailableItems[0].ImageName))
                {
                    LeftOvers.Add(AvailableItems[0]);
                    AvailableItems.RemoveAt(0);
                }
                string Category = AvailableItems[0].ImageName.Substring(0, AvailableItems[0].ImageName.Length - 1);
                PurchasableItem P = new PurchasableItem();
                RemoveAllWith(ref AvailableItems, Category, out Num, out P);
                if (Num > 1)
                    SimilarGroups.Add(Category);
                else
                    LeftOvers.Add(P);
            }
        }

        public void RemoveAllWith(ref List<PurchasableItem> List, string Removal, out int Count, out PurchasableItem LeftOver)
        {
            LeftOver = new PurchasableItem();
            List<int> ToRemove = new List<int>();
            List<PurchasableItem> ToAdd = new List<PurchasableItem>();
            for (int i = 0; i != List.Count; i++)
                if (List[i].ImageName.Contains(Removal))
                    ToRemove.Add(i);
            ToRemove.Reverse();
            for (int i = 0; i != ToRemove.Count; i++)
            {
                ToAdd.Add(List[ToRemove[i]]);
                List.RemoveAt(ToRemove[i]);
            }
            if (ToAdd.Count > 1)
                Categorized.Add(ToAdd);
            else
                LeftOver = ToAdd[0];
            Count = ToRemove.Count;
        }

        public void AddQuantityListing(int ListingIndex, int EquatePrice, string RemoveName)
        {
            ActiveIndex = ListingIndex;
            this.panListings.Location = new System.Drawing.Point(0, 105);
            this.panListings.Size = new System.Drawing.Size(520, 229);
            this.Controls.Add(QuantityListings[ListingIndex]);
            QuantityListings[ListingIndex].Location = new Point(0, 0);
            QuantityListings[ListingIndex].Visible = true;
            QuantityListings[ListingIndex].Controls[1].Controls[4].Text =
                "$" + QuantityPrice.ToString() + QuantityListings[ListingIndex].Controls[1].Controls[4].Text;
            for (int i = 0; i != panListings.Controls.Count; i++)
                if (((ItemListing)panListings.Controls[i]).purchasableItem.ImageName.Contains(RemoveName))
                {
                    panListings.Controls.RemoveAt(i);
                    i--;
                }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp(A.R.GetString("Shop For Goods"));
        }

        public PurchasableItem Purchase(out float Cost, out string called)
        {
            PurchasableItem Item = new PurchasableItem();
            int amount = Convert.ToInt32(QuantityListings[ActiveIndex].Controls[1].Controls[0].Text);
            Item.Name = amount.ToString() + QuantityProducts[ActiveIndex];
            called = amount.ToString() + QuantityProducts[ActiveIndex] + ", ";
            Item.Price = (float)(QuantityPrice * (double)amount);
            Cost = (float)(QuantityPrice * (double)amount);
            return Item;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ArrayList items = new ArrayList();
            string s = "";
            float amount = 0f;
            if (ActiveIndex != -1) {
                if (((CheckBox)QuantityListings[ActiveIndex].Controls[1].Controls[2]).Checked)
                    items.Add(Purchase(out amount, out s).Name);
            }
            foreach (ItemListing listing in this.panListings.Controls) {
                if (listing.chkBuy.Checked) {
                    items.Add(listing.purchasableItem.Name);
                    s = s + listing.labName.Text + ", ";
                    amount += listing.purchasableItem.Price;
                }
            }
            if (items.Count > 0) {
                if (!this.CheckSimilarPurchase(items)) {
                    s = Utilities.FormatCommaSeries(s);
                    Bill bill = A.SA.CreateBill(this.sellerName, "", amount, null);
                    Form form = null;
                    if (this.Car) { form = new frmPayForCar(bill, items); }
                    else { form = new frmPayBy(bill, items); }
                    form.ShowDialog(this);
                    base.Close();
                }
            }
            else {
                MessageBox.Show(A.R.GetString("Please select one or more items to buy."), A.R.GetString("Input Required"));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected bool CheckSimilarPurchase(ArrayList items)
        {
            ArrayList list = new ArrayList();
            foreach (string str in items) {
                if (!str.StartsWith("Art") && !str.StartsWith("Platter")) {
                    string item = str.Substring(0, str.Length - 1);
                    if (list.Contains(item)) {
                        string str3 = "apartment";
                        if (item == "Car") { str3 = "garage"; }
                        MessageBox.Show(A.R.GetString("You are trying to purchase more than one {0}. There is room for only one in your {1}. Please modify your purchases.", new object[] { item, str3 }));
                        return true;
                    }
                    list.Add(item);
                }
            }
            return false;
        }

        #region Designer
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHelp = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panListings2 = new System.Windows.Forms.Panel();
            this.panListings = new System.Windows.Forms.FlowLayoutPanel();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnHelp);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 334);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(520, 60);
            this.panel2.TabIndex = 1;
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(356, 20);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(96, 24);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(212, 20);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 24);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(28, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(116, 44);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Checkout && Pay";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panListings2
            // 
            this.panListings2.AutoScroll = true;
            this.panListings2.Location = new System.Drawing.Point(0, 207);
            this.panListings2.Margin = new System.Windows.Forms.Padding(0);
            this.panListings2.Name = "panListings2";
            this.panListings2.Size = new System.Drawing.Size(520, 127);
            this.panListings2.TabIndex = 3;
            // 
            // panListings
            // 
            this.panListings.AutoScroll = true;
            this.panListings.Location = new System.Drawing.Point(0, 0);
            this.panListings.Margin = new System.Windows.Forms.Padding(0);
            this.panListings.Name = "panListings";
            this.panListings.Size = new System.Drawing.Size(520, 331);
            this.panListings.TabIndex = 2;
            // 
            // frmShop
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(520, 394);
            this.Controls.Add(this.panListings);
            this.Controls.Add(this.panListings2);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmShop";
            this.ShowInTaskbar = false;
            this.Text = "Shop";
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
    }
}

