using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Media.T.Until
{
    internal class OpenDg
    {
        public static string folderRule = "文件夹 | *.*";
        public static string OpenFile(string title, string filter, bool isFolder = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = title,
                Filter = filter
            };
            if (isFolder)
            {
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "选择文件夹";
            }
            if (openFileDialog.ShowDialog() == true)
            {
                if (isFolder)
                    return Path.GetDirectoryName(openFileDialog.FileName);
                return openFileDialog.FileName;
            }
            return "";
        }
        public static IEnumerable<string>? Get_Folder(string FilePath, string filter)
        {
            if (Directory.Exists(FilePath))
            {

                var result = Directory.GetFiles(FilePath).Where(file => Regex.IsMatch(file, filter));
                return result;
            }
            else
            {
                MessageBox.Show("文件夹不存在!");
                return null;
            }
        }
    }
}
