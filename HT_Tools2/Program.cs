using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace HT_Tools2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //判断注册
            var key = PFunc.GetCheckStr();

            var value = ConfigurationManager.AppSettings["Reg"];

            if (PFunc.GetKey(value) != key)
            {
                var f = new RegisterFrom();
                f.ShowDialog();
                if (f.DialogResult != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
            }


            Trace.Listeners.Clear();
            Trace.Listeners.Add(new Log());
            Application.Run(new MainForm());
        }
    }
}