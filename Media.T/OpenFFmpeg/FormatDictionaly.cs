using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Media.T.OpenFFmpeg
{
    internal class FormatDictionaly
    {
        public enum VideoEncodeDev
        {
            h264 = 0,
            h264_qsv = 1,
            h264_cuvid = 2,
            hevc = 3,
            hevc_qsv = 4,
            hevc_cuvid = 5,
        }
        public enum VideoDeCodeDev
        {
            libx264 = 0,
            libx264rgb = 1,
            h264_amf = 2,
            h264_mf = 3,
            h264_nvenc = 4,
            h264_qsv = 5,
            libx265 = 6,
            hevc_amf = 7,
            hevc_mf = 8,
            hevc_nvenc = 9,
            hevc_qsv = 10,
        }
        public static string[] VideoCodeName = new string[] { "h264", "hevc" };
        //硬件编解码参数：
        private static string[] H265decoders = new string[] { "hevc", "hevc_qsv", "hevc_cuvid" };
        private static string[] H264decoders = new string[] { "h264", "h264_qsv", "h264_cuvid" };
        public static string[][] decoders = new string[][] { H264decoders, H265decoders };

        private static string[] H265encoders = new string[] { "libx265", "hevc_qsv", "hevc_nvenc" };
        private static string[] H264encoders = new string[] { "libx264", "h264_qsv", "h264_nvenc" };
        public static string[][] encoders = new string[][] { H264encoders, H265encoders };
        public static string? GetEncoder(string codec_name, int dev_index)
        {
            string arg = "-c:v ";
            if (dev_index == 1) //qsv
            {
                if (codec_name == VideoCodeName[0])
                {
                    return arg + H264decoders[1];
                }
                else if (codec_name == VideoCodeName[1])
                {
                    return arg + H265decoders[1];
                }
            }
            else if (dev_index == 2) //nvdia
            {
                if (codec_name == VideoCodeName[0])
                {
                    return arg + H264decoders[2];
                }
                else if (codec_name == VideoCodeName[1])
                {
                    return arg + H265decoders[2];
                }
            }
            return null;
        }
    }
}
