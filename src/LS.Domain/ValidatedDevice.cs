using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain
{
    public class ValidatedDevice
    {
        public bool IsValid { get; set; }
        public string Type { get; set; }
        public string HEX { get; set; }
        public string DEC { get; set; }
    }
}
