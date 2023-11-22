using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.Until
{
    class EXEUse
    {
        /// <summary>
        /// 阻塞型获取
        /// </summary>
        /// <param name="arr"></param>
        public static async Task<string[]> EXESend(string exePath, string arg)
        {
            Process process = new();
            try
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = exePath;
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
