using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// 获取单个文件
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filter"></param>
        /// <param name="isFolder"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取文件夹下所有相关格式文件
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 获取选择的多个文件夹
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static List<string>? GetFiles(string filter, string title = "打开文件")
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = filter;
            openFileDialog.Title = title;
            if (openFileDialog.ShowDialog() == true)
            {
                //转移文件数据
                List<string> selectedImagePaths = new List<string>(openFileDialog.FileNames);
                return selectedImagePaths;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 从路径中打开文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        static public void OpenFolder(string folderPath)
        {
            Process.Start("explorer.exe", Path.GetFullPath(folderPath));
        }

        /// <summary>
        /// 文件夹创建函数
        /// </summary>
        /// <param name="path">目标文件你家路径</param>
        /// <returns>创建是否成功</returns>
        public static bool CreateDir(string path)
        {
            //检查文件夹结构
            if (!Directory.Exists(path))
            {
                try
                {
                    //不存在文件夹则创建
                    Directory.CreateDirectory(path);
                    return true;
                }
                catch { }
            }
            return false;
        }
    }
}
