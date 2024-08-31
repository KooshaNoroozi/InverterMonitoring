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
            this.RecieveTextBox = new System.Windows.Forms.TextBox();
            this.RunBTN = new System.Windows.Forms.Button();
            this.RegisterBTN = new System.Windows.Forms.Button();
            ListOfInv = new System.Windows.Forms.ListView();
            this.StatusIcon = new System.Windows.Forms.ImageList(this.components);
            this.CmpBTN = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SearchBox = new System.Windows.Forms.TextBox();
            this.deleteinvBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HomeBTN
            // 
            this.HomeBTN.Location = new System.Drawing.Point(99, 378);
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
            // RecieveTextBox
            // 
            this.RecieveTextBox.Location = new System.Drawing.Point(49, 105);
            this.RecieveTextBox.Multiline = true;
            this.RecieveTextBox.Name = "RecieveTextBox";
            this.RecieveTextBox.ReadOnly = true;
            this.RecieveTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.RecieveTextBox.Size = new System.Drawing.Size(206, 201);
            this.RecieveTextBox.TabIndex = 29;
            // 
            // RunBTN
            // 
            this.RunBTN.Location = new System.Drawing.Point(111, 34);
            this.RunBTN.Name = "RunBTN";
            this.RunBTN.Size = new System.Drawing.Size(75, 23);
            this.RunBTN.TabIndex = 30;
            this.RunBTN.Text = "Run";
            this.RunBTN.UseVisualStyleBackColor = true;
            this.RunBTN.Click += new System.EventHandler(this.RunBTN_Click);
            // 
            // RegisterBTN
            // 
            this.RegisterBTN.Location = new System.Drawing.Point(73, 323);
            this.RegisterBTN.Name = "RegisterBTN";
            this.RegisterBTN.Size = new System.Drawing.Size(137, 23);
            this.RegisterBTN.TabIndex = 31;
            this.RegisterBTN.Text = "Register New inverter";
            this.RegisterBTN.UseVisualStyleBackColor = true;
            this.RegisterBTN.Click += new System.EventHandler(this.RegisterBTN_Click);
            // 
            // ListOfInv
            // 
            ListOfInv.Activation = System.Windows.Forms.ItemActivation.OneClick;
            ListOfInv.AllowColumnReorder = true;
            ListOfInv.CheckBoxes = true;
            ListOfInv.FullRowSelect = true;
            ListOfInv.HideSelection = false;
            ListOfInv.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            ListOfInv.Location = new System.Drawing.Point(338, 105);
            ListOfInv.Margin = new System.Windows.Forms.Padding(4);
            ListOfInv.MultiSelect = false;
            ListOfInv.Name = "ListOfInv";
            ListOfInv.Size = new System.Drawing.Size(922, 316);
            ListOfInv.SmallImageList = this.StatusIcon;
            ListOfInv.Sorting = System.Windows.Forms.SortOrder.Ascending;
            ListOfInv.TabIndex = 32;
            ListOfInv.UseCompatibleStateImageBehavior = false;
            ListOfInv.View = System.Windows.Forms.View.Details;
            ListOfInv.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.ListOfInv_ColumnClick);
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
            this.CmpBTN.Location = new System.Drawing.Point(1089, 429);
            this.CmpBTN.Name = "CmpBTN";
            this.CmpBTN.Size = new System.Drawing.Size(114, 23);
            this.CmpBTN.TabIndex = 33;
            this.CmpBTN.Text = "Compare Inverters";
            this.CmpBTN.UseVisualStyleBackColor = true;
            this.CmpBTN.Click += new System.EventHandler(this.CmpBTN_Click);
            // 
            // button1
            // 
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(366, 536);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 34;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // SearchBox
            // 
            this.SearchBox.Location = new System.Drawing.Point(348, 61);
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
            this.deleteinvBTN.Location = new System.Drawing.Point(1209, 428);
            this.deleteinvBTN.Name = "deleteinvBTN";
            this.deleteinvBTN.Size = new System.Drawing.Size(54, 24);
            this.deleteinvBTN.TabIndex = 37;
            this.deleteinvBTN.Text = "Delete";
            this.deleteinvBTN.UseVisualStyleBackColor = false;
            this.deleteinvBTN.Click += new System.EventHandler(this.DeleteinvBTN_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1273, 602);
            this.Controls.Add(this.deleteinvBTN);
            this.Controls.Add(this.SearchBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CmpBTN);
            this.Controls.Add(ListOfInv);
            this.Controls.Add(this.RegisterBTN);
            this.Controls.Add(this.RunBTN);
            this.Controls.Add(this.RecieveTextBox);
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
        private System.Windows.Forms.TextBox RecieveTextBox;
        private System.Windows.Forms.Button RunBTN;
        private System.Windows.Forms.Button RegisterBTN;
        private System.Windows.Forms.Button CmpBTN;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox SearchBox;
        public static System.Windows.Forms.ListView ListOfInv;
        private System.Windows.Forms.Button deleteinvBTN;
        private System.Windows.Forms.ImageList StatusIcon;
    }
}