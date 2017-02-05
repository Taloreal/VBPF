namespace KMI.VBPF1Lib
{
    using KMI.Sim;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class frmResume : frmDrawnReport
    {
        protected static Font bankNameFont = new Font("Times New Roman", 16f, FontStyle.Italic);
        protected static Brush brush = new SolidBrush(Color.Black);
        protected static Font font = new Font("Arial", 8f);
        protected static Font fontB = new Font("Arial", 9f, FontStyle.Bold);
        protected static Font fontLB = new Font("Arial", 10f, FontStyle.Bold);
        protected static Font fontLLB = new Font("Arial", 12f, FontStyle.Bold);
        private Input input;
        protected int margin = 16;
        protected static Pen pen = new Pen(brush, 1f);
        protected static StringFormat sfc = new StringFormat();
        protected static StringFormat sfr = new StringFormat();

        public frmResume()
        {
            this.InitializeComponent();
        }

        protected override void DrawReportVirtual(Graphics g)
        {
            int num2;
            string str;
            sfc.Alignment = StringAlignment.Center;
            sfr.Alignment = StringAlignment.Far;
            int margin = this.margin;
            g.DrawString(this.input.Name, fontLLB, brush, g.ClipBounds.Width / 2f, (float) margin, sfc);
            margin += 0x23;
            g.DrawString(A.R.GetString("EDUCATION"), fontLB, brush, (float) this.margin, (float) margin);
            margin += 0x23;
            for (num2 = this.input.AcademicTaskHistory.Count - 1; num2 >= 0; num2--)
            {
                AttendClass byIndex = (AttendClass) this.input.AcademicTaskHistory.GetByIndex(num2);
                g.DrawString(byIndex.Course.Name, fontB, brush, (float) this.margin, (float) margin);
                str = byIndex.EndDate.ToString("MMM yyyy");
                g.DrawString(A.R.GetString("{0}-{1}", new object[] { byIndex.StartDate.ToString("MMM yyyy"), str }), fontB, brush, g.ClipBounds.Width - this.margin, (float) margin, sfr);
                margin += 20;
                g.DrawString(byIndex.Course.ResumeDescription, font, brush, new RectangleF((float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 1000f));
                margin += (int) g.MeasureString(byIndex.Course.ResumeDescription, font, (int) (((int) g.ClipBounds.Width) - (2 * this.margin))).Height;
                margin += 0x19;
            }
            margin += 0x23;
            g.DrawString(A.R.GetString("EXPERIENCE"), fontLB, brush, (float) this.margin, (float) margin);
            margin += 30;
            for (num2 = this.input.WorkTaskHistory.Count - 1; num2 >= 0; num2--)
            {
                WorkTask task = (WorkTask) this.input.WorkTaskHistory.GetByIndex(num2);
                g.DrawString(task.Name(), fontB, brush, (float) this.margin, (float) margin);
                str = task.EndDate.ToString("MMM yyyy");
                if (task.EndDate == DateTime.MinValue)
                {
                    str = A.R.GetString("Present");
                }
                g.DrawString(A.R.GetString("{0}-{1}", new object[] { task.StartDate.ToString("MMM yyyy"), str }), fontB, brush, g.ClipBounds.Width - this.margin, (float) margin, sfr);
                margin += 20;
                g.DrawString(task.ResumeDescription(), font, brush, new RectangleF((float) this.margin, (float) margin, g.ClipBounds.Width - (2 * this.margin), 1000f));
                margin += (int) g.MeasureString(task.ResumeDescription(), font, (int) (((int) g.ClipBounds.Width) - (2 * this.margin))).Height;
                margin += 0x19;
            }
            if (margin > base.picReport.Height)
            {
                base.picReport.Height = margin + 0x19;
            }
        }

        protected override void GetDataVirtual()
        {
            this.input = A.SA.GetResume(A.MF.CurrentEntityID);
        }

        private void InitializeComponent()
        {
            base.SuspendLayout();
            base.pnlBottom.Location = new Point(0, 0x17a);
            base.pnlBottom.Name = "pnlBottom";
            base.pnlBottom.Size = new Size(0x176, 40);
            base.picReport.BackColor = Color.White;
            base.picReport.BorderStyle = BorderStyle.FixedSingle;
            base.picReport.Location = new Point(0x1c, 0x18);
            base.picReport.Name = "picReport";
            base.picReport.Size = new Size(0x13c, 320);
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x176, 0x1a2);
            base.Name = "frmResume";
            this.Text = "Resume";
            base.ResumeLayout(false);
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public string Name;
            public SortedList WorkTaskHistory;
            public SortedList AcademicTaskHistory;
        }
    }
}

