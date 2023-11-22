using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.data
{
    public class Configs
    {
        private static string configsPath = @"configs\";
        private static string jsonPath = "config.json";
        private static string jsonFilePath = Path.Combine(configsPath, jsonPath);

        public static SettingData data = new();

        /// <summary>
        /// 初始化配置文件
        /// </summary>
        public static void InitConfigFile()
        {
            CreateDir(configsPath);
            if (!File.Exists(jsonFilePath))
            {
                data = new();
                Until.JsonEdit.JsonWrite(jsonFilePath, data);
            }
            else
            {
                data = Until.JsonEdit.JsonRead(jsonFilePath);
            }
        }

        public static void SaveConfigs()
        {
            Until.JsonEdit.JsonWrite(jsonFilePath, data);
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
