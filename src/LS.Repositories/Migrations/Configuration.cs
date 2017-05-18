using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Domain.ExternalApiIntegration.Nlad;
using LS.Repositories.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LS.Repositories.Migrations
{

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        internal static readonly string BudgetMobileName = "Arrow";
        private static readonly string SupportUrl = "https://budgetmobileagent.zendesk.com";
        private static readonly string BudgetMobileAdminUserName = "Administrator";
        private static readonly string BudgetMobileAdminPassword = "Test3r123!";
        private static readonly string BudgetMobileAdminEmail = "kevin@305spin.com";
        private static readonly string BudgetMobileAdminFirstName = "Kevin";
        private static readonly string BudgetMobileAdminLastName = "White";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            this.CommandTimeout = 60 * 30;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false) { System.Diagnostics.Debugger.Launch(); }
            //LoadEntitesToBeUpdated(context);
            //CreateRolesIfDoNotExist(context);
            //var company = CreateCompanyIfDoesNotExist(context);
            //var user = CreateAdminUserIfDoesNotExist(context,company.Id);
            //var user1 = CreateSaUserIfDoesNotExist(context,company.Id);
            //CreateExternaStorageCredentialsIfDoesntExist(context,company);
            //CreateCaliPhoneNumberIfDoesNotExist(context,company);
           
            //CreateSalesTeamAndGroupsIfDoesNotExist(context,company,user);

         
                  //context.SaveChanges();
               

           
            }
        private static void LoadEntitesToBeUpdated(ApplicationDbContext context) {
            //Load all roles so we don't need to hit the DB for every role-existence check
            context.Roles.Load();
            context.Companies.Load();
           
            context.Users.Load();
            //context.CaliPhoneNumbers.Load();
            
            //context.ExternalStorageCredentials.Load();
            //context.SalesTeams.Load();
            //context.Level1SalesGroups.Load();
            //context.Level2TSalesGroups.Load();
            //context.Level3SalesGroups.Load();
          


            }
        private void CreateRolesIfDoNotExist(ApplicationDbContext context) {
            var roleStore = new RoleStore<ApplicationRole,string,ApplicationUserRole>(context);
            var roleManager = new RoleManager<ApplicationRole>(roleStore);

            var roles = ValidApplicationRoles.ValidRoles;
            foreach (var role in roles)
                {
                if (context.Roles.Local.All(r => r.Name != role.Name))
                    {
                    roleManager.Create(role);
                    }
                }
            }
        private static Company CreateCompanyIfDoesNotExist(ApplicationDbContext context) {
            var company = new Company
                {
                Name = BudgetMobileName,
                IsDeleted = false,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                CompanySupportUrl = SupportUrl
                };
            company.Id = "65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c";
            if (context.Companies.Local.All(c => c.Name != BudgetMobileName))
                {
                foreach (var sacEntry in GetSacEntriesForBudgetMobile())
                    {
                    company.SacEntries.Add(sacEntry);
                    }
                context.Companies.Add(company);
                }
            else
                {
                var existingCompany = context.Companies.First(c => c.Name == BudgetMobileName);
                company = existingCompany;
                context.Entry(existingCompany).Collection(c => c.SacEntries).Load();

                if (existingCompany.SacEntries.Count == 0)
                    {
                    foreach (var sacEntry in GetSacEntriesForBudgetMobile())
                        {
                        existingCompany.SacEntries.Add(sacEntry);
                        }
                    }
                }

            context.SaveChanges();

            return company;
            }

        private ApplicationUser CreateAdminUserIfDoesNotExist(ApplicationDbContext context,string companyId) {
            var user = new ApplicationUser
                {
                FirstName = BudgetMobileAdminFirstName,
                LastName = BudgetMobileAdminLastName,
                UserName = BudgetMobileAdminUserName,
                Email = BudgetMobileAdminEmail,
                CompanyId = companyId,
                IsActive = true,
                IsDeleted = false,
                PermissionsBypassTpiv = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
                };

            var userStore = new UserStore<ApplicationUser,ApplicationRole,string,
                IdentityUserLogin,ApplicationUserRole,IdentityUserClaim>(context);

            var userManager = new UserManager<ApplicationUser,string>(userStore);

            if (context.Users.Local.All(u => u.UserName != user.UserName))
                {
                try
                    {    userManager.Create(user,BudgetMobileAdminPassword);}
                catch (DbEntityValidationException e)
                    {

                    }
             

                if (!userManager.IsInRoleAsync(user.Id,ValidApplicationRoles.Admin.Name).Result)
                    {
                    userManager.AddToRole(user.Id,ValidApplicationRoles.Admin.Name);
                    }
                }
            else
                {
                return context.Users.First(u => u.UserName == BudgetMobileAdminUserName);
                }
            context.SaveChanges();

            return user;
            }
        private ApplicationUser CreateSaUserIfDoesNotExist(ApplicationDbContext context,string companyId) {
            var user = new ApplicationUser
                {
                FirstName = "Kevin",
                LastName = "White",
                UserName = "SA",
                Email = "kevin@305spin.com",
                CompanyId = companyId,
                IsActive = true,
                IsDeleted = false,
                PermissionsBypassTpiv = true,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
                };

            var userStore = new UserStore<ApplicationUser,ApplicationRole,string,
                IdentityUserLogin,ApplicationUserRole,IdentityUserClaim>(context);

            var userManager = new UserManager<ApplicationUser,string>(userStore);

            if (context.Users.Local.All(u => u.UserName != user.UserName))
                {
                userManager.Create(user,BudgetMobileAdminPassword);

                if (!userManager.IsInRoleAsync(user.Id,ValidApplicationRoles.SuperAdmin.Name).Result)
                    {
                    userManager.AddToRole(user.Id,ValidApplicationRoles.SuperAdmin.Name);
                    }
                }
            else
                {
                return context.Users.First(u => u.UserName == "SA");
                }
            context.SaveChanges();

            return user;
            }
        private static void CreateCaliPhoneNumberIfDoesNotExist(ApplicationDbContext context,Company company) {
            if (context.CaliPhoneNumbers.Any())
                {
                return;
                }

            var caliPhoneNumberEntry = new CaliPhoneNumber
                {
                AreaCode = 200,
                CompanyId = company.Id,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow,
                Number = 1000000
                };
            context.CaliPhoneNumbers.Add(caliPhoneNumberEntry);
            context.SaveChanges();
            }

        private static void CreateExternaStorageCredentialsIfDoesntExist(ApplicationDbContext context,Company company) {
            if (!context.ExternalStorageCredentials.Any(sc => sc.Type == "Signatures"))
                {
                var companyId = context.Companies.Where(c => c.Name == company.Name).ToList()[0].Id;
                var credentials = new ExternalStorageCredentials
                    {
                    CompanyId = companyId,
                    AccessKey = "AKIAJMXY2WVIV6AWQTOA",
                    SecretKey = "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",
                    Type = "Signatures",
                    System = "AWS",
                    Path = "lifeserv.dev.budgetmobile.signatures",
                    MaxImageSize = 640,
                    Id = Guid.NewGuid().ToString(),
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    IsDeleted = false
                    };
                context.ExternalStorageCredentials.Add(credentials);
                }
            context.SaveChanges();
            if (!context.ExternalStorageCredentials.Any(sc => sc.Type == "Proof"))
                {
                var companyId = context.Companies.Where(c => c.Name == company.Name).ToList()[0].Id;
                var credentials = new ExternalStorageCredentials
                    {
                    CompanyId = companyId,
                    AccessKey = "AKIAIVOMYVXYLQFREYEA",
                    SecretKey = "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",
                    Type = "Proof",
                    System = "AWS",
                    Path = "lifeserv.dev.budgetmobile.proofs",
                    MaxImageSize = 640,
                    Id = Guid.NewGuid().ToString(),
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    IsDeleted = false
                    };
                context.ExternalStorageCredentials.Add(credentials);
                }
            context.SaveChanges();
            if (!context.ExternalStorageCredentials.Any(sc => sc.Type == "Docs"))
                {
                var companyId = context.Companies.Where(c => c.Name == company.Name).ToList()[0].Id;
                var credentials = new ExternalStorageCredentials
                    {
                    CompanyId = companyId,
                    AccessKey = "AKIAJCRW4XAU3QCZO7NA",
                    SecretKey = "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",
                    Type = "Docs",
                    System = "AWS",
                    Path = "lifeserv.dev.budgetmobile.docs",
                    MaxImageSize = 640,
                    Id = Guid.NewGuid().ToString(),
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    IsDeleted = false
                    };
                context.ExternalStorageCredentials.Add(credentials);
                }
            context.SaveChanges();
            }
        private static void CreateSalesTeamAndGroupsIfDoesNotExist(ApplicationDbContext context,Company company,ApplicationUser user) {
            if (!context.Level1SalesGroups.Any())
                {
                var level1SalesGroup = new Level1SalesGroup
                    {
                    CompanyId = company.Id,
                    Name = "Level 1",
                    CreatedByUserId = user.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                    };

                var level2SalesGroup = new Level2SalesGroup
                    {
                    CompanyId = company.Id,
                    CreatedByUserId = user.Id,
                    Name = "Level 2",
                    ParentSalesGroupId = level1SalesGroup.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                    };

                var level3SalesGroup = new Level3SalesGroup
                    {
                    CompanyId = company.Id,
                    CreatedByUserId = user.Id,
                    Name = "Level 3",
                    ParentSalesGroupId = level2SalesGroup.Id,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                    };

                context.Level1SalesGroups.Add(level1SalesGroup);
                context.Level2TSalesGroups.Add(level2SalesGroup);
                context.Level3SalesGroups.Add(level3SalesGroup);

                var salesTeam = new SalesTeam
                    {
                    Address1 = "Test team",
                    CompanyId = company.Id,
                    CreatedByUserId = user.Id,
                    City = "Test team",
                    Level3SalesGroupId = level3SalesGroup.Id,
                    Name = "Test Sales Team",
                    State = "CA",
                    Zip = "90002",
                    IsActive = true,
                    ExternalDisplayName = "Test",
                    ExternalPrimaryId = "Test",
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow
                    };

                context.SalesTeams.Add(salesTeam);

                context.SaveChanges();

                var level1SalesManager = new ApplicationUser
                    {
                    FirstName = "Level 1",
                    LastName = "Level 1",
                    UserName = "Level1",
                    Email = "level1@manager.com",
                    CompanyId = company.Id,
                    IsActive = true,
                    IsDeleted = false,
                    PermissionsBypassTpiv = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    SalesTeamId = salesTeam.Id
                    };

                var level2SalesManager = new ApplicationUser
                    {
                    FirstName = "Level 2",
                    LastName = "Level 2",
                    UserName = "Level2",
                    Email = "level2@manager.com",
                    CompanyId = company.Id,
                    IsActive = true,
                    IsDeleted = false,
                    PermissionsBypassTpiv = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    SalesTeamId = salesTeam.Id
                    };

                var level3SalesManager = new ApplicationUser
                    {
                    FirstName = "Level 3",
                    LastName = "Level 3",
                    UserName = "Level3",
                    Email = "level3@manager.com",
                    CompanyId = company.Id,
                    IsActive = true,
                    IsDeleted = false,
                    PermissionsBypassTpiv = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    SalesTeamId = salesTeam.Id
                    };

                var salesRep = new ApplicationUser
                    {
                    FirstName = "Level 1",
                    LastName = "Level 1",
                    UserName = "SalesRep",
                    Email = "sales@rep.com",
                    CompanyId = company.Id,
                    IsActive = true,
                    IsDeleted = false,
                    PermissionsBypassTpiv = true,
                    DateCreated = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    SalesTeamId = salesTeam.Id
                    };

                var userStore = new UserStore<ApplicationUser,ApplicationRole,string,
                IdentityUserLogin,ApplicationUserRole,IdentityUserClaim>(context);

                var userManager = new UserManager<ApplicationUser,string>(userStore);

                var level1Result = userManager.Create(level1SalesManager,BudgetMobileAdminPassword);
                var level2Result = userManager.Create(level2SalesManager,BudgetMobileAdminPassword);
                var level3Result = userManager.Create(level3SalesManager,BudgetMobileAdminPassword);
                var repResult = userManager.Create(salesRep,BudgetMobileAdminPassword);

                userManager.AddToRole(level1SalesManager.Id,ValidApplicationRoles.LevelOneManager.Name);
                userManager.AddToRole(level2SalesManager.Id,ValidApplicationRoles.LevelTwoManager.Name);
                userManager.AddToRole(level3SalesManager.Id,ValidApplicationRoles.LevelThreeManager.Name);
                userManager.AddToRole(salesRep.Id,ValidApplicationRoles.SalesRep.Name);

                level1SalesGroup.Managers = new Collection<ApplicationUser>();
                level2SalesGroup.Managers = new Collection<ApplicationUser>();
                level3SalesGroup.Managers = new Collection<ApplicationUser>();

                level1SalesGroup.Managers.Add(level1SalesManager);
                level2SalesGroup.Managers.Add(level2SalesManager);
                level3SalesGroup.Managers.Add(level3SalesManager);

                context.SaveChanges();
                }
            }
        private static IEnumerable<SacEntry> GetSacEntriesForBudgetMobile() {
            var sacEntryList = new List<SacEntry>();

            var sacEntry2 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 409019,StateCode = "AR" };
            var sacEntry3 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 459015,StateCode = "AZ" };
            var sacEntry4 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 469018,StateCode = "CO" };
            var sacEntry5 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 229021,StateCode = "GA" };
            var sacEntry6 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 629006,StateCode = "HI" };
            var sacEntry7 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 359135,StateCode = "IA" };
            var sacEntry8 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 479016,StateCode = "ID" };
            var sacEntry9 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 329016,StateCode = "IN" };
            var sacEntry10 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 419029,StateCode = "KS" };
            var sacEntry11 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 269033,StateCode = "KY" };
            var sacEntry12 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 279038,StateCode = "LA" };
            var sacEntry13 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 119006,StateCode = "MA" };
            var sacEntry14 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 189022,StateCode = "MD" };
            var sacEntry15 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 109014,StateCode = "ME" };
            var sacEntry16 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 319034,StateCode = "MI" };
            var sacEntry17 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 369019,StateCode = "MN" };
            var sacEntry18 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 429026,StateCode = "MO" };
            var sacEntry19 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 389016,StateCode = "ND" };
            var sacEntry20 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 379026,StateCode = "NE" };
            var sacEntry21 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 559011,StateCode = "NV" };
            var sacEntry22 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 309012,StateCode = "OH" };
            var sacEntry23 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 439051,StateCode = "OK" };
            var sacEntry24 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 179015,StateCode = "PA" };
            var sacEntry25 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 589009,StateCode = "RI" };
            var sacEntry26 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 249017,StateCode = "SC" };
            var sacEntry27 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 399021,StateCode = "SD" };
            var sacEntry28 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 449070,StateCode = "TX" };
            var sacEntry29 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 509009,StateCode = "UT" };
            var sacEntry30 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 149007,StateCode = "VT" };
            var sacEntry31 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 529016,StateCode = "WA" };
            var sacEntry32 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 339034,StateCode = "WI" };
            var sacEntry33 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 209029,StateCode = "WV" };
            var sacEntry34 = new SacEntry { DateCreated = DateTime.UtcNow,DateModified = DateTime.UtcNow,IsDeleted = false,SacNumber = 519010,StateCode = "WY" };

            sacEntryList.Add(sacEntry2);
            sacEntryList.Add(sacEntry3);
            sacEntryList.Add(sacEntry4);
            sacEntryList.Add(sacEntry5);
            sacEntryList.Add(sacEntry6);
            sacEntryList.Add(sacEntry7);
            sacEntryList.Add(sacEntry8);
            sacEntryList.Add(sacEntry9);
            sacEntryList.Add(sacEntry10);
            sacEntryList.Add(sacEntry11);
            sacEntryList.Add(sacEntry12);
            sacEntryList.Add(sacEntry13);
            sacEntryList.Add(sacEntry14);
            sacEntryList.Add(sacEntry15);
            sacEntryList.Add(sacEntry16);
            sacEntryList.Add(sacEntry17);
            sacEntryList.Add(sacEntry18);
            sacEntryList.Add(sacEntry19);
            sacEntryList.Add(sacEntry20);
            sacEntryList.Add(sacEntry21);
            sacEntryList.Add(sacEntry22);
            sacEntryList.Add(sacEntry23);
            sacEntryList.Add(sacEntry24);
            sacEntryList.Add(sacEntry25);
            sacEntryList.Add(sacEntry26);
            sacEntryList.Add(sacEntry27);
            sacEntryList.Add(sacEntry28);
            sacEntryList.Add(sacEntry29);
            sacEntryList.Add(sacEntry30);
            sacEntryList.Add(sacEntry31);
            sacEntryList.Add(sacEntry32);
            sacEntryList.Add(sacEntry33);
            sacEntryList.Add(sacEntry34);

            return sacEntryList;
            }





        }
    }
