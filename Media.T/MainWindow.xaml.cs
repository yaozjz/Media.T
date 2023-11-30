using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Media.T
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            data.Configs.InitConfigFile();
            Width = data.Configs.data.winSatus.Width;
            Height = data.Configs.data.winSatus.Height;
            //初始化UI显示
            MainContent.Content = new UI.VideoT();
        }
        //快捷键设定
        private void WinKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Close();
            }
        }
        //窗口关闭事件
        private void WinClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            data.Configs.data.winSatus.Width = Width;
            data.Configs.data.winSatus.Height = Height;
            data.Configs.SaveConfigs();
        }
        //视频压缩
        private void VideoFormat_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.VideoT();
        }
        //设置
        private void Setting_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.SettingUI();
        }

        private void zimuQianru_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.InsertSubTitle();
        }

        private void MusicTrans_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.MusicTrans();
        }

        private void OtherFormat_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UI.OtherFormat();
        }
    }
}