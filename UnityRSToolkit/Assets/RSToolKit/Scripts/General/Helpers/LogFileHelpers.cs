namespace RSToolkit.Helpers
{
using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

    /// <summary>
    /// EN: Log file helpers.
    /// JA: ログファイル系ヘルパークラス
    /// </summary>
    public class LogFileHelpers 
    {
        public static string FolderPath = "Logs";

        public static string LogFileName = "Log"; 


        /// <summary>
        /// EN: Returns the CSV file path.
        /// JA: CSVファイルパスを返します。
        /// </summary>
        public static string GetCSVFilePath(){
            var fileName = FileHelpers.GenerateFileName_WithDateStamp(LogFileName, "csv");
            var fullSaveFilePath = FileHelpers.GetFullSaveFilePath(fileName, FolderPath);
        
            FileHelpers.CreateDirectoryIfNotExists(fullSaveFilePath);

            return fullSaveFilePath;
        }

        public static string GetTextFilePath(){
            var fileName = FileHelpers.GenerateFileName_WithDateStamp(LogFileName, "txt");
            var fullSaveFilePath = FileHelpers.GetFullSaveFilePath(fileName, FolderPath);
        
            FileHelpers.CreateDirectoryIfNotExists(fullSaveFilePath);

            return fullSaveFilePath;
        }

        /// <summary>
        /// EN: Logs an object(class) to the CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// If the CSV Log file does not exist, a new one will be generated.
        /// JA: オブジェクト（クラス）をCSV形式に変換して、出力します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// CSVログファイルがもしなければ、作成します。
        /// </summary>
        /// <param name="obj">Object. // オブジェクト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        public static void LogToCSVFile<T>(T obj, string separator = ","){

            var csvFilePath = GetCSVFilePath();

            if (File.Exists(csvFilePath)){
                LogAppendToCSVFile(obj, separator);
            }else{
                LogToNewCSVFile(obj, separator);
            }

        }

        public static void LogToTextFile(string log, bool includeTimestamp = true){
            var log_line = GetTextLogLine(log, includeTimestamp);
            var txtFilePath = GetTextFilePath();

            if (File.Exists(txtFilePath)){
                using(TextWriter tw = File.AppendText(txtFilePath))
                {
                    tw.WriteLine(log_line);
                }
            }else{
                using(TextWriter tw = File.CreateText(txtFilePath))
                {
                    tw.WriteLine(log_line);
                }
            }
        }

        private static string GetTextLogLine(string log, bool includeTimestamp = true){
            if(includeTimestamp){
                return string.Format("[{0}]: {1}",DateTime.Now.ToString(),log);
            }

            return log;
        }
        //型を変換します



        /// <summary>
        /// EN: Logs a list of objects(class) to the CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// If the CSV Log file does not exist, a new one will be generated.
        /// JA: オブジェクトリスト（クラス）をCSV形式に変換して、出力します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// CSVログファイルがもしなければ、作成します。
        /// </summary>
        /// <param name="objectlist">A list of object. // オブジェクトリスト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        public static void LogToCSVFile<T>(IEnumerable<T> objectlist, string separator = ","){

            var csvFilePath = GetCSVFilePath();

            if (File.Exists(csvFilePath)){
                LogAppendToCSVFile(objectlist, separator);
            }else{
                LogToNewCSVFile(objectlist, separator);
            }

        }

        /// <summary>
        /// EN: Logs an object(class) to a new CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// JA: オブジェクト（クラス）をCSV形式に変換します。変換したCSVの内容をもとに、CSVファイルを常に新規
        /// 作成します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// </summary>
        /// <param name="obj">Object. // オブジェクト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        private static void LogToNewCSVFile<T>(T obj, string separator = ","){

            using (TextWriter tw = File.CreateText(GetCSVFilePath()))
            {
                foreach (var line in CSVHelpersLite.ToCsv(obj, separator, true))
                {
                    tw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// EN: Logs a list of objects(class) to a new CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// JA: オブジェクトリスト（クラス）をCSV形式に変換します。変換したCSVの内容をもとに、CSVファイルを常に新規
        /// 作成します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// </summary>
        /// <param name="objectlist">A list of object. // オブジェクトリスト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        private static void LogToNewCSVFile<T>(IEnumerable<T> objectlist, string separator = ","){

            using (TextWriter tw = File.CreateText(GetCSVFilePath()))
            {
                foreach (var line in CSVHelpersLite.ToCsv(objectlist, separator, true))
                {
                    tw.WriteLine(line);
                }
            }
        }


        /// <summary>
        /// EN: Logs an object(class) to an existing CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// JA: オブジェクト（クラス）をCSV形式に変換します。変換したCSVの内容をもとに、既存のCSVファイルに追加されます。
        /// 作成します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// </summary>
        /// <param name="obj">Object. // オブジェクト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        private static void LogAppendToCSVFile<T>(T obj, string separator = ","){

            using (TextWriter tw = File.AppendText(GetCSVFilePath()))
            {
                foreach (var line in CSVHelpersLite.ToCsv(obj, separator, false))
                {
                    tw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// EN: Logs a list of objects(class) to an existing CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// JA: オブジェクトリスト（クラス）をCSV形式に変換します。変換したCSVの内容をもとに、既存のCSVファイルに追加されます。
        /// 作成します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// </summary>
        /// <param name="objectlist">A list of object. // オブジェクトリスト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        private static void LogAppendToCSVFile<T>(IEnumerable<T> objectlist, string separator = ","){

            using (TextWriter tw = File.AppendText(GetCSVFilePath()))
            {
                foreach (var line in CSVHelpersLite.ToCsv(objectlist, separator, false))
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}