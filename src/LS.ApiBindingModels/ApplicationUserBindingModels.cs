using System.ComponentModel.DataAnnotations;
using LS.Domain;
using System;
using System.Collections.Generic;

namespace LS.ApiBindingModels {
    public class UserViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExternalUserID { get; set; }
        public bool IsExternalUserIDActive { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PayPalEmail { get; set; }
        public bool IsActive { get; set; }
        public bool AdditionalDataNeeded { get; set; }
        public string RoleId { get; set; }
        public RoleSimpleViewBindingModel Role { get; set; }
        public bool PermissionsLifelineNlad { get; set; }
        public bool PermissionsLifelineCA { get; set; }
        public bool PermissionsLifelineTX { get; set; }
        public bool PermissionsAllowTpivBypass { get; set; }
        public bool PermissionsAccountOrder { get; set; }
        public string SalesTeamId { get; set; }
        public SalesTeamSimpleViewBindingModel SalesTeam { get; set; }
        public string RowVersion { get; set; }
        public bool CanBeDeleted { get; set; }
    }
    public class UserDrugExpirations {
        public string PortalUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateEnabled { get; set; } = null;
        public DateTime? DateApproved { get; set; } = null;
        public DateTime? DateDrugTest { get; set; } = null;
        public string DrugScreenCertificateID { get; set; }
        public string CompanyID { get; set; }
    }
    public class ExportUserViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyID { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PictureID { get; set; }
        public string Ssn { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
    public class UserOnBoardReturnView {
        public string PictureUrl { get; set; }
        public string Id { get; set; }
        public string CompanyID { get; set; }
        public string StreetAddress1 { get; set; }
        public string StreetAddress2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string PictureID { get; set; }
        public string Ssn { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
    public class TeamUserView {
        public string Id { get; set; }
        public string FullName {
            get {
                if (FirstName.Length > 0 && LastName.Length > 0) {
                    return FirstName + " " + LastName;
                } else if (FirstName.Length > 0) {
                    return FirstName;
                } else {
                    return LastName;
                }
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExternalUserID { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
    }
    public class UserManagedGroupsBindingModel {
        public string UseID { get; set; }
        public string GroupName { get; set; }

    }
    public class UserSimpleViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExternalUserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PayPalEmail { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
        public bool PermissionsLifelineNlad { get; set; }
        public bool PermissionsLifelineCA { get; set; }
        public bool PermissionsLifelineTX { get; set; }
        public bool PermissionsAllowTpivBypass { get; set; }
        public bool PermissionsAccountOrder { get; set; }
        public string SalesTeamId { get; set; }
        public string RowVersion { get; set; }
        public bool CanBeDeleted { get; set; }
    }

    public class UserListViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ExternalUserID { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public SalesTeamNameViewBindingModel SalesTeam { get; set; }
        public RoleSimpleViewBindingModel Role { get; set; }
    }
    public class UserSearchViewBindingModel {
        public string Id { get; set; }
        public string Role { get; set; }

        public string FirstName { get; set; }
        public string FullName {
            get {
                if (FirstName.Length > 0 && LastName.Length > 0) {
                    return FirstName + " " + LastName;
                } else if (FirstName.Length > 0) {
                    return FirstName;
                } else {
                    return LastName;
                }
            }
        }
        public string ExternalUserID { get; set; }
        public bool IsExternalUserIDActive { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public string ExternalDisplayName { get; set; }
        public string Team { get; set; }
    }
    public class UserListScreenViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string FullName {
            get {
                if (FirstName.Length > 0 && LastName.Length > 0) {
                    return FirstName + " " + LastName;
                } else if (FirstName.Length > 0) {
                    return FirstName;
                } else {
                    return LastName;
                }
            }
        }
        public string ExternalUserID { get; set; }
        public bool IsExternalUserIDActive { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public string ExternalDisplayName { get; set; }
        public string Team { get; set; }
    }


    public class UserNameViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserNameRoleViewBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public RoleSimpleViewBindingModel Role { get; set; }
    }

    public class UserListViewNoTeamBindingModel {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public RoleSimpleViewBindingModel Role { get; set; }
    }

    public class LoggedInUserViewBindingModel {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string ExternalUserID { get; set; }
        public Company Company { get; set; }
        public ApplicationRole Role { get; set; }
        public string SalesTeamId { get; set; }
        public string SalesTeamSigType { get; set; }
        public bool? SalesTeamActive { get; set; }

        public bool PermissionsLifelineCA { get; set; }

        public bool PermissionsAllowTpivBypass { get; set; }
        public bool PermissionsAccountOrder { get; set; }
        public string PayPalEmail { get; set; }
        public string Language { get; set; }
        public string ServerEnvironment { get; set; }
    }

    public class UserCreationBindingModel {
        //        [Required]
        public string RoleId { get; set; }
        public string GroupId { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ExternalUserID { get; set; } = "";
        public bool ExternalUserIDActive { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "PayPal Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid Paypal email address")]
        public string PayPalEmail { get; set; }
        public string CompanyId { get; set; }
        public string SalesTeamId { get; set; }

        [Required(ErrorMessage = "NLAD Permissions field is required")]
        public bool PermissionsLifelineNlad { get; set; }

        [Required(ErrorMessage = "CA Lifeline Permissions field is required")]
        public bool PermissionsLifelineCA { get; set; }

        [Required(ErrorMessage = "TX Lifeline Permissions field is required")]
        public bool PermissionsLifelineTX { get; set; }

        [Required(ErrorMessage = "Allow TPIV Bypass field is required")]
        public bool? PermissionsAllowTpivBypass { get; set; }
        [Required(ErrorMessage = "Allow Permission Account field is required")]
        public bool PermissionsAccountOrder { get; set; }

        [Required(ErrorMessage = "Active field is required")]
        public bool? IsActive { get; set; }
        public bool AdditionalDataNeeded { get; set; }
    }

    public class UserCreationAPIBindingModel {
        //        [Required]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }

        public string ExternalUserID { get; set; }
        public bool ExternalUserIDActive { get; set; }

        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "PayPal Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid Paypal email address")]
        public string PayPalEmail { get; set; }

        public string SalesTeamId { get; set; }

        [Required(ErrorMessage = "NLAD Permissions field is required")]
        public bool PermissionsLifelineNlad { get; set; }

        [Required(ErrorMessage = "CA Lifeline Permissions field is required")]
        public bool PermissionsLifelineCA { get; set; }

        [Required(ErrorMessage = "TX Lifeline Permissions field is required")]
        public bool PermissionsLifelineTX { get; set; }

        [Required(ErrorMessage = "Allow TPIV Bypass field is required")]
        public bool? PermissionsAllowTpivBypass { get; set; }
        [Required(ErrorMessage = "Allow Permission Account field is required")]
        public bool PermissionsAccountOrder { get; set; }

        [Required(ErrorMessage = "Active field is required")]
        public bool? IsActive { get; set; }
    }

    public class UserUpdateBindingModel {
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "Please enter a user name")]
        public string UserName { get; set; }
        public string GroupId { get; set; }
        [Required(ErrorMessage = "Please select a role")]
        public string RoleId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        public string ExternalUserID { get; set; }
        public bool ExternalUserIDActive { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid Paypal email address")]
        public string PayPalEmail { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; }
        //place holder for email so it can be tested old vs new
        public string OriginalEmail { get; set; }
        public string SalesTeamId { get; set; }
        public string CompanyId { get; set; }

        [Required(ErrorMessage = "Is active is required")]
        public bool? IsActive { get; set; }
        public bool AdditionalDataNeeded { get; set; }
        public bool PermissionsLifelineNlad { get; set; }

        public bool PermissionsLifelineCA { get; set; }

        public bool PermissionsLifelineTX { get; set; }
        public bool PermissionsAccountOrder { get; set; }

        [Required(ErrorMessage = "Allow TPIV Bypass is required")]
        public bool? PermissionsAllowTpivBypass { get; set; }

        [Required]
        public string RowVersion { get; set; }

    }

    public class ManagerViewBindingModel {
        public List<UserNameViewBindingModel> Level1Managers { get; set; }
        public List<UserNameViewBindingModel> Level2Managers { get; set; }
        public List<UserNameViewBindingModel> Level3Managers { get; set; }
        public List<UserNameViewBindingModel> TeamManagers { get; set; }
    }

    public class ApiGetUserBindingModel {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AgentID { get; set; }
        public bool Active { get; set; }
        public bool Permissions_AccounOrder { get; set; }
        public bool Permissions_AccountOrder_NLAD { get; set; }
        public bool Permissions_AccountOrder_CA { get; set; }
        public bool Permissions_AccountOrder_TX { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage_Developer { get; set; }
        public string ErrorMessage_User { get; set; }

    }
}
