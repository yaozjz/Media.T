using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Media.T.OpenFFmpeg
{
    internal class FormatTrans
    {
        /// <summary>
        /// 获取简单的命令参数
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <param name="otherArg"></param>
        /// <returns></returns>
        public static string GetSimpleTransArg(string inputFile, string outputFile, string otherArg = null)
        {
            string Args = $"-i \"{inputFile}\" ";
            if (otherArg != null)
            {
                Args += otherArg ;
            }
            return Args += $" \"{outputFile}\"";
        }

        
    }
}
