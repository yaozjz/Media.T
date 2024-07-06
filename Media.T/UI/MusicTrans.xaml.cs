using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using Media.T.Until;
using System.Collections.ObjectModel;

namespace Media.T.UI
{
    /// <summary>
    /// MusicTrans.xaml 的交互逻辑
    /// </summary>
    public partial class MusicTrans : Page
    {
        void AddLogs(string msg)
        {
            logs.AppendText(msg + "\r");
        }
        public MusicTrans()
        {
            InitializeComponent();

            MusicListView.ItemsSource = fileList;
            OutAudioFormat.ItemsSource = data.MediaFormat.AudioFormats;
            OutAudioFormat.SelectedIndex = 0;
            BitRate.IsEnabled = false;
        }

        //===============
        private ObservableCollection<string> fileList = new ObservableCollection<string>();
        /// <summary>
        /// 列表拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                string extension = Path.GetExtension(file).ToLowerInvariant();
                if (!fileList.Contains(file) && data.MediaFormat.AudioFormats.Contains(extension))
                {
                    fileList.Add(file);
                }
            }
        }

        private void ViewAudioFile_Click(object sender, RoutedEventArgs e)
        {
            MusicOutputDir.Text = Until.OpenDg.OpenFile("选择输出文件夹", Until.OpenDg.folderRule, true);
        }
        //=============
        /// <summary>
        /// 在文件夹管理器中打开文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenOutAudioFolder_Click(object sender, RoutedEventArgs e)
        {
            Until.OpenDg.OpenFolder(MusicOutputDir.Text);
        }
        /// <summary>
        /// 开始转换音频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransAudio_Click(object sender, RoutedEventArgs e)
        {
            //检查目标文件夹是否存在，不存在则创建
            string output_dir = MusicOutputDir.Text.Trim();
            Until.OpenDg.CreateDir(output_dir);
            if(fileList.Count > 0)
            {
                List<string> Audios = new List<string>();
                foreach(string _name in MusicListView.Items)
                {
                    string out_name = "1-" + Path.GetFileNameWithoutExtension(_name) + OutAudioFormat.Text;
                    Audios.Add($"-i \"{_name}\" \"{Path.Combine(output_dir, out_name)}\"");
                }
                //批量转换
                ffmpegUse.BatchRun(Audios, logs);
            }
            else
            {
                AddLogs("列表为空，没有需要转换的音频文件");
            }
        }
        /// <summary>
        /// 防止内存泄露
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogsTextChange(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (logs.LineCount > 3001)
            {
                logs.Text = logs.Text.Substring(logs.GetLineText(0).Length + 1);
            }
            //滚动条滚动
            logs.ScrollToEnd();
        }
        /// <summary>
        /// 手动选择音频文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenAudioFile_Click(object sender, RoutedEventArgs e)
        {
            string filter = "音频文件 (*.wav,*.mp3,*.flac,*.aac)|*.wav;*.mp3;*.flac;*.aac";
            var newAudioList = Until.OpenDg.GetFiles(filter);
            fileList.Clear();
            foreach(string i in newAudioList)
                fileList.Add(i);
        }
        /// <summary>
        /// 移除所选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelect_Click(object sender, RoutedEventArgs e)
        {
            if (MusicListView.SelectedItem != null)
            {
                string selectedFile = MusicListView.SelectedItem.ToString();
                fileList.Remove(selectedFile);
            }
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
        }

        private async void AudioFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (MusicListView.SelectedItem != null)
            {
                string file = MusicListView.SelectedItem.ToString();
                var info = new OpenFFmpeg.StreamInfo();
                var r = await info.GetInfo(file, data.Configs.data.ffmpegPath);
                if(r != null)
                {
                    if(r.AudioStream.Format != null)
                    {
                        AddLogs($"音频格式：{r.AudioStream.Format}, 音频时长：{r.AudioStream.Duration}, 采样率：{r.AudioStream.Hz}.");
                    }
                }
            }
        }
    }
}
