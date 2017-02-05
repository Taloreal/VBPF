using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using KMI.Utility;

namespace KMI.VBPF1Lib.Custom_Controls
{
	public partial class CategoryListing : ItemListing
	{
        List<ItemListing> ControlSettings = new List<ItemListing>();
        List<PurchasableItem> CategoryItems = new List<PurchasableItem>();
        int WorkingIndex = 0;

		public CategoryListing(List<PurchasableItem> PIs)
		{
            PIs.Reverse();
            InitializeComponent();
            if (PIs.Count == 0)
                return;
            CategoryItems = PIs;
            for (int i = 0; i != PIs.Count; i++)
            {
                ControlSettings.Add(new ItemListing(PIs[i]));
                Choices.Items.Add("Type: " + (i + 1).ToString());
            }
            this.purchasableItem = PIs[0];
            Choices.SelectedIndex = 0;
            UpdateInterface();
		}

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            WorkingIndex = Choices.SelectedIndex;
            purchasableItem = CategoryItems[WorkingIndex];
            UpdateInterface();
        }

        private void UpdateInterface()
        {
            this.labDescription.Text = purchasableItem.Description;
            this.labImage.Image = A.R.GetImage(purchasableItem.ImageName);
            this.labName.Text = purchasableItem.FriendlyName;
            this.labPrice.Text = Utilities.FC(purchasableItem.Price, A.I.CurrencyConversion);
            if (purchasableItem.saleDiscount > 0f)
            {
                this.labPrice.ForeColor = Color.Red;
                this.labOnSale.Visible = true;
            }
        }
	}
}
