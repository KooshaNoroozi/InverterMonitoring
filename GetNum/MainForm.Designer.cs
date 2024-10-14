namespace GetNum
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.HomeBTN = new System.Windows.Forms.Button();
            this.COM3 = new System.IO.Ports.SerialPort(this.components);
            this.RegisterBTN = new System.Windows.Forms.Button();
            this.ListOfInv = new System.Windows.Forms.ListView();
            this.StatusIcon = new System.Windows.Forms.ImageList(this.components);
            this.CmpBTN = new System.Windows.Forms.Button();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.deleteinvBTN = new System.Windows.Forms.Button();
            this.ClearLogBTn = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // HomeBTN
            // 
            this.HomeBTN.Location = new System.Drawing.Point(398, 22);
            this.HomeBTN.Name = "HomeBTN";
            this.HomeBTN.Size = new System.Drawing.Size(75, 23);
            this.HomeBTN.TabIndex = 27;
            this.HomeBTN.Text = "LogOut";
            this.HomeBTN.UseVisualStyleBackColor = true;
            this.HomeBTN.Click += new System.EventHandler(this.LogOutBtn_Click);
            // 
            // COM3
            // 
            this.COM3.PortName = "COM3";
            // 
            // RegisterBTN
            // 
            this.RegisterBTN.Location = new System.Drawing.Point(212, 22);
            this.RegisterBTN.Name = "RegisterBTN";
            this.RegisterBTN.Size = new System.Drawing.Size(137, 23);
            this.RegisterBTN.TabIndex = 31;
            this.RegisterBTN.Text = "Register New inverter";
            this.RegisterBTN.UseVisualStyleBackColor = true;
            this.RegisterBTN.Click += new System.EventHandler(this.RegisterBTN_Click);
            // 
            // ListOfInv
            // 
            this.ListOfInv.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.ListOfInv.AllowColumnReorder = true;
            this.ListOfInv.CheckBoxes = true;
            this.ListOfInv.FullRowSelect = true;
            this.ListOfInv.HideSelection = false;
            this.ListOfInv.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ListOfInv.Location = new System.Drawing.Point(13, 88);
            this.ListOfInv.Margin = new System.Windows.Forms.Padding(4);
            this.ListOfInv.MultiSelect = false;
            this.ListOfInv.Name = "ListOfInv";
            this.ListOfInv.Size = new System.Drawing.Size(961, 614);
            this.ListOfInv.SmallImageList = this.StatusIcon;
            this.ListOfInv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.ListOfInv.TabIndex = 32;
            this.ListOfInv.UseCompatibleStateImageBehavior = false;
            this.ListOfInv.View = System.Windows.Forms.View.Details;
            this.ListOfInv.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListOfInv_ColumnClick);
            // 
            // StatusIcon
            // 
            this.StatusIcon.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("StatusIcon.ImageStream")));
            this.StatusIcon.TransparentColor = System.Drawing.Color.Transparent;
            this.StatusIcon.Images.SetKeyName(0, "green.png");
            this.StatusIcon.Images.SetKeyName(1, "yello.png");
            this.StatusIcon.Images.SetKeyName(2, "red.png");
            this.StatusIcon.Images.SetKeyName(3, "960-9601714_yellow-circle-png - Copy.png");
            // 
            // CmpBTN
            // 
            this.CmpBTN.Location = new System.Drawing.Point(503, 22);
            this.CmpBTN.Name = "CmpBTN";
            this.CmpBTN.Size = new System.Drawing.Size(114, 23);
            this.CmpBTN.TabIndex = 33;
            this.CmpBTN.Text = "Compare Inverters";
            this.CmpBTN.UseVisualStyleBackColor = true;
            this.CmpBTN.Click += new System.EventHandler(this.CmpBTN_Click);
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(13, 22);
            this.SearchBox.Name = "SearchBox";
            this.SearchBox.Size = new System.Drawing.Size(156, 20);
            this.SearchBox.TabIndex = 35;
            this.SearchBox.Text = "SearchBox";
            this.SearchBox.TextChanged += new System.EventHandler(this.SearchBox_TextChanged);
            // 
            // deleteinvBTN
            // 
            this.deleteinvBTN.BackColor = System.Drawing.Color.Transparent;
            this.deleteinvBTN.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.deleteinvBTN.ForeColor = System.Drawing.Color.Black;
            this.deleteinvBTN.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.deleteinvBTN.Location = new System.Drawing.Point(886, 21);
            this.deleteinvBTN.Name = "deleteinvBTN";
            this.deleteinvBTN.Size = new System.Drawing.Size(88, 24);
            this.deleteinvBTN.TabIndex = 37;
            this.deleteinvBTN.Text = "Delete Inverter";
            this.deleteinvBTN.UseVisualStyleBackColor = false;
            this.deleteinvBTN.Click += new System.EventHandler(this.DeleteinvBTN_Click);
            // 
            // ClearLogBTn
            // 
            this.ClearLogBTn.Location = new System.Drawing.Point(682, 22);
            this.ClearLogBTn.Name = "ClearLogBTn";
            this.ClearLogBTn.Size = new System.Drawing.Size(75, 23);
            this.ClearLogBTn.TabIndex = 38;
            this.ClearLogBTn.Text = "Clear Logs";
            this.ClearLogBTn.UseVisualStyleBackColor = true;
            this.ClearLogBTn.Click += new System.EventHandler(this.ClearLogBTn_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(22, 64);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 39;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 732);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.ClearLogBTn);
            this.Controls.Add(this.deleteinvBTN);
            this.Controls.Add(this.SearchBox);
            this.Controls.Add(this.CmpBTN);
            this.Controls.Add(this.ListOfInv);
            this.Controls.Add(this.RegisterBTN);
            this.Controls.Add(this.HomeBTN);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "DataTransfer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button HomeBTN;
        private System.IO.Ports.SerialPort COM3;
        private System.Windows.Forms.Button RegisterBTN;
        private System.Windows.Forms.Button CmpBTN;
        private System.Windows.Forms.TextBox SearchBox;
        private System.Windows.Forms.Button deleteinvBTN;
        private System.Windows.Forms.ImageList StatusIcon;
        private System.Windows.Forms.Button ClearLogBTn;
        public System.Windows.Forms.ListView ListOfInv;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}