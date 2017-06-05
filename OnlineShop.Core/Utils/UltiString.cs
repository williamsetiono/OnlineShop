using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OnlineShop.Core.Utils
{
    public static class UtilString
    {
        public static string GenTempName(string longName)
        {
            if (string.IsNullOrEmpty(longName)) return longName;
            var ret = string.Empty;
            var regs = new List<string>()
            {
                " ", ")", "(", "*", "[", "]", "{", "}", ">", "<", "=", ":", ",", "'", @"\", @"/", @"\\", "#", "?"
                , ";", ":", ".", "\"", "|", "+", "=", "_", "^", "%", "$", "#", "@", "!", "~", "`", "&","”","“"
            };

            longName = regs.Aggregate(longName, (current, reg) => current.Replace(reg, "_"));
            longName = longName.ToLower();

            var len = longName.Length;
            if (longName.Length > 0)
            {
                int i;
                for (i = 0; i < len; i++)
                {
                    var currentchar = longName.Substring(i, 1);
                    ret = ret + ChangeChar(currentchar);
                }
                if (ret.First() == '_')
                    ret = ret.Remove(0, 1);
                if (ret.Last() == '_')
                    ret = ret.Remove(ret.Length - 1);
            }
            else
                ret = "";
            return ret;
        }

        private static string ChangeChar(string charinput)
        {
            var a = new[]
                                             {
                                                 "à", "á", "ạ", "ả", "ã", "â", "ầ", "ấ", "ậ", "ẩ", "ẫ", "ă", "ắ", "ằ", "ắ",
                                                 "ặ", "ẳ", "ẵ", "a"
                                             };

            var d = new[] { "đ", "d" };
            var e = new[] { "è", "é", "ẹ", "ẻ", "ẽ", "ê", "ề", "ế", "ệ", "ể", "ễ", "e" };
            var ii = new[] { "ì", "í", "ị", "ỉ", "ĩ", "i" };
            var y = new[] { "ỳ", "ý", "ỵ", "ỷ", "ỹ", "y" };

            var o = new[]
                                             {
                                                 "ò", "ó", "ọ", "ỏ", "õ", "ô", "ồ", "ố", "ộ", "ổ", "ỗ", "ơ", "ờ", "ớ", "ợ",
                                                 "ở", "ỡ", "o"
                                             };

            var u = new[] { "ù", "ú", "ụ", "ủ", "ũ", "ừ", "ứ", "ự", "ử", "ữ", "u", "ư" };

            if (a.Any(t => t.Equals(charinput)))
                return "a";
            if (d.Any(t => t.Equals(charinput)))
                return "d";
            if (e.Any(t => t.Equals(charinput)))
                return "e";
            if (ii.Any(t => t.Equals(charinput)))
                return "i";
            if (y.Any(t => t.Equals(charinput)))
                return "y";
            if (o.Any(t => t.Equals(charinput)))
                return "o";
            return u.Any(t => t.Equals(charinput)) ? "u" : charinput;
        }
    }
}
