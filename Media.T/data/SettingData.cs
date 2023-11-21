using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.data
{
    public class SettingData
    {
        public WinStatus winSatus { get; set; } = new();
        public string ffmpegPath { get; set; } = "ffmpeg";

        //字幕嵌入目录参数
        public SubTitle subTitle {  get; set; } = new();
        //编解码器设置
        public Code code { get; set; } = new();
    }
    /// <summary>
    /// 窗口状态
    /// </summary>
    public class WinStatus
    {
        public double Width { get; set; } = 1152;
        public double Height { get; set; } = 640;
    }
    /// <summary>
    /// 字幕目录设置
    /// </summary>
    public class SubTitle
    {
        public string? InputPath { get; set; } = "";
        public string? OutputPath { get; set; } = "";
        public string? SubTitlePath { get; set; } = "";
    }
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
    /// <summary>
    /// 编码器质量设置
    /// </summary>
    public class EnCodeArg
    {
        public static List<string> preset = new List<string>
        {
            "default",
            "slow",
            "medium",
            "fast",
            "hp",
            "hq",
            "bd",
            "ll",
            "llhq",
            "llhp",
            "lossless",
            "losslesshp",
            "p1",
            "p2",
            "p3",
            "p4",
            "p5",
            "p6",
            "p7"
        };
        public static List<string> tune = new List<string>
        {
            "hq",
            "ll",
            "ull",
            "lossless"
        };
        public static List<string> profile = new List<string>
        {
            "baseline",
            "main",
            "high",
            "high444p"
        };
    }
    public class Code
    {
        public int decode { get; set; } = 0;
        public int encode { get; set; } = 0;

        public int preset { get; set; } = 5;
        public int tune { get; set; } = 0;
        public int profile { get; set; } = 2;
    }
}
