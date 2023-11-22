using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Media.T.Until
{
    internal class ffmpegUse
    {
        public TextBox tb {  get; set; }
        private Process process;

        public void Start(string tartget, string arg)
        {
            string[] arr = new string[] { tartget, arg };
            //后台线程
            Thread t = new Thread(RunFFmpeg) { IsBackground = true };
            t.Start(arr);
        }
        public void BatchStart(List<string> args)
        {
            Thread t = new Thread(AllStart) { IsBackground = true };
            t.Start(args);
        }
        public void AllStart(object arr)
        {
            List<string> arg = arr as List<string>;
            foreach(string ars in arg)
            {
                Application.Current.Dispatcher.Invoke(delegate { AddLogs(ars); });
                RunFFmpeg(new string[]
                {
                    data.Configs.data.ffmpegPath, ars
                });
            }
        }
        private void AddLogs(string msg)
        {
            tb.AppendText(msg + '\r');
        }
        /// <summary>
        /// 运行ffmpeg程序
        /// </summary>
        /// <param name="arr">arr[0]程序路径，arr[1]参数</param>
        public void RunFFmpeg(object arr)
        {
            string[] arg = arr as string[];
            process = new Process();
            try
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                process.EnableRaisingEvents = true;
                process.StartInfo.FileName = arg[0];
                process.StartInfo.Arguments = arg[1];
                process.Start();
                process.BeginOutputReadLine(); //开始异步读取输出
                process.BeginErrorReadLine(); //异步读取错误
                process.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler); //设置回调函数
                process.ErrorDataReceived += new DataReceivedEventHandler(ProcessErrorOutputHandler);
                //while (!process.StandardOutput.EndOfStream)
                //{
                //    string line = process.StandardOutput.ReadLine();
                //    string lineEorr = process.StandardError.ReadLine();
                //    process.StandardInput.AutoFlush = true;
                //    Application.Current.Dispatcher.Invoke(delegate { AddLogs(line); });
                //    Application.Current.Dispatcher.Invoke(delegate { AddLogs(lineEorr); });
                //}
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
        /// <summary>
        /// 关闭进程
        /// </summary>
        public void Close()
        {
            process.WaitForExit();  //等待程序执行完退出进程
            process.Close();
        }
    }
}
