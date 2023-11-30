using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.OpenFFmpeg
{
    public class FFmpegTerminal
    {
        /// <summary>
        /// ffmpeg地址
        /// </summary>
        public string FFmpegPath { get; set; } = "ffmpeg";
        public async Task<string[]> FFmpegSend(string arg, string coding = "utf-8")
        {
            Process process = new();
            var encoding = Encoding.GetEncoding(coding);
            try
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.StandardOutputEncoding = encoding;
                process.StartInfo.StandardErrorEncoding = encoding;
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = FFmpegPath;
                process.StartInfo.Arguments = arg;
                process.Start();

                // 异步读取标准输出和标准错误流
                Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                Task<string> errorTask = process.StandardError.ReadToEndAsync();
                // 等待进程完成
                await Task.WhenAll(outputTask, errorTask);

                string standOutput = outputTask.Result;
                string errorOutput = errorTask.Result;

                process.WaitForExit();  //等待程序执行完退出进程
                process.Close();

                return new string[] { standOutput, errorOutput };
            }
            catch (Exception ex)
            {
                return new string[] { ex.Message };
            }
        }
    }
}
