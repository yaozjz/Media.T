using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Media.T.UI
{
    /// <summary>
    /// SettingUI.xaml 的交互逻辑
    /// </summary>
    public partial class SettingUI : Page
    {
        //
        private void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }
        public SettingUI()
        {
            InitializeComponent();
            ffmpeg_path.Text = data.Configs.data.ffmpegPath;
        }

        private void View_Path(object sender, RoutedEventArgs e)
        {
            ffmpeg_path.Text = Until.OpenDg.OpenFile("选择执行文件","ffmpeg (*.exe)|*.exe|All files (*.*)|*.*");
        }

        private async void Save_config_Click(object sender, RoutedEventArgs e)
        {
            data.Configs.data.ffmpegPath = ffmpeg_path.Text;
            data.Configs.SaveConfigs();
            SaveDone_msg.Visibility = Visibility.Visible;
            //定时让消息框消失
            await Task.Run(() =>
            {
                Thread.Sleep(3000);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SaveDone_msg.Visibility = Visibility.Collapsed;
                });
            });
        }
        //输出文本变更
        private void Logs_Update(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (Logs.LineCount > 3001)
            {
                Logs.Text = Logs.Text.Substring(Logs.GetLineText(0).Length + 1);
            }
            Logs.ScrollToEnd();
        }

        private void LookatFFmpeg_Click(object sender, RoutedEventArgs e)
        {
            Until.ffmpegUse fu = new() { tb = Logs };
            fu.Start(data.Configs.data.ffmpegPath, Args.Text.Trim());
            //清空参数框
            Args.Text = "";
        }
        //快捷键
        private void Send_keydown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                LookatFFmpeg_Click(sender, e);
            }
        }
        /// <summary>
        /// 查看支持的硬件编解码方式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LookAtCode_Click(object sender, RoutedEventArgs e)
        {
            var result = await Until.EXEUse.EXESend(data.Configs.data.ffmpegPath, "-hwaccels");
            if (result.Length == 1)
            {
                AddLogs("出现错误: " + result[0]);
            }
            else
            {
                string rule = "Hardware acceleration methods:";
                var e_r = Regex.Matches(result[0], rule + @"(\r\n|\r|\n)");
                foreach (Match o in e_r)
                {
                    if (o.Success)
                    {
                        Logs.AppendText("设备支持的硬件加速方式：");
                        AddLogs(result[0].Substring(o.Index + rule.Length));
                    }
                }
            }
        }

        private async void LookAtFormat_Click(object sender, RoutedEventArgs e)
        {
            var result = await Until.EXEUse.EXESend(data.Configs.data.ffmpegPath, "-codecs");
            var h264_surpot = Regex.Match(result[0], @"H.264(.*?)(\r\n|\r|\n)");
            var h265_surpot = Regex.Match(result[0], @"H.265(.*?)(\r\n|\r|\n)");
            if(h264_surpot.Success)
            {
                var decode264 = Regex.Match(h264_surpot.Value, @"\(decoders:(.*?)\)").Value;
                AddLogs("支持的解码方式：" + decode264);
                if (decode264.IndexOf("h264_qsv") > -1)
                    AddLogs("-   设备支持qsv（Intel显卡/核显）硬解");
                if (decode264.IndexOf("h264_cuvid") > -1)
                    AddLogs("-   设备支持NVEnc(英伟达显卡）硬解");

                var encode264 = Regex.Match(h264_surpot.Value, @"\(encoders:(.*?)\)").Value;
                AddLogs("支持的编码码方式：" + encode264);
                if (encode264.IndexOf("h264_qsv") > -1)
                    AddLogs("-   设备支持H.264 qsv（Intel显卡/核显）硬件编码");
                if (encode264.IndexOf("h264_nvenc") > -1)
                    AddLogs("-   设备支持H.264 NVEnc(英伟达显卡）硬件编码");
            }
            else
            {
                AddLogs("没有查找到有关H.264的编解码信息");
            }
            if (h265_surpot.Success)
            {
                var decode265 = Regex.Match(h265_surpot.Value, @"\(decoders:(.*?)\)").Value;
                AddLogs("支持的解码方式：" + decode265);
                if (decode265.IndexOf("hevc_qsv") > -1)
                    AddLogs("-   设备支持H.265 qsv（Intel显卡/核显）硬解");
                if (decode265.IndexOf("hevc_cuvid") > -1)
                    AddLogs("-   设备支持H.265 NVEnc(英伟达显卡）硬解");

                var encode265 = Regex.Match(h265_surpot.Value, @"\(encoders:(.*?)\)").Value;
                AddLogs("支持的编码码方式：" + encode265);
                if (encode265.IndexOf("hevc_qsv") > -1)
                    AddLogs("-   设备支持H.265 qsv（Intel显卡/核显）硬件编码");
                if (encode265.IndexOf("hevc_nvenc") > -1)
                    AddLogs("-   设备支持H.265 NVEnc(英伟达显卡）硬件编码");
            }
            else
            {
                AddLogs("没有查找到有关H.265的编解码信息");
            }
            //// 异步更新 TextBox 的内容
            //await Logs.Dispatcher.InvokeAsync(() =>
            //{
            //    AddLogs(result[0]);
            //});
        }
    }
}
