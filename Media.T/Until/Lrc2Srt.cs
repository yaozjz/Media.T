using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Media.T.Until
{
    internal class Lrc2Srt
    {
        public static void ConvertLrcToSrt(string inputLrcPath, string outputSrtPath, int startTime, int endTime)
        {
            try
            {
                // 读取 LRC 文件内容
                string lrcContent = File.ReadAllText(inputLrcPath);

                // 正则表达式匹配时间戳和文本
                Regex regex = new Regex(@"\[(\d+:\d+\.\d+)\](.*)");
                MatchCollection matches = regex.Matches(lrcContent);

                // 使用 StreamWriter 写入 SRT 文件
                using (StreamWriter writer = new StreamWriter(outputSrtPath, false, Encoding.UTF8))
                {
                    int counter = 1;
                    for (int i = 0; i < matches.Count; i++)
                    {
                        // 获取当前时间戳和文本
                        string timestamp = matches[i].Groups[1].Value;
                        string text = matches[i].Groups[2].Value.Trim();

                        // 解析当前时间戳为秒数
                        double currentTimestampSeconds = ParseTimestamp(timestamp);

                        // 判断是否在时间范围内
                        if (currentTimestampSeconds >= startTime && currentTimestampSeconds <= endTime)
                        {
                            // 获取下一个时间戳的秒数
                            double nextTimestampSeconds = (i + 1 < matches.Count)
                                ? ParseTimestamp(matches[i + 1].Groups[1].Value)
                                : endTime;

                            // 写入 SRT 文件
                            writer.WriteLine(counter);
                            writer.WriteLine($"{FormatTimestamp(currentTimestampSeconds)} --> {FormatTimestamp(nextTimestampSeconds)}");
                            writer.WriteLine(text);
                            writer.WriteLine();
                            counter++;
                        }
                    }
                }

                Console.WriteLine("Conversion completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static double ParseTimestamp(string timestamp)
        {
            // 解析时间戳为秒数
            string[] parts = timestamp.Split(':');
            int minutes = int.Parse(parts[0]);
            double seconds = double.Parse(parts[1], CultureInfo.InvariantCulture);
            return minutes * 60 + seconds;
        }

        static string FormatTimestamp(double totalSeconds)
        {
            // 格式化秒数为时间戳
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00},{time.Milliseconds:000}";
        }
    }
}
