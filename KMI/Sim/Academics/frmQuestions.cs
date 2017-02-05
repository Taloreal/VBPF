namespace KMI.Sim.Academics
{
    using KMI.Utility;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class frmQuestions : Form
    {
        private Button btnContinue;
        private Button btnSubmit;
        private Container components = null;
        private Label labScore;
        public Modes Mode = Modes.Quiz;
        private Panel panel1;
        private Panel panel3;
        private Panel panQuestions;
        private Panel panScore;
        private Question[] questions;

        public frmQuestions(Modes mode, Question[] questions)
        {
            this.InitializeComponent();
            this.Mode = mode;
            this.questions = questions;
            int num = 0;
            int num2 = 0;
            foreach (Question question in questions)
            {
                if (this.Mode == Modes.LevelEndTest)
                {
                    question.Answer = null;
                }
                QuestionControl control = new QuestionControl {
                    Question = question
                };
                if ((num++ % 2) == 0)
                {
                    control.BackColor = Color.FromArgb(240, 240, 240);
                }
                else
                {
                    control.BackColor = Color.FromArgb(230, 230, 230);
                }
                control.Top = num2;
                num2 += control.Height;
                this.panQuestions.Controls.Add(control);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            int num2;
            QuestionControl control;
            if ((this.Mode == Modes.LevelEndTest) || (this.Mode == Modes.TestReview))
            {
                float num = 0f;
                for (num2 = 0; num2 < this.questions.Length; num2++)
                {
                    control = (QuestionControl) this.panQuestions.Controls[num2];
                    this.questions[num2].Answer = control.txtAnswer.Text;
                    control.Question = this.questions[num2];
                    control.txtAnswer.Enabled = false;
                    if (this.questions[num2].Correct)
                    {
                        num++;
                    }
                }
                this.labScore.Text = Utilities.FP(num / ((float) this.questions.Length));
                this.btnSubmit.Enabled = false;
                this.btnContinue.Enabled = true;
                this.panScore.Visible = true;
            }
            else
            {
                bool flag = true;
                for (num2 = 0; num2 < this.questions.Length; num2++)
                {
                    control = (QuestionControl) this.panQuestions.Controls[num2];
                    this.questions[num2].Answer = control.txtAnswer.Text;
                    control.Question = this.questions[num2];
                    flag = this.questions[num2].Correct && flag;
                }
                this.btnSubmit.Enabled = !flag;
                this.btnContinue.Enabled = flag;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmQuestions_Closing(object sender, CancelEventArgs e)
        {
            if (!this.btnContinue.Enabled)
            {
                e.Cancel = true;
            }
            if (base.Owner != null)
            {
                ((frmPage) base.Owner).okToClose = true;
                base.Owner.Close();
            }
        }

        private void frmQuestions_Load(object sender, EventArgs e)
        {
            if (this.Mode == Modes.TestReview)
            {
                this.btnSubmit.PerformClick();
            }
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.panQuestions = new System.Windows.Forms.Panel();
            this.panScore = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.labScore = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panScore.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnContinue);
            this.panel1.Controls.Add(this.btnSubmit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 302);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(472, 40);
            this.panel1.TabIndex = 0;
            // 
            // btnContinue
            // 
            this.btnContinue.Enabled = false;
            this.btnContinue.Location = new System.Drawing.Point(376, 8);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(88, 24);
            this.btnContinue.TabIndex = 1;
            this.btnContinue.Text = "Continue";
            this.btnContinue.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(256, 8);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(88, 24);
            this.btnSubmit.TabIndex = 0;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // panQuestions
            // 
            this.panQuestions.AutoScroll = true;
            this.panQuestions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panQuestions.Location = new System.Drawing.Point(0, 42);
            this.panQuestions.Name = "panQuestions";
            this.panQuestions.Size = new System.Drawing.Size(472, 260);
            this.panQuestions.TabIndex = 1;
            // 
            // panScore
            // 
            this.panScore.Controls.Add(this.panel3);
            this.panScore.Dock = System.Windows.Forms.DockStyle.Top;
            this.panScore.Location = new System.Drawing.Point(0, 0);
            this.panScore.Name = "panScore";
            this.panScore.Size = new System.Drawing.Size(472, 42);
            this.panScore.TabIndex = 2;
            this.panScore.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.labScore);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(344, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(128, 42);
            this.panel3.TabIndex = 0;
            // 
            // labScore
            // 
            this.labScore.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labScore.Location = new System.Drawing.Point(8, 8);
            this.labScore.Name = "labScore";
            this.labScore.Size = new System.Drawing.Size(104, 32);
            this.labScore.TabIndex = 0;
            this.labScore.Text = "100%";
            this.labScore.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // frmQuestions
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(472, 342);
            this.Controls.Add(this.panQuestions);
            this.Controls.Add(this.panScore);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(488, 2000);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(488, 100);
            this.Name = "frmQuestions";
            this.ShowInTaskbar = false;
            this.Text = "Questions";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmQuestions_Closing);
            this.Load += new System.EventHandler(this.frmQuestions_Load);
            this.panel1.ResumeLayout(false);
            this.panScore.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public enum Modes
        {
            Quiz,
            LevelEndTest,
            TestReview
        }
    }
}

