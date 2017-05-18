using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Domain
{
   public class Managers
    {
        public List<ApplicationUser> Level1Managers { get; set; }
        public List<ApplicationUser> Level2Managers { get; set; }
        public List<ApplicationUser> Level3Managers { get; set; }
        public List<ApplicationUser> TeamManagers { get; set; }
    }
}
