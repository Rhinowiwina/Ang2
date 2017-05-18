using System.Collections.Generic;
using LS.Domain;

namespace LS.WebApp.Models
{
    public class ManagersByLevelViewModel
    {
        public List<ApplicationUser> Level1Managers { get; set; }
        public List<ApplicationUser> Level2Managers { get; set; }
        public List<ApplicationUser> Level3Managers { get; set; }
        public List<ApplicationUser> SalesTeamManagers { get; set; }
    }
}