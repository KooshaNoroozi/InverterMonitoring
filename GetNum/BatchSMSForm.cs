using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace GetNum
{
    public partial class BatchSMSForm : Form
    {
        List<string> ListOfSimNum = new List<string>();
        public SerialPort _serialPort;
        public bool LineIsAvialbale = true;
        private Thread ReadThread;
        private Thread RunThread;
        public static Thread monitoringThread;
        private Thread ParseThread;
        private string Question;
        private int ErrNum = 0;
        private int IgnNum = 0;
        private string[,] DataInDB;
        private int TrialNum = 0;
        public string Buffer = null;
        private string buffer = string.Empty;
        private string serialResponse = string.Empty;
        private ManualResetEventSlim waitForOkEvent = new ManualResetEventSlim(false);

        public BatchSMSForm(List<int> SidList)
        {
            InitializeComponent();
            GetDataFromSids(SidList);
            
        }

        private void GetDataFromSids(List<int> SidList)
        {
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (int SID in SidList)
                {
                    string query = @"SELECT SimNum FROM DeviceInfoTable WHERE SID = @SID";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SID", SID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListOfSimNum.Add(reader["SimNum"].ToString());
                            }

                        }

                    }

                }
            }

        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            ReadThread.Abort();
            _serialPort.Close();
            _serialPort.Dispose();

        }
        private void SendMSG_Click(object sender, EventArgs e)
        {
            RunThread.Start();
            Loading.Visible = true;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            this.Invoke((MethodInvoker)delegate
            {
                string data = _serialPort.ReadExisting();
                // Update UI with received data
                buffer += data;
                if (LineIsAvialbale || data.Contains("OK") || data.Contains(">") || data.Contains("ERR"))
                {

                    LogBox.Text = buffer;
                    LogBox.SelectionStart = LogBox.Text.Length;
                    LogBox.ScrollToCaret();


                    serialResponse += data;
                    // Check if the response contains "OK"
                    if (serialResponse.Contains("OK") || serialResponse.Contains(">") || serialResponse.Contains("AT") || serialResponse.Contains("+C"))
                    {
                        TrialNum = 0;
                        waitForOkEvent.Set();  // Signal the waiting thread
                    }
                }

            });




            
           

        }


        public void SendCommand(string command)
        {
            waitForOkEvent.Reset();  // Reset the event before sending the command
            serialResponse = string.Empty;  // Clear any previous response

            // Send the command to the serial port
            _serialPort.WriteLine(command);

            // Wait for the "OK" response (timeout in milliseconds can be adjusted)
            if (!waitForOkEvent.Wait(10000))  // Timeout after 5 seconds
            {
                throw new TimeoutException("No OK response received within the timeout period.");
            }
        }

        private string ListAvailablePorts()
        {
            try
            {
                // Get all available COM ports
                string[] ports = SerialPort.GetPortNames();
                string myavport = ports[0];
                return myavport[3].ToString();
                // Display available ports in a Label or MessageBox for reference
            }
            catch (IndexOutOfRangeException c)
            {
                MessageBox.Show(c.Message);
                return "";
            }

        }

        private void InitializeSerialPort()
        {
            _serialPort = new SerialPort($"COM{ListAvailablePorts()}"); // Replace with your COM port
            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
            OpeningPort();
        }

        private void ReadInitializeThreads()
        {
            // Initialize and start the COM port listening thread
            ReadThread = new Thread(ListenToComPort);
            ReadThread.IsBackground = true;
            ReadThread.Start();

        }

        private void ListenToComPort()
        {
            ReadSms();
            _serialPort.DataReceived += SerialPort_DataReceived;

            while (true)
            {

                Thread.Sleep(200);
            }
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
            ReadSms();
        }

        private void ReadSms()
        {

            try
            {
                _serialPort.WriteLine("AT+CMGD=0,4\r"); // delete pre sms
                Thread.Sleep(500);

                _serialPort.WriteLine("AT+CMGF=1\r"); // Set SMS text mode
                Thread.Sleep(500);

                _serialPort.WriteLine("AT+CPMS=\"SM\"\r"); // Select SIM storage
                Thread.Sleep(500);

                _serialPort.WriteLine("AT+CNMI=1,2,0,0,0\r");
                Thread.Sleep(500);
                _serialPort.WriteLine("ATE0\r"); // delete pre sms
                Thread.Sleep(500);


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }

        private void TrySndCmd(string Command)
        {
            try
            {
                SendCommand(Command);  // Example command
                                       // Continue execution after receiving "OK"
            }
            catch (TimeoutException ex)
            {
                ErrNum++;
                TrialNum++;
                if (TrialNum < 4)
                {
                    Thread.Sleep(5000);
                    TrySndCmd(Command);
                }
                else
                {
                    IgnNum++;
                }

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

                RunInitializeThreads();

            }


        }

        private void RunInitializeThreads()
        {

            // Initialize and start the Run thread
            RunThread = new Thread(RunningMethod);
            RunThread.IsBackground = true;

        }

        private void SendingMessage()
        {
            Question = QuesBox.Text;
            TrySndCmd("AT+CMGF=1\r");
            Thread.Sleep(300);
            TrySndCmd("AT+CSCS=\"GSM\"" + '\r');
            Thread.Sleep(300);

            foreach (var item in ListOfSimNum)
            {

                LineIsAvialbale = false;
                TrySndCmd("AT+CMGS=" + "\"" + item + "\"" + '\r');
                TrySndCmd(Question + (char)26 + '\r');
                LineIsAvialbale = true;
                Thread.Sleep(3000);
            }
            ParsInitializeThreads();

            
            
        }
        private void ParsInitializeThreads()
        {
            //Initialize and start the parsing thraed
            ParseThread = new Thread(ParsingMethod);
            ParseThread.IsBackground = true;
            ParseThread.Start();
        }
        public static string[] ExtractCMTMessages(string input)//only uses for one line answers
        {
            List<string> messages = new List<string>();
            string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("+CMT:") && !lines[i].Contains("rancel") && lines[i].Contains("+989"))
                {
                    string message = lines[i];
                    if (i + 1 < lines.Length) message += "\n" + lines[i + 1];
                    messages.Add(message);
                }
            }

            return messages.ToArray();
        }
        private void ParsingMethod()
        {
            MessageBox.Show("Im in parsing ");
            string IgnoranceRep = "In this run " + IgnNum + " Ignorance happend";
            string ErrRep = "In this run " + ErrNum + " errors happend and "+ IgnoranceRep;

            int WaitTime = 60000;

            
            Thread.Sleep(WaitTime);
            
            _serialPort.Close();
            MessageBox.Show("Serial closed");

           



            string[] Messages = ExtractCMTMessages(buffer);
            MessageBox.Show("message created :"+ Messages.Length);
            for (int f = 0; f < Messages.Length; f++)
            {
                MessageBox.Show(Messages[f]);
            }
            DataInDB = CreateArray(Messages);
            MessageBox.Show("dataindb created :" + DataInDB.Length);

           
            MessageBox.Show(ErrRep);
           
            Thread.Sleep(2000);
            ReadThread.Abort();
            MessageBox.Show("ReadThread created :" + DataInDB.Length);
            RunThread.Abort();
            MessageBox.Show("RunThread created :" + DataInDB.Length);
            ParseThread.Abort();
            MessageBox.Show("ParseThread created :" + DataInDB.Length);



        }
        private string ShowInOutPut(string[,] array)
        {
            StringBuilder sb = new StringBuilder();
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    sb.Append(array[i, j].ToString() + "\t"); // Tab for spacing
                }
                sb.AppendLine(); // New line after each row
            }

            return sb.ToString();
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


            return resultArray;
        }

        private void SaveAnswers_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files|*.csv";
            saveFileDialog.Title = "Save a CSV File";
            saveFileDialog.ShowDialog();

            // If the file path is valid
            if (!string.IsNullOrEmpty(saveFileDialog.FileName))
            {
                ExportArrayToCSV(DataInDB, saveFileDialog.FileName);
                MessageBox.Show("Export to CSV Successful!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ExportArrayToCSV(string[,] dataArray, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                int rows = dataArray.GetLength(0);
                int columns = dataArray.GetLength(1);

                // Write each row to the CSV file
                for (int i = 0; i < rows; i++)
                {
                    string[] rowValues = new string[columns];

                    for (int j = 0; j < columns; j++)
                    {
                        rowValues[j] = dataArray[i, j].ToString(); // Convert each element to string
                    }

                    // Join the values with commas and write to the file
                    writer.WriteLine(string.Join(",", rowValues));
                }
            }
        }

        private void BatchSMSForm_Load(object sender, EventArgs e)
        {
            InitializeSerialPort();
            RunInitializeThreads();
            ReadInitializeThreads();
        }

        private void Loading_Click(object sender, EventArgs e)
        {

        }
    }
}
