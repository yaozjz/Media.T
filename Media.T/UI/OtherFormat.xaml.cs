using Media.T.OpenFFmpeg;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Media.T.Until;

namespace Media.T.UI
{
    /// <summary>
    /// OtherFormat.xaml 的交互逻辑
    /// </summary>
    public partial class OtherFormat : Page
    {
        private ObservableCollection<data.StreamGrid> fileList = new ObservableCollection<data.StreamGrid>();
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
                var select_item = ListView.SelectedItem as data.StreamGrid;
                StreamInfo stream_info = new StreamInfo();
                var r = await stream_info.GetInfo(select_item.Name, data.Configs.data.ffmpegPath);
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
        /// <summary>
        /// 拖拽检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void fileListView_Drop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            var ffprobe = new StreamInfo();

            // 添加文件路径到列表
            foreach (var file in files)
            {
                bool isDuplicate = fileList.Any(filePath => filePath.Name == file);
                if (!isDuplicate)
                {
                    // 如果文件路径还未存在于fileList中，则开始添加内容
                    var stream_info = await ffprobe.GetInfo(file, data.Configs.data.ffmpegPath);
                    var _grid = new data.StreamGrid();
                    _grid.Name = file;
                    _grid.FileType = System.IO.Path.GetExtension(file);
                    if(stream_info != null )
                    {
                        if (stream_info.VideoStream.Format != null)
                        {
                            _grid.VideoFormat = stream_info.VideoStream.Format;
                        }
                        else
                        {
                            _grid.VideoFormat = "无";
                        }
                        if(stream_info.AudioStream.Format != null)
                        {
                            _grid.AudioFormat = stream_info.AudioStream.Format;
                        }
                        else
                        {
                            _grid.AudioFormat = "无";
                        }
                    }
                    else
                    {
                        AddLogs("无法获取文件信息.");
                    }
                    fileList.Add(_grid);
                }
            }
        }
        /// <summary>
        /// 开始转换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunTrans_Click(object sender, RoutedEventArgs e)
        {
            List<string> args = new List<string>();
            foreach (data.StreamGrid file in fileList)
            {
                string path = System.IO.Path.GetDirectoryName(file.Name);
                string out_name = "1-" + System.IO.Path.GetFileNameWithoutExtension(file.Name) + OutFormat.Text;
                string arg = OpenFFmpeg.FormatTrans.GetSimpleTransArg(file.Name, System.IO.Path.Combine(path, out_name), OtherArg.Text);
                args.Add(arg);
            }
            ffmpegUse.BatchRun(args, Logs);
        }
    }
}
