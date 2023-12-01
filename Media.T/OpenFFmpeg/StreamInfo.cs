using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Media.T.OpenFFmpeg
{
    public class FFmpegStream
    {
        public string? Duration { get; set; } = null;
        public string? Format { get; set; } = null;
    }
    public class VideoStream : FFmpegStream
    {
        public string? ColorDepth { get; set; } = null;
        public int Height { get; set; } = 0;
        public int Width { get; set; } = 0;
        public double fps { get; set; } = 0;
        public string? level { get; set; } = null;
    }
    public class AudioStream : FFmpegStream
    {
        public int Hz { get; set; } = 0;
        public int channels { get; set; } = 0;
        public int bits { get; set; } = 0;
    }

    public class AllInfo
    {
        public VideoStream? VideoStream { get; set; } = new VideoStream();
        public AudioStream? AudioStream { get; set; } = new AudioStream();
        public FFmpegStream? SubTitles { get; set; } = new FFmpegStream();
    }
    /// <summary>
    /// 要获取文件信息请调用这个类
    /// </summary>
    public class StreamInfo
    {
        private string FFmpegPath { get; set; } = "where ffmpeg";
        /// <summary>
        /// 获取打印信息
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ffmpeg_path"></param>
        /// <returns></returns>
        public async Task<AllInfo?> GetInfo(string fileName, string ffmpeg_path)
        {
            FFmpegPath = Path.GetDirectoryName(ffmpeg_path);
            FFmpegTerminal ffmpeg_terminal = new FFmpegTerminal() { FFmpegPath = Path.Combine(FFmpegPath, "ffprobe.exe") };
            string[] StreamStatus = await ffmpeg_terminal.FFmpegSend($"-show_streams \"{fileName}\"");
            if (StreamStatus.Length > 1)
            {
                //参数初始化
                AllInfo? info = new AllInfo();

                List<Dictionary<string, string>> stream_info = new List<Dictionary<string, string>>();
                //解析返回的内容
                var result = Regex.Matches(StreamStatus[0], @"\[STREAM\]([\s\S]*?)\[/STREAM\]");
                foreach (Match item in result)
                {
                    Dictionary<string, string> one_info = new Dictionary<string, string>();
                    // 将输入字符串按换行符分割成行  
                    string[] lines = item.Value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    // 遍历每一行，并根据'='号进行拆分  
                    foreach (string line in lines)
                    {
                        if (line.Contains('='))
                        {
                            string[] parts = line.Split('=');
                            string key = parts[0].Trim(); // 去除行首行尾的空白字符  
                            string value = parts[1].Trim(); // 去除行首行尾的空白字符  
                            one_info.Add(key, value);
                        }
                    }
                    stream_info.Add(one_info);
                }
                foreach (var d in stream_info)
                {
                    if (d["codec_type"] == "audio")
                    {
                        info.AudioStream.Format = d["codec_name"];
                        info.AudioStream.Duration = Sec2TimeSpan(d["duration"]);
                        info.AudioStream.Hz = Convert.ToInt32(d["sample_rate"]);
                        info.AudioStream.channels = Convert.ToInt32(d["channels"]);
                        info.AudioStream.bits = Convert.ToInt32(d["bits_per_sample"]);

                    }
                    if (d["codec_type"] == "video")
                    {
                        info.VideoStream.Format = d["codec_name"];
                        info.VideoStream.Duration = Sec2TimeSpan(d["duration"]);
                        info.VideoStream.fps = GetFPS(d["r_frame_rate"]);
                        info.VideoStream.Width = Convert.ToInt32(d["width"]);
                        info.VideoStream.Height = Convert.ToInt32(d["height"]);
                        info.VideoStream.level = d["level"];
                        info.VideoStream.ColorDepth = d["pix_fmt"];
                    }
                    if (d["codec_type"] == "subtitle")
                    {
                        info.SubTitles.Format = d["codec_name"];
                        info.SubTitles.Duration = Sec2TimeSpan(d["duration"]);
                    }
                }
                return info;
            }
            else
            {
                Debug.WriteLine(StreamStatus[0]);
                return null;
            }
        }

        private string? Sec2TimeSpan(string sec_time)
        {
            if (sec_time == "N/A")
            {
                return null;
            }
            double seconds = Convert.ToDouble(sec_time);
            int hours = (int)(seconds / 3600);
            int minutes = (int)(seconds % 3600) / 60;
            int remainingSeconds = (int)(seconds % 60);

            string timeFormat = $"{hours:00}:{minutes:00}:{remainingSeconds:00}";
            return timeFormat;
        }

        private double GetFPS(string frame_rate)
        {
            string[] parts = frame_rate.Split('/');

            if (parts.Length != 2)
            {
                return 0;
            }

            double numerator = double.Parse(parts[0]);
            double denominator = double.Parse(parts[1]);

            if (denominator == 0)
            {
                return 0;
            }

            double result = numerator / denominator;
            return result;
        }
    }
}
