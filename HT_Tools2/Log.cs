using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace HT_Tools2
{
    public class Log : TraceListener
    {
       public string GetLogFile()
        {
            //日志文件名
            var date = DateTime.Now.ToString("yyyy-MM-dd");

            var strAssemblyFilePath = Assembly.GetExecutingAssembly().Location;
            var strAssemblyDirPath = Path.GetDirectoryName(strAssemblyFilePath);

            //如果不存在就创建log文件夹　　        
            if (!Directory.Exists(strAssemblyDirPath + $"\\log"))     　　              
                Directory.CreateDirectory(strAssemblyDirPath + $"\\log");

            return strAssemblyDirPath + $"\\log\\{date}.txt";
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