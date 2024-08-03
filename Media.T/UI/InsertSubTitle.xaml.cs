using Media.T.data;
using Media.T.Until;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Media.T.UI
{
    /// <summary>
    /// InsertSubTitle.xaml 的交互逻辑
    /// </summary>
    public partial class InsertSubTitle : Page
    {
        ObservableCollection<string> VideoItems { get; set; } = new ObservableCollection<string>();
        ObservableCollection<string> SubTitleItems { get; set; } = new ObservableCollection<string>();
        private void AddLogs(string msg)
        {
            Logs.AppendText(msg + '\r');
        }
        /// <summary>
        /// 列表刷新
        /// </summary>
        /// <param name="listView"></param>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        private void FreshList(ObservableCollection<string> listView, string path, string filter)
        {
            if(path != "")
            {
                IEnumerable<string> videos = Until.OpenDg.Get_Folder(path, filter);
                if(videos != null)
                {
                    listView.Clear();
                    foreach (var video in videos)
                    {
                        listView.Add(video);
                    }
                }
            }
        }
        /// <summary>
        /// 视频列表刷新
        /// </summary>
        private void FreshVedeo()
        {
            string now_path = InputPath.Text.Trim();
            if (Directory.Exists(now_path))
            {
                //是否存在该目录，如果存在则检索
                FreshList(VideoItems, now_path, @"(.*)\.mkv");
                SortListView(VideoItems);
            }
            
        }
        /// <summary>
        /// 字幕列表刷新
        /// </summary>
        private void FreshSubtitle()
        {
            string subtitle_path = SubTitilePath.Text.Trim();
            if (Directory.Exists(subtitle_path))
            {
                 
                FreshList(SubTitleItems, subtitle_path, @"(.*)\" + TitleFormat.Text);
                SortListView(SubTitleItems);
            }                
        }
        private void SortListView(ObservableCollection<string> FileItems)
        {
            // 按照文件名排序
            var sortedItems = FileItems.OrderBy(path => path, new NaturalSortComparer()).ToList();

            // 清除并重新添加已排序的项
            FileItems.Clear();
            foreach (var sortedItem in sortedItems)
            {
                FileItems.Add(sortedItem);
            }
        }
        public InsertSubTitle()
        {
            InitializeComponent();
            //初始化
            InputPath.Text = data.Configs.data.subTitle.InputPath;
            OutputPath.Text = data.Configs.data.subTitle.OutputPath;
            SubTitilePath.Text = data.Configs.data.subTitle.SubTitlePath;
            VideoList.ItemsSource = VideoItems;
            SubtitleList.ItemsSource = SubTitleItems;
            TitleFormat.ItemsSource = MediaFormat.SubTitleFormat;
            TitleFormat.SelectedIndex = 0;
            FreshVedeo();
            FreshSubtitle();
        }
        //查看输入文件夹
        private void ViewInputFolder(object sender, RoutedEventArgs e)
        {
            InputPath.Text = Until.OpenDg.OpenFile("选择输入文件夹", Until.OpenDg.folderRule, true);
            FreshVedeo();
        }

        private void ViewSubTitleFolder(object sender, RoutedEventArgs e)
        {
            SubTitilePath.Text = Until.OpenDg.OpenFile("选择字幕文件夹", Until.OpenDg.folderRule, true);
            FreshSubtitle();
        }

        private void ViewOutputFolder(object sender, RoutedEventArgs e)
        {
            OutputPath.Text = Until.OpenDg.OpenFile("选择输出文件夹", Until.OpenDg.folderRule, true);
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
            string output_dir = OutputPath.Text.Trim();
            Until.OpenDg.CreateDir(output_dir);
            if(VideoList.Items.Count != SubtitleList.Items.Count)
            {
                AddLogs("视频文件数量与字幕文件数量不一致，请检查你的文件。");
                return;
            }
            List<string> args = new List<string>();
            foreach (var pair in VideoItems.Zip(SubTitleItems, (item1, item2) => new { Item1 = item1, Item2 = item2 }))
            {
                string _name = System.IO.Path.GetFileNameWithoutExtension(pair.Item1);
                string _format = Path.GetExtension(pair.Item1);
                string _subtitle = pair.Item2;
                string _out_name = Path.Combine(output_dir, $"1-{_name}{_format}");
                string arg = $"-i \"{pair.Item1}\" -i \"{_subtitle}\" -c copy \"{_out_name}\" ";
                args.Add(arg);
            }
            ffmpegUse.BatchRun(args, Logs);
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


        private void FreshVideoList(object sender, RoutedEventArgs e)
        {
            FreshVedeo();
        }
        /// <summary>
        /// 刷新字幕栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreshSubtitle_Click(object sender, RoutedEventArgs e)
        {
            FreshSubtitle();
        }

        private void SubtitleSelectChange(object sender, SelectionChangedEventArgs e)
        {
            FreshSubtitle();
        }

        /// <summary>
        /// 输入框字体发生改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputPath_change(object sender, TextChangedEventArgs e)
        {
            FreshVedeo();
        }
        /// <summary>
        /// 输入的字幕路径发生变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubTitle_change(object sender, TextChangedEventArgs e)
        {
            FreshSubtitle();
        }
    }
}
