using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO.Ports;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace GetNum
{

    public partial class MainForm : Form
    {
       
        private Thread ReadThread;
        private Thread RunThread;
        public static Thread monitoringThread;
        private Thread ParseThread;
        
        public string Buffer = null;
        public static int RefreshFlag = 0;
        public MainForm()
        {
            InitializeComponent();
           
            InitializeListOfInverter();
            LoadData();
            ListOfInv.MouseDoubleClick += new MouseEventHandler(ListOfInv_MouseDoubleClick);
        }
        private void UpdateListView()
        {
            if (ListOfInv.InvokeRequired)
            {
                ListOfInv.Invoke(new Action(UpdateListView));
            }
            else
            {
                // Your logic to update the ListView
                InitializeListOfInverter();
            }
        }
        
        private void ListOfInv_MouseDoubleClick(object sender, EventArgs e)
        {
            if (ListOfInv.SelectedItems.Count > 0)
            {
                // Get the selected item
                ListViewItem selectedItem = ListOfInv.SelectedItems[0];
                // Extract the data from the selected item
                string data = selectedItem.SubItems[0].Text; // Adjust the index based on your data
                // Create and show the DetailForm
                SingleInverterForm SingleInverterForm = new SingleInverterForm(data);
                SingleInverterForm.FormClosed += new FormClosedEventHandler(SingleInverterForm_FormClosed);
                SingleInverterForm.ShowDialog();
            }
        }
        public static void LoadData()
        {
            ListOfInv.Items.Clear();//clearing previous list to creating new one
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                                                                          
                 string query = @"
                                   SELECT
                                        d.sid,
                                        d.serialnum,
                                        d.ownername,
                                        AVG(dt.energy) AS avg_energy,
                                        SUM(dt.energy) AS tot_energy,
                                        d.address,
                                        (SELECT dt2.status
                                         FROM devicetodayfeed dt2
                                         WHERE dt2.sid = d.sid
                                         ORDER BY dt2.date DESC
                                         LIMIT 1) AS last_status
                                    FROM
                                        deviceinfotable d
                                    JOIN
                                        devicetodayfeed dt ON d.sid = dt.sid
                                    GROUP BY
                                        d.sid;
                                ";
          

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(" " +reader["sid"].ToString());
                            item.SubItems.Add(reader["serialnum"].ToString());
                            item.SubItems.Add(reader["ownername"].ToString());
                            item.SubItems.Add(reader["last_status"].ToString());
                            switch (reader["last_status"].ToString())
                            {
                                case "OK" :
                                    item.ImageIndex = 0;
                                    break;
                                case "Warning":
                                    item.ImageIndex = 1;
                                    break;
                                case "Fail":
                                    item.ImageIndex = 2;
                                    break;
                                default:
                                    item.ImageIndex = 3;
                                    break;
                            }
                            item.SubItems.Add((Convert.ToInt32(reader["avg_energy"])).ToString()+ " Wh");
                            item.SubItems.Add(((Convert.ToInt32(reader["tot_energy"]))/1000).ToString()+" KWh");
                            item.SubItems.Add(reader["address"].ToString());

                          
                            ListOfInv.Items.Add(item);
                        }
                    }
                }
                

            }
        }
        public static void InitializeListOfInverter()
        {
            
            ListOfInv.Columns.Clear();
            ListOfInv.View = View.Details;
            ListOfInv.FullRowSelect = true;
            ListOfInv.GridLines = true;

            // Add columns to ListView
            ListOfInv.Columns.Add("SID", 50, HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Serial Number", 90, HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Name", 150, HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Status", 90, HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Avrage Energy", 90 , HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Total Energy", 90, HorizontalAlignment.Center);
            ListOfInv.Columns.Add("Address", 250, HorizontalAlignment.Right);

            ListOfInv.ListViewItemSorter = new ListViewItemComparer(0);

            LoadData();


        }
        private class ListViewItemComparer : IComparer
        {
            private readonly int columnIndex;

            public ListViewItemComparer(int columnIndex)
            {
                this.columnIndex = columnIndex;
            }

            public int Compare(object x, object y)
            {
                return string.Compare(((ListViewItem)x).SubItems[columnIndex].Text,
                    ((ListViewItem)y).SubItems[columnIndex].Text);
            }
        }
       
       
      
       
       
        
       
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            base.OnFormClosing(e);
            
            Application.Exit();


        }
        private void SingleInverterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            InitializeListOfInverter();
        }
        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            AuthenticationForm FrstPageFrom = new AuthenticationForm();
           
            FrstPageFrom.Show();
            this.Hide();
        }
       
        
       
        
        private void RegisterBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            RegistrationForm RegistrationForm = new RegistrationForm();
            RegistrationForm.Show();
           

        }
        private void CmpBTN_Click(object sender, EventArgs e)
        {
            if (ListOfInv.CheckedItems.Count <2)
            {
                
                MessageBox.Show("Please select at least 2 inverters....", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int SID;
            List<int> SidList = new List<int>();
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT sid FROM DeviceInfoTable WHERE sid= @sid";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    foreach (ListViewItem selectedItem in ListOfInv.CheckedItems)
                    {
                        command.Parameters.AddWithValue("@sid", selectedItem.Text);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SID = int.Parse(reader["SID"].ToString());
                                SidList.Add(SID);
                            }
                        }
                    }
                }
            }
            CompInv CompInv = new CompInv(SidList);
            CompInv.ShowDialog();
        }

        static Random random = new Random();
        static int GetRandomNumber(int userInput)
        {
            // Ensure userInput is non-negative
            userInput = Math.Max(userInput, 0);

            // Generate a random number between 0 and 30000
            int randomNumber = random.Next(30000);

            // Add the user input to the random number
            int result = randomNumber + userInput;

            return result + 5000;
        }
        public static string ConvertGregorianToSolar(DateTime gregorianDate)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(gregorianDate);
            int month = persianCalendar.GetMonth(gregorianDate);
            int day = persianCalendar.GetDayOfMonth(gregorianDate);

            return $"{year}-{month:D2}-{day:D2}";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    int rndenergy = 0;
                    int totalenergy = 0;
                    DateTime currentDate = DateTime.Today;
                    DateTime thatdate;
                    string spcdate = null;
                    int numdate;
                    // Create a table if it doesn't exist
                    for (int k = 1; k < 5; k++)
                    {
                        totalenergy = 0;
                        string autoinsertQ = "insert into devicefeedlog (sid,date,energy) values (@k,@spcdate,@totalenergy) ";
                        using (SQLiteCommand inscmd = new SQLiteCommand(autoinsertQ, connection))
                        {
                            for (int i = 0; i < 1100; i++)
                            {
                                Console.WriteLine(i + ":  \n");
                                rndenergy = GetRandomNumber(i);
                                numdate = i;
                                thatdate = currentDate.AddDays(numdate);
                                spcdate = ConvertGregorianToSolar(thatdate);
                               
                               
                                if (spcdate.Substring(5, 2) == "03" || spcdate.Substring(5, 2) == "06") { rndenergy = (int)Math.Round(rndenergy * 0.9); }
                                if (spcdate.Substring(5, 2) == "02" || spcdate.Substring(5, 2) == "07") { rndenergy = (int)Math.Round(rndenergy * 0.8); }
                                if (spcdate.Substring(5, 2) == "01" || spcdate.Substring(5, 2) == "08") { rndenergy = (int)Math.Round(rndenergy * 0.7); }
                                if (spcdate.Substring(5, 2) == "12" || spcdate.Substring(5, 2) == "09") { rndenergy = (int)Math.Round(rndenergy * 0.6); }
                                if (spcdate.Substring(5, 2) == "11" || spcdate.Substring(5, 2) == "10") { rndenergy = (int)Math.Round(rndenergy * 0.5); }
                                totalenergy += rndenergy;
                                inscmd.Parameters.AddWithValue("@k", k);
                                inscmd.Parameters.AddWithValue("@totalenergy", totalenergy);
                                inscmd.Parameters.AddWithValue("@spcdate", spcdate);
                                inscmd.ExecuteNonQuery();
                            }

                        }
                    }



                    MessageBox.Show("Data changed successfully!", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            InitializeListOfInverter();
            string searchTerm = SearchBox.Text.ToLower();
            var filteredItems = ListOfInv.Items.Cast<ListViewItem>()
                .Where(item => item.SubItems.Cast<ListViewItem.ListViewSubItem>()
                    .Any(subItem => subItem.Text.ToLower().Contains(searchTerm)))
                .ToArray();

            ListOfInv.BeginUpdate();
            ListOfInv.Items.Clear();
            
            if (string.IsNullOrEmpty(searchTerm))
            {
                // If search box is empty, show all original items
                InitializeListOfInverter();
            }
            else
            {
                ListOfInv.Items.AddRange(filteredItems);
            }

            ListOfInv.EndUpdate();
        }
        private void ListOfInv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListOfInv.ListViewItemSorter = new ListViewItemComparer(e.Column);
            ListOfInv.Sort();
        }
        private void DeleteinvBTN_Click(object sender, EventArgs e)
        {
            if (ListOfInv.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please Select an inverter first...", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult result = MessageBox.Show("Are you sure you want to delete inverter? \nPlease be careful, All data and history logs will be lost after deleting an inverter. ", "Confirmation", MessageBoxButtons.YesNo , MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string connectionString = "Data Source=library.db;Version=3;";
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        foreach (ListViewItem selectedItem in ListOfInv.CheckedItems)
                        {
                            string delQuery = "delete from deviceinfotable where SID=@SID ;";
                            using (SQLiteCommand delCmd = new SQLiteCommand(delQuery, connection))
                            {
                                delCmd.Parameters.AddWithValue("@SID", selectedItem.Text);
                                delCmd.ExecuteNonQuery();
                            }
                            delQuery = "delete from DeviceTodayFeed where SID=@SID ;";
                            using (SQLiteCommand delCmd = new SQLiteCommand(delQuery, connection))
                            {
                                delCmd.Parameters.AddWithValue("@SID", selectedItem.Text);
                                delCmd.ExecuteNonQuery();
                            }
                            delQuery = "delete from devicefeedlog where SID=@SID ;";
                            using (SQLiteCommand delCmd = new SQLiteCommand(delQuery, connection))
                            {
                                delCmd.Parameters.AddWithValue("@SID", selectedItem.Text);
                                delCmd.ExecuteNonQuery();
                            }
                        }
                        InitializeListOfInverter();
                        MessageBox.Show("Selected Inverters deleted successfully. ", "Confirmation!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }

                }
            }
            else
            {
                return;
            }
            
        }
        private void ClearLogBTn_Click(object sender, EventArgs e)
        {
            
                if (ListOfInv.CheckedItems.Count == 0)
                {
                    MessageBox.Show("Please Select an inverter first...", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                DialogResult result = MessageBox.Show("Are you sure you want to delete all logs of inverter? \nPlease be careful, All data and history logs will be lost after deleting and it's not recoverable! ", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Data Source=library.db;Version=3;";
                    using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                    {
                        try
                        {
                            connection.Open();
                            foreach (ListViewItem selectedItem in ListOfInv.CheckedItems)
                            {
                                 
                                string delQuery = "delete from DeviceTodayFeed where (SID=@SID AND date > (SELECT MIN(date) FROM devicetodayfeed));";
                                using (SQLiteCommand delCmd = new SQLiteCommand(delQuery, connection))
                                {
                                    delCmd.Parameters.AddWithValue("@SID", selectedItem.Text);
                                    delCmd.ExecuteNonQuery();
                                }
                                delQuery = "delete from devicefeedlog where  (SID=@SID AND date > (SELECT MIN(date) FROM devicefeedlog)) ;";
                                using (SQLiteCommand delCmd = new SQLiteCommand(delQuery, connection))
                                {
                                    delCmd.Parameters.AddWithValue("@SID", selectedItem.Text);
                                    delCmd.ExecuteNonQuery();
                                }
                            }
                            InitializeListOfInverter();
                            MessageBox.Show("Selected Inverters deleted successfully. ", "Confirmation!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}");
                        }

                    }
                }
                else
                {
                    return;
                }

            
        }
    }
}
