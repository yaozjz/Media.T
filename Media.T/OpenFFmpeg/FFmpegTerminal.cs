using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Media.T.OpenFFmpeg
{
    public class FFmpegTerminal
    {
        /// <summary>
        /// ffmpeg地址
        /// </summary>
        public string FFmpegPath { get; set; } = "ffmpeg";
        /// <summary>
        /// 异步全信息获取
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="coding"></param>
        /// <returns></returns>
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

        //批量执行与串流信息发送
        private TextBox Logs { get; set; }
        private Process process;
        public void BatchStartAndStreamOut(List<string> args, TextBox tb)
        {
            Logs = tb;
            Thread t = new Thread(BatchStart) { IsBackground = true };
            t.Start(args);
        }
        public void OnesStartAndStreamOut(string arg, TextBox tb)
        {
            Logs = tb;
            Thread t = new Thread(RunFFmpegAtStreamOut) { IsBackground = true };
            t.Start(arg);
        }
        private void BatchStart(object arr)
        {
            List<string> arg = arr as List<string>;
            foreach (string ars in arg)
            {
                Application.Current.Dispatcher.Invoke(delegate { AddLogs(ars); });
                RunFFmpegAtStreamOut(ars);
                Application.Current.Dispatcher.Invoke(delegate { AddLogs("转换结束。"); });
            }
        }
        private void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }

        /// <summary>
        /// 异步串流读取
        /// </summary>
        /// <param name="arr">arr[0]程序路径，arr[1]参数</param>
        private void RunFFmpegAtStreamOut(object arr)
        {
            process = new Process();
            try
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = FFmpegPath;
                process.StartInfo.Arguments = arr as string;
                process.Start();
                process.BeginOutputReadLine(); //开始异步读取输出
                process.BeginErrorReadLine(); //异步读取错误
                process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler); //设置回调函数
                process.ErrorDataReceived += new DataReceivedEventHandler(ProcessErrorOutputHandler);
                process.WaitForExit();  //等待程序执行完退出进程
                process.Close();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(delegate { AddLogs(ex.Message); });
            }
        }
        /// <summary>
        /// 异步线程回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate { AddLogs(e.Data); });
            process.StandardInput.AutoFlush = true;
        }
        private void ProcessErrorOutputHandler(object sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate { AddLogs(e.Data); });
            process.StandardInput.AutoFlush = true;
        }
    }
}
