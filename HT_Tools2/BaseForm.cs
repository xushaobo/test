using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HT_Tools2
{
    public partial class BaseForm : Form
    {
        public BaseForm()
        {
            InitializeComponent();
        }

        private void BaseForm_Load(object sender, EventArgs e)
        {
            //读取窗体参数

            if (Name == "BaseForm") return;
            var filestr = GetFormSetFile();
            var temp = "";
            //判断存不存在设置文件,防止第一次加载报错
            if (File.Exists(filestr))
            {
                using (var sr = new StreamReader(filestr, Encoding.Default))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        temp = line;
                    }
                }

                var formsettemp = temp.Split('/');

                Height = int.Parse(formsettemp[0]);
                Width = int.Parse(formsettemp[1]);
                Location = new Point(int.Parse(formsettemp[2]), int.Parse(formsettemp[3]));
            }
        }

        private string GetFormSetFile()
        {
            var strAssemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var strAssemblyDirPath = Path.GetDirectoryName(strAssemblyFilePath);

            //如果不存在就创建log文件夹　　        
            if (!Directory.Exists(strAssemblyDirPath + $"\\FormSet"))
                Directory.CreateDirectory(strAssemblyDirPath + $"\\FormSet");

            return strAssemblyDirPath + $"\\FormSet\\{Name}.txt";
        }

        private void BaseForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //写窗体设置

            File.WriteAllText(GetFormSetFile(), $"{Height}/{Width}/{Location.X}/{Location.Y}");
        }
    }
}