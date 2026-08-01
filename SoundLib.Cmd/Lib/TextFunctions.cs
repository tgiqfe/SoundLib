using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SoundLib.Cmd.Lib
{
    internal class TextFunctions
    {
        public static Regex WildcardMatch(string text)
        {
            string patternString = Regex.Replace(text, ".",
            x =>
            {
                string y = x.Value;
                if (y.Equals("?")) { return "."; }
                else if (y.Equals("*")) { return ".*"; }
                else { return Regex.Escape(y); }
            });
            if (!patternString.StartsWith("*")) { patternString = "^" + patternString; }
            if (!patternString.EndsWith("*")) { patternString = patternString + "$"; }
            return new Regex(patternString, RegexOptions.IgnoreCase);
        }

        private static string[] candidate_enable = new string[]
        {
            "true", "yes", "on", "enable", "enabled", "1",
        };

        public static bool IsTrue(string text)
        {
            return candidate_enable.Any(x => x.Equals(text, StringComparison.OrdinalIgnoreCase));
        }

    }
}
