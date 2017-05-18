using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Core.Interfaces
{
    public class IHandset
    {
        public int HandsetID { get; set; }
        public string Model { get; set; }
        public double SRP { get; set; }
        public int Category { get; set; }
        public int Quantity { get; set; }
        public double HandsetTotal { get; set; }
    }
}
