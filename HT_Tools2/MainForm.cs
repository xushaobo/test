using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using HT_Tools2.Properties;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HT_Tools2
{
    public partial class MainForm : BaseForm, ICmdAction
    {
        protected Log _log = new Log();

        private DataTable _valusetTable = new DataTable();

        private string _maxValue = "-100";

        private double _qty = 0;
        private double _sumValue = 0;

        private Queue<double> dataQueue = new Queue<double>();
        private int num = 1; //每次删除增加几个点
        private int sum = 1;

        private ICmdAction _this;

        protected MainForm()
        {
            InitializeComponent();

            MaximizeBox = false;
            MinimizeBox = false;

            _valusetTable.Columns.Add("Time");
            _valusetTable.Columns.Add("Value");

            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DataSource = _valusetTable;
            _valusetTable.DefaultView.Sort = "Time DESC";

            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            this.comboBox1.Text = "0.1"; //默认量程

            _this = (ICmdAction) this;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CreateCmd();
            pictureBox1.Image = Resources.logo;
            InitChart();

            Type dgvType = this.dataGridView1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        /// <summary>
        /// 获取命令结合 用于创建btn,toolstrip
        /// </summary>
        /// <returns></returns>
        private List<Cmds> GetCmds()
        {
            var cmds = new List<Cmds>();

            var cmd = new Cmds
            {
                Cmd = "开始接收",
                Click = (s, e) => { _this.Start(); }
            };

            cmds.Add(cmd);

            var cmd2 = new Cmds
            {
                Cmd = "停止接收",
                Click = (s, e) => { _this.Stop(); }
            };

            cmds.Add(cmd2);

            var cmd3 = new Cmds
            {
                Cmd = "重启应用",
                Click = (s, e) =>
                {
#if DEBUG
                    MessageBox.Show(_valusetTable.Rows.Count.ToString());
#endif
                    Application.Restart();
                }
            };

            cmds.Add(cmd3);

            var cmd4 = new Cmds
            {
                Cmd = "打开",
                Click = (s, e) =>
                {
                    var of = new OpenFileDialog
                    {
                        InitialDirectory = _log._direc,
                        Filter = "日志文件(*.ht2)|*.ht2"
                    };

                    if (of.ShowDialog() != DialogResult.OK) return;

                    var f = new FileForm
                    {
                        LogFile = of.FileName
                    };
                    f.ShowDialog();
                }
            };

            cmds.Add(cmd4);

            return cmds;
        }

        /// <summary>
        /// 接受数据处理
        /// </summary>
        /// <param name="str"></param>
        protected void MyAction(string str)
        {
            //忽略负值
            if (float.Parse(str) < 0.000001)
            {
                return;
            }

            Invoke(new Action(() =>
            {
                var t = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                label1.Text = str;
                var row = _valusetTable.NewRow();
                row["time"] = t;
                row["Value"] = str;
                _valusetTable.Rows.Add(row);
                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];

                //计算最大值

                var temp = float.Parse(str);

                if (float.Parse(_maxValue) < temp)
                {
                    _maxValue = str;
                }

                label2.Text = $"{_maxValue}";
                _qty++;
                _sumValue += temp;
                //计算平均
                label3.Text = Math.Round(_sumValue / _qty, 3).ToString(CultureInfo.InvariantCulture);

                UpdateChart(str);
            }));
        }

        /// <summary>
        /// 初始化图表
        /// </summary>
        private void InitChart()
        {
            //时间控件t开始
            timer1.Start();
            //
            //
            //定义图表区域
            this.chart1.ChartAreas.Clear();
            ChartArea chartArea1 = new ChartArea("C1");


            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorX.SelectionColor = Color.SkyBlue;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.AutoScroll = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.CursorY.SelectionColor = Color.SkyBlue;

            //chartArea1.CursorX.IntervalType = DateTimeIntervalType.Auto;
            //chartArea1.AxisX.ScaleView.Zoomable = false;
            chartArea1.AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.All; //启用X轴滚动条按钮
            //chartArea1.AxisX.LabelStyle.Format = "mm:ss";

            chartArea1.BackColor = Color.AliceBlue; //背景色
            chartArea1.BackSecondaryColor = Color.White; //渐变背景色
            chartArea1.BackGradientStyle = GradientStyle.TopBottom; //渐变方式
            chartArea1.BackHatchStyle = ChartHatchStyle.None; //背景阴影
            chartArea1.BorderDashStyle = ChartDashStyle.NotSet; //边框线样式
            chartArea1.BorderWidth = 1; //边框宽度
            chartArea1.BorderColor = Color.Black;


            chartArea1.AxisX.MajorGrid.Enabled = true;
            chartArea1.AxisY.MajorGrid.Enabled = true;

            // Axis
            chartArea1.AxisY.Title = @"Value";
            chartArea1.AxisY.LabelAutoFitMinFontSize = 5;
            chartArea1.AxisY.LineWidth = 2;
            chartArea1.AxisY.LineColor = Color.Black;
            chartArea1.AxisY.Enabled = AxisEnabled.True;

            chartArea1.AxisX.Title = @"Time";
            chartArea1.AxisX.IsLabelAutoFit = true;
            chartArea1.AxisX.LabelAutoFitMinFontSize = 5;
            chartArea1.AxisX.LabelStyle.Angle = -15;


            chartArea1.AxisX.LabelStyle.IsEndLabelVisible = true; //show the last label
            chartArea1.AxisX.Interval = 10;
            chartArea1.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea1.AxisX.IntervalType = DateTimeIntervalType.NotSet;
            chartArea1.AxisX.TextOrientation = TextOrientation.Auto;
            chartArea1.AxisX.LineWidth = 2;
            chartArea1.AxisX.LineColor = Color.Black;
            chartArea1.AxisX.Enabled = AxisEnabled.True;
            chartArea1.AxisX.ScaleView.MinSizeType = DateTimeIntervalType.Months;
            chartArea1.AxisX.Crossing = 0;

            chartArea1.Position.Height = 85;
            chartArea1.Position.Width = 85;
            chartArea1.Position.X = 5;
            chartArea1.Position.Y = 7;


            chart1.BackGradientStyle = GradientStyle.TopBottom;
            //图表的边框颜色、
            chart1.BorderlineColor = Color.FromArgb(26, 59, 105);
            //图表的边框线条样式
            chart1.BorderlineDashStyle = ChartDashStyle.Solid;
            //图表边框线条的宽度
            chart1.BorderlineWidth = 2;
            //图表边框的皮肤
            chart1.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

            this.chart1.ChartAreas.Add(chartArea1);


            //
            //
            //定义存储和显示点的容器
            this.chart1.Series.Clear();
            Series series1 = new Series("S1");
            series1.ChartArea = "C1";
            this.chart1.Series.Add(series1);
            //设置图表显示样式
            series1.ToolTip = "#VALX,#VALY"; //鼠标停留在数据点上，显示XY值
            series1.ChartType = SeriesChartType.Spline; // type
            series1.BorderWidth = 2;
            series1.Color = Color.Red;
            //series1.XValueType = ChartValueType.DateTime;//x axis type
            series1.XValueType = ChartValueType.Int64;
            series1.YValueType = ChartValueType.Double; //y axis type

            //Marker
            series1.MarkerStyle = MarkerStyle.Square;
            series1.MarkerSize = 4;
            series1.MarkerColor = Color.Black;

            this.chart1.Legends.Clear();
            this.chart1.ChartAreas[0].AxisY.Minimum = 0;
            this.chart1.ChartAreas[0].AxisY.Maximum = double.Parse(comboBox1.Text);
            this.chart1.ChartAreas[0].AxisX.Interval = 5;
            this.chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            this.chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;


            //this.chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";

            //设置标题
            this.chart1.Titles.Clear();
            this.chart1.Titles.Add("st1");
            this.chart1.Titles[0].Text = "实时数据 mg/m³";
            this.chart1.Titles[0].ForeColor = Color.RoyalBlue;
            this.chart1.Titles[0].Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);

            this.chart1.Series[0].Points.Clear();


            chart1.ChartAreas[0].AxisX.Interval = 10D;
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 60D;
        }

        /// <summary>
        /// 创建btn,toolstrip
        /// </summary>
        private void CreateCmd()
        {
            var cmds = GetCmds();
            var posx = 10;
            foreach (var cmdse in cmds)
            {
                var cmd = new ToolStripMenuItem
                {
                    Text = cmdse.Cmd
                };
                cmd.Click += cmdse.Click;
                命令ToolStripMenuItem.DropDownItems.Add(cmd);

                var btn = new Button
                {
                    Text = cmdse.Cmd
                };

                btn.Click += cmdse.Click;

                groupBox1.Controls.Add(btn);

                btn.Location = new Point(posx, 33);

                posx += btn.Width + 10;
            }
        }

        /// <summary>
        /// 更新图标数据
        /// </summary>
        /// <param name="str"></param>
        private void UpdateChart(string str)
        {
            try
            {
                this.chart1.ChartAreas[0].AxisY.Maximum = double.Parse(comboBox1.Text);
                DateTime t = System.DateTime.Now;
                if (dataQueue.Count > 10000)
                {
                    //先出列
                    for (int i = 0; i < num; i++)
                    {
                        dataQueue.Dequeue();
                        sum--;
                    }
                }

                for (int i = 0; i < num; i++)
                {
                    sum++;
                    dataQueue.Enqueue(float.Parse(str));
                }

                this.chart1.Series[0].Points.Clear();

                for (int i = 0; i < dataQueue.Count; i++)
                {
                    List<DateTime> DT1 = new List<DateTime>();
                    this.chart1.Series[0].Points.AddXY(i, dataQueue.ElementAt(i));
                }

                //让x轴能自动移动
                if (sum <= chart1.ChartAreas[0].AxisX.ScaleView.Size)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = 0;
                }
                else
                    chart1.ChartAreas[0].AxisX.ScaleView.Position = sum - chart1.ChartAreas[0].AxisX.ScaleView.Size - 2;
            }
            catch (Exception)
            {
                //ignore
            }
        }

        /// <summary>
        /// 时钟控制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            var str = DateTime.Now.ToString("MMddHHmmss");

            buttonNumber1.Number = Convert.ToInt32(str.Substring(0, 1));
            buttonNumber2.Number = Convert.ToInt32(str.Substring(1, 1));
            buttonNumber3.Number = Convert.ToInt32(str.Substring(2, 1));
            buttonNumber4.Number = Convert.ToInt32(str.Substring(3, 1));
            buttonNumber5.Number = Convert.ToInt32(str.Substring(4, 1));
            buttonNumber6.Number = Convert.ToInt32(str.Substring(5, 1));
            buttonNumber7.Number = Convert.ToInt32(str.Substring(6, 1));
            buttonNumber8.Number = Convert.ToInt32(str.Substring(7, 1));
            buttonNumber9.Number = Convert.ToInt32(str.Substring(8, 1));
            buttonNumber10.Number = Convert.ToInt32(str.Substring(9, 1));
        }

        public void ClearChart()
        {
            _valusetTable.Clear();
            chart1.Series[0].Points.Clear();
            dataQueue.Clear();
            sum = 1;
        }

        public void Start()
        {
            //todo
        }

        public void Stop()
        {
            //todo
        }
    }
}