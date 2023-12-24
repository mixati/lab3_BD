using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab3_BD
{
    public partial class Form1 : Form
    {
        class Database
        {
            MySqlConnection connection = new MySqlConnection("Server=localhost; Database=dashboard; User ID=root; Password=12345");
            public void openConnection()
            {
                if (connection.State == System.Data.ConnectionState.Closed) connection.Open();
            }
            public void closeConnection()
            {
                if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            }
            public MySqlConnection GetConnection()
            {
                return connection;
            }
        }
        private Database DB;
        private DataTable table;
        private MySqlDataAdapter adapter;
        public Form1()
        {
            InitializeComponent();
            DB = new Database();
            table = new DataTable();
            adapter = new MySqlDataAdapter();
        }

        Label[] _Labels = new Label[6];

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            DB.openConnection();
            MySqlCommand command = new MySqlCommand("SELECT type FROM dashboard.machine_tool_type", DB.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                comboBox1.Items.Add((string)(row.ItemArray[0])); // заполнение типов станков
            }

            DataTable dt2 = new DataTable();
            command = new MySqlCommand("SELECT machine_tool_name FROM dashboard.machine_tool_name", DB.GetConnection());
            
            adapter.SelectCommand = command;
            adapter.Fill(dt2);
            foreach (DataRow row in dt2.Rows)
            {
                comboBox2.Items.Add((string)(row.ItemArray[0])); // заполнение наименований станков
            }
            DB.closeConnection();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;

            _Labels[0] = this.drive1;
            _Labels[1] = this.drive2;
            _Labels[2] = this.drive3;
            _Labels[3] = this.drive4;
            _Labels[4] = this.drive5;
            _Labels[5] = this.drive6;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            current_machine_name.Text = (sender as ComboBox).Text;
            FillHeader();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillHeader();
        }

        private void FillHeader()
        {
            form_header.Text = comboBox1.Text + ' ' + comboBox2.Text;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void cncButton_Click(object sender, EventArgs e)
        {
            DB.openConnection();
            MySqlCommand command = new MySqlCommand("SELECT * FROM dashboard.machine_tool_state", DB.GetConnection());
            adapter.SelectCommand = command;
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            DB.closeConnection();
        }

        private void workingButton_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            DB.openConnection();
            MySqlCommand comamand = new MySqlCommand("SELECT * FROM dashBoard.machine_tool_load", DB.GetConnection());
            adapter.SelectCommand = comamand;
            adapter.Fill(dt);
            DB.closeConnection();

            chart1.Series.Clear();

            double v;
            int s_counter = 0;
            bool onLegendVisible = true;
            bool offLegendVisible = true;
            bool loadLegendVisible = true;
            bool overloadLegendVisible = true;

            foreach (DataRow dr in dt.Rows)
            {
                v = (double.Parse((string)(dr["time_mtl"]))) / 60.0;
                string seriesName = "s" + s_counter.ToString();
                chart1.Series.Add(seriesName);
                chart1.Series[seriesName].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedBar;
                
                if ((string)dr["status"] == "on")
                {
                    chart1.Series[seriesName].Color = Color.Green;
                    chart1.Series[seriesName].LegendText = "on";
                    chart1.Series[seriesName].IsVisibleInLegend = onLegendVisible;
                    chart1.Series[seriesName].SetCustomProperty("PixelPointWidth", "150");
                    onLegendVisible = false;
                }

                if ((string)dr["status"] == "load")
                {
                    chart1.Series[seriesName].Color = Color.Yellow;
                    chart1.Series[seriesName].LegendText = "load";
                    chart1.Series[seriesName].IsVisibleInLegend = loadLegendVisible;
                    chart1.Series[seriesName].SetCustomProperty("PixelPointWidth", "150");
                    loadLegendVisible = false;
                }

                if ((string)dr["status"] == "overload")
                {
                    chart1.Series[seriesName].Color = Color.Orange;
                    chart1.Series[seriesName].LegendText = "overload";
                    chart1.Series[seriesName].IsVisibleInLegend = overloadLegendVisible;
                    chart1.Series[seriesName].SetCustomProperty("PixelPointWidth", "150");
                    overloadLegendVisible = false;
                }

                if ((string)dr["status"] == "off")
                {
                    chart1.Series[seriesName].Color = Color.Black;
                    chart1.Series[seriesName].LegendText = "off";
                    chart1.Series[seriesName].IsVisibleInLegend = offLegendVisible;
                    chart1.Series[seriesName].SetCustomProperty("PixelPointWidth", "150");
                    offLegendVisible = false;
                }

                chart1.Series[seriesName].Points.AddY(v);
                s_counter++;
            }
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.LabelStyle.Interval = 3;
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void driverButton_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            DB.openConnection();
            MySqlCommand command = new MySqlCommand("SELECT drive_status FROM dashboard.drive_status", DB.GetConnection());

            adapter.SelectCommand = command;
            adapter.Fill(dt);
            DB.closeConnection();
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() == "ready")
                {
                    _Labels[i].Text = "Готово";
                    _Labels[i].BackColor = Color.Green;
                }
                else
                {
                    _Labels[i].Text = "Не готово";
                    _Labels[i].BackColor = Color.Red;
                }
                i++;   
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
