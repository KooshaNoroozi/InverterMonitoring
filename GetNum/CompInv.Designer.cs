namespace GetNum
{
    partial class CompInv
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompInv));
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.ToCmpPicker = new System.Windows.Forms.DateTimePicker();
            this.FromCmpPicker = new System.Windows.Forms.DateTimePicker();
            this.CmpChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ListOfSelectInv = new System.Windows.Forms.ListView();
            this.radioyearcmp = new System.Windows.Forms.RadioButton();
            this.radiomonthcmp = new System.Windows.Forms.RadioButton();
            this.radiodaycmp = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.CmpChart)).BeginInit();
            this.SuspendLayout();
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(513, 489);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(48, 16);
            this.label12.TabIndex = 12;
            this.label12.Text = "تا تاریخ";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(803, 489);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 16);
            this.label13.TabIndex = 11;
            this.label13.Text = "از تاریخ";
            // 
            // ToCmpPicker
            // 
            this.ToCmpPicker.Location = new System.Drawing.Point(286, 485);
            this.ToCmpPicker.Name = "ToCmpPicker";
            this.ToCmpPicker.Size = new System.Drawing.Size(200, 20);
            this.ToCmpPicker.TabIndex = 10;
            this.ToCmpPicker.ValueChanged += new System.EventHandler(this.ToCmpPicker_ValueChanged);
            
            // 
            // FromCmpPicker
            // 
            this.FromCmpPicker.CustomFormat = "dd-MM-yyyy";
            this.FromCmpPicker.Location = new System.Drawing.Point(583, 485);
            this.FromCmpPicker.Name = "FromCmpPicker";
            this.FromCmpPicker.Size = new System.Drawing.Size(200, 20);
            this.FromCmpPicker.TabIndex = 9;
            this.FromCmpPicker.ValueChanged += new System.EventHandler(this.FromCmpPicker_ValueChanged);
            
            // 
            // CmpChart
            // 
            this.CmpChart.BackColor = System.Drawing.Color.Transparent;
            this.CmpChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.CmpChart.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CmpChart.BorderlineColor = System.Drawing.Color.Transparent;
            chartArea1.Area3DStyle.Inclination = 10;
            chartArea1.Area3DStyle.IsRightAngleAxes = false;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.DarkHorizontal;
            chartArea1.BackImageAlignment = System.Windows.Forms.DataVisualization.Charting.ChartImageAlignmentStyle.BottomRight;
            chartArea1.BackImageTransparentColor = System.Drawing.Color.Transparent;
            chartArea1.BackImageWrapMode = System.Windows.Forms.DataVisualization.Charting.ChartImageWrapMode.Scaled;
            chartArea1.BackSecondaryColor = System.Drawing.Color.Transparent;
            chartArea1.BorderColor = System.Drawing.Color.Transparent;
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.BorderWidth = 10;
            chartArea1.Name = "ChartArea1";
            chartArea1.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.CmpChart.ChartAreas.Add(chartArea1);
            this.CmpChart.Cursor = System.Windows.Forms.Cursors.Hand;
            legend1.BackColor = System.Drawing.Color.Transparent;
            legend1.BackImageTransparentColor = System.Drawing.Color.Transparent;
            legend1.Name = "Legend1";
            this.CmpChart.Legends.Add(legend1);
            this.CmpChart.Location = new System.Drawing.Point(37, 33);
            this.CmpChart.Name = "CmpChart";
            this.CmpChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            this.CmpChart.RightToLeft = System.Windows.Forms.RightToLeft.No;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.CmpChart.Series.Add(series1);
            this.CmpChart.Size = new System.Drawing.Size(1023, 431);
            this.CmpChart.TabIndex = 8;
            this.CmpChart.Text = "chart of compare";
            // 
            // ListOfSelectInv
            // 
            this.ListOfSelectInv.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.ListOfSelectInv.AllowColumnReorder = true;
            this.ListOfSelectInv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ListOfSelectInv.FullRowSelect = true;
            this.ListOfSelectInv.HideSelection = false;
            this.ListOfSelectInv.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ListOfSelectInv.Location = new System.Drawing.Point(224, 512);
            this.ListOfSelectInv.Margin = new System.Windows.Forms.Padding(4);
            this.ListOfSelectInv.MultiSelect = false;
            this.ListOfSelectInv.Name = "ListOfSelectInv";
            this.ListOfSelectInv.Size = new System.Drawing.Size(675, 152);
            this.ListOfSelectInv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ListOfSelectInv.TabIndex = 33;
            this.ListOfSelectInv.UseCompatibleStateImageBehavior = false;
            this.ListOfSelectInv.View = System.Windows.Forms.View.Details;
            // 
            // radioyearcmp
            // 
            this.radioyearcmp.AccessibleName = "radioyearcmp";
            this.radioyearcmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radioyearcmp.AutoSize = true;
            this.radioyearcmp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radioyearcmp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioyearcmp.Location = new System.Drawing.Point(1053, 624);
            this.radioyearcmp.Name = "radioyearcmp";
            this.radioyearcmp.Size = new System.Drawing.Size(82, 17);
            this.radioyearcmp.TabIndex = 36;
            this.radioyearcmp.Text = "تزریق سالانه";
            this.radioyearcmp.UseVisualStyleBackColor = true;
            this.radioyearcmp.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radiomonthcmp
            // 
            this.radiomonthcmp.AccessibleName = "radiomonthcmp";
            this.radiomonthcmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radiomonthcmp.AutoSize = true;
            this.radiomonthcmp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radiomonthcmp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radiomonthcmp.Location = new System.Drawing.Point(1052, 556);
            this.radiomonthcmp.Name = "radiomonthcmp";
            this.radiomonthcmp.Size = new System.Drawing.Size(83, 17);
            this.radiomonthcmp.TabIndex = 35;
            this.radiomonthcmp.Text = "تزریق ماهانه";
            this.radiomonthcmp.UseVisualStyleBackColor = true;
            this.radiomonthcmp.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // radiodaycmp
            // 
            this.radiodaycmp.AccessibleName = "radiodaycmp";
            this.radiodaycmp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radiodaycmp.AutoSize = true;
            this.radiodaycmp.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.radiodaycmp.Checked = true;
            this.radiodaycmp.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radiodaycmp.Location = new System.Drawing.Point(1053, 495);
            this.radiodaycmp.Name = "radiodaycmp";
            this.radiodaycmp.Size = new System.Drawing.Size(82, 17);
            this.radiodaycmp.TabIndex = 34;
            this.radiodaycmp.TabStop = true;
            this.radiodaycmp.Text = "تزریق روزانه";
            this.radiodaycmp.UseVisualStyleBackColor = true;
            this.radiodaycmp.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // CompInv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1180, 679);
            this.Controls.Add(this.radioyearcmp);
            this.Controls.Add(this.radiomonthcmp);
            this.Controls.Add(this.radiodaycmp);
            this.Controls.Add(this.ListOfSelectInv);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.ToCmpPicker);
            this.Controls.Add(this.FromCmpPicker);
            this.Controls.Add(this.CmpChart);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CompInv";
            this.Text = "CompInv";
            ((System.ComponentModel.ISupportInitialize)(this.CmpChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DateTimePicker ToCmpPicker;
        private System.Windows.Forms.DateTimePicker FromCmpPicker;
        private System.Windows.Forms.DataVisualization.Charting.Chart CmpChart;
        private System.Windows.Forms.ListView ListOfSelectInv;
        private System.Windows.Forms.RadioButton radioyearcmp;
        private System.Windows.Forms.RadioButton radiomonthcmp;
        private System.Windows.Forms.RadioButton radiodaycmp;
    }
}