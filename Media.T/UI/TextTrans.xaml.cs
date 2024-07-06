using Media.T.Until;
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

namespace Media.T.UI
{
    /// <summary>
    /// TextTrans.xaml 的交互逻辑
    /// </summary>
    public partial class TextTrans : Page
    {
        void AddLogs(string msg)
        {
            logs.AppendText(msg + "\r");
        }
        public TextTrans()
        {
            InitializeComponent();
        }

        private void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            InputFile.Text = Until.OpenDg.OpenFile("选择输入文件", "字幕文件 (*.lrc,*.srt,*.ass)|*.lrc;*.srt;*.ass");
        }

        private void OpenOutFolder_Click(object sender, RoutedEventArgs e)
        {
            Until.OpenDg.OpenFolder(Path.GetDirectoryName(InputFile.Text));
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

        private void vfFile_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
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
    }
}
