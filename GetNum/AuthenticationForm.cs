using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace GetNum
{
    public partial class AuthenticationForm : Form
    {
        int TryCounter = 0;
        public AuthenticationForm()
        {
            InitializeComponent();
        }
        private void RegBTN_Click(object sender, EventArgs e)
        {
            RegistrationForm RegistrationForm = new RegistrationForm();
            RegistrationForm.Show();
            this.Hide();
        }
        private bool CheckingPassword(string uname ,string pword)
        {
            return uname == "0" && pword == "0";
        }
        private void SystemLock()
        {
            MessageBox.Show("Sorry! Your wrong try was too much, System is locked for 1 minute!", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Thread.Sleep(60000);
            TryCounter--;
            MessageBox.Show("Now you can try again...", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            base.OnFormClosing(e);
            Application.Exit();


        }
        public void LoginBTN(object sender, EventArgs e)
        {
            string uname = usernametxt.Text;
            string pword = passwordtxt.Text;
            

            if (TryCounter < 3)
            {
               if (CheckingPassword(uname, pword))
                {
                    MessageBox.Show("Login successful!", "Welcome!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                    TryCounter = 0;
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid username or password.", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TryCounter++;
                    if (TryCounter>=3)
                    {
                        SystemLock();
                    }
                }
            }
            
            
            
            
        }
    }
}
