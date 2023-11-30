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
        public string ffmpegPath { get; set; } = @"tools\ffmpeg\bin\ffmpeg.exe";

        //字幕嵌入目录参数
        public SubTitle subTitle { get; set; } = new();
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

        //固定质量编码
        private static List<int> cqNum = new List<int> { 17, 19, 21, 23, 25, 30 };
        //码率
        private static List<int> bv = new List<int> { 50, 100, 200, 500, 1000, 2000, 5000, 10000, 2000, 50000, 100000, 500000 };
        //编码方式
        public static string[] EnCodeMods = new string[] { "cq", "qp", "b:v" };
        //索引
        public static List<List<int>> EncodeModIndex = new List<List<int>>() { cqNum, cqNum, bv };

        //硬件编解码参数：
        private static string[] H265decoders = new string[] { "", "hevc_cuvid", "hevc_qsv" };
        private static string[] H264decoders = new string[] { "", "h264_cuvid", "h264_qsv" };
        public static string[][] decoders = new string[][] { H264decoders, H265decoders };

        private static string[] H265encoders = new string[] { "", "hevc_nvenc", "hevc_qsv" };
        private static string[] H264encoders = new string[] { "", "h264_nvenc", "h264_qsv" };
        public static string[][] encoders = new string[][] { H264encoders, H265encoders };
    }
    /// <summary>
    /// 编解码参数
    /// </summary>
    public class Code
    {
        public int decode { get; set; } = 0;
        public int encode { get; set; } = 0;

        public int preset { get; set; } = 5;
        public int tune { get; set; } = 0;
        public int profile { get; set; } = 2;

        public int encode_mod { get; set; } = 0;
        public int encode_index { get; set; } = 1;
        //输入输出文件夹
        public string InputDir { get; set; } = "";
        public string OutputDir { get; set; } = "";

    }

}
