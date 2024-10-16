
namespace GetNum
{
    partial class BatchSMSForm
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
            this.QuesBox = new System.Windows.Forms.TextBox();
            this.SendMSG = new System.Windows.Forms.Button();
            this.SaveAnswers = new System.Windows.Forms.Button();
            this.LogBox = new System.Windows.Forms.TextBox();
            this.Loading = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // QuesBox
            // 
            this.QuesBox.Location = new System.Drawing.Point(26, 34);
            this.QuesBox.Multiline = true;
            this.QuesBox.Name = "QuesBox";
            this.QuesBox.Size = new System.Drawing.Size(177, 24);
            this.QuesBox.TabIndex = 5;
            this.QuesBox.Text = "Question Box";
            // 
            // SendMSG
            // 
            this.SendMSG.Location = new System.Drawing.Point(54, 82);
            this.SendMSG.Name = "SendMSG";
            this.SendMSG.Size = new System.Drawing.Size(116, 23);
            this.SendMSG.TabIndex = 6;
            this.SendMSG.Text = "Send message";
            this.SendMSG.UseVisualStyleBackColor = true;
            this.SendMSG.Click += new System.EventHandler(this.SendMSG_Click);
            // 
            // SaveAnswers
            // 
            this.SaveAnswers.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SaveAnswers.Location = new System.Drawing.Point(54, 432);
            this.SaveAnswers.Name = "SaveAnswers";
            this.SaveAnswers.Size = new System.Drawing.Size(116, 22);
            this.SaveAnswers.TabIndex = 18;
            this.SaveAnswers.Text = "Save Answers";
            this.SaveAnswers.UseVisualStyleBackColor = true;
            this.SaveAnswers.Click += new System.EventHandler(this.SaveAnswers_Click);
            // 
            // LogBox
            // 
            this.LogBox.Location = new System.Drawing.Point(221, 26);
            this.LogBox.Multiline = true;
            this.LogBox.Name = "LogBox";
            this.LogBox.ReadOnly = true;
            this.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.LogBox.Size = new System.Drawing.Size(289, 400);
            this.LogBox.TabIndex = 55;
            // 
            // Loading
            // 
            this.Loading.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.Loading.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Loading.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Loading.Location = new System.Drawing.Point(12, 26);
            this.Loading.Name = "Loading";
            this.Loading.Size = new System.Drawing.Size(203, 400);
            this.Loading.TabIndex = 56;
            this.Loading.Text = "We are Sending messages please wait until it ends";
            this.Loading.UseVisualStyleBackColor = false;
            this.Loading.Visible = false;
            this.Loading.Click += new System.EventHandler(this.Loading_Click);
            // 
            // BatchSMSForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.ClientSize = new System.Drawing.Size(535, 466);
            this.Controls.Add(this.Loading);
            this.Controls.Add(this.LogBox);
            this.Controls.Add(this.SaveAnswers);
            this.Controls.Add(this.SendMSG);
            this.Controls.Add(this.QuesBox);
            this.Name = "BatchSMSForm";
            this.Text = "BatchSMSForm";
            this.Load += new System.EventHandler(this.BatchSMSForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox QuesBox;
        private System.Windows.Forms.Button SendMSG;
        private System.Windows.Forms.Button SaveAnswers;
        private System.Windows.Forms.TextBox LogBox;
        private System.Windows.Forms.Button Loading;
    }
}