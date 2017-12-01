using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace IPCameraMonitor
{
    class LogManager
    {
        public static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        public static readonly string LogPath = AppDataPath + "\\IPCameraMonitorLog\\";
        private static Queue<string> logMsg = new Queue<string>();
        
        static LogManager()
        {
            if (!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);
        }

        private static void WriteLog()
        {
            try
            {
                if (logMsg.Count == 0) return;

                string curYearMonth = DateTime.Now.Year.ToString() + "-" +
                    DateTime.Now.Month.ToString();
                string filePath = LogPath + "RunError-" + 
                    curYearMonth + ".slog";
                Write(filePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void Write(string filePath)
        {
            try
            {
                while (logMsg.Count > 0)
                {
                    File.AppendAllText(filePath, logMsg.Dequeue());
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void Tm_Elapsed(object sender, ElapsedEventArgs e)
        {
            WriteLog();
        }

        public static void Insert(object sender,string funcName,string msg)
        {
            string text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "--" +
            sender.ToString() + "->" + funcName + ":" + msg + "\r\n";
            logMsg.Enqueue(text);
            WriteLog();
        }

        public static void Close()
        {
            WriteLog();
        }
    }
}
