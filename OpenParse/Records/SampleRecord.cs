using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using OpenParse.Common;

namespace OpenParse.Records
{
    class SampleRecord
    {
        [FieldOptions(FieldWidth = 48, TrimType = TrimType.Both)]
        public string FirstName { get; set; }

        [FieldOptions(FieldWidth = 48, TrimType = TrimType.Both)]
        public string LastName { get; set; }
        
        [FieldOptions(FieldWidth = 128, TrimType = TrimType.Both)]
        public string Email { get; set; }
    }
}
