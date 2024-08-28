using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace GetNum
{
    public partial class RegistrationForm : Form
    {
       
        public RegistrationForm()
        {
            InitializeComponent();
                    
        }


        private void InsertDataBTN(object sender, EventArgs e)
        {

            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Create a table if it doesn't exist
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS DeviceInfoTable (SID INTEGER PRIMARY KEY not null ,SerialNum TEXT , SimNum TEXT,OwnerName TEXT,OwnerPhone TEXT,Address TEXT)";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }
                    //checking for repeatance
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM DeviceInfoTable WHERE SimNum = @SimNum", connection))
                    {
                        cmd.Parameters.AddWithValue("@SimNum", "+98" + SimNum.Text);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // SIM number already exists, handle accordingly (e.g., show an error message)
                            MessageBox.Show("SIM number already exists!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return; 
                        }
                       
                    }
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM DeviceInfoTable WHERE SerialNum = @SerialNum", connection))
                    {
                        cmd.Parameters.AddWithValue("@SerialNum", SerialNum.Text);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            // SIM number already exists, handle accordingly (e.g., show an error message)
                            MessageBox.Show("Serial number already exists!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                    }

                    // Insert data from TextBox
                    string insertDataQuery = "INSERT INTO DeviceInfoTable (SerialNum, SimNum, OwnerName, OwnerPhone, Address) VALUES (@SerialNum, @SimNum, @OwnerName, @OwnerPhone, @Address)";
                    using (SQLiteCommand insertDataCmd = new SQLiteCommand(insertDataQuery, connection))
                    {
                        insertDataCmd.Parameters.AddWithValue("@SerialNum", SerialNum.Text);
                        insertDataCmd.Parameters.AddWithValue("@SimNum", "+98"+SimNum.Text);
                        insertDataCmd.Parameters.AddWithValue("@OwnerName", OwnerName.Text);
                        insertDataCmd.Parameters.AddWithValue("@OwnerPhone", "+98" + OwnerPhone.Text);
                        insertDataCmd.Parameters.AddWithValue("@Address",Address.Text );
                        insertDataCmd.ExecuteNonQuery();
                    }

                    string findSIDquery = "select SID from DeviceInfoTable where (SerialNum=@SerialNum AND SimNum=@SimNum AND OwnerName=@OwnerName AND OwnerPhone=@OwnerPhone AND Address=@Address)";
                    int sid;
                    using (SQLiteCommand findsidcommand = new SQLiteCommand(findSIDquery, connection))
                    {
                        findsidcommand.Parameters.AddWithValue("@SerialNum", SerialNum.Text);
                        findsidcommand.Parameters.AddWithValue("@SimNum", "+98" + SimNum.Text);
                        findsidcommand.Parameters.AddWithValue("@OwnerName", OwnerName.Text);
                        findsidcommand.Parameters.AddWithValue("@OwnerPhone", "+98" + OwnerPhone.Text);
                        findsidcommand.Parameters.AddWithValue("@Address", Address.Text);
                        sid = Convert.ToInt32(findsidcommand.ExecuteScalar());
                    }
                    string yesterday = (DateTime.Today.AddDays(-1)).ToString("yyyy-MM-dd");

                    // initializing in tables
                    createTableQuery = "CREATE TABLE IF NOT EXISTS DeviceFeedLog(SID INTEGER, Energy Integer, Date TEXT)";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }
                                                            
                    insertDataQuery = "INSERT INTO DeviceFeedLog (SID, Energy, Date) VALUES (@SID, 0 , @yesterday )";
                    using (SQLiteCommand insertDataCmd = new SQLiteCommand(insertDataQuery, connection))
                    {
                        insertDataCmd.Parameters.AddWithValue("@SID", sid);
                        insertDataCmd.Parameters.AddWithValue("@yesterday", yesterday);
                        insertDataCmd.ExecuteNonQuery();
                    }

                    createTableQuery = "CREATE TABLE IF NOT EXISTS DevicetodayFeed(SID INTEGER, Energy Integer, Date TEXT)";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }

                    insertDataQuery = "INSERT INTO DevicetodayFeed (SID, Energy, Date) VALUES (@SID, 0 , @yesterday )";
                    using (SQLiteCommand insertDataCmd = new SQLiteCommand(insertDataQuery, connection))
                    {
                        insertDataCmd.Parameters.AddWithValue("@SID", sid);
                        insertDataCmd.Parameters.AddWithValue("@yesterday", yesterday);
                        insertDataCmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Data inserted successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            
            
        }

        private void RegistrationForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void BackBTN_Click(object sender, EventArgs e)
        {
           
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Hide();
        }

        
    }
}
