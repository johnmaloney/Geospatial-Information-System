using System;
using System.Collections.Generic;
using System.Text;

namespace Universal.Contracts.Files
{
    public static class Extensions
    {
        public static string ConvertDataToString(this IFile file)
        {
            if (file.DataContents != null && file.DataContents.Length > 0)
            {
                file.TextContents = file.DataContents.ToUTF8String();
            }
            return string.Empty;
        }

        public static string ToUTF8String(this byte[] contents)
        {

        }
    }
}
