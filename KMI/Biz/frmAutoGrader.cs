namespace KMI.Biz
{
    using KMI.Sim;
    using KMI.Utility;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmAutoGrader : frmDrawnReport
    {
        public static float ExtraPointsPerEntity = 0.05f;
        public static int margin = 16;
        public static string Notes = "Note: Use Actions->Consultant for critique of each topic.";
        private ConsultantReport[] reports = null;

        public frmAutoGrader()
        {
            this.InitializeComponent();
            base.picReport.Parent.BackColor = Color.White;
        }

        protected override void DrawReportVirtual(Graphics g)
        {
            int num7;
            int num8;
            int num10;
            int num = 1;
            if (this.reports.Length > 1)
            {
                num = this.reports.Length + 1;
            }
            int count = this.reports[0].Sections.Count;
            int num3 = 220;
            int num4 = 90;
            int num5 = 90;
            int num6 = 30;
            Brush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush, 1f);
            Pen pen2 = new Pen(new SolidBrush(Color.DarkGray), 1f);
            Font font = new Font("Arial", 14f);
            Font font2 = new Font("Arial", 10f);
            StringFormat format = new StringFormat {
                Alignment = StringAlignment.Far
            };
            for (num7 = 0; num7 < (count + 1); num7++)
            {
                num8 = (margin + num5) + (num7 * num6);
                g.DrawLine(pen, new Point(margin, num8), new Point((margin + num3) + (num * num4), num8));
                if (num7 < count)
                {
                    g.DrawString(((ConsultantReportSection) this.reports[0].Sections[num7]).Topic, font, brush, (float) margin, (float) (num8 + 5));
                }
            }
            g.DrawLine(pen, new Point(margin, margin + num5), new Point(margin, (margin + num5) + (num6 * count)));
            int index = 0;
            while (index < (num + 1))
            {
                num10 = (margin + num3) + (index * num4);
                g.DrawLine(pen, new Point(num10, margin + num5), new Point(num10, (margin + num5) + (num6 * count)));
                if (index < num)
                {
                    g.DrawLine(pen2, new Point(num10 + (num4 / 2), margin + num5), new Point(num10 + (num4 / 2), (margin + num5) + (num6 * count)));
                    string s = "Average";
                    if ((index < (num - 1)) || ((index == 0) && (num == 1)))
                    {
                        s = this.reports[index].EntityName;
                    }
                    g.TranslateTransform((float) (num10 + (num4 / 2)), (float) ((margin + num5) - 20));
                    g.RotateTransform(-58f);
                    g.DrawString(s, font, brush, (float) 0f, (float) 0f);
                    g.RotateTransform(58f);
                    g.TranslateTransform((float) -(num10 + (num4 / 2)), (float) -((margin + num5) - 20));
                }
                g.DrawLine(pen, new Point(num10, margin + num5), new Point(num10 + ((2 * num4) / 3), margin));
                index++;
            }
            for (num7 = 0; num7 < count; num7++)
            {
                float num11 = 0f;
                num8 = ((margin + num5) + (num7 * num6)) + 7;
                for (index = 0; index < num; index++)
                {
                    float grade;
                    string str2 = "";
                    if ((index < (num - 1)) || ((index == 0) && (num == 1)))
                    {
                        grade = ((ConsultantReportSection) this.reports[index].Sections[num7]).Grade;
                        num11 += grade / ((float) (num - 1));
                    }
                    else
                    {
                        grade = num11;
                        if ((num7 == (count - 1)) && (num > 1))
                        {
                            grade = Math.Min((float) 1f, (float) (grade + (ExtraPointsPerEntity * (num - 2))));
                            g.DrawString(S.R.GetString("* Includes {0} points for each additional {1} managed.", new object[] { (int) (ExtraPointsPerEntity * 100f), S.I.EntityName.ToLower() }), font2, brush, (float) margin, (float) (num8 + 30));
                            str2 = "*";
                        }
                        if (num7 == (count - 1))
                        {
                            g.DrawString(Notes, font2, brush, (float) margin, (float) (num8 + 0x2d));
                        }
                    }
                    if ((index != 1) || (num != 1))
                    {
                        num10 = (((margin + num3) + (index * num4)) + (num4 / 2)) - 2;
                        g.DrawString(Utilities.FP(grade) + str2, font2, brush, (float) num10, (float) num8, format);
                        g.DrawString(this.LetterGrade(grade), font2, brush, (float) ((num10 + (num4 / 2)) - 12), (float) num8, format);
                    }
                }
            }
        }

        protected override void GetDataVirtual()
        {
            this.reports = ((BizStateAdapter) Simulator.Instance.SimStateAdapter).GetGrades(frmMainBase.Instance.CurrentEntityID);
        }

        private void InitializeComponent()
        {
            ((ISupportInitialize) base.picReport).BeginInit();
            base.SuspendLayout();
            base.pnlBottom.Location = new Point(0, 480);
            base.pnlBottom.Size = new Size(0x2a2, 40);
            base.picReport.BackColor = Color.White;
            base.picReport.Dock = DockStyle.Fill;
            base.picReport.Location = new Point(0, 0);
            base.picReport.Size = new Size(0x2a2, 520);
            this.AutoScaleBaseSize = new Size(5, 13);
            base.ClientSize = new Size(0x2a2, 520);
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            base.Name = "frmAutoGrader";
            this.Text = "AutoGrader";
            ((ISupportInitialize) base.picReport).EndInit();
            base.ResumeLayout(false);
        }

        string Grades = "FFFFFFDCBAA";
        private string LetterGrade(float f)
        {
            return Grades[(int)(f * (float)10)].ToString();

            //Obsolete Code 5/17/2016 04:52
            if (f < 0.6) 
                return "F"; 
            if (f < 0.7) 
                return "D"; 
            if (f < 0.8) 
                return "C"; 
            if (f < 0.9) 
                return "B"; 
            return "A";
        }
    }
}

