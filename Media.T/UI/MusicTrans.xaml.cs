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
        }

        private void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            InputFile.Text = Until.OpenDg.OpenFile("选择输入文件", "字幕文件 (*.lrc,*.srt,*.ass)|*.lrc;*.srt;*.ass");
        }

        private async void Trans_Click(object sender, RoutedEventArgs e)
        {
            if (OutputName.Text == "")
            {
                AddLogs("输出文件名称不能为空！");
                return;
            }
            string output_name = Path.GetDirectoryName(InputFile.Text);
            try
            {
                string hou_zhui = Path.GetExtension(InputFile.Text.Trim());
                if (hou_zhui.IndexOf(".lrc") > -1)
                {
                    //lrc转srt
                    output_name = Path.Combine(output_name, Path.GetFileNameWithoutExtension(OutputName.Text.Trim()) + ".srt");
                    Until.Lrc2Srt.ConvertLrcToSrt(InputFile.Text, output_name, 0, int.Parse(TutalTime.Text) * 60);
                }
                else
                {
                    //任意转换
                    output_name = Path.Combine(output_name, Path.GetFileNameWithoutExtension(OutputName.Text.Trim()) + VFFormat.Text);
                    var r = await ffmpegUse.FFmpegTerminal($"-i \"{InputFile.Text.Trim()}\" \"{output_name}\"");
                    logs.AppendText(r[1]);
                }
                AddLogs($"文件保存到{output_name}");
            }
            catch (Exception ex)
            {
                AddLogs(ex.Message);
            }
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
        private void OpenOutFolder_Click(object sender, RoutedEventArgs e)
        {
            Until.OpenDg.OpenFolder(Path.GetDirectoryName(InputFile.Text));
        }

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
        /// <summary>
        /// 字幕文件拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vfFile_DragEnter(object sender, DragEventArgs e)
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

        private void vfFile_Drop(object sender, DragEventArgs e)
        {
            // 获取拖拽的文件路径
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            // 添加文件路径到列表
            string file_name = files[0];
            string extension = System.IO.Path.GetExtension(file_name).ToLowerInvariant();
            if (data.MediaFormat.VFFormat.Contains(extension))
            {
                InputFile.Text = file_name;
            }
            else
            {
                AddLogs("拖拽文件格式不支持");
            }
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
