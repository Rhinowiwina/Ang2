using System;
using LS.Domain;
using System.Collections.Generic;

namespace LS.ApiBindingModels {
    public static class DomainModelsConverter {
        public static UserViewBindingModel ToUserViewBindingModel(this ApplicationUser user) {
            var convertedRole = user.Role.ToRoleSimpleViewBindingModel();
            var convertedSalesTeam = new SalesTeamSimpleViewBindingModel();
            if (user.SalesTeam != null) { convertedSalesTeam = user.SalesTeam.ToSalesTeamSimpleViewBindingModel(); }

            return new UserViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                PayPalEmail = user.PayPalEmail,
                IsActive = user.IsActive,
                RoleId = user.Role.Id,
                Role = convertedRole,
                SalesTeamId = user.SalesTeamId,
                SalesTeam = convertedSalesTeam,
                PermissionsAccountOrder = user.PermissionsAccountOrder,
                PermissionsAllowTpivBypass = user.PermissionsBypassTpiv,
                RowVersion = Convert.ToBase64String(user.RowVersion)
            };
        }

        public static UserViewBindingModel ToUserViewBindingModel(this ApplicationUser user, bool canBeDeleted) {
            var convertedRole = user.Role.ToRoleSimpleViewBindingModel();
            var convertedSalesTeam = new SalesTeamSimpleViewBindingModel();
            if (user.SalesTeam != null) { convertedSalesTeam = user.SalesTeam.ToSalesTeamSimpleViewBindingModel(); }

            return new UserViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ExternalUserID = user.ExternalUserID,
                IsExternalUserIDActive = user.IsExternalUserIDActive,
                Email = user.Email,
                PayPalEmail=user.PayPalEmail,
                UserName = user.UserName,
                IsActive = user.IsActive,
                AdditionalDataNeeded=user.AdditionalDataNeeded,
                RoleId = user.Role.Id,
                Role = convertedRole,
                SalesTeamId = user.SalesTeamId,
                SalesTeam = convertedSalesTeam,      
                PermissionsAllowTpivBypass = user.PermissionsBypassTpiv,
                PermissionsAccountOrder = user.PermissionsAccountOrder,
                PermissionsLifelineCA = user.PermissionsLifelineCA,
                RowVersion = Convert.ToBase64String(user.RowVersion),
                CanBeDeleted = canBeDeleted
            };
        }

        public static UserSimpleViewBindingModel ToUserSimpleViewBindingModel(this ApplicationUser user) {
            return new UserSimpleViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                PayPalEmail = user.PayPalEmail,
                IsActive = user.IsActive,
                RoleId = user.Role.Id,
                SalesTeamId = user.SalesTeamId,
                PermissionsAllowTpivBypass = user.PermissionsBypassTpiv,
                PermissionsAccountOrder = user.PermissionsAccountOrder,
                PermissionsLifelineCA = user.PermissionsLifelineCA,
                RowVersion = Convert.ToBase64String(user.RowVersion)
            };
        }

        public static UserListViewBindingModel ToUserListViewBindingModel(this ApplicationUser user) {
            //var convertedRole = new RoleSimpleViewBindingModel();
            var convertedRole = user.Role.ToRoleSimpleViewBindingModel();
            
            var convertedSalesTeam = new SalesTeamNameViewBindingModel();
            if (user.SalesTeam != null) { convertedSalesTeam = user.SalesTeam.ToSalesTeamNameViewBindingModel(); }

            return new UserListViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ExternalUserID=user.ExternalUserID,
                UserName = user.UserName,
                IsActive = user.IsActive,
                Role = convertedRole,
                SalesTeam = convertedSalesTeam
            };
        }

        public static UserListViewNoTeamBindingModel ToUserListViewNoTeamBindingModel(this ApplicationUser user) {
            var convertedRole = user.Role.ToRoleSimpleViewBindingModel();
            var convertedSalesTeam = new SalesTeamNameViewBindingModel();
            if (user.SalesTeam != null) { convertedSalesTeam = user.SalesTeam.ToSalesTeamNameViewBindingModel(); }

            return new UserListViewNoTeamBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                IsActive = user.IsActive,
                Role = convertedRole
            };
        }

        public static UserNameViewBindingModel ToUserNameViewBindingModel(this ApplicationUser user) {
            return new UserNameViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public static UserNameRoleViewBindingModel ToUserNameRoleViewBindingModel(this ApplicationUser user) {
            var convertedRole = user.Role.ToRoleSimpleViewBindingModel();

            return new UserNameRoleViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = convertedRole
            };
        }

        public static UserNameViewBindingModel ToUserNameViewBindingModel(this UserNameRoleViewBindingModel user) {
            return new UserNameViewBindingModel {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }       
        public static ApiGetUserBindingModel ToApiGetUserBindingModel(this ApplicationUser user)
        {
            return new ApiGetUserBindingModel
            {
                FirstName=user.FirstName,
                LastName=user.LastName,
                Email=user.Email,
                AgentID=user.SalesTeam !=null ? user.SalesTeam.ExternalDisplayName:"",
                Active=user.IsActive,
                Permissions_AccounOrder=user.PermissionsAccountOrder,
                Permissions_AccountOrder_CA = user.PermissionsLifelineCA,
                IsError =false,
                ErrorMessage_Developer="",
                ErrorMessage_User="",
            };

        }
        public static LoggedInUserViewBindingModel ToLoggedInUserViewBindingModel(this ApplicationUser user, string sigType,bool? TeamActive) {
            return new LoggedInUserViewBindingModel {
                Id = user.Id,
                Username = user.UserName,
                Company = user.Company,
                Role = user.Role,
                SalesTeamId = user.SalesTeamId,
                SalesTeamSigType = sigType,
                PermissionsAccountOrder = user.PermissionsAccountOrder,
                PermissionsLifelineCA=user.PermissionsLifelineCA,
                ExternalUserID=user.ExternalUserID,
                PermissionsAllowTpivBypass = user.PermissionsBypassTpiv,
                Firstname = user.FirstName,
                Lastname= user.LastName,
                Language = user.Language,
                SalesTeamActive = TeamActive
            };
        }


        public static List<GroupTreeViewBindingModel> ToLevel1GroupTreeViewBindingModelList(this List<Level1SalesGroup> groups) {
            var convertedGroups = new List<GroupTreeViewBindingModel>();
            foreach (var group in groups) {
                var convertedGroup = ToLevel1GroupTreeViewBindingModel(group);
                convertedGroups.Add(convertedGroup);
            }

            return convertedGroups;
        }

        public static List<GroupAdminTreeViewBindingModel> ToLevel1GroupAdminTreeViewBindingModelList(this List<Level1SalesGroup> groups) {
            var convertedGroups = new List<GroupAdminTreeViewBindingModel>();
            foreach (var group in groups) {
                var convertedGroup = ToLevel1GroupAdminTreeViewBindingModel(group);
                convertedGroups.Add(convertedGroup);
            }

            return convertedGroups;
        }

        public static List<UserTreeViewBindingModel> ToLevel1UserTreeViewBindingModelList(this List<Level1SalesGroup> groups) {
            var convertedGroups = new List<UserTreeViewBindingModel>();
            foreach (var group in groups) {
                var convertedGroup = ToLevel1UserTreeViewBindingModel(group);
                convertedGroups.Add(convertedGroup);
            }

            return convertedGroups;
        }

        public static GroupTreeViewBindingModel ToLevel1GroupTreeViewBindingModel(this Level1SalesGroup group) {
            var convertedManagers = new List<UserNameRoleViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserNameRoleViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedGroups = new List<GroupTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel2GroupTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new GroupTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups
            };
        }

        public static GroupAdminTreeViewBindingModel ToLevel1GroupAdminTreeViewBindingModel(this Level1SalesGroup group) {
            var convertedGroups = new List<GroupAdminTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel2GroupAdminTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new GroupAdminTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                ChildSalesGroups = convertedGroups
            };
        }

        public static UserTreeViewBindingModel ToLevel1UserTreeViewBindingModel(this Level1SalesGroup group) {
            var convertedManagers = new List<UserListViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserListViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedGroups = new List<UserTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel2UserTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new UserTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups
            };
        }

        public static GroupTreeViewBindingModel ToLevel2GroupTreeViewBindingModel(this Level2SalesGroup group) {
            var convertedManagers = new List<UserNameRoleViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserNameRoleViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedGroups = new List<GroupTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel3GroupTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new GroupTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups
            };
        }

        public static GroupAdminTreeViewBindingModel ToLevel2GroupAdminTreeViewBindingModel(this Level2SalesGroup group) {
            var convertedGroups = new List<GroupAdminTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel3GroupAdminTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new GroupAdminTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                ChildSalesGroups = convertedGroups
            };
        }

        public static UserTreeViewBindingModel ToLevel2UserTreeViewBindingModel(this Level2SalesGroup group) {
            var convertedManagers = new List<UserListViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserListViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedGroups = new List<UserTreeViewBindingModel>();
            foreach (var childGroup in group.ChildSalesGroups) {
                var convertedGroup = childGroup.ToLevel3UserTreeViewBindingModel();
                convertedGroups.Add(convertedGroup);
            }

            return new UserTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups
            };
        }

        public static GroupTreeViewBindingModel ToLevel3GroupTreeViewBindingModel(this Level3SalesGroup group) {
            var convertedManagers = new List<UserNameRoleViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserNameRoleViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedTeams = new List<SalesTeamNameUsersNoTeamViewBindingModel>();
            foreach (var team in group.SalesTeams) {
                var convertedTeam = team.ToSalesTeamNameUsersNoTeamViewBindingModel();
                convertedTeams.Add(convertedTeam);
            }

            var convertedGroups = new List<GroupTreeViewBindingModel>();

            return new GroupTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups,
                SalesTeams = convertedTeams
            };
        }

        public static GroupAdminTreeViewBindingModel ToLevel3GroupAdminTreeViewBindingModel(this Level3SalesGroup group) {
            var convertedGroups = new List<GroupAdminTreeViewBindingModel>();

            return new GroupAdminTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                ChildSalesGroups = convertedGroups
            };
        }

        public static UserTreeViewBindingModel ToLevel3UserTreeViewBindingModel(this Level3SalesGroup group) {
            var convertedManagers = new List<UserListViewBindingModel>();
            foreach (var manager in group.Managers) {
                var convertedManager = manager.ToUserListViewBindingModel();
                convertedManagers.Add(convertedManager);
            }

            var convertedTeams = new List<SalesTeamNameUsersViewBindingModel>();
            foreach (var team in group.SalesTeams) {
                var convertedTeam = team.ToSalesTeamNameUsersViewBindingModel();
                convertedTeams.Add(convertedTeam);
            }

            var convertedGroups = new List<UserTreeViewBindingModel>();

            return new UserTreeViewBindingModel {
                Id = group.Id,
                Name = group.Name,
                Managers = convertedManagers,
                ChildSalesGroups = convertedGroups,
                SalesTeams = convertedTeams
            };
        }

        public static GroupSimpleViewBindingModel ToLevel1GroupSimpleViewBindingModel(this Level1SalesGroup group) {
            return new GroupSimpleViewBindingModel {
                Id = group.Id,
                Name = group.Name
            };
        }

        public static GroupSimpleViewBindingModel ToLevel2GroupSimpleViewBindingModel(this Level2SalesGroup group) {
            return new GroupSimpleViewBindingModel {
                Id = group.Id,
                Name = group.Name
            };
        }

        public static GroupSimpleViewBindingModel ToLevel3GroupSimpleViewBindingModel(this Level3SalesGroup group) {
            return new GroupSimpleViewBindingModel {
                Id = group.Id,
                Name = group.Name
            };
        }

        public static RoleSimpleViewBindingModel ToRoleSimpleViewBindingModel(this ApplicationRole role) {
            return new RoleSimpleViewBindingModel()
                {
                Id = role.Id,
                Name = role.Name,
                Rank = role.Rank,
                };
            }

        public static SalesTeamViewBindingModel ToSalesTeamViewBindingModel(this SalesTeam team) {
            var convertedGroup = new GroupSimpleViewBindingModel();
            if (team.Level3SalesGroup != null) { team.Level3SalesGroup.ToLevel3GroupSimpleViewBindingModel(); }

            var convertedUsers = new List<UserListViewBindingModel>();
            foreach (var user in team.Users) {
                var convertedUser = user.ToUserListViewBindingModel();
                convertedUsers.Add(convertedUser);
            }

            return new SalesTeamViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalPrimaryId = team.ExternalPrimaryId,
                ExternalDisplayName = team.ExternalDisplayName,
                Address1 = team.Address1,
                Address2 = team.Address2,
                City = team.City,
                State = team.State,
                Zip = team.Zip,
                Phone = team.Phone,
                TaxId = team.TaxId,
                PayPalEmail = team.PayPalEmail,
                CycleCountTypeDevice = team.CycleCountTypeDevice,
                CycleCountTypeSim = team.CycleCountTypeSim,
                Level3SalesGroupId = team.Level3SalesGroupId,
                Level3SalesGroup = convertedGroup,
                Users = convertedUsers,
                IsActive = team.IsActive
            };
        }

        public static SalesTeamSimpleViewBindingModel ToSalesTeamSimpleViewBindingModel(this SalesTeam team) {
            return new SalesTeamSimpleViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalPrimaryId = team.ExternalPrimaryId,
                ExternalDisplayName = team.ExternalDisplayName,
                Address1 = team.Address1,
                Address2 = team.Address2,
                City = team.City,
                State = team.State,
                Zip = team.Zip,
                Phone = team.Phone,
                TaxId = team.TaxId,
                PayPalEmail = team.PayPalEmail,
                CycleCountTypeDevice = team.CycleCountTypeDevice,
                CycleCountTypeSim = team.CycleCountTypeSim,
                Level3SalesGroupId = team.Level3SalesGroupId,
                IsActive = team.IsActive
            };
        }

        public static SalesTeamNameViewBindingModel ToSalesTeamNameViewBindingModel(this SalesTeam team) {
            return new SalesTeamNameViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalDisplayName = team.ExternalDisplayName
            };
        }

        public static SalesTeamNameActiveViewBindingModel ToSalesTeamNameActiveViewBindingModel(this SalesTeam team) {
            return new SalesTeamNameActiveViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalDisplayName = team.ExternalDisplayName,
                IsActive = team.IsActive
            };
        }

        public static SalesTeamNameUsersViewBindingModel ToSalesTeamNameUsersViewBindingModel(this SalesTeam team) {
            var convertedUsers = new List<UserListViewBindingModel>();
            foreach (var user in team.Users) {
                var convertedUser = user.ToUserListViewBindingModel();
                convertedUsers.Add(convertedUser);
            }

            return new SalesTeamNameUsersViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalDisplayName = team.ExternalDisplayName,
                Users = convertedUsers
            };
        }

        public static SalesTeamNameUsersNoTeamViewBindingModel ToSalesTeamNameUsersNoTeamViewBindingModel(this SalesTeam team) {
            var convertedUsers = new List<UserListViewNoTeamBindingModel>();
            foreach (var user in team.Users) {
                var convertedUser = user.ToUserListViewNoTeamBindingModel();
                convertedUsers.Add(convertedUser);
            }

            return new SalesTeamNameUsersNoTeamViewBindingModel {
                Id = team.Id,
                Name = team.Name,
                ExternalDisplayName = team.ExternalDisplayName,
                IsActive = team.IsActive,
                Users = convertedUsers
            };
        }
    }
}
