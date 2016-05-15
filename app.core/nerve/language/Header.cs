using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app.core.nerve.language
{
    public class Header
    {
        private string _headerVal;
        public Header(string val)
        {
            _headerVal = val;
        }

        public bool IsEqual(string val)
        {
            return val == _headerVal;
        }
    }
}
