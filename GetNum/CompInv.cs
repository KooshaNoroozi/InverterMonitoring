using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using OfficeOpenXml;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace GetNum
{
    public partial class CompInv : Form
    {
        List<int> PublicSIDList = new List<int>();
        public CompInv(List<int> SidList)
        {
            PublicSIDList = SidList;
            InitializeComponent();
            GetDataFromSids(SidList);
            FetchCompData();
            InitializeListOfSelectedInverter(SidList);
        }
        List<string[]> DateArrList= new List<string[]>();
        List<int[]> EnergyArrList = new List<int[]>();
        List<string> DateOfLog = new List<string>();
        List<int> EnergyOfLog = new List<int>();
        List<string> Datelist = new List<string>();
        List<string> NameList= new List<string>();
        List<int> EnergyList = new List<int>();
        private void GetDataFromSids(List<int> SidList)
        {
            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (int SID in SidList)
                {
                    string query = @"SELECT * FROM DeviceTodayFeed WHERE SID = @SID";
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
                    EnergyArrList.Add(EnergyOfLog.ToArray()) ;
                    EnergyOfLog.Clear();
                    DateArrList.Add( DateOfLog.ToArray());
                    DateOfLog.Clear();
                    
                }
            }
                       
        }

        private void FromCmpPicker_ValueChanged(object sender, EventArgs e)
        {
            FromCmpPicker.MaxDate = ToCmpPicker.Value;
            ToCmpPicker.MinDate = FromCmpPicker.Value;
            FetchCompData();
        }
        private void ToCmpPicker_ValueChanged(object sender, EventArgs e)
        {
            FromCmpPicker.MaxDate = ToCmpPicker.Value;
            ToCmpPicker.MinDate = FromCmpPicker.Value;
            FetchCompData();
        }
        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton selectedRadioButton = sender as RadioButton;
            FromCmpPicker.MaxDate = ToCmpPicker.Value;
            ToCmpPicker.MinDate = FromCmpPicker.Value;
            if (selectedRadioButton == null)
                return; // Safety check

            if (selectedRadioButton == radiodaycmp)
            {
                FetchCompData();

            }
            else if (selectedRadioButton == radiomonthcmp)
            {
                FetchCompData();
            }
            else if (selectedRadioButton == radioyearcmp)
            {
                FetchCompData();
            }
        }

        private void FetchCompData()
        {
            string firstdate = FromCmpPicker.Value.ToString("yyyy-MM-dd");
            string lastdate = ToCmpPicker.Value.ToString("yyyy-MM-dd");
            DateTime thisdate ;
            Datelist.Clear();
            int counter=0;
            if (ToCmpPicker.Value.Year < FromCmpPicker.Value.Year || (ToCmpPicker.Value.Year == FromCmpPicker.Value.Year && ToCmpPicker.Value.DayOfYear < FromCmpPicker.Value.DayOfYear) )
            {
                MessageBox.Show("Sorry, It seems that you choose the wrong date!!! \nPick another one...", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
           
            while (true)
            {
                thisdate = FromCmpPicker.Value.AddDays(counter);
                Datelist.Add(thisdate.ToString("yyyy-MM-dd"));
                if (thisdate.ToString("yyyy-MM-dd") == lastdate)
                {
                    break;
                }
                counter++;
             //   Console.WriteLine(thisdate);
            }
            

            int[,] Energy2dArray = new int[Datelist.Count, EnergyArrList.Count];
            for (int i = 0; i < Datelist.Count; i++)
            {
                for (int j = 0; j < EnergyArrList.Count; j++)
                {
                    Energy2dArray[i,j] = -1;
                }
            }
            for (int i=0 ; i< Datelist.Count; i++)
            {
                for (int j = 0; j < EnergyArrList.Count; j++)
                {
                    for (int k = 0; k < EnergyArrList[j].Length; k++)
                    {
                        if (DateArrList[j][k]== Datelist[i])
                        {
                            Energy2dArray[i, j] = EnergyArrList[j][k];
                        }
                       
                    }
                }
            }
            for (int i = 0; i < Datelist.Count; i++)
            {
                for (int j = 0; j < EnergyArrList.Count; j++)
                {
                    if (Energy2dArray[i,j] == -1) { Energy2dArray[i,j] = 0; }
                }
            }
            var (result1, result2) = SelectTypeOfCpmChart(Datelist, Energy2dArray);
            DrawCmpChart(result1, result2);
        }
        public (List<string> , int[,] ) SelectTypeOfCpmChart (List<string> Datelist, int[,] Energy2dArray)
        {

            List<string> TimeList = new List<string>();
            

            if (radiodaycmp.Checked || radiomonthcmp.Checked || radioyearcmp.Checked)
            {
               
                if (radiodaycmp.Checked)
                {
                    return (Datelist, Energy2dArray);
                }
                else if (radiomonthcmp.Checked)
                {
                    // creating the the Timelist for month
                    TimeList.Add(Datelist[0].Substring(0, 7));
                    for (int i=0; i<Datelist.Count;i++)
                    {
                        if (! TimeList.Contains(Datelist[i].Substring(0, 7)))
                        {
                            TimeList.Add(Datelist[i].Substring(0, 7));
                        }
                    }
                    int[,] SumOfEnergy = new int[Datelist.Count,Energy2dArray.GetLength(1)];
                    //creating the 2darray of month energy
                    for (int col=0 ; col< Energy2dArray.GetLength(1);col++)
                    {
                        for (int row=0; row < Energy2dArray.GetLength(0); row++)
                        {
                            SumOfEnergy[TimeList.IndexOf(Datelist[row].Substring(0, 7)), col ] += Energy2dArray[row,col];
                        }
                    }
                    return (TimeList, SumOfEnergy);
                }
                else if (radioyearcmp.Checked)
                {
                    TimeList.Add(Datelist[0].Substring(0, 4));
                    for (int i = 0; i < Datelist.Count; i++)
                    {
                        if (!TimeList.Contains(Datelist[i].Substring(0,4)))
                        {
                            TimeList.Add(Datelist[i].Substring(0, 4));
                        }
                    }
                    int[,] SumOfEnergy = new int[Datelist.Count, Energy2dArray.GetLength(1)];
                    //creating the 2darray of month energy
                    for (int col = 0; col < Energy2dArray.GetLength(1); col++)
                    {
                        for (int row = 0; row < Energy2dArray.GetLength(0); row++)
                        {
                            SumOfEnergy[TimeList.IndexOf(Datelist[row].Substring(0,4)), col] += Energy2dArray[row, col];
                        }
                    }
                    return (TimeList, SumOfEnergy);
                }
            }
            else
            {
                MessageBox.Show("Please Select a Report Method ", "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return (TimeList, Energy2dArray);

        }
        public void DrawCmpChart(List<string> Datelist, int[,] Energy2dArray)
        {
            CmpChart.Series.Clear();
            for (int col = 0; col < Energy2dArray.GetLength(1); col++)
            {
                Series series = new Series($"SID : {PublicSIDList[col]}");
                series.ChartType = SeriesChartType.Column; // Choose the appropriate chart type

                // Add data points from Datelist and Energy2dArray
                for (int row = 0; row < Datelist.Count; row++)
                {
                    series.Points.AddXY(Datelist[row], Energy2dArray[row, col]);
                }

                CmpChart.Series.Add(series);
            }
            CmpChart.Invalidate();
        }
        public void InitializeListOfSelectedInverter(List<int> SidList)
        {

            ListOfSelectInv.View = View.Details;
            ListOfSelectInv.FullRowSelect = true;
            ListOfSelectInv.GridLines = true;

            // Add columns to ListView
            ListOfSelectInv.Columns.Add("SID", 75, HorizontalAlignment.Center);
            ListOfSelectInv.Columns.Add("Serial Number", 75, HorizontalAlignment.Center);
            ListOfSelectInv.Columns.Add("Name", 100, HorizontalAlignment.Center);
            ListOfSelectInv.Columns.Add("Owner Phone", 100, HorizontalAlignment.Center);
            ListOfSelectInv.Columns.Add("Address", 250, HorizontalAlignment.Right);

            string connectionString = "Data Source=library.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                foreach (int SID in SidList)
                {
                    string query1 = @"SELECT * FROM DeviceInfoTable WHERE SID = @SID";
                    
                    using (SQLiteCommand command1 = new SQLiteCommand(query1, connection))
                    {
                        command1.Parameters.AddWithValue("@SID", SID);
                        using (SQLiteDataReader reader1 = command1.ExecuteReader())
                        {
                            while (reader1.Read())
                            {
                                ListViewItem item = new ListViewItem(reader1["SID"].ToString());
                                item.SubItems.Add(reader1["serialnum"].ToString());
                                item.SubItems.Add(reader1["ownername"].ToString());
                                item.SubItems.Add(reader1["OwnerPhone"].ToString());
                                item.SubItems.Add(reader1["address"].ToString());

                                ListOfSelectInv.Items.Add(item);
                            }
                        }
                    }

                }
            }


        }

        
    }

}
