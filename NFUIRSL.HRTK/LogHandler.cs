﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFUIRSL.HRTK
{
    /// <summary>
    /// 日誌等級。<br/>
    /// 數值越大表示越嚴重。
    /// </summary>
    public enum LoggingLevel : byte
    {
        /// <summary>
        /// 蹤跡。
        /// </summary>
        Trace = 0,

        /// <summary>
        /// 資訊。
        /// </summary>
        Info,

        /// <summary>
        /// 警告。
        /// </summary>
        Warn,

        /// <summary>
        /// 錯誤。
        /// </summary>
        Error,

        /// <summary>
        /// 致命。
        /// </summary>
        Fatal = byte.MaxValue
    }

    public interface ILogHandler
    {
        string Path { get; }

        void Write(string message, LoggingLevel loggingLevel);

        void Write(Exception ex, LoggingLevel loggingLevel);
    }

    public class LogHandler : ILogHandler
    {
        private readonly LoggingLevel LoggingLevel;
        private string Filename;

        public LogHandler(string path = "",
                          LoggingLevel loggingLevel = LoggingLevel.Trace)
        {
            Path = path;
            LoggingLevel = loggingLevel;
            CreateFile();
        }

        ~LogHandler()
        {
            Write("LogHandler Destruct.", LoggingLevel.Fatal);
        }

        public string Path { get; }

        public void Write(Exception ex, LoggingLevel loggingLevel)
        {
            Write($"{ex.Message}. {ex.StackTrace}", loggingLevel);
        }

        public void Write(string message, LoggingLevel loggingLevel)
        {
            if (loggingLevel >= LoggingLevel)
            {
                string text = DateTime.Now.ToString("HH:mm:ss") +
                              $"[{loggingLevel}]" +
                              $"{message.Replace("\r", "").Replace("\n", ";").Trim()}";

                var file = MakeStreamWriter();
                file.WriteLine(text);
                file.Close();
            }
        }

        private void CreateFile()
        {
            var dateTimeNow = DateTime.Now;
            var num = 1;

            // Update filename.
            while (true)
            {
                var targetFilename = $"{dateTimeNow:MMMdd-HH}_{num}.log";
                if (File.Exists(Path + targetFilename))
                {
                    num++;
                }
                else
                {
                    Filename = targetFilename;
                    break;
                }
            }

            var sw = MakeStreamWriter();
            sw.WriteLine($"{dateTimeNow:yyyy-MM-dd_HH:mm:ss}  " +
                         $"LogLv: {LoggingLevel}\r\n---");
            sw.Close();
        }

        /// <summary>
        /// Factory pattern。
        /// </summary>
        /// <returns></returns>
        private StreamWriter MakeStreamWriter()
        {
            try
            {
                return File.AppendText(Path + Filename);
            }
            catch (DirectoryNotFoundException)
            {
                Directory.CreateDirectory(Path);
                return File.AppendText(Path + Filename);
            }
        }
    }
}