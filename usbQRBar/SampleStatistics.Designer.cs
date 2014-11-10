namespace usbQRBar
{
    partial class SampleStatistics
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
            this.btnStartSO = new System.Windows.Forms.Button();
            this.btnGenerateStatistics = new System.Windows.Forms.Button();
            this.btnRetrieveStatistics = new System.Windows.Forms.Button();
            this.txtStatisticName = new System.Windows.Forms.TextBox();
            this.txtRetrievedStatistics = new System.Windows.Forms.TextBox();
            this.btnRetreiveStatistic = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartSO
            // 
            this.btnStartSO.Location = new System.Drawing.Point(45, 35);
            this.btnStartSO.Name = "btnStartSO";
            this.btnStartSO.Size = new System.Drawing.Size(133, 23);
            this.btnStartSO.TabIndex = 0;
            this.btnStartSO.Text = "Start SO";
            this.btnStartSO.UseVisualStyleBackColor = true;
            this.btnStartSO.Click += new System.EventHandler(this.StartServiceObject);
            // 
            // btnGenerateStatistics
            // 
            this.btnGenerateStatistics.Location = new System.Drawing.Point(45, 75);
            this.btnGenerateStatistics.Name = "btnGenerateStatistics";
            this.btnGenerateStatistics.Size = new System.Drawing.Size(133, 23);
            this.btnGenerateStatistics.TabIndex = 1;
            this.btnGenerateStatistics.Text = "GenerateStatistics";
            this.btnGenerateStatistics.UseVisualStyleBackColor = true;
            this.btnGenerateStatistics.Click += new System.EventHandler(this.GenerateStatistics);
            // 
            // btnRetrieveStatistics
            // 
            this.btnRetrieveStatistics.Location = new System.Drawing.Point(45, 117);
            this.btnRetrieveStatistics.Name = "btnRetrieveStatistics";
            this.btnRetrieveStatistics.Size = new System.Drawing.Size(133, 23);
            this.btnRetrieveStatistics.TabIndex = 2;
            this.btnRetrieveStatistics.Text = "Retrieve Statistics";
            this.btnRetrieveStatistics.UseVisualStyleBackColor = true;
            this.btnRetrieveStatistics.Click += new System.EventHandler(this.RetrieveStatistics);
            // 
            // txtStatisticName
            // 
            this.txtStatisticName.Location = new System.Drawing.Point(16, 68);
            this.txtStatisticName.Name = "txtStatisticName";
            this.txtStatisticName.Size = new System.Drawing.Size(205, 20);
            this.txtStatisticName.TabIndex = 4;
            this.txtStatisticName.TextChanged += new System.EventHandler(this.StatisticSizeChanged);
            // 
            // txtRetrievedStatistics
            // 
            this.txtRetrievedStatistics.BackColor = System.Drawing.Color.White;
            this.txtRetrievedStatistics.Location = new System.Drawing.Point(45, 157);
            this.txtRetrievedStatistics.Multiline = true;
            this.txtRetrievedStatistics.Name = "txtRetrievedStatistics";
            this.txtRetrievedStatistics.ReadOnly = true;
            this.txtRetrievedStatistics.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRetrievedStatistics.Size = new System.Drawing.Size(476, 247);
            this.txtRetrievedStatistics.TabIndex = 5;
            // 
            // btnRetreiveStatistic
            // 
            this.btnRetreiveStatistic.Location = new System.Drawing.Point(16, 30);
            this.btnRetreiveStatistic.Name = "btnRetreiveStatistic";
            this.btnRetreiveStatistic.Size = new System.Drawing.Size(133, 23);
            this.btnRetreiveStatistic.TabIndex = 6;
            this.btnRetreiveStatistic.Text = "Retrieve Statistic";
            this.btnRetreiveStatistic.UseVisualStyleBackColor = true;
            this.btnRetreiveStatistic.Click += new System.EventHandler(this.RetrieveOneStatistic);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRetreiveStatistic);
            this.groupBox1.Controls.Add(this.txtStatisticName);
            this.groupBox1.Location = new System.Drawing.Point(219, 35);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 105);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Single Statistic";
            // 
            // SampleStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuBar;
            this.ClientSize = new System.Drawing.Size(565, 439);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtRetrievedStatistics);
            this.Controls.Add(this.btnRetrieveStatistics);
            this.Controls.Add(this.btnGenerateStatistics);
            this.Controls.Add(this.btnStartSO);
            this.Name = "SampleStatistics";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartSO;
        private System.Windows.Forms.Button btnGenerateStatistics;
        private System.Windows.Forms.Button btnRetrieveStatistics;
        private System.Windows.Forms.TextBox txtStatisticName;
        private System.Windows.Forms.TextBox txtRetrievedStatistics;
        private System.Windows.Forms.Button btnRetreiveStatistic;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}