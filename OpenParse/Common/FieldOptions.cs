using System;
using System.Collections.Generic;
using System.Text;

namespace OpenParse.Common
{
    class FieldOptions : Attribute
    {
        public int FieldWidth { get; set; }
        public TrimType TrimType { get; set; }
    }
}
