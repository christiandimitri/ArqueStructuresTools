using System;
using System.Linq;

namespace WarehouseLib.Utilities
{
    public class TrimWhiteSpaceFromString
    {
        public string TrimmedString;

        public TrimWhiteSpaceFromString(string str)
        {
            TrimmedString = TrimWhitespace(str);
        }

        private string TrimWhitespace(string str)
        {
            str = String.Concat(str.Where(c => !Char.IsWhiteSpace(c)));
            return str;
        }
    }
}