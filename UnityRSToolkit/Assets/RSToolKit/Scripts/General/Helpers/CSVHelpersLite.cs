namespace RSToolkit.Helpers
{
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

    public class CSVHelpersLite
    {
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<T> Read<T>(string filePath)
        {
            return Read<T>(filePath, Encoding.UTF8);
        }

        public static List<T> Read<T>(string filePath, Encoding encoding)
        {
            var csvDict = Read(filePath, encoding);
            return CSVTo<T>(csvDict);
        }

        public static List<Dictionary<string, object>> Read(string filePath)
        {
            return Read(filePath, Encoding.UTF8);
        }

        public static List<Dictionary<string, object>> Read(string filePath, Encoding encoding)
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

            var header = Regex.Split(lines[0], SPLIT_RE);

            for (var j = 0; j < header.Length; j++)
            {
                header[j] = header[j].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
            }

            for (var i = 1; i < lines.Length; i++)
            {

                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    if(value.Contains(@"\r\n")){
                        var foo = true;
                    }
                    var multi_line = Regex.Split(value, @"\\r\\n");
                    if (multi_line.Length > 1){
                        var sb = new StringBuilder();
                        for(int ml = 0; ml < multi_line.Length; ml++){
                            sb.AppendLine(multi_line[ml].TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""));
                        }
                        value = sb.ToString();
                    }
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
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
                    }else if(TryParseBool(value, out b)){
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
                yield return String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray());
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
            PropertyInfo[] properties = typeof(T).GetProperties();

            var headers = fields.Select(f => f.Name).Concat(properties.Select(p => p.Name));
            var tEntries = new List<T>();

            foreach (var csvEntry in csvEntries)
            {
                var tEntry = Activator.CreateInstance(typeof(T));

                foreach (var h in headers)
                {
                    var pi = tEntry.GetType().GetProperty(h);
                    

                    if (pi.CanWrite)
                    {
                        try{
                            pi.SetValue(tEntry, Convert.ChangeType(csvEntry[h], pi.PropertyType), null);
                        }catch(Exception ex){
                            //Unable to convert
                        }
                    }
                }
                tEntries.Add((T)tEntry);
            }

            return tEntries;
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
                yield return String.Join(separator, fields.Select(f => f.Name).Concat(properties.Select(p => p.Name)).ToArray());
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

            using (TextWriter tw = File.CreateText(filepath))
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
        private static void WriteToNewCSVFile<T>(IEnumerable<T> objectlist, string filepath, string separator = ",", bool overwrite = false)
        {

            if (overwrite)
            {
                FileHelpers.DeleteFileIfExists(filepath);
            }

            using (TextWriter tw = File.CreateText(filepath))
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
        private static void AppendToCSVFile<T>(T obj, string filepath, string separator = ",")
        {
            using (TextWriter tw = File.AppendText(filepath))
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
        private static void AppendToCSVFile<T>(IEnumerable<T> objectlist, string filepath, string separator = ",")
        {
            using (TextWriter tw = File.AppendText(filepath))
            {
                foreach (var line in CSVHelpersLite.ToCsv(objectlist, separator, false))
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}