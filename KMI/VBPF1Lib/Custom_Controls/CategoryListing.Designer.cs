namespace KMI.VBPF1Lib.Custom_Controls
{
	partial class CategoryListing
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.Choices = new System.Windows.Forms.ComboBox();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Choices);
            this.panel2.Controls.SetChildIndex(this.chkBuy, 0);
            this.panel2.Controls.SetChildIndex(this.Choices, 0);
            // 
            // Choices
            // 
            this.Choices.FormattingEnabled = true;
            this.Choices.Location = new System.Drawing.Point(8, 75);
            this.Choices.Name = "Choices";
            this.Choices.Size = new System.Drawing.Size(93, 21);
            this.Choices.TabIndex = 4;
            this.Choices.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // CategoryListing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "CategoryListing";
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.ComboBox Choices;
	}
}
