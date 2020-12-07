using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;
using System.Text;
using System.IO;

namespace RSToolkit.Data.Helpers
{
    public class CSVLogFileHelpers
    {
        public static List<T> Read<T>(string filePath, Encoding encoding, int startFromLine = 0, bool ignoreNonCsv = true)
        {
            var csvDict = BasicCSVHelpers.Read(filePath, encoding, startFromLine, ignoreNonCsv);
            return BasicCSVHelpers.CSVTo<T>(csvDict);
        }

        /// <summary>
        /// EN: Returns the CSV file path.
        /// JA: CSVファイルパスを返します。
        /// </summary>
        public static string GetCSVFilePath()
        {
            var fileName = FileHelpers.GenerateFileName_WithDateStamp(LogFileHelpers.LogFileName, "csv");
            var fullSaveFilePath = FileHelpers.GetFullSaveFilePath(fileName, LogFileHelpers.FolderPath);

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
        public static void LogToCSVFile<T>(T obj, string separator = ",")
        {

            var csvFilePath = GetCSVFilePath();

            if (File.Exists(csvFilePath))
            {
                LogAppendToCSVFile(obj, separator);
            }
            else
            {
                LogToNewCSVFile(obj, separator);
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
        private static void LogToNewCSVFile<T>(T obj, string separator = ",")
        {

            //using (TextWriter tw = File.CreateText(GetCSVFilePath()))
            using (var tw = new StreamWriter(GetCSVFilePath(), false, Encoding.UTF8)) // File.CreateText(GetCSVFilePath()))
            {
                foreach (var line in BasicCSVHelpers.ToCsv(obj, separator, true))
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
        private static void LogToNewCSVFile<T>(IEnumerable<T> objectlist, string separator = ",")
        {

            //using (TextWriter tw = File.CreateText(GetCSVFilePath()))
            using (var tw = new StreamWriter(GetCSVFilePath(), false, Encoding.UTF8)) // File.CreateText(GetCSVFilePath()))
            {
                foreach (var line in BasicCSVHelpers.ToCsv(objectlist, separator, true))
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
        private static void LogAppendToCSVFile<T>(T obj, string separator = ",")
        {

            //using (TextWriter tw = File.AppendText(GetCSVFilePath()))
            using (var tw = new StreamWriter(GetCSVFilePath(), true, Encoding.UTF8)) // File.AppendText(GetCSVFilePath()))
            {
                foreach (var line in BasicCSVHelpers.ToCsv(obj, separator, false))
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
        private static void LogAppendToCSVFile<T>(IEnumerable<T> objectlist, string separator = ",")
        {

            //using (TextWriter tw = File.AppendText(GetCSVFilePath()))
            using (var tw = new StreamWriter(GetCSVFilePath(), true, Encoding.UTF8)) // File.AppendText(GetCSVFilePath()))
            {
                foreach (var line in BasicCSVHelpers.ToCsv(objectlist, separator, false))
                {
                    tw.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// EN: Logs a list of objects(class) to the CSV Log file. The properties in the object(class) must have a toString() functionality.
        /// If the CSV Log file does not exist, a new one will be generated.
        /// JA: オブジェクトリスト（クラス）をCSV形式に変換して、出力します。オブジェクト（クラス）のプロパティーはtoStringのメソッドが必要です。
        /// CSVログファイルがもしなければ、作成します。
        /// </summary>
        /// <param name="objectlist">A list of object. // オブジェクトリスト</param>
        /// <param name="separator">CSV Separator. // CSVのセパレータ</param>
        /// <typeparam name="T">The 1st type parameter.// 与えたオブジェクトのT型</typeparam>
        public static void LogToCSVFile<T>(IEnumerable<T> objectlist, string separator = ",")
        {

            var csvFilePath = GetCSVFilePath();

            if (File.Exists(csvFilePath))
            {
                LogAppendToCSVFile(objectlist, separator);
            }
            else
            {
                LogToNewCSVFile(objectlist, separator);
            }

        }
    }
}