using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUWENINKHopenTools.Tools
{
    /// <summary>
/// /
/// </summary>
   public static class LogWriter
    {
        public static void WriteLogInfo(string strLog)
        {
            Write(strLog, "InfoLogFile");
        }
        public static void WriteLogError(string strLog)
        {
            Write(strLog, "ErrorLogFile");
        }
        private static void Write(string strLog,string logFileName)
        {
            string strLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, logFileName);
            //日志文件夹
            if (!Directory.Exists(strLogPath))
                Directory.CreateDirectory(strLogPath);
            strLogPath = Path.Combine(strLogPath, "ServiceLog_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log");
            using (FileStream fs = new FileStream(strLogPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                StreamWriter mStreamWriter = new StreamWriter(fs);
                mStreamWriter.BaseStream.Seek(0, SeekOrigin.End);
                mStreamWriter.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ":  " + strLog + "\r\n");
                mStreamWriter.Close();
            }
        }

    }
}
