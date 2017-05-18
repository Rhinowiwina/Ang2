using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using LS.Core;
using LS.Domain;
using LS.Utilities;
using LS.Repositories;

namespace LS.Services
{
    public class TestDataService
    {
        public string test(string input)
        {
            ValidateDevice deviceTest = new ValidateDevice();

            deviceTest.ValidateDeviceID(input);


            return null;
        }
    }
}
