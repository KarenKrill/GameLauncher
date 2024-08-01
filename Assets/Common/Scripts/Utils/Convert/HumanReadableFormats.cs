using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Common.Scripts.Utils.Convert
{
    internal static class HumanReadableFormats
    {
        public static string FormatBytes(long bytes)
        {
            if (bytes > 1024 * 1024 * 1024)
            {
                return $"{(double)bytes / (1024 * 1024 * 1024):.##} GB";
            }
            else if (bytes > 1024 * 1024)
            {
                return $"{(double)bytes / (1024 * 1024):.##} MB";
            }
            else if (bytes > 1024)
            {
                return $"{(double)bytes / 1024:.##} kB";
            }
            else
            {
                return $"{bytes} B";
            }
        }
    }
}
