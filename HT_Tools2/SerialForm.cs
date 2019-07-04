using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HT_Tools2
{
    public partial class SerialForm : MainForm, ICmdAction
    {
        private SerialPort _port;
        private string _logfile;

        public SerialForm()
        {
            InitializeComponent();
        }

        public new void Start()
        {
            _logfile = _log.GetFileNum();
            _port.WriteLine("X");
        }

        public new void Stop()
        {
            _port.WriteLine("Z");
        }

        private void SerialForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
            }
        }

        private void SerialForm_Load(object sender, EventArgs e)
        {
//#if DEBUG
         //   var f = new FileForm()
        //    {
        //        LogFile = Directory.GetCurrentDirectory() + "\\log\\test.ht2"
        //    };
         //   f.ShowDialog();
         //   this.Close();
         //   return;
//#endif

            //SerialPort(String, Int32, Parity, Int32, StopBits)	使用指定的端口名、波特率、奇偶校验位、数据位和停止位初始化 SerialPort 类的新实例。

            try
            {
                var ss = ConfigurationManager.AppSettings["SerialSetting"];

                var temp = ss.Split('/');

                _port = new SerialPort(temp[0], int.Parse(temp[1]), (Parity) Enum.Parse(typeof(Parity), temp[2]),
                    int.Parse(temp[3]), (StopBits) Enum.Parse(typeof(StopBits), temp[4]));

                _port.Open();

                _port.DataReceived += (s1, e1) =>
                {
                    var indata = _port.ReadLine();
                    _log.WriteLine(_log._direc + _logfile, indata);
                    MyAction(indata);
                };
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                Application.Exit();
            }
        }
    }
}