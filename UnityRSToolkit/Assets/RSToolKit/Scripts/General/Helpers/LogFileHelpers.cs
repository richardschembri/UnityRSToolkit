namespace RSToolkit.Helpers
{
    using UnityEngine;
    using System;
    using System.Collections;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// EN: Log file helpers.
    /// JA: ログファイル系ヘルパークラス
    /// </summary>
    public class LogFileHelpers
    {
        public static string FolderPath = "Logs";

        public static string LogFileName = "Log";

        public static string GetTextFilePath()
        {
            var fileName = FileHelpers.GenerateFileName_WithDateStamp(LogFileName, "txt");
            var fullSaveFilePath = FileHelpers.GetFullSaveFilePath(fileName, FolderPath);

            FileHelpers.CreateDirectoryIfNotExists(fullSaveFilePath);

            return fullSaveFilePath;
        }

        public static void LogToTextFile(string log, bool includeTimestamp = true)
        {
            var log_line = GetTextLogLine(log, includeTimestamp);
            var txtFilePath = GetTextFilePath();

            if (File.Exists(txtFilePath))
            {
                //using(TextWriter tw = File.AppendText(txtFilePath))
                using (var tw = new StreamWriter(txtFilePath, true, Encoding.UTF8)) // File.AppendText(txtFilePath))
                {
                    tw.WriteLine(log_line);
                }
            }
            else
            {
                //using(TextWriter tw = File.CreateText(txtFilePath))
                using (var tw = new StreamWriter(txtFilePath, false, Encoding.UTF8)) // File.CreateText(txtFilePath))
                {
                    tw.WriteLine(log_line);
                }
            }
        }

        private static string GetTextLogLine(string log, bool includeTimestamp = true)
        {
            if (includeTimestamp)
            {
                return string.Format("[{0}]: {1}", DateTime.Now.ToString(), log);
            }

            return log;
        }
        //型を変換します

    }

}