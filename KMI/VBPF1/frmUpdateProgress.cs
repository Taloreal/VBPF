using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace KMI.VBPF1
{
	public partial class frmUpdateProgress: Form
	{
		public frmUpdateProgress()
		{
			InitializeComponent();
		}

        public void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Text = "Updating... " + e.ProgressPercentage + "%";
            progressBar1.Value = e.ProgressPercentage;
        }

        public void ULProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            this.Text = "Updating... " + e.ProgressPercentage + "%";
            progressBar1.Value = e.ProgressPercentage;
        }
	}
}
