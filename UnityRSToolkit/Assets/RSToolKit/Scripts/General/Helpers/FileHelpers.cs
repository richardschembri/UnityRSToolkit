namespace RSToolkit.Helpers
{
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

    /// <summary>
    /// EN: Helper class for File related functions.
    /// JA: ファイル関連ヘルパークラス
    /// </summary>
    public static class FileHelpers
    {

        /// <summary>
        /// EN: Trims and Appends "name" and "extension" with a "." in the middle.
        /// JA: 拡張子付きファイル名を返します。空白はトリムされます。
        /// </summary>
        public static string GenerateFileName(string name, string extension = "")
        {
            return String.Format("{0}.{1}", name.Trim(), extension.Trim());
        }

        /// <summary>
        /// EN: Generates a unique file name
        /// JA: 一意なファイル名を返します 
        /// </summary>
        public static string GenerateUniqueFileName(string name, string extension = "")
        {
            return GenerateFileName_WithDateTimeStamp(name, extension);
        }

        /// <summary> 
        /// EN: Generates a file name with a date time stamp. 
        /// JA: タイムスタンプ（年月日時分秒ミリ秒）付きでファイル名を返します。
        /// </summary>
        public static string GenerateFileName_WithDateTimeStamp(string name, string extension = "")
        {
            var dateStamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            return String.Format("{0}_{1}.{2}", name, dateStamp, extension);
        }

        /// <summary>
        /// EN: Generates a file name with a date stamp.
        /// JA: タイムスタンプ（年月日）付きでファイル名を返します。
        /// </summary>
        public static string GenerateFileName_WithDateStamp(string name, string extension = "")
        {
            var dateTimeStamp = DateTime.Now.ToString("yyyyMMdd");
            return String.Format("{0}_{1}.{2}", name, dateTimeStamp, extension);
        }

        /// <summary>
        /// EN: Returns the absolute folder path
        /// JA: 相対フォルダパスを渡し絶対フォルダパスを返します。
        /// </summary>

        /// <param name="relativePath">相対パス</param>
        public static string GetFullSaveFolderPath(string relativePath)
        {
            return String.Format("{0}/{1}", Application.dataPath, relativePath);
        }

        /// <summary>
        /// EN: Gets the absolute file path.
        /// JA: 相対ファイルパスを渡し絶対ファイルパスを返します。
        /// </summary>
        /// <param name="relativePath">相対パス</param>
        public static string GetFullSaveFilePath(string fileName, string relativePath = "")
        {
            if (!String.IsNullOrEmpty(relativePath))
            {
                return String.Format("{0}/{1}", GetFullSaveFolderPath(relativePath), fileName);
            }
            else
            {
                return String.Format("{0}/{1}", Application.dataPath, fileName);
            }
        }

        /// <summary>
        /// EN: Creates the directory if it does not exists.
        /// JA: 与えたフォルダパスがもしなければ、作成します。
        /// </summary>
        public static void CreateDirectoryIfNotExists(string directory)
        {
            var file = new System.IO.FileInfo(directory);
            file.Directory.Create();
        }

        //CONTINUE FROM HERE!!!!!! Richard
        public static List<string> GetFolders(string path, bool isRelativePath = false)
        {
            var lstFolderNames = new List<string>();

            var absolutePath = path;
            if (!isRelativePath)
            {
                absolutePath = GetFullSaveFolderPath(path);
            }

            DirectoryInfo di = new DirectoryInfo(path);
            var directories = di.GetDirectories();
            for (int i = 0; i < directories.Count(); i++)
            {
                lstFolderNames.Add(di.GetDirectories()[i].Name);
            }

            return lstFolderNames;
        }

        public static bool FilePathHasExtension(string path, string[] extensions){
            return extensions.Any( e => path.EndsWith(string.Format(".{0}", e)));
        }

        public static bool FileExistsWithExt(string path, string[] extensions){
            return extensions.Any( ext => File.Exists( string.Format("{0}.{1}",
                                                        path, ext.Replace(".", string.Empty)))
            );
        }

        public static void DeleteFileIfExists(string filepath)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
        }

        public static FileInfo[] GetFileInfoList(string[] extensions, string FolderPath, bool isRelativePath)
        {
            var absoluteFolderPath = FolderPath;
            if (isRelativePath)
            {
                absoluteFolderPath  = string.Format("{0}/{1}", Application.dataPath, FolderPath);
            }

            FileInfo[] fileInfoList;

            FileHelpers.CreateDirectoryIfNotExists(absoluteFolderPath + "/foo.foo");

            DirectoryInfo dirInfo = new DirectoryInfo(absoluteFolderPath);

            var allowedExtensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

            fileInfoList = dirInfo.EnumerateFiles()
                  .Where(f => allowedExtensions.Contains(f.Extension)).ToArray();

            return fileInfoList;
        }

        public static void OpenWindowsDirectory(string path, bool isRelative = true){
            string absolutepath = path;

            if (isRelative){
                absolutepath = GetFullSaveFolderPath(path);
            }

            System.Diagnostics.Process.Start("explorer.exe", absolutepath.Replace("/", @"\")); 
        }

    }
}