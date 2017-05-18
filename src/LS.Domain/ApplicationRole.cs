using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LS.Core.Interfaces;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LS.Domain
{
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>, IEntity<string>
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string roleName)
        {
            Id = Guid.NewGuid().ToString();
            Name = roleName;
        }

        public int Rank { get; set; }
        [NotMapped]
        public DateTime DateCreated { get; set; }
        [NotMapped]
        public DateTime DateModified { get; set; }
        [NotMapped]
        public bool IsDeleted { get; set; }
    }

    public static class ValidApplicationRoles
    {
        public static readonly int SuperAdminRank = 0;
        public static readonly int AdminRank = 1;
        public static readonly int LevelOneManagerRank = 2;
        public static readonly int LevelTwoManagerRank = 3;
        public static readonly int LevelThreeManagerRank = 4;
        public static readonly int SalesTeamManagerRank = 5;
        public static readonly int SalesRepRank = 6;

        public static readonly ApplicationRole SuperAdmin = new ApplicationRole
        {
            Name = "Super Administrator",
            Rank = SuperAdminRank
        };

        public static readonly ApplicationRole Admin = new ApplicationRole
        {
            Name = "Administrator",
            Rank = AdminRank
        };
        public static readonly ApplicationRole LevelOneManager = new ApplicationRole
        {
            Name = "Level 1 Manager",
            Rank = LevelOneManagerRank
        };
        public static readonly ApplicationRole LevelTwoManager = new ApplicationRole
        {
            Name = "Level 2 Manager",
            Rank = LevelTwoManagerRank
        };
        public static readonly ApplicationRole LevelThreeManager = new ApplicationRole
        {
            Name = "Level 3 Manager",
            Rank = LevelThreeManagerRank
        };
        public static readonly ApplicationRole SalesTeamManager = new ApplicationRole
        {
            Name = "Sales Team Manager",
            Rank = SalesTeamManagerRank
        };
        public static readonly ApplicationRole SalesRep = new ApplicationRole
        {
            Name = "Sales Rep",
            Rank = SalesRepRank
        };

        public static readonly List<ApplicationRole> ValidRoles = new List<ApplicationRole>
        {
            SuperAdmin,
            Admin,
            LevelOneManager,
            LevelTwoManager,
            LevelThreeManager,
            SalesTeamManager,
            SalesRep
        };
    }
}
