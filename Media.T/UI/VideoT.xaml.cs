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
    /// VideoT.xaml 的交互逻辑
    /// </summary>
    public partial class VideoT : Page
    {
        private void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }
        private ObservableCollection<string> fileList = new ObservableCollection<string>();
        public VideoT()
        {
            InitializeComponent();
            //视频列表源
            VedioList.ItemsSource = fileList;
            //编解码设置
            Preset.ItemsSource = data.EnCodeArg.preset;
            Preset.SelectedIndex = data.Configs.data.code.preset;

            Tune.ItemsSource = data.EnCodeArg.tune;
            Tune.SelectedIndex = data.Configs.data.code.tune;

            Profile.ItemsSource = data.EnCodeArg.profile;
            Profile.SelectedIndex = data.Configs.data.code.profile;

            Encodes.SelectedIndex = data.Configs.data.code.encode;
            Decodes.SelectedIndex = data.Configs.data.code.decode;
            //编码器方式
            EncodeMod.SelectedIndex = data.Configs.data.code.encode_mod;
            EncodeModNum.ItemsSource = data.EnCodeArg.EncodeModIndex[EncodeMod.SelectedIndex];
            EncodeModNum.SelectedIndex = data.Configs.data.code.encode_index;
            //输入输出文件夹
            InputDir.Text = data.Configs.data.code.InputDir;
            OutputDir.Text = data.Configs.data.code.OutputDir;
        }

        private void Logs_Update(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (Logs.LineCount > 3001)
            {
                Logs.Text = Logs.Text.Substring(Logs.GetLineText(0).Length + 1);
            }
            Logs.ScrollToEnd();
        }
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            data.Configs.data.code.preset = Preset.SelectedIndex;
            data.Configs.data.code.tune = Tune.SelectedIndex;
            data.Configs.data.code.profile = Profile.SelectedIndex;
            data.Configs.data.code.encode = Encodes.SelectedIndex;
            data.Configs.data.code.decode = Decodes.SelectedIndex;
            data.Configs.data.code.encode_mod = EncodeMod.SelectedIndex;
            data.Configs.data.code.encode_index = EncodeModNum.SelectedIndex;
            data.Configs.data.code.OutputDir = OutputDir.Text;
            data.Configs.data.code.InputDir = InputDir.Text;
            data.Configs.SaveConfigs();
            AddLogs("当前配置保存成功");
        }
        //===========拖拽设置
        /// <summary>
        /// 拖拽文件进去时检测
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
        /// <summary>
        /// 导进去后显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// 浏览输入文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewInDir_Click(object sender, RoutedEventArgs e)
        {
            InputDir.Text = Until.OpenDg.OpenFile("选择输入文件夹", Until.OpenDg.folderRule, true);
            FreshList_Click(sender, e);
        }
        /// <summary>
        /// 刷新列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreshList_Click(object sender, RoutedEventArgs e)
        {
            string dri = InputDir.Text;
            if (dri.Trim().Length > 0)
            {
                try
                {
                    var r = Until.OpenDg.Get_Folder(dri, @"^.+\.(mkv|mp4)$");
                    if (r == null)
                        return;
                    fileList.Clear();
                    foreach (string file in r)
                    {
                        fileList.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    AddLogs(ex.Message);
                }
            }
            else
            {
                AddLogs("操作错误，输入文件夹为空.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewOutputDir_Click(object sender, RoutedEventArgs e)
        {
            OutputDir.Text = Until.OpenDg.OpenFile("选择输出文件夹", Until.OpenDg.folderRule, true);
        }
        //清除列表
        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            fileList.Clear();
        }
        /// <summary>
        /// 移除当前选择的项目
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSelect_Click(object sender, RoutedEventArgs e)
        {
            if (VedioList.SelectedItem != null)
            {
                string selectedFile = VedioList.SelectedItem.ToString();
                fileList.Remove(selectedFile);
            }
        }
        /// <summary>
        /// 编码方式转变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EncodeMode_change(object sender, SelectionChangedEventArgs e)
        {
            if (EncodeMod.SelectedIndex >= 0 && EncodeModNum != null)
            {
                EncodeModNum.ItemsSource = data.EnCodeArg.EncodeModIndex[EncodeMod.SelectedIndex];
                EncodeModNum.SelectedIndex = 2;
            }
        }
        /// <summary>
        /// 开始转码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BeginCode_Click(object sender, RoutedEventArgs e)
        {
            List<string> arglist = new List<string>();
            foreach (string file in VedioList.Items)
            {
                string args = string.Empty;
                var outfile = System.IO.Path.GetFileNameWithoutExtension(file);
                if (Decodes.SelectedIndex > 0)
                    args += $"-c:v {data.EnCodeArg.decoders[1][Decodes.SelectedIndex]} ";
                args += $"-i \"{file}\" ";
                if (Encodes.SelectedIndex > 0)
                    args += $"-c:v {data.EnCodeArg.encoders[OutputFormat.SelectedIndex][Encodes.SelectedIndex]} -pix_fmt yuv420p ";
                args += $"-preset {Preset.Text} -tune {Tune.Text} -{data.EnCodeArg.EnCodeMods[EncodeMod.SelectedIndex]} {EncodeModNum.Text} -profile:v {Profile.Text} ";
                if (EncodeMod.SelectedIndex == 0)
                {
                    args += "-qmin 17 -qmax 30 ";
                }
                args += "-c:a copy ";
                if (AddSubTitle.IsChecked == true)
                {
                    string path = file;
                    path = path.Replace("\\", "\\\\");
                    args += $"-vf subtitles=\"\'{path.Replace(":", "\\:")}\'\" ";
                }
                args += $"-map_metadata -1 -map_chapters -1 -y \"{System.IO.Path.Combine(OutputDir.Text, $"{outfile}.{OutputFormat.Text}")}\"";
                arglist.Add(args);
            }

            var go = new Until.ffmpegUse() { tb = Logs };
            go.BatchStart(arglist);
        }
        /// <summary>
        /// 查看视频流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ViewVedioStream_Click(object sender, RoutedEventArgs e)
        {
            var result = await Until.EXEUse.EXESend(data.Configs.data.ffmpegPath, $"-i {VedioList.SelectedItem} -vstats");
            var r = Regex.Matches(result[1], @"Stream #(.*?)(\r\n|\r|\n)");
            foreach (Match r1 in r)
            {
                Logs.AppendText(r1.Value);
            }
        }
    }
}
