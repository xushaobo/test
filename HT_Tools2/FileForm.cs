using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HT_Tools2
{
    public partial class FileForm : MainForm, ICmdAction
    {
        private Thread _th;

        public string LogFile { get; set; }

        public FileForm()
        {
            InitializeComponent();
        }

        public new void Start()
        {
            ClearChart();
            GotTestDataAction();
        }

        public new void Stop()
        {
#if DEBUG
          ClearChart();
           return;
#endif

            MessageBox.Show(@"查看文件时不自持此命令");
        }

        private void GotTestDataAction()
        {
            StreamReader sr = new StreamReader(LogFile, Encoding.Default);
            String line;

            _th = new Thread(() =>
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        int index = line.LastIndexOf(' ');
                        string line2 = line.Substring(index + 1);
                        MyAction(line2);
                        Thread.Sleep(10);
                    }
                })
                {IsBackground = true};
            _th.Start();
        }

        private void FileForm_Load(object sender, EventArgs e)
        {
            Start();
        }
    }
}