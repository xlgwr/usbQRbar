namespace WHOperation
{
    partial class vendorLabelMaster
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(vendorLabelMaster));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.bhAdd = new System.Windows.Forms.ToolStripButton();
            this.bhEdit = new System.Windows.Forms.ToolStripButton();
            this.bhDelete = new System.Windows.Forms.ToolStripButton();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.VendorID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TemplateID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Default = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xmlData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cbdefault = new System.Windows.Forms.CheckBox();
            this.bhgetimagefile = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tffilename = new System.Windows.Forms.TextBox();
            this.lhMode = new System.Windows.Forms.Label();
            this.bhSave = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.tfvendor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tftemplateid = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tflabeltype = new System.Windows.Forms.ComboBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.DataName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Prefix = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Seperator = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tfprefix = new System.Windows.Forms.TextBox();
            this.cbdataname = new System.Windows.Forms.ComboBox();
            this.bdAdd = new System.Windows.Forms.Button();
            this.tfsearchvendor = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.tf2dseperator = new System.Windows.Forms.TextBox();
            this.tfindex = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tfseperator = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.bdsavexml = new System.Windows.Forms.Button();
            this.bdDelete = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panelHeader.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bhAdd,
            this.bhEdit,
            this.bhDelete});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(878, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // bhAdd
            // 
            this.bhAdd.Image = global::WHOperation.Properties.Resources.bplus1;
            this.bhAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bhAdd.Name = "bhAdd";
            this.bhAdd.Size = new System.Drawing.Size(49, 22);
            this.bhAdd.Text = "Add";
            this.bhAdd.Click += new System.EventHandler(this.bhAdd_Click);
            // 
            // bhEdit
            // 
            this.bhEdit.Image = global::WHOperation.Properties.Resources.bpen;
            this.bhEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bhEdit.Name = "bhEdit";
            this.bhEdit.Size = new System.Drawing.Size(47, 22);
            this.bhEdit.Text = "Edit";
            this.bhEdit.Click += new System.EventHandler(this.bhEdit_Click);
            // 
            // bhDelete
            // 
            this.bhDelete.Image = global::WHOperation.Properties.Resources.bdelete;
            this.bhDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.bhDelete.Name = "bhDelete";
            this.bhDelete.Size = new System.Drawing.Size(60, 22);
            this.bhDelete.Text = "Delete";
            this.bhDelete.Click += new System.EventHandler(this.bhDelete_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VendorID,
            this.TemplateID,
            this.Default,
            this.xmlData});
            this.dataGridView1.Location = new System.Drawing.Point(6, 49);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(409, 502);
            this.dataGridView1.TabIndex = 3;
            // 
            // VendorID
            // 
            this.VendorID.HeaderText = "VendorID";
            this.VendorID.Name = "VendorID";
            this.VendorID.ReadOnly = true;
            // 
            // TemplateID
            // 
            this.TemplateID.HeaderText = "TemplateID";
            this.TemplateID.Name = "TemplateID";
            this.TemplateID.ReadOnly = true;
            // 
            // Default
            // 
            this.Default.HeaderText = "Default";
            this.Default.Name = "Default";
            this.Default.ReadOnly = true;
            // 
            // xmlData
            // 
            this.xmlData.HeaderText = "xmlData";
            this.xmlData.Name = "xmlData";
            this.xmlData.ReadOnly = true;
            this.xmlData.Visible = false;
            // 
            // panelHeader
            // 
            this.panelHeader.Controls.Add(this.groupBox2);
            this.panelHeader.Location = new System.Drawing.Point(439, 33);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(390, 266);
            this.panelHeader.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox1);
            this.groupBox2.Controls.Add(this.cbdefault);
            this.groupBox2.Controls.Add(this.bhgetimagefile);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.tffilename);
            this.groupBox2.Controls.Add(this.lhMode);
            this.groupBox2.Controls.Add(this.bhSave);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tfvendor);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.tftemplateid);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(375, 250);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vendor Label Header-Input Data";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(107, 129);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 115);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // cbdefault
            // 
            this.cbdefault.AutoSize = true;
            this.cbdefault.Location = new System.Drawing.Point(245, 80);
            this.cbdefault.Name = "cbdefault";
            this.cbdefault.Size = new System.Drawing.Size(107, 17);
            this.cbdefault.TabIndex = 4;
            this.cbdefault.Text = "Default Template";
            this.cbdefault.UseVisualStyleBackColor = true;
            // 
            // bhgetimagefile
            // 
            this.bhgetimagefile.Location = new System.Drawing.Point(323, 103);
            this.bhgetimagefile.Name = "bhgetimagefile";
            this.bhgetimagefile.Size = new System.Drawing.Size(45, 23);
            this.bhgetimagefile.TabIndex = 7;
            this.bhgetimagefile.Text = "Get";
            this.bhgetimagefile.UseVisualStyleBackColor = true;
            this.bhgetimagefile.Click += new System.EventHandler(this.bhgetimagefile_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 103);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 5;
            this.label7.Text = "Label Image File";
            // 
            // tffilename
            // 
            this.tffilename.Location = new System.Drawing.Point(107, 103);
            this.tffilename.Name = "tffilename";
            this.tffilename.Size = new System.Drawing.Size(215, 20);
            this.tffilename.TabIndex = 6;
            // 
            // lhMode
            // 
            this.lhMode.AutoSize = true;
            this.lhMode.Location = new System.Drawing.Point(334, 16);
            this.lhMode.Name = "lhMode";
            this.lhMode.Size = new System.Drawing.Size(34, 13);
            this.lhMode.TabIndex = 10;
            this.lhMode.Text = "Mode";
            this.lhMode.Visible = false;
            // 
            // bhSave
            // 
            this.bhSave.Enabled = false;
            this.bhSave.Location = new System.Drawing.Point(15, 19);
            this.bhSave.Name = "bhSave";
            this.bhSave.Size = new System.Drawing.Size(52, 23);
            this.bhSave.TabIndex = 8;
            this.bhSave.Text = "Save";
            this.bhSave.UseVisualStyleBackColor = true;
            this.bhSave.Click += new System.EventHandler(this.bhSave_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Vendor ID";
            // 
            // tfvendor
            // 
            this.tfvendor.Location = new System.Drawing.Point(107, 52);
            this.tfvendor.Name = "tfvendor";
            this.tfvendor.Size = new System.Drawing.Size(121, 20);
            this.tfvendor.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Template ID";
            // 
            // tftemplateid
            // 
            this.tftemplateid.Location = new System.Drawing.Point(107, 77);
            this.tftemplateid.Name = "tftemplateid";
            this.tftemplateid.Size = new System.Drawing.Size(121, 20);
            this.tftemplateid.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(63, 17);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Label Type";
            // 
            // tflabeltype
            // 
            this.tflabeltype.FormattingEnabled = true;
            this.tflabeltype.Items.AddRange(new object[] {
            "Single",
            "Compound",
            "General"});
            this.tflabeltype.Location = new System.Drawing.Point(66, 35);
            this.tflabeltype.Name = "tflabeltype";
            this.tflabeltype.Size = new System.Drawing.Size(121, 21);
            this.tflabeltype.TabIndex = 1;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DataName,
            this.Prefix,
            this.Seperator,
            this.Index});
            this.dataGridView2.Location = new System.Drawing.Point(66, 61);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(308, 182);
            this.dataGridView2.TabIndex = 3;
            // 
            // DataName
            // 
            this.DataName.HeaderText = "DataName";
            this.DataName.Name = "DataName";
            this.DataName.ReadOnly = true;
            // 
            // Prefix
            // 
            this.Prefix.HeaderText = "Prefix";
            this.Prefix.Name = "Prefix";
            this.Prefix.ReadOnly = true;
            this.Prefix.Width = 60;
            // 
            // Seperator
            // 
            this.Seperator.HeaderText = "Seperator";
            this.Seperator.Name = "Seperator";
            this.Seperator.ReadOnly = true;
            this.Seperator.Width = 60;
            // 
            // Index
            // 
            this.Index.HeaderText = "Index";
            this.Index.Name = "Index";
            this.Index.ReadOnly = true;
            this.Index.Width = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 249);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Prefix";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 249);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Data Name";
            // 
            // tfprefix
            // 
            this.tfprefix.Location = new System.Drawing.Point(200, 265);
            this.tfprefix.Name = "tfprefix";
            this.tfprefix.Size = new System.Drawing.Size(78, 20);
            this.tfprefix.TabIndex = 8;
            // 
            // cbdataname
            // 
            this.cbdataname.FormattingEnabled = true;
            this.cbdataname.ItemHeight = 13;
            this.cbdataname.Items.AddRange(new object[] {
            "LOTNUMBER",
            "MFGDATE",
            "EXPIREDATE",
            "RECQTY",
            "DATECODE",
            "MFGRPART",
            "DNPARTNUMBER"});
            this.cbdataname.Location = new System.Drawing.Point(66, 264);
            this.cbdataname.Name = "cbdataname";
            this.cbdataname.Size = new System.Drawing.Size(128, 21);
            this.cbdataname.TabIndex = 6;
            // 
            // bdAdd
            // 
            this.bdAdd.Location = new System.Drawing.Point(6, 262);
            this.bdAdd.Name = "bdAdd";
            this.bdAdd.Size = new System.Drawing.Size(54, 23);
            this.bdAdd.TabIndex = 4;
            this.bdAdd.Text = "Add";
            this.bdAdd.UseVisualStyleBackColor = true;
            this.bdAdd.Click += new System.EventHandler(this.bdSave_Click);
            // 
            // tfsearchvendor
            // 
            this.tfsearchvendor.Location = new System.Drawing.Point(191, 13);
            this.tfsearchvendor.Name = "tfsearchvendor";
            this.tfsearchvendor.Size = new System.Drawing.Size(136, 20);
            this.tfsearchvendor.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(107, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Search Vendor";
            // 
            // button1
            // 
            this.button1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.button1.Location = new System.Drawing.Point(333, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(61, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.tf2dseperator);
            this.groupBox1.Controls.Add(this.tfindex);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.tfseperator);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.bdsavexml);
            this.groupBox1.Controls.Add(this.bdDelete);
            this.groupBox1.Controls.Add(this.tfprefix);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbdataname);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dataGridView2);
            this.groupBox1.Controls.Add(this.tflabeltype);
            this.groupBox1.Controls.Add(this.bdAdd);
            this.groupBox1.Location = new System.Drawing.Point(439, 305);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(381, 299);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Data Definition";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(193, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(91, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "Seperator (for 2D)";
            // 
            // tf2dseperator
            // 
            this.tf2dseperator.Location = new System.Drawing.Point(193, 35);
            this.tf2dseperator.Name = "tf2dseperator";
            this.tf2dseperator.Size = new System.Drawing.Size(90, 20);
            this.tf2dseperator.TabIndex = 14;
            // 
            // tfindex
            // 
            this.tfindex.Location = new System.Drawing.Point(341, 265);
            this.tfindex.Name = "tfindex";
            this.tfindex.Size = new System.Drawing.Size(32, 20);
            this.tfindex.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(338, 249);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(33, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Index";
            // 
            // tfseperator
            // 
            this.tfseperator.Location = new System.Drawing.Point(284, 264);
            this.tfseperator.Name = "tfseperator";
            this.tfseperator.Size = new System.Drawing.Size(50, 20);
            this.tfseperator.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(281, 249);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Seperator";
            // 
            // bdsavexml
            // 
            this.bdsavexml.Location = new System.Drawing.Point(289, 30);
            this.bdsavexml.Name = "bdsavexml";
            this.bdsavexml.Size = new System.Drawing.Size(75, 23);
            this.bdsavexml.TabIndex = 13;
            this.bdsavexml.Text = "Save";
            this.bdsavexml.UseVisualStyleBackColor = true;
            this.bdsavexml.Click += new System.EventHandler(this.bdsavexml_Click);
            // 
            // bdDelete
            // 
            this.bdDelete.Location = new System.Drawing.Point(6, 61);
            this.bdDelete.Name = "bdDelete";
            this.bdDelete.Size = new System.Drawing.Size(54, 23);
            this.bdDelete.TabIndex = 2;
            this.bdDelete.Text = "Delete";
            this.bdDelete.UseVisualStyleBackColor = true;
            this.bdDelete.Click += new System.EventHandler(this.bdDelete_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dataGridView1);
            this.groupBox3.Controls.Add(this.tfsearchvendor);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Location = new System.Drawing.Point(12, 38);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(421, 566);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Vendor Label Header";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // vendorLabelMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 616);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "vendorLabelMaster";
            this.Text = "Vendor Label Master";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panelHeader.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton bhAdd;
        private System.Windows.Forms.ToolStripButton bhEdit;
        private System.Windows.Forms.ToolStripButton bhDelete;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TextBox tfprefix;
        private System.Windows.Forms.Button bdAdd;
        private System.Windows.Forms.ComboBox cbdataname;
        private System.Windows.Forms.TextBox tfvendor;
        private System.Windows.Forms.TextBox tftemplateid;
        private System.Windows.Forms.ComboBox tflabeltype;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button bhSave;
        private System.Windows.Forms.TextBox tfsearchvendor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lhMode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button bdDelete;
        private System.Windows.Forms.Button bhgetimagefile;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tffilename;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn VendorID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TemplateID;
        private System.Windows.Forms.DataGridViewTextBoxColumn Default;
        private System.Windows.Forms.DataGridViewTextBoxColumn xmlData;
        private System.Windows.Forms.CheckBox cbdefault;
        private System.Windows.Forms.Button bdsavexml;
        private System.Windows.Forms.TextBox tfseperator;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tfindex;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewTextBoxColumn DataName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Prefix;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seperator;
        private System.Windows.Forms.DataGridViewTextBoxColumn Index;
        private System.Windows.Forms.TextBox tf2dseperator;
        private System.Windows.Forms.Label label10;
    }
}