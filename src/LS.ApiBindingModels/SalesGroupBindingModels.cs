using LS.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LS.ApiBindingModels
{
    public class SalesGroupAddBindingModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string ParentSalesGroupId { get; set; }
        public List<string> ManagerIds { get; set; }
    }

    public class SalesGroupEditBindingModel : SalesGroupAddBindingModel
    {
        [Required(ErrorMessage = "Id is required.")]
        public string Id { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class GroupTreeViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserNameRoleViewBindingModel> Managers { get; set; }
        public List<GroupTreeViewBindingModel> ChildSalesGroups { get; set; }
        public List<SalesTeamNameUsersNoTeamViewBindingModel> SalesTeams { get; set; }
    }

    public class GroupAdminTreeViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<GroupAdminTreeViewBindingModel> ChildSalesGroups { get; set; }
    }

    public class UserTreeViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserListViewBindingModel> Managers { get; set; }
        public List<UserTreeViewBindingModel> ChildSalesGroups { get; set; }
        public List<SalesTeamNameUsersViewBindingModel> SalesTeams { get; set; }
    }
    
    public class GroupSimpleViewBindingModel {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
