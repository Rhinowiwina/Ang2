using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LS.Domain;

namespace LS.ApiBindingModels
{

    public class SalesTeamEditBindingModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Please select a Level 3 Sales Group")]
        public string Level3SalesGroupId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address 1 is required")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "Signature pad type is required")]
        public string SigType { get; set;}

        [Required(ErrorMessage = "Zip is required")]
        public string Zip { get; set; }

        public string Phone { get; set; }
        public string TaxId { get; set; }
        public string PayPalEmail { get; set; }
        public string CycleCountTypeDevice { get; set; }
        public string CycleCountTypeSim { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class SalesTeamCreationBindingModel : SalesTeamEditBindingModel
    {
        [Required(ErrorMessage = "External Primary Id is required")]
        public string ExternalPrimaryId { get; set; }

        [Required(ErrorMessage = "External Display Name is required")]
        public string ExternalDisplayName { get; set; }
    }

    // Is it okay to have a view model in here? 
    // Or should it be in it's own file in the Models folder?
    public class SalesTeamAndManagersViewModel
    {
        public SalesTeam SalesTeam { get; set; }
        public List<ApplicationUser> SalesTeamManagers { get; set; } 
    }

    public class SalesTeamViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalPrimaryId { get; set; }
        public string ExternalDisplayName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string SigType { get; set; }
        public string TaxId { get; set; }
        public string PayPalEmail { get; set; }
        public string CycleCountTypeDevice { get; set; }
        public string CycleCountTypeSim { get; set; }
        public string Level3SalesGroupId { get; set; }
        public GroupSimpleViewBindingModel Level3SalesGroup { get; set; }
        public List<UserListViewBindingModel> Users { get; set; }
        public bool IsActive { get; set; }
    }

    public class SalesTeamSimpleViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalPrimaryId { get; set; }
        public string ExternalDisplayName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string SigType { get; set; }
        public string Phone { get; set; }
        public string TaxId { get; set; }
        public string PayPalEmail { get; set; }
        public string CycleCountTypeDevice { get; set; }
        public string CycleCountTypeSim { get; set; }
        public string Level3SalesGroupId { get; set; }
        public bool IsActive { get; set; }
    }

    public class SalesTeamNameViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalDisplayName { get; set; }
    }

    public class SalesTeamNameActiveViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalDisplayName { get; set; }
        public bool IsActive { get; set; }
    }

    public class SalesTeamNameUsersViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalDisplayName { get; set; }
        public List<UserListViewBindingModel> Users { get; set; }
    }

    public class SalesTeamNameUsersNoTeamViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalDisplayName { get; set; }
        public bool IsActive { get; set; }
        public List<UserListViewNoTeamBindingModel> Users { get; set; }
        public List<ProductCommissions> Commissions { get; set; }
    }
}