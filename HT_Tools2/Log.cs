using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HT_Tools2.Properties;

namespace HT_Tools2
{
    public class Log : TraceListener
    {
       public string GetLogFile()
        {
            //日志文件名
            var date = DateTime.Now.ToString("yyyyMMdd-");
            int fileId = 0;

            var strAssemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var strAssemblyDirPath = Path.GetDirectoryName(strAssemblyFilePath);

            //如果不存在就创建log文件夹　　        
            if (!Directory.Exists(strAssemblyDirPath + $"\\log"))     　　              
                Directory.CreateDirectory(strAssemblyDirPath + $"\\log");

             return strAssemblyDirPath + $"\\log\\{date}{fileId.ToString()}.txt";
        }


        public override void Write(string message)
        {
            File.AppendAllText(GetLogFile(), message);
        }

        public override void WriteLine(string message)
        {
            File.AppendAllText(GetLogFile(), $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {message}");
            File.AppendAllText(GetLogFile(),$"\r\n");
        }
    }
}