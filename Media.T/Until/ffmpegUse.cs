using Media.T.OpenFFmpeg;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Media.T.Until
{
    internal class ffmpegUse
    {
        public static void BatchRun(List<string> args, TextBox tb)
        {
            OpenFFmpeg.FFmpegTerminal ffmpeg = new OpenFFmpeg.FFmpegTerminal() { FFmpegPath = data.Configs.data.ffmpegPath };
            ffmpeg.BatchStartAndStreamOut(args, tb);
        }
        public static void OnesRun(string arg, TextBox tb)
        {
            OpenFFmpeg.FFmpegTerminal ffmpeg = new OpenFFmpeg.FFmpegTerminal() { FFmpegPath = data.Configs.data.ffmpegPath };
            ffmpeg.OnesStartAndStreamOut(arg, tb);
        }
        public async static Task<string[]> FFmpegTerminal(string arg)
        {
            FFmpegTerminal ffmpeg_terminal = new FFmpegTerminal() { FFmpegPath = data.Configs.data.ffmpegPath };
            string[] StreamStatus = await ffmpeg_terminal.FFmpegSend(arg);
            return StreamStatus;
        }
        public async static Task<AllInfo> GetInfo(string filename)
        {
            var info = new StreamInfo();
            var r = await info.GetInfo(filename, data.Configs.data.ffmpegPath);
            return r;
        }
    }
}
