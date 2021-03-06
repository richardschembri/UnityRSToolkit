﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using RSToolkit.Helpers;

namespace RSToolkit.Data.Helpers
{
    public class BasicCSVHelpers
    {

        [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
        public class CSVHeader : System.Attribute
        {
            string m_header;

            public CSVHeader(string header)
            {
                m_header = header;
            }

            public string GetHeader()
            {
                return m_header;
            }
        }

        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<T> Read<T>(string filePath, int startFromLine = 0, bool ignoreNonCsv = true)
        {
            return Read<T>(filePath, Encoding.UTF8, startFromLine, ignoreNonCsv);
        }

        public static List<T> Read<T>(string filePath, Encoding encoding, int startFromLine = 0, bool ignoreNonCsv = true)
        {
            var csvDict = Read(filePath, encoding, startFromLine, ignoreNonCsv);
            return CSVTo<T>(csvDict);
        }

        public static List<Dictionary<string, object>> Read(string filePath, int startFromLine = 0, bool ignoreNonCsv = true)
        {
            return Read(filePath, Encoding.UTF8, startFromLine, ignoreNonCsv);
        }


        private static string NormalizeField(string csvfield)
        {
            return csvfield.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "").Trim();
        }


        public static string NormalizeHeader(string header)
        {
            return NormalizeField(header);
        }


        public static List<Dictionary<string, object>> Read(string filePath, Encoding encoding, int startFromLine = 0, bool ignoreNonCsv = true)
        {
            var list = new List<Dictionary<string, object>>();

            if (!File.Exists(filePath))
            {
                return list;
            }
            string data = string.Empty;
            using (var sr = new StreamReader(filePath, encoding))
            {
                data = sr.ReadToEnd();
            }

            var lines = Regex.Split(data, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[startFromLine], SPLIT_RE);

            for (var j = 0; j < header.Length; j++)
            {
                header[j] = NormalizeHeader(header[j]);
            }

            for (var i = startFromLine + 1; i < lines.Length; i++)
            {
                if (!lines[i].Contains(",")) continue;

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];

                    var multi_line = Regex.Split(value, @"\\r\\n");
                    if (multi_line.Length > 1)
                    {
                        var sb = new StringBuilder();
                        for (int ml = 0; ml < multi_line.Length; ml++)
                        {
                            sb.AppendLine(NormalizeField(multi_line[ml]));
                        }
                        value = sb.ToString();
                    }
                    value = NormalizeField(value);
                    object finalvalue = value;
                    int n;
                    long l;
                    float f;
                    bool b;
                    if (int.TryParse(value, out n))
                    {
                        finalvalue = n;
                    }
                    else if (long.TryParse(value, out l))
                    {
                        finalvalue = l;
                    }
                    else if (float.TryParse(value, out f))
                    {
                        finalvalue = f;
                    }
                    else if (TryParseBool(value, out b))
                    {
                        finalvalue = b;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }

        public static bool TryParseBool(object inVal, out bool retVal)
        {
            // There are a couple of built-in ways to convert values to boolean, but unfortunately they skip things like YES/NO, 1/0, T/F
            //bool.TryParse(string, out bool retVal) (.NET 4.0 Only); Convert.ToBoolean(object) (requires try/catch)
            inVal = (inVal ?? "").ToString().Trim().ToUpper();
            switch ((string)inVal)
            {
                case "TRUE":
                case "T":
                case "YES":
                case "Y":
                    retVal = true;
                    return true;
                case "FALSE":
                case "F":
                case "NO":
                case "N":
                    retVal = false;
                    return true;
                default:
                    // If value can be parsed as a number, 0==false, non-zero==true (old C/C++ usage)
                    double number;
                    if (double.TryParse((string)inVal, out number))
                    {
                        retVal = (number != 0);
                        return true;
                    }
                    // If not a valid value for conversion, return false (not parsed)
                    retVal = false;
                    return false;
            }
        }

        /// <summary>
        /// EN: Uses the FieldInfo of the object to generate CSV data.
        /// JA: オブジェクトのFieldInfoで、CSVを作成します。
        /// </summary>
        /// <param name="separator">CSV Separator // CSVセパレータ</param>
        private static string ToCsvFields(FieldInfo[] fields, object o, string separator = ",")
        {
            StringBuilder linie = new StringBuilder();

            foreach (var f in fields)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                    linie.Append(x.ToString());
            }

            return linie.ToString();
        }

        /// <summary>
        /// EN: Converts a object's(class) properties into CSV format.
        /// JA: オブジェクト（クラス）のプロパティーをCSV形式に変換します。
        /// </summary>
        /// <param name="obj">Object list // オブジェクトリスト</param>
        /// <param name="separator">CSV Separator // CSVセパレータ</param>
        /// <param name="header">
        /// EN: If set to <c>true</c>,  headers will be generated.
        /// JA: <c>true</c>の場合は、CSVヘッダーも作成します。
        /// </param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<string> ToCsv<T>(IEnumerable<T> objectlist, string separator = ",", bool header = true)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();
            if (header)
            {
                yield return GetHeadersString<T>(separator);
            }
            foreach (var o in objectlist)
            {
                yield return string.Join(separator, fields.Select(f => (f.GetValue(o) ?? "").ToString())
                    .Concat(properties.Select(p => (p.GetValue(o, null) ?? "").ToString())).ToArray());
            }
        }

        public static List<T> CSVTo<T>(List<Dictionary<string, object>> csvEntries)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            //PropertyInfo[] properties = typeof(T).GetProperties();
            //var headers = fields.Select(f => f.Name).Concat(properties.Select(p => p.Name));

            var headerAttributes = typeof(T).GetProperties().Where(
                            prop => Attribute.IsDefined(prop, typeof(CSVHeader)) && prop.CanWrite).ToArray();
            PropertyInfo[] properties = typeof(T).GetProperties().Where(p => !headerAttributes.Contains(p) && p.CanWrite).ToArray();


            var headers = fields.Select(f => f.Name).Concat(properties.Select(p => p.Name))
                                                    .Concat(headerAttributes.Select(h => ((CSVHeader)h.GetCustomAttribute(typeof(CSVHeader)))
                                                        .GetHeader())
                                                    );

            var tEntries = new List<T>();

            foreach (var csvEntry in csvEntries)
            {
                var tEntry = Activator.CreateInstance(typeof(T));

                foreach (var h in headers)
                {
                    var normHeader = NormalizeHeader(h);
                    var pi = tEntry.GetType().GetProperties().FirstOrDefault(p => NormalizeHeader(((CSVHeader)p.GetCustomAttribute(typeof(CSVHeader))).GetHeader()) == normHeader);

                    if (pi == null)
                    {
                        pi = tEntry.GetType().GetProperty(h);
                    }

                    if (pi != null && pi.CanWrite)
                    {
                        try
                        {
                            pi.SetValue(tEntry, Convert.ChangeType(csvEntry[h], pi.PropertyType), null);
                        }
                        catch (Exception)
                        {
                            //Unable to convert
                        }
                    }
                }
                tEntries.Add((T)tEntry);
            }

            return tEntries;
        }

        public static string GetHeadersString<T>(string separator = ",")
        {
            return string.Join(separator, GetHeaders<T>());
        }
        public static IEnumerable<string> GetHeaders<T>()
        {

            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            List<string> headers = new List<string>();
            for (int i = 0; i < fields.Length; i++)
            {
                var h = fields[i].GetCustomAttribute(typeof(CSVHeader));
                if (h != null)
                {
                    headers.Add((((CSVHeader)h).GetHeader()));
                }
                else
                {
                    headers.Add(fields[i].Name);
                }
            }
            for (int i = 0; i < properties.Length; i++)
            {
                var h = properties[i].GetCustomAttribute(typeof(CSVHeader));
                if (h != null)
                {
                    headers.Add((((CSVHeader)h).GetHeader()));
                }
                else
                {
                    headers.Add(properties[i].Name);
                }
            }

            return headers;
        }

        /// <summary>
        /// EN: Converts a object's(class) properties into CSV format.
        /// JA: オブジェクト（クラス）のプロパティーをCSV形式に変換します。
        /// </summary>
        /// <param name="obj">Object // オブジェクト</param>
        /// <param name="separator">CSV Separator // CSVセパレータ</param>
        /// <param name="header">
        /// EN: If set to <c>true</c>,  headers will be generated.
        /// JA: <c>true</c>の場合は、CSVヘッダーも作成します。
        /// </param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<string> ToCsv<T>(T obj, string separator = ",", bool header = true)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            PropertyInfo[] properties = typeof(T).GetProperties();

            if (header)
            {
                // yield return String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray());
                yield return GetHeadersString<T>(separator);
            }

            yield return string.Join(separator, fields.Select(f => (f.GetValue(obj) ?? "").ToString())
                .Concat(properties.Select(p => (p.GetValue(obj, null) ?? "").ToString())).ToArray());
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
        public static void WriteToCSVFile<T>(T obj, string filepath, string separator = ",", bool overwrite = false)
        {
            if (File.Exists(filepath) && !overwrite)
            {
                AppendToCSVFile(obj, filepath, separator);
            }
            else
            {
                WriteToNewCSVFile(obj, filepath, separator, overwrite);
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
        public static void WriteToCSVFile<T>(IEnumerable<T> objectlist, string filepath, string separator = ",", bool overwrite = false)
        {

            if (File.Exists(filepath) && !overwrite)
            {
                AppendToCSVFile(objectlist, filepath, separator);
            }
            else
            {
                WriteToNewCSVFile(objectlist, filepath, separator, true);
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
        private static void WriteToNewCSVFile<T>(T obj, string filepath, string separator = ",", bool overwrite = false)
        {
            if (overwrite)
            {
                FileHelpers.DeleteFileIfExists(filepath);
            }

            //using (TextWriter tw = File.CreateText(filepath))
            using (var tw = new StreamWriter(filepath, false, Encoding.UTF8)) // File.CreateText(filepath))
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
        private static void WriteToNewCSVFile<T>(IEnumerable<T> objectlist, string filepath, string separator = ",", bool overwrite = false)
        {

            if (overwrite)
            {
                FileHelpers.DeleteFileIfExists(filepath);
            }

            //using (TextWriter tw = File.CreateText(filepath))
            using (var tw = new StreamWriter(filepath, false, Encoding.UTF8)) // File.CreateText(filepath))
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
        private static void AppendToCSVFile<T>(T obj, string filepath, string separator = ",")
        {
            //using (TextWriter tw = File.AppendText(filepath))
            using (var tw = new StreamWriter(filepath, true, Encoding.UTF8)) // File.AppendText(filepath))
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
        private static void AppendToCSVFile<T>(IEnumerable<T> objectlist, string filepath, string separator = ",")
        {
            //using (TextWriter tw = File.AppendText(filepath))
            using (var tw = new StreamWriter(filepath, true, Encoding.UTF8)) // File.AppendText(filepath))
            {
                foreach (var line in BasicCSVHelpers.ToCsv(objectlist, separator, false))
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}
