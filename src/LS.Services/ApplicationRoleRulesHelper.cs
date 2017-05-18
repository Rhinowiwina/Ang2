using LS.Core;
using LS.Domain;

namespace LS.Services
{
    public static class ApplicationRoleRulesHelper
    {
        private static readonly string RoleValidationFailedUserMessage = "Invalid role assignemnt.";

        private static readonly string CannotAssignRoleToUserUserHelp =
            "You do not have permission to assign this role to a user.";

        public static readonly int SalesTeamManagerRank = ValidApplicationRoles.SalesTeamManagerRank;

        public static readonly int Level1SalesGroupManagerRank = ValidApplicationRoles.LevelOneManagerRank;
        public static readonly int Level2SalesGroupManagerRank = ValidApplicationRoles.LevelTwoManagerRank;
        public static readonly int Level3SalesGroupManagerRank = ValidApplicationRoles.LevelThreeManagerRank;
        public static readonly int MinimumRankForSalesTeamAssignment = SalesTeamManagerRank;
        public static readonly int MaximumRankForSalesTeamCreation = ValidApplicationRoles.LevelThreeManagerRank;


        public static bool CanBeAddedToSalesTeam(this ApplicationRole roleBeingAdded)
        {
            return roleBeingAdded.Rank >= MinimumRankForSalesTeamAssignment;
        }


        public static bool CanCreateOrUpdateSalesUser(this ApplicationRole role)
        {
            return role.Rank <= Level3SalesGroupManagerRank;
        }

        public static bool CanCrudSalesTeam(this ApplicationRole role)
        {
            return role.Rank <= MaximumRankForSalesTeamCreation;
        }
        public static bool IsSuperAdmin(this ApplicationRole role)
        {
            return role.Rank == ValidApplicationRoles.SuperAdmin.Rank;
        }
        public static bool IsAdmin(this ApplicationRole role)
        {
            return role.Rank == ValidApplicationRoles.Admin.Rank;
        }

        public static bool IsLevelManager(this ApplicationRole role)
        {
            return role.Rank <= Level3SalesGroupManagerRank && role.Rank >= Level1SalesGroupManagerRank;
        }

        public static bool IsNonSalesRole(this ApplicationRole role)
        {
            return role.Rank <= Level3SalesGroupManagerRank;
        }
        public static bool CanAssign(this ApplicationRole creatorRole, ApplicationRole roleBeingAssigned)
        {
            if (roleBeingAssigned == null || (roleBeingAssigned.IsLevelManager() && creatorRole.Rank >= roleBeingAssigned.Rank))
            {
                return false;
            }
            return creatorRole.Rank < roleBeingAssigned.Rank;
        }

        public static ServiceProcessingResult<ValidationResult> ValidateRoleAssignmentFor(ApplicationRole roleBeingAssigned,
            ApplicationRole loggedInUserRole)
        {
            var processingResult = new ServiceProcessingResult<ValidationResult> { IsSuccessful = true };
            var validationResult = new ValidationResult
            {
                IsValid = true
            };

            if (roleBeingAssigned == null)
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add("Invalid role. Please select a valid role and try again.");
            }
            else if (!loggedInUserRole.CanAssign(roleBeingAssigned))
            {
                validationResult.IsValid = false;
                validationResult.Errors.Add(CannotAssignRoleToUserUserHelp);
            }
            processingResult.Data = validationResult;

            return processingResult;
        }
    }
}
