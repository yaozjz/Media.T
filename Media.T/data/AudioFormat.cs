using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Media.T.data
{
    internal class MediaFormat
    {
        /// <summary>
        /// 支持的音频格式
        /// </summary>
        static public List<string> AudioFormats { get; private set; } = new List<string>() { ".wav", ".mp3", ".flac", ".aac" };

        static public List<string> VideoFormats { get; private set; } = new List<string>()
        {
            ".mp4", ".mkv"
        };

        static public List<string> VFFormat { get; private set; } = new List<string>()
        {
            ".lrc", ".srt", ".ass"
        };

        static public List<string> SubTitleFormat { get; private set; } = new List<string>() { ".ass", ".srt" };
    }
}
