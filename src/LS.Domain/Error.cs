using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exceptionless.Models;
namespace LS.Domain {
    public class Error {
        public string Message { get; set; }
        public TagSet Tags { get; set; }
        public string Type { get; set; }
        public DataDictionary Objects { get; set; }
    }
}
