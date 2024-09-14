using System;
using System.IO;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using System.Threading;
using System.IO.Ports;
using System.Drawing;
using System.Globalization;

namespace GetNum
{
    public partial class SingleInverterForm : Form
    {
        int SID;
        int flag=0;
        int portflag = 0;
        public string Question = null;
        public string ChatBuffer = null;
        private string buffer = string.Empty;
        private Thread ReadThread;
        public SerialPort _serialPort;
        string SerialNum = null, SimNum = null, OwnerName = null, OwnerPhone = null, Address = null;
        List<string> DateOfLog = new List<string>();
        List<int> EnergyOfLog = new List<int>();

        int[] EnergyArray;
        string[] DateArray;
        string[] year = { "1403", "1404", "1405", "1406", "1407", "1408", "1409", "1410" };
        string[] month = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };

        public SingleInverterForm(string data)
        {
            InitializeComponent();
            InverterDashBoardOpen_DataFetch(data);
            SID = Int32.Parse(data);
            
        }
       
        private void InitializeThread()
        {
            ReadThread = new Thread(ListenToComPort);
            ReadThread.IsBackground = true;
            ReadThread.Start();
        }
        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort("COM3"); // Replace with your COM port
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }
       
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = _serialPort.ReadExisting();
            this.Invoke((MethodInvoker)delegate
            {
                // Update UI with received data
                buffer += data;
                LogBox.Text = buffer;
                
                AnswerBox.Text= string.Join(Environment.NewLine, ExtractCMTMessages(buffer));
            });
        }
        private void ListenToComPort()
        {
            while (true)
            {
                _serialPort.DataReceived += SerialPort_DataReceived;
                // Keep the thread alive
                Thread.Sleep(200);
            }
        }
        public static List<string> ExtractCMTMessages(string input)
        {
            List<string> messages = new List<string>();
            string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("+CMT:"))
                {
                    string message = lines[i];

                    
                    if (i + 1 < lines.Length) message = lines[i + 1]+"\r\n";
                    //if (i + 2 < lines.Length && !lines[i+2].Contains("AT") ) message += "\n" + lines[i + 2];
                    for(int j=2; j+i< lines.Length && !lines[i + j].Contains("AT") && !lines[i + j].Contains("+CMT"); j++)
                    {
                         message += lines[i + j]+"\r\n";
                    }

                    messages.Add(message);
                }
            }

            return messages;
        }
        public void OpeningPort()
        {
            if (!_serialPort.IsOpen)
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
              
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
        public static string ConvertGregorianToSolar(DateTime gregorianDate)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(gregorianDate);
            int month = persianCalendar.GetMonth(gregorianDate);
            int day = persianCalendar.GetDayOfMonth(gregorianDate);

            return $"{year}-{month:D2}-{day:D2}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            ReadThread.Abort();
            _serialPort.Close();
            _serialPort.Dispose();
          
        }
       
        public void InverterDashBoardOpen_DataFetch(string data)
        {
            SID = Int32.Parse(data);
            string connectionString = "Data Source=library.db;Version=3;";

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query = @"SELECT * FROM DeviceInfoTable WHERE sid= @sid";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@sid",SID);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                            SerialNum = reader["serialnum"].ToString();
                            SimNum = reader["simnum"].ToString();
                            OwnerName = reader["ownername"].ToString();
                            OwnerPhone = reader["ownerphone"].ToString();
                            Address = reader["address"].ToString();
                            Console.WriteLine("Done");

                        }
                        else
                        {
                            Console.WriteLine("No data found for the given name.");
                        }
                    }

                }
                query = @"SELECT* FROM DeviceTodayFeed WHERE SID = @SID";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SID", SID);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EnergyOfLog.Add(int.Parse(reader["Energy"].ToString()));
                            DateOfLog.Add(reader["Date"].ToString());
                        }

                    }

                }
                EnergyArray = EnergyOfLog.ToArray();
                DateArray = DateOfLog.ToArray();

            }
            InverterSimpleData_load();
            ListOfAllData_Load();
        }
        private void ListOfAllData_Load()
        {
            InitializeListOfAllResponses();
            InitializeListOfChat();

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
        private void InitializeListOfChat()
        {

            ListOfChat.Columns.Clear();
            ListOfChat.View = View.Details;
            ListOfChat.FullRowSelect = true;
            ListOfChat.GridLines = true;

            // Add columns to ListView
            ListOfChat.Columns.Add("Date", 100, HorizontalAlignment.Center);

            ListOfChat.Columns.Add("Questions", 150, HorizontalAlignment.Center);

            ListOfChat.Columns.Add("Answers", 300, HorizontalAlignment.Center);

            ListOfChat.ListViewItemSorter = new ListViewItemComparer(0);

            LoadChatData();
        }
        private void InitializeListOfAllResponses()
        {
            //  ListOfAllresponse = new FilterableListView();
            ListOfAllresponse.Columns.Clear();
            ListOfAllresponse.View = View.Details;
            ListOfAllresponse.FullRowSelect = true;
            ListOfAllresponse.GridLines = true;

            // Add columns to ListView
            ListOfAllresponse.Columns.Add("Date", 100, HorizontalAlignment.Center);

            ListOfAllresponse.Columns.Add("Energy", 100, HorizontalAlignment.Center);

            ListOfAllresponse.Columns.Add("Status", 70, HorizontalAlignment.Center);

            ListOfAllresponse.ListViewItemSorter = new ListViewItemComparer(0);

            LoadListData();
        }
        private void LoadChatData()
        {
            ListOfChat.Items.Clear();
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query1 = @"SELECT * FROM DeviceChatTable WHERE SID = @SID";

                using (SQLiteCommand command1 = new SQLiteCommand(query1, connection))
                {
                    command1.Parameters.AddWithValue("@SID", SID);
                    using (SQLiteDataReader reader1 = command1.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            ListViewItem item = new ListViewItem(reader1["Date"].ToString());
                            item.SubItems.Add(reader1["Questions"].ToString().Replace("\n"  , "  "));
                            item.SubItems.Add(reader1["Answers"].ToString().Replace("\n"  , "  "));
                            ListOfChat.Items.Add(item);
                        }
                    }
                }


            }


        }
        private void LoadListData()
        {
            ListOfAllresponse.Items.Clear();
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string query1 = @"SELECT * FROM DeviceTodayFeed WHERE SID = @SID";

                using (SQLiteCommand command1 = new SQLiteCommand(query1, connection))
                {
                    command1.Parameters.AddWithValue("@SID", SID);
                    using (SQLiteDataReader reader1 = command1.ExecuteReader())
                    {
                        while (reader1.Read())
                        {
                            ListViewItem item = new ListViewItem(reader1["Date"].ToString());
                            item.SubItems.Add(reader1["Energy"].ToString());
                            item.SubItems.Add(reader1["status"].ToString());
                            switch (reader1["status"].ToString())
                            {
                                case "OK":
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

                            ListOfAllresponse.Items.Add(item);
                        }
                    }
                }


            }
                     
            
        }
       
        private void InverterSimpleData_load()
        {
            SerialnumLabel.Text = SerialNum;
            ownernameLabel.Text = OwnerName;
            simNumLabel.Text = SimNum;
            addressLabel.Text = Address;
            OwnerphoneLabel.Text = OwnerPhone;
        }
        private void FirstLoadGraph()
        {
            AllDataChartLoad();
            DrawMonthlyGraphs(MonthYearPicker.Value.ToString("yyyy-MM"));
            TotalChartLoad();
            DrawYearlyGraphs(yearpicker.Value.ToString("yyyy"));


        }
        private void DrawYearlyGraphs(string selectedOption)
        {

            selectedOption = yearpicker.Value.ToString("yyyy");
            int[] oneyeararr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i].Contains(selectedOption))
                {
                    if (DateArray[i].Contains("-01-"))
                    {
                        oneyeararr[0] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-02-"))
                    {
                        oneyeararr[1] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-03-"))
                    {
                        oneyeararr[2] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-04-"))
                    {
                        oneyeararr[3] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-05-"))
                    {
                        oneyeararr[4] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-06-"))
                    {
                        oneyeararr[5] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-07-"))
                    {
                        oneyeararr[6] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-08-"))
                    {
                        oneyeararr[7] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-09-"))
                    {
                        oneyeararr[8] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-10-"))
                    {
                        oneyeararr[9] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-11-"))
                    {
                        oneyeararr[10] += EnergyArray[i];
                    }
                    if (DateArray[i].Contains("-12-"))
                    {
                        oneyeararr[11] += EnergyArray[i];
                    }
                }
            }
            YearlyChartLoad(oneyeararr);
        }
        private void Yearpicker_ValueChanged(object sender, EventArgs e)
        {
          
            string selectedOption = yearpicker.Value.ToString("yyyy");
            DrawYearlyGraphs(selectedOption);
                    
        }

        private void YearlyChartLoad(int[] oneyeararr)
        {
            
            // Clear any existing series
            YearlyChart.Series.Clear();

            // Create a new series (e.g., "Energy")
            Series energySeries = new Series("Energy");
            energySeries.ChartType = SeriesChartType.Column;
            
            // Add data points from your arrays
            for (int i = 0; i < oneyeararr.Length; i++)
            {
               energySeries.Points.AddXY(month[i], oneyeararr[i]);
            }
           
           
            // Add the series to the chart
            YearlyChart.Series.Add(energySeries);

            // Customize chart appearance (if needed)
            YearlyChart.ChartAreas[0].AxisX.Interval = 1;
        //    YearlyChart.Series["Energy"].IsValueShownAsLabel = true;
            YearlyChart.ChartAreas[0].AxisX.Title = "month";
            YearlyChart.ChartAreas[0].AxisY.Title = "Energy";
            //       YearlyChart.ChartAreas[0].AxisX.Maximum = 15;
            //       YearlyChart.ChartAreas[0].AxisX.Minimum = 0;
            YearlyChart.Invalidate();

        }
        private void DrawMonthlyGraphs(string selectedOption)
        {

            selectedOption = MonthYearPicker.Value.ToString("yyyy-MM");
            int[] oneMontharr=new int[31] ;
            for (int j =0; j<31;j++)
            {
                oneMontharr[j] = 0;
            }
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i].Contains(selectedOption))
                {
                    if (DateArray[i].EndsWith("-01"))
                    {
                        oneMontharr[0] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-02"))
                    {
                        oneMontharr[1] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-03"))
                    {
                        oneMontharr[2] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-04"))
                    {
                        oneMontharr[3] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-05"))
                    {
                        oneMontharr[4] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-06"))
                    {
                        oneMontharr[5] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-07"))
                    {
                        oneMontharr[6] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-08"))
                    {
                        oneMontharr[7] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-09"))
                    {
                        oneMontharr[8] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-10"))
                    {
                        oneMontharr[9] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-11"))
                    {
                        oneMontharr[10] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-12"))
                    {
                        oneMontharr[11] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-13"))
                    {
                        oneMontharr[12] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-14"))
                    {
                        oneMontharr[13] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-15"))
                    {
                        oneMontharr[14] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-16"))
                    {
                        oneMontharr[15] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-17"))
                    {
                        oneMontharr[16] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-18"))
                    {
                        oneMontharr[17] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-19"))
                    {
                        oneMontharr[18] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-20"))
                    {
                        oneMontharr[19] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-21"))
                    {
                        oneMontharr[20] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-22"))
                    {
                        oneMontharr[21] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-23"))
                    {
                        oneMontharr[22] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-24"))
                    {
                        oneMontharr[23] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-25"))
                    {
                        oneMontharr[24] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-26"))
                    {
                        oneMontharr[25] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-27"))
                    {
                        oneMontharr[26] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-28"))
                    {
                        oneMontharr[27] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-29"))
                    {
                        oneMontharr[28] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-30"))
                    {
                        oneMontharr[29] += EnergyArray[i];
                    }
                    if (DateArray[i].EndsWith("-31"))
                    {
                        oneMontharr[30] += EnergyArray[i];
                    }
                }
            }
            MonthlyChartLoad(oneMontharr);
        }
        private void MonthYearPicker_ValueChanged(object sender, EventArgs e)
        {
            string selectedOption = MonthYearPicker.Value.ToString("yyyy-MM");
            DrawMonthlyGraphs(selectedOption);
        }
        private void MonthlyChartLoad(int[] oneMontharr)
        {

            // Clear any existing series
            monthlyChart.Series.Clear();

            // Create a new series (e.g., "Energy")
            Series energySeries = new Series("Energy");
            energySeries.ChartType = SeriesChartType.Column;

            // Add data points from your arrays
            for (int i = 0; i < oneMontharr.Length; i++)
            {
                energySeries.Points.AddXY((i+1).ToString(), oneMontharr[i]);
            }


            // Add the series to the chart
            monthlyChart.Series.Add(energySeries);

            // Customize chart appearance (if needed)
            monthlyChart.ChartAreas[0].AxisX.Interval = 1;
            //    YearlyChart.Series["Energy"].IsValueShownAsLabel = true;
            monthlyChart.ChartAreas[0].AxisX.Title = "day";
            monthlyChart.ChartAreas[0].AxisY.Title = "Energy";
            //       YearlyChart.ChartAreas[0].AxisX.Maximum = 15;
            //       YearlyChart.ChartAreas[0].AxisX.Minimum = 0;
            monthlyChart.Invalidate();




        }
        private void AllDataChartLoad()
        {
            // Clear any existing series
            AllDataChart.Series.Clear();
            string firstdate = FromPicker.Value.ToString("yyyy-MM-dd");
            string lastdate = ToPicker.Value.ToString("yyyy-MM-dd");
            if (ToPicker.Value.Year < FromPicker.Value.Year || (ToPicker.Value.Year == FromPicker.Value.Year && ToPicker.Value.DayOfYear < FromPicker.Value.DayOfYear))
            {
                MessageBox.Show("Sorry, It seems that you choose the wrong date!!! \nPick another one...", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int Fromindex = 0;
            int Toindex = DateArray.Length - 1;
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i] == firstdate) { Fromindex = i; }
                if (DateArray[i] == lastdate) { Toindex = i; }
            }
            

            // Create a new series (e.g., "Energy")
            Series energySeries = new Series("Energy");
            energySeries.ChartType = SeriesChartType.Column;

            // Add data points from your arrays
            for (int i =Fromindex; i <= Toindex; i++)
            {
                energySeries.Points.AddXY(DateArray[i], EnergyArray[i]);
            }

            // Add the series to the chart
            AllDataChart.Series.Add(energySeries);

            // Customize chart appearance (if needed)
            AllDataChart.ChartAreas[0].AxisX.Title = "Date";
            AllDataChart.ChartAreas[0].AxisY.Title = "Energy";

            // Show the chart
            AllDataChart.Invalidate();
        }
        private void MakeListBTN_Click(object sender, EventArgs e)
        {
            ExportList.Items.Clear();
            ExportList.Clear();
            ExportList.Enabled = true;
            ExportList.Visible = true;
            if (radioday.Checked || radiomonth.Checked || radioyear.Checked)
            {
                ExportList.Enabled = true;
                if (radioday.Checked)
                {
                    ShowDailyList();
                }
                else if (radiomonth.Checked)
                {
                    ShowMonthlyList();
                }
                else if (radioyear.Checked)
                {
                    ShowYearlyList();
                }
            }
            else 
            {
                MessageBox.Show("Please Select a Report Method First", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void ShowMonthlyList()
        {
            ExportList.View = View.Details;
            ExportList.FullRowSelect = true;
            ExportList.GridLines = true;
            ExportList.Columns.Add("Month", 90, HorizontalAlignment.Right);
            ExportList.Columns.Add("Energy", 90, HorizontalAlignment.Center);




            string firstmonth = FromMonthPicker.Value.ToString("yyyy-MM-01");
            string lastmonth = ToMonthPicker.Value.ToString("yyyy-MM-31");
            if (ToMonthPicker.Value.Month == 12) {  lastmonth = ToMonthPicker.Value.ToString("yyyy-MM-29"); }
            if (6 < ToMonthPicker.Value.Month && ToMonthPicker.Value.Month < 12) {  lastmonth = ToMonthPicker.Value.ToString("yyyy-MM-30"); }
        
            int countOfMonth = (ToMonthPicker.Value.Year - FromMonthPicker.Value.Year) * 12 + ToMonthPicker.Value.Month - FromMonthPicker.Value.Month + 1;
            if (countOfMonth < 0 )
            {
                MessageBox.Show("Sorry, It seems that you choose the wrong date!!!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            int[] monthenergy = new int[countOfMonth];
            string[] monthstring = new string[countOfMonth];
            int count =0 ;
            for (int i = 0; i < countOfMonth; i++)
            {
                monthstring[i] = (FromMonthPicker.Value.AddMonths(count)).ToString("yyyy-MM");
                count++;
            }
            int Fromindex = 0;
            int Toindex = DateArray.Length - 1; ;
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i] == firstmonth) { Fromindex = i; }
                if (DateArray[i] == lastmonth) { Toindex = i; }
            }
            for (int i = Fromindex; i <= Toindex; i++)
            {
                string yearmonth = DateArray[i].Substring(0,7);
                for (int j = 0; j < countOfMonth; j++)
                {
                   
                    if (yearmonth == monthstring[j])
                    {
                        monthenergy[j] += EnergyArray[i];
                    }
                }
            }
            for (int i = 0 ; i < monthenergy.Length ; i++)
            {
                ListViewItem item = new ListViewItem(monthstring[i]);
                item.SubItems.Add(monthenergy[i].ToString());
                ExportList.Items.Add(item);
            }

        }
        private void ShowDailyList()
        {
            ExportList.View = View.Details;
            ExportList.FullRowSelect = true;
            ExportList.GridLines = true;
            ExportList.Columns.Add("Date", 90, HorizontalAlignment.Right);
            ExportList.Columns.Add("Energy", 90, HorizontalAlignment.Center);
            


            string firstdate= FromDatePicker.Value.ToString("yyyy-MM-dd");
            string lastdate= ToDatePicker.Value.ToString("yyyy-MM-dd");
            if (ToDatePicker.Value < FromDatePicker.Value)
            {
                MessageBox.Show("Sorry, It seems that you choose the wrong date!!!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int Fromindex = 0;
            int Toindex = DateArray.Length-1;
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i]== firstdate) { Fromindex = i;  }
                if (DateArray[i] == lastdate) { Toindex = i; }
            }
            for (int i = Fromindex; i <= Toindex ; i++)
            {
                ListViewItem item = new ListViewItem(DateArray[i]);
                item.SubItems.Add(EnergyArray[i].ToString());
                ExportList.Items.Add(item);
            }

        }
        private void ShowYearlyList()
        {
            ExportList.View = View.Details;
            ExportList.FullRowSelect = true;
            ExportList.GridLines = true;
            ExportList.Columns.Add("Year", 90, HorizontalAlignment.Right);
            ExportList.Columns.Add("Energy", 90, HorizontalAlignment.Center);



            string firstyear = FromYearPicker.Value.ToString("yyyy-01-01");
            string lastyear = ToYearPicker.Value.ToString("yyyy-12-29");
            int countOfyear = (ToYearPicker.Value.Year - FromYearPicker.Value.Year)+1;
            if (countOfyear < 0)
            {
                MessageBox.Show("Sorry, It seems that you choose the wrong date!!!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int[] yearenergy = new int[countOfyear];
            string[] yearstring = new string[countOfyear];
            int count = 0;
            for (int i = 0; i < countOfyear; i++)
            {
                yearstring[i] = (FromYearPicker.Value.AddYears(count)).ToString("yyyy");
                count++;
            }
            int Fromindex = 0;
            int Toindex = DateArray.Length-1;
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i] == firstyear) { Fromindex = i; }
                if (DateArray[i] == lastyear) { Toindex = i; }
            }
            for (int i = Fromindex; i <= Toindex; i++)
            {
                string year = DateArray[i].Substring(0, 4);
                for (int j = 0; j < countOfyear; j++)
                {

                    if (year == yearstring[j])
                    {
                        yearenergy[j] += EnergyArray[i];
                    }
                }
            }
            for (int i = 0; i < yearenergy.Length; i++)
            {
                ListViewItem item = new ListViewItem(yearstring[i]);
                item.SubItems.Add(yearenergy[i].ToString());
                ExportList.Items.Add(item);
            }

        }
        private void EditInfoBTN_Click(object sender, EventArgs e)
        {
            //hiding labels
            SerialnumLabel.Visible = false;
            ownernameLabel.Visible = false;
            simNumLabel.Visible = false;
            OwnerphoneLabel.Visible = false;
            addressLabel.Visible = false;

            //show textboxes
            serialnumbox.Visible = true;
            ownernamebox.Visible = true;
            simNumBox.Visible = true;
            ownerPhonebox.Visible = true;
            addressbox.Visible = true;

            //show buttons
            ConfirmBTN.Visible = true;
            CancelBTN.Visible = true;


            // Transfer text from labels to textboxes
            serialnumbox.Text = SerialnumLabel.Text;
            ownernamebox.Text = ownernameLabel.Text;
            simNumBox.Text = simNumLabel.Text;
            ownerPhonebox.Text = OwnerphoneLabel.Text;
            addressbox.Text = addressLabel.Text;
        }
        private void CancelEditing()
        {
            //show labels
            SerialnumLabel.Visible = true;
            ownernameLabel.Visible = true;
            simNumLabel.Visible = true;
            OwnerphoneLabel.Visible = true;
            addressLabel.Visible = true;

            //hide textboxes
            serialnumbox.Visible = false;
            ownernamebox.Visible = false;
            simNumBox.Visible = false;
            ownerPhonebox.Visible = false;
            addressbox.Visible = false;

            //hide buttons
            ConfirmBTN.Visible = false;
            CancelBTN.Visible = false;
        }
        private void CancelBTN_Click(object sender, EventArgs e)
        {
            CancelEditing();
        }
        private void ConfirmBTN_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //checking for repeatance
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM DeviceInfoTable WHERE SimNum = @SimNum", connection))
                    {
                        cmd.Parameters.AddWithValue("@SimNum", simNumBox.Text);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        
                        if (count > 0 && (simNumBox.Text != simNumLabel.Text) )
                        {
                            // SIM number already exists, handle accordingly (e.g., show an error message)
                            MessageBox.Show("SIM number already exists!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            simNumBox.Text = simNumLabel.Text;
                            return;
                        }

                    }
                    using (SQLiteCommand cmd = new SQLiteCommand("SELECT COUNT(*) FROM DeviceInfoTable WHERE SerialNum = @SerialNum", connection))
                    {
                        cmd.Parameters.AddWithValue("@SerialNum", serialnumbox.Text);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0 && (serialnumbox.Text != SerialnumLabel.Text))
                        {
                            // SIM number already exists, handle accordingly (e.g., show an error message)
                            MessageBox.Show("Serial number already exists!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            serialnumbox.Text = SerialnumLabel.Text;
                            return;
                        }

                    }




                    string EditQuery = "UPDATE  deviceinfotable SET  serialnum=@serialnum , simnum =@simnum , OwnerName=@OwnerName , OwnerPhone=@OwnerPhone , Address=@Address  where SID=@SID ;";
                    using (SQLiteCommand EditCmd = new SQLiteCommand(EditQuery, connection))
                    {
                        EditCmd.Parameters.AddWithValue("@SID", SID);
                        EditCmd.Parameters.AddWithValue("@serialnum", serialnumbox.Text);
                        EditCmd.Parameters.AddWithValue("@simnum", simNumBox.Text);
                        EditCmd.Parameters.AddWithValue("@OwnerName", ownernamebox.Text);
                        EditCmd.Parameters.AddWithValue("@OwnerPhone", ownerPhonebox.Text);
                        EditCmd.Parameters.AddWithValue("@Address", addressbox.Text);
                        EditCmd.ExecuteNonQuery();
                    }


                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }

            }
            InverterDashBoardOpen_DataFetch(SID.ToString());
            //show labels
            SerialnumLabel.Visible = true;
            ownernameLabel.Visible = true;
            simNumLabel.Visible = true;
            OwnerphoneLabel.Visible = true;
            addressLabel.Visible = true;

            //hide textboxes
            serialnumbox.Visible = false;
            ownernamebox.Visible = false;
            simNumBox.Visible = false;
            ownerPhonebox.Visible = false;
            addressbox.Visible = false;

            //hide buttons
            ConfirmBTN.Visible = false;
            CancelBTN.Visible = false;
        }
        private void FromDatePicker_ValueChanged(object sender, EventArgs e)
        {
            FromDatePicker.MaxDate = ToDatePicker.Value;
            ToDatePicker.MinDate = FromDatePicker.Value;
        }
        private void ToDatePicker_ValueChanged(object sender, EventArgs e)
        {
            FromDatePicker.MaxDate = ToDatePicker.Value;
            ToDatePicker.MinDate = FromDatePicker.Value;
        }
        private void FromMonthPicker_ValueChanged(object sender, EventArgs e)
        {
            FromMonthPicker.MaxDate = ToMonthPicker.Value;
            ToMonthPicker.MinDate = FromMonthPicker.Value;
        }
        private void ToMonthPicker_ValueChanged(object sender, EventArgs e)
        {
            FromMonthPicker.MaxDate = ToMonthPicker.Value;
            ToMonthPicker.MinDate = FromMonthPicker.Value;
        }
        private void FromYearPicker_ValueChanged(object sender, EventArgs e)
        {
            FromYearPicker.MaxDate = ToYearPicker.Value;
            ToYearPicker.MinDate = FromYearPicker.Value;
        }

       

        private void ListOfAllresponse_ColumnClick(object sender, ColumnClickEventArgs e)
        {

            ListOfAllresponse.ListViewItemSorter = new ListViewItemComparer(e.Column);
            ListOfAllresponse.Sort();
           
        }

        private void SearchBTN_Click(object sender, EventArgs e)
        {
            InitializeListOfAllResponses();
            string searchTerm = SearchBox.Text.ToLower();
            var filteredItems = ListOfAllresponse.Items.Cast<ListViewItem>()
                .Where(item => item.SubItems.Cast<ListViewItem.ListViewSubItem>()
                    .Any(subItem => subItem.Text.ToLower().Contains(searchTerm)))
                .ToArray();

            ListOfAllresponse.BeginUpdate();
            ListOfAllresponse.Items.Clear();

            if (string.IsNullOrEmpty(searchTerm))
            {
                // If search box is empty, show all original items
                InitializeListOfAllResponses();
            }
            else
            {

                ListOfAllresponse.Items.AddRange(filteredItems);

            }

            ListOfAllresponse.EndUpdate();
        }

        
        

        private void SndMsgBtn_Click_1(object sender, EventArgs e)
        {
            Question = SndBox.Text;
            DialogResult result = MessageBox.Show($"Are you sure you want to send '{Question}' to the inverter? \nPlease be careful, The  Wrong commands can cause serious problems! ", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            if (flag == 0)
            {
                _serialPort.WriteLine("AT+CMGF=1\r"); // Set SMS text mode
                Thread.Sleep(300);
                _serialPort.WriteLine("AT+CSCS=\"GSM\"" + '\r');
                Thread.Sleep(300);
                flag = 1;
            }
            _serialPort.WriteLine("AT+CMGS=" + "\"" + SimNum + "\"" + '\r');
            Thread.Sleep(300);
            _serialPort.WriteLine(Question + (char)26 + '\r');
            Thread.Sleep(300);
            if (AskedBox.Text.Length>0)
            {
                AskedBox.AppendText(Environment.NewLine);
            }
            AskedBox.AppendText(Question);
            SndBox.Clear();
        }

        

        private void SingleInverterForm_Load(object sender, EventArgs e)
        {
            
            FirstLoadGraph();
            InitializeSerialPort();
            InitializeThread();
            OpeningPort();
            ReadSms();
            InitializeComboBox();
        }
        private void InitializeComboBox()
        {
            DefaultQuestionCombo.Items.Add("سایر");
            DefaultQuestionCombo.Items.Add("مقدار توان");
            DefaultQuestionCombo.Items.Add("زمان اینورتر");
            DefaultQuestionCombo.Items.Add("تعیین ساعت اینورتر");
            DefaultQuestionCombo.Items.Add("کیفیت سینگال آنتن دهی");
            DefaultQuestionCombo.Items.Add("آخرین اتفاق رخ داده");
            DefaultQuestionCombo.Items.Add("گزارش اتفاق با شماره معلوم");
            DefaultQuestionCombo.Items.Add("سوال از مقدار رجیستر های داخلی ");
            DefaultQuestionCombo.Items.Add("تعیین رجیستر های داخلی");

            
            DefaultQuestionCombo.SelectedIndex = 0;
        }
        

        private void DefaultQuestionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DefaultQuestionCombo.SelectedIndex == 0)
            {
                SndBox.Clear();
            }
            if (DefaultQuestionCombo.SelectedIndex == 1)
            {
                SndBox.Text = "P?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 2)
            {
                SndBox.Text = "T?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 3)
            {
                SndBox.Text = "T=.";
            }
            if (DefaultQuestionCombo.SelectedIndex == 4)
            {
                SndBox.Text = "Q?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 5)
            {
                SndBox.Text = "EVT?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 6)
            {
                SndBox.Text = "EVT__?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 7)
            {
                SndBox.Text = "MB__?";
            }
            if (DefaultQuestionCombo.SelectedIndex == 8)
            {
                SndBox.Text = "MB__=__";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            DateTime currentDate = DateTime.Today;
            string today = ConvertGregorianToSolar(currentDate);
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Create a table if it doesn't exist
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS DeviceChatTable (SID INTEGER ,Date TEXT , Questions TEXT,Answers TEXT)";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, connection))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }
                    
                    // Insert data from TextBox
                    string insertDataQuery = "INSERT INTO DeviceChatTable (SID, Date, Questions, Answers) VALUES (@SID, @Date, @Questions, @Answers)";
                    using (SQLiteCommand insertDataCmd = new SQLiteCommand(insertDataQuery, connection))
                    {
                        insertDataCmd.Parameters.AddWithValue("@SID", SID);
                        insertDataCmd.Parameters.AddWithValue("@Date", today);
                        insertDataCmd.Parameters.AddWithValue("@Questions",AskedBox.Text);
                        insertDataCmd.Parameters.AddWithValue("@Answers", AnswerBox.Text);
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

        

        private void ExportBTN_Click(object sender, EventArgs e)
        {

               SaveFileDialog saveFileDialog1 = new SaveFileDialog();
               saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
               saveFileDialog1.Title = "Export to CSV";
               StringBuilder sb = new StringBuilder();
               int flag = 0;
               foreach (ColumnHeader ch in ExportList.Columns)
               {
                    sb.Append(ch.Text);
                    if (flag == 0) { sb.Append(","); }
                    flag = 1;
               }
               sb.AppendLine();

               foreach (ListViewItem lvi in ExportList.Items)
               {
                   flag = 0;
                   foreach (ListViewItem.ListViewSubItem lvs in lvi.SubItems)
                   {
                       if (lvs.Text.Trim() == string.Empty)
                            sb.Append(" ,");
                       else
                           sb.Append(lvs.Text);
                       if (flag == 0)
                           sb.Append(","); 
                       flag = 1;

                }
                   sb.AppendLine();

               }
               DialogResult dr = saveFileDialog1.ShowDialog();
               if (dr == DialogResult.OK)
               {
                   using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                   {
                       sw.Write(sb.ToString());
                   }
               }
               
            

        }
        private void FromPicker_ValueChanged(object sender, EventArgs e)
        {
            FromPicker.MaxDate = ToPicker.Value;
            ToPicker.MinDate = FromPicker.Value;
            AllDataChartLoad();
        }
        private void ToPicker_ValueChanged(object sender, EventArgs e)
        {
            FromPicker.MaxDate = ToPicker.Value;
            ToPicker.MinDate = FromPicker.Value;
            AllDataChartLoad();
        }
        private void TotalChartLoad()
        {
           
            int[] totalarr = new int[8];
            for (int j = 0; j < 8; j++)
            {
                totalarr[j] = 0;
            }
            for (int i = 0; i < DateArray.Length; i++)
            {
                if (DateArray[i].Contains("1403"))
                {
                    totalarr[0] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1404"))
                {
                    totalarr[1] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1405"))
                {
                    totalarr[2] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1406"))
                {
                    totalarr[3] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1407"))
                {
                    totalarr[4] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1408"))
                {
                    totalarr[5] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1409"))
                {
                    totalarr[6] += EnergyArray[i];
                }
                if (DateArray[i].Contains("1410"))
                {
                    totalarr[7] += EnergyArray[i];
                }
            }

            // Clear any existing series
            TotalChart.Series.Clear();

            // Create a new series (e.g., "Energy")
            Series energySeries = new Series("Energy");
            energySeries.ChartType = SeriesChartType.StackedColumn;

            // Add data points from your arrays
            for (int i = 0; i < totalarr.Length; i++)
            {
                energySeries.Points.AddXY(year[i], totalarr[i]);
            }


            // Add the series to the chart
            TotalChart.Series.Add(energySeries);

            // Customize chart appearance (if needed)
            TotalChart.ChartAreas[0].AxisX.Interval = 1;
            //    YearlyChart.Series["Energy"].IsValueShownAsLabel = true;
            TotalChart.ChartAreas[0].AxisX.Title = "month";
            TotalChart.ChartAreas[0].AxisY.Title = "Energy";
            //       YearlyChart.ChartAreas[0].AxisX.Maximum = 15;
            //       YearlyChart.ChartAreas[0].AxisX.Minimum = 0;
            TotalChart.Invalidate();


        }
        
        
    }
    
}
