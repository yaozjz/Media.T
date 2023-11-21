using System;
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
using System.Windows.Shapes;

namespace Media.T.UI
{
    /// <summary>
    /// InsertSubTitle.xaml 的交互逻辑
    /// </summary>
    public partial class InsertSubTitle : Page
    {
        private void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }
        public InsertSubTitle()
        {
            InitializeComponent();
            //初始化
            InputPath.Text = data.Configs.data.subTitle.InputPath;
            OutputPath.Text = data.Configs.data.subTitle.OutputPath;
            SubTitilePath.Text = data.Configs.data.subTitle.SubTitlePath;
        }
        //查看输入文件夹
        private void ViewInputFolder(object sender, RoutedEventArgs e)
        {
            string file_name = Until.OpenDg.OpenFile("选择输入文件夹", Until.OpenDg.folderRule, true);
            InputPath.Text = System.IO.Path.GetDirectoryName(file_name);
        }

        private void ViewSubTitleFolder(object sender, RoutedEventArgs e)
        {
            string file_name = Until.OpenDg.OpenFile("选择字幕文件夹", Until.OpenDg.folderRule, true);
            SubTitilePath.Text = System.IO.Path.GetDirectoryName(file_name);
        }

        private void ViewOutputFolder(object sender, RoutedEventArgs e)
        {
            string file_name = Until.OpenDg.OpenFile("选择输出文件夹", Until.OpenDg.folderRule, true);
            OutputPath.Text = System.IO.Path.GetDirectoryName(file_name);
        }
        /// <summary>
        /// 保存目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveDir_Click(object sender, RoutedEventArgs e)
        {
            data.Configs.data.subTitle.InputPath = InputPath.Text;
            data.Configs.data.subTitle.OutputPath = OutputPath.Text;
            data.Configs.data.subTitle.SubTitlePath = SubTitilePath.Text;
            data.Configs.SaveConfigs();
            AddLogs("已保存目录");
        }
        /// <summary>
        /// 开始嵌入字幕
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InsertBegin_Click(object sender, RoutedEventArgs e)
        {
            //获取输入文件夹下所有的视频文件
            IEnumerable<string> videos = Until.OpenDg.Get_Folder(InputPath.Text, "*.mkv");
            //var sub_titles = Until.OpenDg.Get_Folder(SubTitilePath.Text, $"*.{TitleFormat.Text}");
            if (videos != null)
            {
                List<string> args = new List<string>();
                foreach (var video in videos)
                {
                    string _name = System.IO.Path.GetFileNameWithoutExtension(video);
                    string title_name = _name + "." + TitleFormat.Text;
                    string output_name = _name + ".mkv";
                    string arg = $"-i \"{System.IO.Path.Combine(InputPath.Text, video)}\" " +
                        $"-i \"{System.IO.Path.Combine(SubTitilePath.Text, title_name)}\" -c copy " +
                        $"\"{System.IO.Path.Combine(OutputPath.Text, output_name)}\" -y";
                    args.Add(arg);
                }
                Until.ffmpegUse ffmpeg_go = new Until.ffmpegUse() { tb = Logs };
                //后台线程
                ffmpeg_go.BatchStart(args);
            }
        }

        private void LogsUpdate(object sender, TextChangedEventArgs e)
        {
            //防止内存泄漏
            if (Logs.LineCount > 3001)
            {
                Logs.Text = Logs.Text.Substring(Logs.GetLineText(0).Length + 1);
            }
            Logs.ScrollToEnd();
        }
    }
}
