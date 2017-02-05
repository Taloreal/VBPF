namespace KMI.Sim
{
    using KMI.Utility;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    public class frmEditSimSettings : Form
    {
        private Button btnCancel;
        private Button btnDeleteAssignment;
        private Button btnDisableAllActions;
        private Button btnHelp;
        private Button btnLoadAssignment;
        private Button btnOK;
        private Button btnReset;
        private Container components = null;
        private Label label1;
        private Label label2;
        private Label label3;
        private string lastEntry;
        private Hashtable localLanguageAssignments = new Hashtable();
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
        private Panel panel5;
        private byte[] pdfAssignment;
        private PropertyInfo[] propertyInfo;
        private TextBox txtCountryCode;
        private TextBox txtName;
        private TextBox txtValue;

        public frmEditSimSettings()
        {
            this.InitializeComponent();
            SimSettings simSettings = S.SA.getSimSettings();
            this.LoadSettingsIntoTextBoxes(simSettings);
            this.pdfAssignment = simSettings.PdfAssignment;
            this.btnReset.Visible = S.MF.DesignerMode;
            this.btnDisableAllActions.Visible = S.MF.DesignerMode;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void btnDeleteAssignment_Click(object sender, EventArgs e)
        {
            this.pdfAssignment = null;
            this.localLanguageAssignments = new Hashtable();
        }

        private void btnDisableAllActions_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.panel3.Controls.Count; i += 2)
            {
                string text = this.panel3.Controls[i].Text;
                int index = text.IndexOf("Enabled For");
                if (index > -1)
                {
                    text = text.Substring(0, index).TrimEnd(new char[] { ' ' });
                    if (S.MF.IsActionMenuItem(text))
                    {
                        this.panel3.Controls[i + 1].Text = "False";
                    }
                }
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            KMIHelp.OpenHelp("Instructor's Area");
        }

        private void btnLoadAssignment_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "PDF Files (*.pdf)|*.pdf",
                DefaultExt = ".pdf"
            };
            if ((dialog.ShowDialog() == DialogResult.OK) && File.Exists(dialog.FileName))
            {
                FileStream input = new FileStream(dialog.FileName, FileMode.Open);
                byte[] buffer = new BinaryReader(input).ReadBytes((int) input.Length);
                if (this.txtCountryCode.Text == "")
                {
                    this.pdfAssignment = buffer;
                }
                else if (this.localLanguageAssignments.ContainsKey(this.txtCountryCode.Text))
                {
                    this.localLanguageAssignments[this.txtCountryCode.Text] = buffer;
                }
                else
                {
                    this.localLanguageAssignments.Add(this.txtCountryCode.Text, buffer);
                }
                input.Close();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SimStateAdapter adapter;
            foreach (Control control in this.panel3.Controls)
            {
                if (control.Left == this.txtValue.Left)
                {
                    object obj2 = this.ConvertStringToPropertyType(control.Text, (int) control.Tag);
                    MethodInfo setMethod = this.propertyInfo[(int) control.Tag].GetSetMethod();
                    lock ((adapter = S.SA))
                    {
                        setMethod.Invoke(Simulator.Instance.SimState.SimSettings, new object[] { obj2 });
                    }
                }
            }
            lock ((adapter = S.SA))
            {
                S.SS.PdfAssignment = this.pdfAssignment;
                if (S.ST.Reserved.ContainsKey("LocalLanguageAssignments"))
                {
                    S.ST.Reserved["LocalLanguageAssignments"] = this.localLanguageAssignments;
                }
                else
                {
                    S.ST.Reserved.Add("LocalLanguageAssignments", this.localLanguageAssignments);
                }
            }
            base.Close();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to reset all settings?", "Confirm Reset", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.LoadSettingsIntoTextBoxes(S.I.DefaultSimSettings);
                this.pdfAssignment = S.I.DefaultSimSettings.PdfAssignment;
            }
        }

        private object ConvertStringToPropertyType(string s, int propertyIndex)
        {
            object obj2;
            MethodInfo getMethod = this.propertyInfo[propertyIndex].GetGetMethod();
            lock (S.SA)
            {
                obj2 = getMethod.Invoke(Simulator.Instance.SimState.SimSettings, new object[0]);
            }
            try
            {
                if (obj2 is int)
                {
                    return Convert.ToInt32(s);
                }
                if (obj2 is float)
                {
                    return Convert.ToSingle(s);
                }
                if (obj2 is DateTime)
                {
                    return Convert.ToDateTime(s);
                }
                if (obj2 is bool)
                {
                    return Convert.ToBoolean(s);
                }
                if (obj2 is string)
                {
                    return Convert.ToString(s);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void frmEditSimSettings_Load(object sender, EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCountryCode = new System.Windows.Forms.TextBox();
            this.btnDisableAllActions = new System.Windows.Forms.Button();
            this.btnDeleteAssignment = new System.Windows.Forms.Button();
            this.btnLoadAssignment = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 286);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(576, 104);
            this.panel1.TabIndex = 0;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label3);
            this.panel5.Controls.Add(this.txtCountryCode);
            this.panel5.Controls.Add(this.btnDisableAllActions);
            this.panel5.Controls.Add(this.btnDeleteAssignment);
            this.panel5.Controls.Add(this.btnLoadAssignment);
            this.panel5.Controls.Add(this.btnReset);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(400, 104);
            this.panel5.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(208, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Country Code:";
            // 
            // txtCountryCode
            // 
            this.txtCountryCode.Location = new System.Drawing.Point(288, 8);
            this.txtCountryCode.Name = "txtCountryCode";
            this.txtCountryCode.Size = new System.Drawing.Size(56, 20);
            this.txtCountryCode.TabIndex = 4;
            // 
            // btnDisableAllActions
            // 
            this.btnDisableAllActions.Location = new System.Drawing.Point(208, 72);
            this.btnDisableAllActions.Name = "btnDisableAllActions";
            this.btnDisableAllActions.Size = new System.Drawing.Size(168, 24);
            this.btnDisableAllActions.TabIndex = 3;
            this.btnDisableAllActions.Text = "Disable All Actions";
            this.btnDisableAllActions.Click += new System.EventHandler(this.btnDisableAllActions_Click);
            // 
            // btnDeleteAssignment
            // 
            this.btnDeleteAssignment.Location = new System.Drawing.Point(16, 40);
            this.btnDeleteAssignment.Name = "btnDeleteAssignment";
            this.btnDeleteAssignment.Size = new System.Drawing.Size(168, 24);
            this.btnDeleteAssignment.TabIndex = 1;
            this.btnDeleteAssignment.Text = "Delete Assignment";
            this.btnDeleteAssignment.Click += new System.EventHandler(this.btnDeleteAssignment_Click);
            // 
            // btnLoadAssignment
            // 
            this.btnLoadAssignment.Location = new System.Drawing.Point(16, 8);
            this.btnLoadAssignment.Name = "btnLoadAssignment";
            this.btnLoadAssignment.Size = new System.Drawing.Size(168, 24);
            this.btnLoadAssignment.TabIndex = 0;
            this.btnLoadAssignment.Text = "Load Assignment from File";
            this.btnLoadAssignment.Click += new System.EventHandler(this.btnLoadAssignment_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(16, 72);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(168, 24);
            this.btnReset.TabIndex = 2;
            this.btnReset.Text = "Reset to Defaults";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnCancel);
            this.panel4.Controls.Add(this.btnHelp);
            this.panel4.Controls.Add(this.btnOK);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(448, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(128, 104);
            this.panel4.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(16, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 24);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Location = new System.Drawing.Point(16, 72);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(96, 24);
            this.btnHelp.TabIndex = 2;
            this.btnHelp.Text = "Help";
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(16, 8);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 24);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(10, 30, 10, 10);
            this.panel2.Size = new System.Drawing.Size(576, 286);
            this.panel2.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.txtValue);
            this.panel3.Controls.Add(this.txtName);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(10, 30);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(556, 246);
            this.panel3.TabIndex = 3;
            this.panel3.Resize += new System.EventHandler(this.panel3_Resize);
            // 
            // txtValue
            // 
            this.txtValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValue.Location = new System.Drawing.Point(231, 0);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(150, 20);
            this.txtValue.TabIndex = 1;
            this.txtValue.Text = "textBox2";
            this.txtValue.Enter += new System.EventHandler(this.txtValue_Enter);
            this.txtValue.Validating += new System.ComponentModel.CancelEventHandler(this.txtValue_Validating);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Location = new System.Drawing.Point(0, 0);
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(232, 20);
            this.txtName.TabIndex = 0;
            this.txtName.Text = "textBox1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Setting";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(240, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value";
            // 
            // frmEditSimSettings
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(576, 390);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(456, 300);
            this.Name = "frmEditSimSettings";
            this.ShowInTaskbar = false;
            this.Text = "Customize Your Simulation";
            this.Load += new System.EventHandler(this.frmEditSimSettings_Load);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void InsertRow(int i, int j, string name, string val, bool changed)
        {
            TextBox box = new TextBox {
                Size = this.txtName.Size,
                Left = this.txtName.Left,
                Top = (i * (this.txtName.Height - 1)) + this.txtName.Top,
                ReadOnly = true,
                BackColor = this.txtName.BackColor,
                BorderStyle = this.txtName.BorderStyle,
                Text = name
            };
            this.panel3.Controls.Add(box);
            TextBox box2 = new TextBox {
                Size = this.txtValue.Size,
                Left = this.txtValue.Left,
                Top = (i * (this.txtValue.Height - 1)) + this.txtValue.Top,
                BackColor = this.txtValue.BackColor,
                BorderStyle = this.txtValue.BorderStyle,
                Tag = j,
                Text = val
            };
            if (changed)
            {
                box2.Font = new Font(box2.Font, FontStyle.Bold);
            }
            this.panel3.Controls.Add(box2);
            box2.Validating += new CancelEventHandler(this.txtValue_Validating);
            box2.Enter += new EventHandler(this.txtValue_Enter);
            box2.DoubleClick += new EventHandler(this.txtValue_DoubleClick);
        }

        private void LoadSettingsIntoTextBoxes(SimSettings simSettings)
        {
            this.panel3.Controls.Clear();
            this.propertyInfo = simSettings.GetType().GetProperties();
            int i = 0;
            int j = 0;
            foreach (PropertyInfo info in this.propertyInfo)
            {
                MethodInfo getMethod = info.GetGetMethod();
                object obj2 = getMethod.Invoke(simSettings, new object[0]);
                object obj3 = getMethod.Invoke(S.I.DefaultSimSettings, new object[0]);
                bool changed = true;
                if ((obj2 != null) && (obj3 != null))
                {
                    changed = obj2.ToString() != obj3.ToString();
                }
                if ((((obj2 != null) && !obj2.GetType().IsArray) && (((info.Name.IndexOf("Enabled") > -1) || S.MF.DesignerMode) || simSettings.AllowInstructorToEdit(info.Name))) && (info.Name != "Assignment"))
                {
                    this.InsertRow(i, j, Utilities.AddSpaces(info.Name), obj2.ToString(), changed);
                    i++;
                }
                j++;
            }
            this.panel3_Resize(new object(), new EventArgs());
        }

        private void panel3_Resize(object sender, EventArgs e)
        {
            foreach (Control control in this.panel3.Controls)
            {
                if (control.Left == this.txtValue.Left)
                {
                    control.Width = (this.panel3.ClientRectangle.Right - control.Left) - 1;
                }
            }
        }

        private void txtValue_DoubleClick(object sender, EventArgs e)
        {
            TextBox box = (TextBox) sender;
            if (box.Text.ToUpper() == "TRUE")
            {
                box.Text = "False";
            }
            else if (box.Text.ToUpper() == "FALSE")
            {
                box.Text = "True";
            }
        }

        private void txtValue_Enter(object sender, EventArgs e)
        {
            this.lastEntry = ((TextBox) sender).Text;
        }

        private void txtValue_Validating(object sender, CancelEventArgs e)
        {
            TextBox box = (TextBox) sender;
            if (this.ConvertStringToPropertyType(box.Text, (int) box.Tag) == null)
            {
                MessageBox.Show("Invalid entry. Please try again.");
                box.Text = this.lastEntry;
                box.SelectAll();
                e.Cancel = true;
            }
        }
    }
}

