using Media.T.OpenFFmpeg;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
    /// OtherFormat.xaml 的交互逻辑
    /// </summary>
    public partial class OtherFormat : Page
    {
        private ObservableCollection<string> fileList = new ObservableCollection<string>();
        void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }
        public OtherFormat()
        {
            InitializeComponent();
            ListView.ItemsSource = fileList;
        }

        private void LogsUpdate(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (Logs.LineCount > 3001)
            {
                Logs.Text = Logs.Text.Substring(Logs.GetLineText(0).Length + 1);
            }
            //滚动条滚动
            Logs.ScrollToEnd();
        }

        private async void StreamInfo_Click(object sender, RoutedEventArgs e)
        {
            if (ListView.SelectedItem != null)
            {
                string ffmpeg_path = System.IO.Path.GetDirectoryName(data.Configs.data.ffmpegPath);
                StreamInfo stream_info = new StreamInfo() { FFmpegPath = ffmpeg_path };
                var r = await stream_info.GetInfo(ListView.SelectedItem.ToString());
                if (r != null)
                {
                    if(r.VideoStream.Format != null)
                    {
                        AddLogs($"视频格式：{r.VideoStream.Format}, {r.VideoStream.Duration}, {r.VideoStream.Width}x{r.VideoStream.Height}, {r.VideoStream.ColorDepth}, {r.VideoStream.level}");
                    }
                    if(r.AudioStream.Format != null)
                    {
                        AddLogs($"音频格式：{r.AudioStream.Format}, 时长:{r.AudioStream.Duration}, {r.AudioStream.Hz}, {r.AudioStream.channels}, {r.AudioStream.bits}");
                    }
                }
            }
        }

        private void fileListView_DragEnter(object sender, DragEventArgs e)
        {
            // 检查拖拽的数据是否包含文件
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void fileListView_Drop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // 添加文件路径到列表
            foreach (var file in files)
            {
                if (!fileList.Contains(file))
                {
                    fileList.Add(file);
                }
            }
        }
    }
}
