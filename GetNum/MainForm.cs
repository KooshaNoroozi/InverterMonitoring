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

namespace GetNum
{

    public partial class MainForm : Form
    {
        private System.Threading.Timer _timer;
        private DateTime _targetTime;
        private Thread ReadThread;
        private Thread RunThread;
        public static Thread monitoringThread;
        private Thread ParseThread;
        public SerialPort _serialPort;
        public string Buffer = null;
        public static int RefreshFlag = 0;
        public MainForm()
        {
            InitializeComponent();
            OpeningPort();
            _targetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 17,12 , 0); // Set target time to 2:00 PM
            _timer = new System.Threading.Timer(CheckTime, null, 0, 60000); // Check every minute
            InitializeThreads();
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
        private void MonitorFlag()
        {
            while (true)
            {
                if (RefreshFlag == 1)
                {
                    UpdateListView();
                    RefreshFlag = 0; // Reset the flag
                }
                Thread.Sleep(100); // Check every 100 milliseconds
            }
        }
        private void CheckTime(object state)
        {
            if (DateTime.Now >= _targetTime && DateTime.Now < _targetTime.AddMinutes(1))
            {
                RunThread.Start();
            }
            
        }
        private void OpeningPort()
        {
            _serialPort = new SerialPort("COM3"); // Replace with your COM port
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            try
            {
                _serialPort.Open();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
        private void InitializeThreads()
        {
            // Initialize and start the COM port listening thread
            ReadThread = new Thread(ListenToComPort);
            ReadThread.IsBackground = true;
            ReadThread.Start();

            // Initialize and start the Run thread
            RunThread = new Thread(RunningMethod);
            RunThread.IsBackground = true;

            monitoringThread = new Thread(MonitorFlag);
            monitoringThread.IsBackground = true;
            monitoringThread.Start();


            //Initialize and start the parsing thraed
            ParseThread = new Thread(ParsingMethod);
            ParseThread.IsBackground = true;
            //  ParseThread.Start();
        }
        private void ParsingMethod()
        {
            int WaitTime = 60000;
            string[,] DataInDB;
            Thread.Sleep(WaitTime);
            string[] Messages = ExtractCMTMessages(Buffer);
            DataInDB = CreateArray(Messages);
            InsertInDataBase(DataInDB);
            while (true)
            {
                // Keep the thread alive
                Thread.Sleep(200);
            }

        }
        private void ChekingError(string[,] DataInDB, int[] TodayEnergy)
        {



            string connectionString = "Data Source=library.db;Version=3;";

            string GetSimNumQuery = "SELECT SimNum , SID FROM DeviceInfoTable";
            List<string> AllSimList = new List<string>();
            List<string> SidList = new List<string>();
            
            List<string> ResSimList = new List<string>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(GetSimNumQuery, connection);
                connection.Open();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AllSimList.Add(reader["SimNum"].ToString());
                        SidList.Add(reader["sid"].ToString());
                    }
                }
            }
            string[,] StatArr = new string[SidList.Count, 3];
                        
            for (int i = 0; i < DataInDB.GetLength(0); i++)
            {
                ResSimList.Add(DataInDB[i, 0]);
            }
            for (int i = 0; i < SidList.Count; i++)
            {
                StatArr[i, 0] = SidList[i];
            }

            for (int i = 0; i < AllSimList.Count; i++)
            {
                StatArr[i, 1] = AllSimList[i];
            }
            for (int i = 0; i < AllSimList.Count; i++)
            {
                int index = ResSimList.IndexOf(AllSimList[i]);
                if (index != -1)// contians the number
                {
                    if (TodayEnergy[index] < 10)
                    {
                        StatArr[i, 2] = "Warning";
                    }
                    else
                    {
                        StatArr[i, 2] = "OK";
                    }
                }
                else // this number not responses
                {
                    StatArr[i, 2] = "Fail";
                }
            }

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                connection.Open();
                string InsStatusQuery;
                DateTime currentDate = DateTime.Today;
                string today = currentDate.ToString("yyyy-MM-dd");
                for (int i = 0; i < StatArr.GetLength(0); i++)
                {
                    if (StatArr[i, 2] == "OK")
                    {
                        InsStatusQuery = " update DeviceTodayFeed set status =@status where (sid=@OKsid And date=@date );";
                        using (SQLiteCommand command = new SQLiteCommand(InsStatusQuery, connection))
                        {
                            command.Parameters.AddWithValue("@OKsid", StatArr[i, 0]);
                            command.Parameters.AddWithValue("@status", StatArr[i, 2]);
                            command.Parameters.AddWithValue("@date", today);
                            command.ExecuteNonQuery();
                        }
                    }
                    if (StatArr[i, 2] == "Warning")
                    {
                        InsStatusQuery = " update DeviceTodayFeed set status =@status where (sid=@Warningsid And date=@date);";
                        using (SQLiteCommand command = new SQLiteCommand(InsStatusQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Warningsid", StatArr[i, 0]);
                            command.Parameters.AddWithValue("@status", StatArr[i, 2]);
                            command.Parameters.AddWithValue("@date", today);
                            command.ExecuteNonQuery();
                        }
                    }
                    if (StatArr[i, 2] == "Fail")
                    {
                        InsStatusQuery = "INSERT INTO DeviceTodayFeed (sid, energy, date, status) VALUES (@sid, 0 , @date, @status);";
                        using (SQLiteCommand command = new SQLiteCommand(InsStatusQuery, connection))
                        {
                            command.Parameters.AddWithValue("@sid", StatArr[i, 0]);
                            command.Parameters.AddWithValue("@date", today);
                            command.Parameters.AddWithValue("@status", StatArr[i, 2]);
                            command.ExecuteNonQuery();
                        }
                    }

                }


            }

        }
        private void InsertInDataBase(string[,] DataInDB)
        {
            
            int[] YesterdayEnergy = new int[DataInDB.GetLength(0)];
            int[] TodayEnergy = new int[DataInDB.GetLength(0)];
            
            string simnum;
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();
                  // Create a table if it doesn't exist
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS DeviceFeedLog (SID INTEGER,Energy Integer,Date TEXT)";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }

                    // Insert data from TextBox
                    DateTime currentDate = DateTime.Today;
                    string today = currentDate.ToString("yyyy-MM-dd");

                    for (int i = 0; i < DataInDB.GetLength(0); i++)
                    {

                        
                        simnum = DataInDB[i, 0];
                        
                        string selectSidQuery = $"SELECT sid FROM deviceinfotable WHERE simnum = '{simnum}'";
                        using (var command = new SQLiteCommand(selectSidQuery, connection))
                        {
                            int sid = Convert.ToInt32(command.ExecuteScalar());

                            // Insert new row into devicefeedlog
                            string insertDataQuery = "INSERT INTO devicefeedlog (sid, energy, date) VALUES (@sid, @energy, @date)";
                            using (var insertCommand = new SQLiteCommand(insertDataQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@sid", sid);
                                insertCommand.Parameters.AddWithValue("@energy", DataInDB[i, 1]);
                                insertCommand.Parameters.AddWithValue("@date", today);
                                insertCommand.ExecuteNonQuery();
                            }

                        }

                    }




                    // Create a table2 if it doesn't exist
                    string createTable2Query = "CREATE TABLE IF NOT EXISTS DeviceTodayFeed (SID INTEGER,Energy Integer,Date TEXT, Status TEXT)";
                    using (SQLiteCommand createTable2Cmd = new SQLiteCommand(createTable2Query, connection))
                    {
                        createTable2Cmd.ExecuteNonQuery();
                    }

                    // Insert data from TextBox
                    currentDate = DateTime.Today;
                    today = currentDate.ToString("yyyy-MM-dd");
                    string yesterday = (currentDate.AddDays(-1)).ToString("yyyy-MM-dd");
                    
                    for (int i = 0; i < DataInDB.GetLength(0); i++)
                    {
                        simnum = DataInDB[i, 0];
                        string selectSidQuery = $"SELECT sid FROM deviceinfotable WHERE simnum = '{simnum}'";
                        using (var command = new SQLiteCommand(selectSidQuery, connection))
                        {
                            int sid = Convert.ToInt32(command.ExecuteScalar());

                            // Insert new row into devicefeedlog
                            string SelectyesterdayEnergy = $"SELECT energy FROM devicefeedlog WHERE(sid={sid} AND date='{yesterday}')";
                            using (var selectyesterdayenergyCMD = new SQLiteCommand(SelectyesterdayEnergy, connection))
                            {
                                var yesenergy= selectyesterdayenergyCMD.ExecuteScalar();
                                YesterdayEnergy[i] = yesenergy != null ? Convert.ToInt32(yesenergy) : 0 ;
                                selectyesterdayenergyCMD.ExecuteNonQuery();
                            }
                        }
                    }
                    for (int i = 0; i < DataInDB.GetLength(0); i++)
                    {
                        TodayEnergy[i] = Convert.ToInt32(DataInDB[i, 1]) - YesterdayEnergy[i];
                    }




                    for (int i = 0; i < DataInDB.GetLength(0); i++)
                    {
                        simnum = DataInDB[i, 0];
                        string selectSidQuery = $"SELECT sid FROM deviceinfotable WHERE simnum = '{simnum}'";
                        using (var command = new SQLiteCommand(selectSidQuery, connection))
                        {
                            int sid = Convert.ToInt32(command.ExecuteScalar());
                            // Insert new row into devicefeedlog
                            string insertDataQuery = "INSERT INTO DeviceTodayFeed (sid, energy, date) VALUES (@sid, @energy, @date)";
                            using (var insertCommand = new SQLiteCommand(insertDataQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@sid", sid);
                                insertCommand.Parameters.AddWithValue("@energy", TodayEnergy[i]);
                                insertCommand.Parameters.AddWithValue("@date", today);
                                insertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

           
            ChekingError(DataInDB, TodayEnergy);
            RunThread.Abort();
            ReadThread.Abort();
            ParseThread.Abort();


        }
        private string[,] CreateArray(string[] Messages)
        {
            string[,] resultArray = new string[Messages.Length, 2];

            for (int t = 0; t < Messages.Length; t++)
            {
                string input = Messages[t];

                // Split the input string into lines
                string[] lines = input.Split(new[] { '\n' }, StringSplitOptions.None);

                for (int i = 0; i < lines.Length - 1; i++)
                {
                    // Regex to find 13-digit substrings starting with +98
                    Match match = Regex.Match(lines[i], @"\+98\d{10}");

                    if (match.Success)
                    {
                        resultArray[t, 0] = match.Value;
                        resultArray[t, 1] = lines[i + 1];
                        break; // Stop after finding the first match
                    }
                }
            }

            HashSet<string> uniqueSims = new HashSet<string>();
            for (int j = 0; j < Messages.Length; j++)
            {
                uniqueSims.Add(resultArray[j, 0]);
            }
            int sims = uniqueSims.Count;
            string[] arrayOfSims = uniqueSims.ToArray();
            string[,] finalArray = new string[sims, 4]; // [0-simnumbers][1-highword][2-lowword][3-energy]
            string[,] returnArray = new string[sims, 2];
            for (int k = 0; k < sims; k++)
            {
                finalArray[k, 0] = arrayOfSims[k];
            }
            for (int j = 0; j < Messages.Length; j++)
            {
                for (int i = 0; i < sims; i++)
                {
                    if (finalArray[i, 0] == resultArray[j, 0] && resultArray[j, 1].Contains("MB30119="))
                    {
                        finalArray[i, 1] = resultArray[j, 1].Substring(8);
                    }
                    if (finalArray[i, 0] == resultArray[j, 0] && resultArray[j, 1].Contains("MB30120="))
                    {
                        finalArray[i, 2] = resultArray[j, 1].Substring(8);
                    }
                }
            }

            for (int i = 0; i < sims; i++)
            {
                int energy = (int.Parse(finalArray[i, 2]) << 16) + int.Parse(finalArray[i, 1]);
                finalArray[i, 3] = energy.ToString();
            }
            for (int i = 0; i < sims; i++)
            {
                returnArray[i, 0] = finalArray[i, 0];
                returnArray[i, 1] = finalArray[i, 3];
            }
            return returnArray;
        }
        public static string[] ExtractCMTMessages(string input)
        {
            List<string> messages = new List<string>();
            string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("+CMT:"))
                {
                    string message = lines[i];
                    if (i + 1 < lines.Length) message += "\n" + lines[i + 1];
                    messages.Add(message);
                }
            }

            return messages.ToArray();
        }
        private void ListenToComPort()
        {
            _serialPort.DataReceived += SerialPort_DataReceived;

            while (true)
            {
                // Keep the thread alive
                Thread.Sleep(200);
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            string data = _serialPort.ReadExisting();
            this.Invoke((MethodInvoker)delegate
            {
                // Update UI with received data
                Buffer += data;
                RecieveTextBox.AppendText(data);
            });

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            base.OnFormClosing(e);
            ReadThread.Abort();
            RunThread.Abort();
            _serialPort.Close();
            Application.Exit();


        }
        private void SingleInverterForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            InitializeListOfInverter();
        }
        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            AuthenticationForm FrstPageFrom = new AuthenticationForm();
            try
            {
                _serialPort.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            FrstPageFrom.Show();
            this.Hide();
        }
        private void ReadSms()
        {

            try
            {
                _serialPort.WriteLine("AT+CMGD=0,4\r"); // delete pre sms
                Thread.Sleep(400);

                _serialPort.WriteLine("AT+CMGF=1\r"); // Set SMS text mode
                Thread.Sleep(400);

                _serialPort.WriteLine("AT+CPMS=\"SM\"\r"); // Select SIM storage
                Thread.Sleep(400);

                _serialPort.WriteLine("AT+CNMI=2,2,0,0,0\r");
                Thread.Sleep(400);

                // Read a specific message (example: message at index 1)
               // _serialPort.WriteLine("AT+CMGR=1\r");
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
        public void RunningMethod()
        {
            try
            {
                SendingMessage();
                while (true)
                {
                    // Keep the thread alive
                    Thread.Sleep(200);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("ThreadAbortException caught. Cleaning up...");
            }
            finally
            {
                RefreshFlag = 1;
            }
            
        }
        private void SendingMessage()
        {
            string QuestionEnergyLowWord = "MB30119=?";
            string QuestionEnergyHighWord = "MB30120=?";
            ReadSms();
            string connectionString = "Data Source=library.db;Version=3;";
            string GetSimNumQuery = "SELECT SimNum FROM DeviceInfoTable";
            List<string> columnData = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(GetSimNumQuery, connection);
                connection.Open();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        columnData.Add(reader["SimNum"].ToString());
                    }
                }
            }

            string[] ListOfNumbers = columnData.ToArray();

            //opening the port for sending message
            if (!_serialPort.IsOpen)
            {
                _serialPort.PortName = "COM3";
                _serialPort.BaudRate = 9600;
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            //initializing the sms procedure

            _serialPort.WriteLine("AT+CMGF=1\r"); // Set SMS text mode
            Thread.Sleep(400);
            _serialPort.WriteLine("AT+CSCS=\"GSM\"" + '\r');
            Thread.Sleep(400);






            

            //sending batch messages
            Thread.Sleep(2000);
            ParseThread.Start();
            foreach (var item in ListOfNumbers)
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.WriteLine("AT+CMGS=" + "\"" + item + "\"" + '\r');
                    Thread.Sleep(400);
                    _serialPort.WriteLine(QuestionEnergyHighWord + (char)26 + '\r');
                    Thread.Sleep(400);

                }
                else
                {
                    try
                    {
                        _serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
                Thread.Sleep(5000);

            } 
            Thread.Sleep(2000);
            foreach (var item in ListOfNumbers)
            {
                if (_serialPort.IsOpen)
                {
                    Thread.Sleep(400);
                    _serialPort.WriteLine("AT+CMGS=" + "\"" + item + "\"" + '\r');
                    Thread.Sleep(300);
                    _serialPort.WriteLine(QuestionEnergyLowWord + (char)26 + '\r');
                    Thread.Sleep(300);

                }
                else
                {
                    try
                    {
                        _serialPort.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
                Thread.Sleep(5000);

            }
            RunThread.Abort();
            
        }
        private void RunBTN_Click(object sender, EventArgs e)
        {
            RunThread.Start();
        }
        private void RegisterBTN_Click(object sender, EventArgs e)
        {
            this.Hide();
            RegistrationForm RegistrationForm = new RegistrationForm();
            RegistrationForm.Show();
            _serialPort.Close();
            ReadThread.Abort();
            RunThread.Abort();
            ParseThread.Abort();
           
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
                                spcdate = thatdate.ToString("yyyy-MM-dd");

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

        
    }
}
