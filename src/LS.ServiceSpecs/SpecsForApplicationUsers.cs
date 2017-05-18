using System;
using System.Collections.Generic;
using LS.Core;
using LS.Domain;
using LS.Repositories.DBContext;
using LS.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace LS.ServiceSpecs
{
//    [TestFixture]
//    public class SpecsForApplicationUsers
//    {
//        private UserManager<ApplicationUser, string> _userManager;
 
//        private static Company _firstCompany = new Company
//        {
//            Name = "First Company"
//        };

//        private static Company _secondCompany = new Company
//        {
//            Name = "Second Company"
//        };

//        private static Level1SalesGroup _level1SalesGroup = new Level1SalesGroup
//        {
//            CompanyId = _firstCompany.Id,
//            IsDeleted = false
//        };

//        private static Level2SalesGroup _level2SalesGroup = new Level2SalesGroup
//        {
//            CompanyId = _firstCompany.Id,
//            ParentSalesGroupId = _level1SalesGroup.Id,
//            IsDeleted = false
//        };

//        private static Level3SalesGroup _level3SalesGroup = new Level3SalesGroup
//        {
//            CompanyId = _firstCompany.Id,
//            ParentSalesGroupId = _level2SalesGroup.Id,
//            IsDeleted = false
//        };

//        private static SalesTeam _firstCompanySalesTeam = new SalesTeam
//        {
//            CompanyId = _firstCompany.Id,
//            Level3SalesGroupId = _level3SalesGroup.Id,
//            State = "CO",
//            IsActive = true,
//            IsDeleted = false
//        };

//        private static List<ApplicationUser> _firstCompanyUsers = new List<ApplicationUser>
//        {
//            new ApplicationUser
//            {
//                UserName = "AdminUser",
//                Email = "user1@user.com",
//                FirstName = "user1",
//                LastName = "user1",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.Admin
//            },
//            new ApplicationUser
//            {
//                UserName = "SalesTeamLevel1Manager",
//                Email = "user2@user.com",
//                FirstName = "user2",
//                LastName = "user2",
//                IsActive = true,
//                PermissionsBypassTpiv = false,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.LevelOneManager
//            },
//            new ApplicationUser
//            {
//                UserName = "Level2Manager",
//                Email = "user3@user.com",
//                FirstName = "user3",
//                LastName = "user3",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.LevelTwoManager
//            },
//            new ApplicationUser
//            {
//                UserName = "Level3Manager",
//                Email = "user4@user.com",
//                FirstName = "user4",
//                LastName = "user4",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.LevelThreeManager
//            },
//            new ApplicationUser
//            {
//                UserName = "SalesUser",
//                Email = "user5@user.com",
//                FirstName = "user5",
//                LastName = "user5",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                SalesTeamId = _firstCompanySalesTeam.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.SalesRep
//            },
//            new ApplicationUser
//            {
//                UserName = "OtherLevel1Manager",
//                Email = "user6@user.com",
//                FirstName = "user6",
//                LastName = "user6",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.LevelOneManager
//            }
//        };

//        private static int _adminUserIndex = 0;
//        private static int _salesTeamL1UserIndex = 1;
//        private static int _l2UserIndex = 2;
//        private static int _l3UserIndex = 3;
//        private static int _salesUserIndex = 4;
//        private static int _oL1UserIndex = 5;

//        private static ApplicationUser _secondCompanyUser = new ApplicationUser
//        {
//            UserName = "SecondCompanyUser",
//            Email = "c2@user.com",
//            FirstName = "c2user",
//            LastName = "c2user",
//            IsActive = true,
//            PermissionsBypassTpiv = true,
//            CompanyId = _secondCompany.Id,
//            DateCreated = DateTime.UtcNow,
//            DateModified = DateTime.UtcNow
//        };




//        [TestFixtureSetUp]
//        public async void FixtureSetUp()
//        {
//            var companyService = new CompanyDataService();
//            companyService.Add(_firstCompany);
//            companyService.Add(_secondCompany);

//            var l1Service = new Level1SalesGroupDataService();
//            var l2Service = new Level2SalesGroupDataService();
//            var l3Service = new Level3SalesGroupDataService();

//            var stService = new SalesTeamDataService(null);

//            l1Service.Add(_level1SalesGroup);
//            l2Service.Add(_level2SalesGroup);
//            l3Service.Add(_level3SalesGroup);
//            ApplicationUser adminuser = new ApplicationUser
//            {
//                UserName = "AdminUser",
//                Email = "user1@user.com",
//                FirstName = "user1",
//                LastName = "user1",
//                IsActive = true,
//                PermissionsBypassTpiv = true,
//                CompanyId = _firstCompany.Id,
//                DateCreated = DateTime.UtcNow,
//                DateModified = DateTime.UtcNow,
//                Role = ValidApplicationRoles.Admin
                
//            };
//            await stService.AddSalesTeamAsync(_firstCompanySalesTeam, adminuser);

//            _userManager =
//                new UserManager<ApplicationUser, string>(
//                    new UserStore
//                        <ApplicationUser, ApplicationRole, string, IdentityUserLogin, ApplicationUserRole,
//                            IdentityUserClaim>(new ApplicationDbContext()));
//            _firstCompanyUsers.ForEach(u => _userManager.Create(u));
//            _userManager.Create(_secondCompanyUser);

//            _userManager.AddToRole(_firstCompanyUsers[_adminUserIndex].Id, ValidApplicationRoles.Admin.Name);
//            _userManager.AddToRole(_firstCompanyUsers[_salesTeamL1UserIndex].Id,
//                ValidApplicationRoles.LevelOneManager.Name);
//            _userManager.AddToRole(_firstCompanyUsers[_l2UserIndex].Id, ValidApplicationRoles.LevelTwoManager.Name);
//            _userManager.AddToRole(_firstCompanyUsers[_l3UserIndex].Id, ValidApplicationRoles.LevelThreeManager.Name);
//            _userManager.AddToRole(_firstCompanyUsers[_salesUserIndex].Id, ValidApplicationRoles.SalesRep.Name);
//            _userManager.AddToRole(_firstCompanyUsers[_oL1UserIndex].Id, ValidApplicationRoles.LevelOneManager.Name);

//            _level1SalesGroup.Managers.Add(_firstCompanyUsers[_salesTeamL1UserIndex]);
//            l1Service.Update(_level1SalesGroup);
//            _level2SalesGroup.Managers.Add(_firstCompanyUsers[_l2UserIndex]);
//            l2Service.Update(_level2SalesGroup);
//            _level3SalesGroup.Managers.Add(_firstCompanyUsers[_l3UserIndex]);
//            l3Service.Update(_level3SalesGroup);

//        }

//        [TestFixtureTearDown]
//        public void FixtureTearDown()
//        {
//            var companyService = new CompanyDataService();
//            companyService.Delete(_firstCompany.Id);
            
//        }

//        [Test]
//        public async void _001_We_Should_Be_Able_To_Properly_Validate_User_Creation()
//        {
//            var appUserService = new ApplicationUserDataService();

//            var adminUser = _firstCompanyUsers[_adminUserIndex];
//            var salesTeamLevel1User = _firstCompanyUsers[_salesTeamL1UserIndex];
//            var otherLevel1User = _firstCompanyUsers[_oL1UserIndex];
//            var leve2User = _firstCompanyUsers[_l2UserIndex];
//            var salesUser = _firstCompanyUsers[_salesUserIndex];
            
//            var permissionsValidationResult = await appUserService.ValidateUserCreationAsync(salesUser,
//                ValidApplicationRoles.SalesRep, salesUser);
//            Assert.That(permissionsValidationResult.IsSuccessful, Is.False);
//            Assert.That(permissionsValidationResult.Error, Is.EqualTo(ErrorValues.USER_CRUD_PERMISSIONS_ERROR));


//            var userRoleValidationResult =
//                await appUserService.ValidateUserCreationAsync(salesTeamLevel1User, ValidApplicationRoles.LevelOneManager,
//                        leve2User);
//            Assert.That(userRoleValidationResult.IsSuccessful, Is.False);
//            var error = userRoleValidationResult.Error;
//            Assert.That(error.UserHelp, Contains.Substring(ErrorValues.CannotAssignRoleToUserUserHelp));

//            var salesTeamOwnershipValidationResult =
//                await appUserService.ValidateUserCreationAsync(salesUser, ValidApplicationRoles.SalesRep, otherLevel1User);
//            Assert.That(salesTeamOwnershipValidationResult.IsSuccessful, Is.False);
//            error = salesTeamOwnershipValidationResult.Error;
//            Assert.That(error.UserHelp, Contains.Substring(ErrorValues.MANAGER_DOESNT_OWN_SALES_TEAM_ERROR.UserHelp));

//        }

//        [Test]
//        public async void _002_We_Should_Be_Able_To_Update_Logged_In_User()
//        {
//            var loggedInUser = _firstCompanyUsers[0];

//            var newUserName = "user1UNupdated";
//            var newFirstName = "user1FNupdated";
//            var newLastName = "user1LNupdated";
//            var newEmail = "user1Eupdated";

//            var userService = new ApplicationUserDataService();

//            loggedInUser.FirstName = newFirstName;
//            loggedInUser.LastName = newLastName;
//            loggedInUser.Email = newEmail;
//            loggedInUser.UserName = newUserName;
//            loggedInUser.IsActive = false;

//            var updateResult = await userService.UpdateUserOwnedByLoggedInUserAsync(loggedInUser);

//            Assert.That(updateResult.IsSuccessful, Is.True);

//            var updatedUser = userService.Get(loggedInUser.Id).Data;

//            Assert.That(updatedUser.FirstName, Is.EqualTo(newFirstName));
//            Assert.That(updatedUser.LastName, Is.EqualTo(newLastName));
//            Assert.That(updatedUser.Email, Is.EqualTo(newEmail));
//            Assert.That(updatedUser.UserName, Is.EqualTo(newUserName));
//            Assert.That(updatedUser.IsActive, Is.True);
//        }

//        [Test]
//        public async void _002_We_Should_Not_Be_Able_To_Properly_Validate_Non_Logged_In_User_Update()
//        {

//        }

//        [Test]
//        public async void _002_We_Should_Be_Able_To_Update_User_Not_Owned_By_Current_User()
//        {
//            var userToUpdate = _firstCompanyUsers[1];

//            var newUserName = "user2UNupdated";
//            var newFirstName = "user2FNupdated";
//            var newLastName = "user2LNupdated";
//            var newEmail= "user2Eupdated";

//            var userService = new ApplicationUserDataService();

//            userToUpdate.FirstName = newFirstName;
//            userToUpdate.LastName = newLastName;
//            userToUpdate.Email = newEmail;
//            userToUpdate.UserName = newUserName;
//            userToUpdate.SalesTeamId = "bogusid";
//            userToUpdate.IsActive = false;
//            userToUpdate.PermissionsBypassTpiv = true;

////            var updateResult = await userService.UpdateUserNotOwnedByLoggedInUserAsync(
////                    userToUpdate, ValidApplicationRoles.LevelOneManager, userToUpdate.CompanyId
////                );
//            var updateResult = new ServiceProcessingResult<ApplicationUser>();
            
//            Assert.That(updateResult.IsSuccessful, Is.True);

//            var updatedUser = userService.Get(userToUpdate.Id).Data;

//            Assert.That(updatedUser.FirstName,Is.EqualTo(newFirstName));
//            Assert.That(updatedUser.LastName,Is.EqualTo(newLastName));
//            Assert.That(updatedUser.Email,Is.EqualTo(newEmail));
//            Assert.That(updatedUser.UserName,Is.Not.EqualTo(newUserName));
//            Assert.That(updatedUser.SalesTeamId, Is.Null);
//            Assert.That(updatedUser.IsActive,Is.False);
//            Assert.That(updatedUser.PermissionsBypassTpiv,Is.True);
//        }

//        [Test]
//        public async void _003_We_Should_Be_Able_To_Mark_A_User_As_Deleted()
//        {
//            var userToMarkAsDeleted = _firstCompanyUsers[1];

//            var userService = new ApplicationUserDataService();

//            var deleteResult = await userService.MarkUserAsDeletedAsync(userToMarkAsDeleted.Id, userToMarkAsDeleted.CompanyId,
//                ValidApplicationRoles.LevelOneManager);
//            Assert.That(deleteResult.IsSuccessful, Is.False);

//            deleteResult = await userService.MarkUserAsDeletedAsync(userToMarkAsDeleted.Id, userToMarkAsDeleted.CompanyId,
//                ValidApplicationRoles.Admin);
//            Assert.That(deleteResult.IsSuccessful, Is.True);

//            var deletedUser = userService.Get(userToMarkAsDeleted.Id).Data;
//            Assert.That(deletedUser, Is.Not.Null);
//            Assert.That(deletedUser.IsDeleted, Is.True);

//        }
    //}
}
