using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using HT_Tools2.Properties;

namespace HT_Tools2
{
    public class Log
    {
        public string _direc { get; }

        public Log()
        {
            _direc = Directory.GetCurrentDirectory() + @"\log\";

            //如果不存在就创建log文件夹　　        
            if (!Directory.Exists(_direc))
                Directory.CreateDirectory(_direc);

            GetFileNum();
        }

        public void WriteLine(string file, string message)
        {
            var tempfile = $"{_direc}{file}.ht2";

            File.AppendAllText(tempfile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}");
            File.AppendAllText(tempfile, $"\r\n");
        }

        public string GetFileNum()
        {
            //当天的日期
            var d = DateTime.Now.ToString("yyyy-MM-dd");
            //获取日志文件列表
            var templist = Directory.GetFiles(_direc, "*.ht2");

            //获取当天的文件
            var templist2 = templist.Where(f=>f.StartsWith(_direc+d)).ToList();

            return $"{d}-{templist2.Count}.ht2";
        }
    }
}