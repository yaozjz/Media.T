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
    }
}
