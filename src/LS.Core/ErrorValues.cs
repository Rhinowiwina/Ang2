using System.Globalization;

namespace LS.Core
{
    public static class ErrorValues
    {
        public static readonly string ContactSupportUserHelp = "An error occurred, please try again later. If the problem persists, contact support";

        public static readonly string UserDoesNotExistInCompanyUserHelp = "This user does not belong to your company.";
        public static readonly string UserUpdateFailedUserMessage = "Failed to edit user.";
        public static readonly string UserUpdateValidationFailedUserMessage = "User update validation failed.";
        public static readonly string UserDeleteFailedUserMessage = "Failed to delete user.";
        public static readonly string UserCreationFailedUserMessage = "Failed to create user.";
        public static readonly string UserCreationValidationFailedUserMessage = "User create validation failed.";
        public static readonly string UserNameAlreadyExistsUserHelp = "User failed validation. Username, Email or External User ID already exist in the database.";

        public static readonly string UserCrudPermissionsUserHelp = "You do not have a high enough role to perform this action on this user.  If you think this is a mistake, please contact an Administrator";
        public static readonly string CreateUserNotInCompanyUserHelp = "You are attempting to create a user in a different company.";
        public static readonly string UserMustBeAssignedToSalesTeamUserHelp = "This user must be assigned to a Sales Team. Please select a Sales Team and try again.";
        public static readonly string InvalidRoleForSalesTeamAssignmentUserHelp = "A user with this role cannot be assigned to a sales team.";
        public static readonly string ManagerDoesntOwnSalesTeamUserHelp = "You do not manage this sales team and you can only assign users to sales teams that you manage.";
        public static readonly string CannotAssignRoleToUserUserHelp = "You do not have permission to assign this role to a user.";

        public static readonly string UserDeleteFailedDueToManagerAssignmentUserHelp = "User cannot be deleted because they are assigned as a Manager.";
        public static readonly string AdminsCannotBeDeletedUserHelp = "Unable to delete user because Administrators cannot be deleted.";

        public static readonly string SalesGroupCreateFailedUserMessage = "Failed to create sales group.";
        public static readonly string SalesGroupUpdateFailedUserMessage = "Failed to update sales group.";
        public static readonly string SalesGroupGetFailedUserMessage = "Failed to get sales group.";
        public static readonly string SalesGroupDeleteFailedUserMessage = "Failed to delete sales team.";
        public static readonly string SalesGroupDeleteValidationFailedUserMessage = "Sales Group deletion validation failed.";
        public static readonly string CannotDeleteSalesGroupWithChildSalesGroupsOrTeamsUserHelp = "A sales group that has sales group or sales team children cannot be deleted. If you think this is a mistake, please contact support.";
        public static readonly string CannotDeleteSalesGroupWithManagersAssignedUserHelp = "A sales group that has managers assigned to it cannot be deleted. If you think this is a mistake, please contact support.";

        public static readonly string SalesTeamCreateFailedUserMessage = "Failed to create sales team.";
        public static readonly string SalesTeamCreateValidationFailedUserMessage = "Sales Team add validation failed.";
        public static readonly string SalesTeamUpdateFailedUserMessage = "Failed to update sales team.";
        public static readonly string SalesTeamUpdateValidationFailedUserMessage = "Sales Team update validation failed.";
        public static readonly string SalesTeamDeleteFailedUserMessage = "Failed to delete sales team.";
        public static readonly string SalesTeamDeleteValidationFailedUserMessage = "Sales Team deletion validation failed.";
        public static readonly string SalesTeamAssignmentInvalidUserMessage = "User cannot be assigned to this sales team.";
        public static readonly string SalesTeamCrudPermissionsUserHelp = "You do not have a high enough role to perform this action on a sales team.  If you think this is a mistake, please contact an Administrator";
        public static readonly string ManageSalesTeamUserPermissionUserHelp = "You do not have permission to manage sales teams in this sales group. If you think this is a mistake, please contact an Administrator.";
        public static readonly string ModifySalesTeamSalesGroupNotInCompanyUserHelp = "The sales group you attempted to add the sales team to does not exist within your company.  If you think this is a mistake, please contact support";
        public static readonly string DeleteSalesTeamNotInCompanyUserHelp = "You are attempting to delete a sales team that does not belong to your company. If you think this is a mistake, please contact support";

        public static readonly string SalesTeamCommissionFailedUserMessage = "Failed to create commissions for this order";
        public static readonly string SalesTeamCommissionFailedUserHelp = "A problem occurred saving commissions for this order. An error log has been created.";

        public static readonly string CannotTakeOrderWithInactiveUserUserMessage = "You are attempting to take an order but your account is inactive. If you think this is a mistake, please contact an Administrator.";
        public static readonly string CannotTakeOrderWithInactiveSalesTeamUserMessage = "You are attempting to take an order with a Sales Team that is inactive. If you think this is a mistake, please contact an Administrator.";
        public static readonly string CannotTakeOrderOutsideOfCompanyUserMessage = "Failed to create order because there is a problem with you account. If the problem persists, please contact support.";

        public static readonly string ComplianceQuestionResponsesInvalidUserMessage = "Order validation failed. If household receives benefits, they must be received by the customer.";
        public static readonly string MiddleInitialMustBeOnlyOneLetterUserMessage = "Order validation failed. Middle initial field must be no longer than one letter.";
        public static readonly string LifelineProgramProofNumbersMustMatchUserMessage = "Order validation failed. Lifeline program proof numbers must match.";
        public static readonly string InvalidLifelineProgramIdUserMessage = "Order validation failed. Lifeline program id provided is invalid.";
        public static readonly string StateProgramNumberRequiredButMissingUserMessage = "Order validation failed. State program number is required but was not provided.";
        public static readonly string SalesTeamForOrderIsInactiveUserMessage = "Order validation failed. The sales team you attempted to take an order for is currently inactive.";
        public static readonly string EmailRequiredButMissingUserMessage = "Order validation failed. Customer email is required in this company and no email was provided.";
        public static readonly string PhoneNumberRequiredButMissingUserMessage = "Order validation failed. Customer phone number is required in this company and no phone number was provided.";

        public static readonly string CannotBeginOrderUserMessage = "Unable to begin order.";

        public static readonly string InvalidStateUserHelp = "Please provide a valid 2 letter state code.";

        public static readonly string ValidationFailedUserMessage = "Validation failed for one or more entities.";

        public static readonly string NoValidNladSacNumber = "No valid SAC number exists for that state.";
        public static readonly string NladPhoneNumberGenericError = "A valid phone number could not be generated for this order. Please try again.";

        public static readonly string PuertoRicoPhoneNumberGenericError = "A valid phone number could not be generated for this order. Please try again.";

        public static readonly string MissingTpivBypassFields = "Missing required fields for TPIV bypass. Please fill out all sections of TPIV bypass section";

        public static readonly ProcessingError GENERIC_FATAL_BACKEND_ERROR = new ProcessingError(ContactSupportUserHelp, ContactSupportUserHelp, true);

        public static readonly ProcessingError USER_CRUD_PERMISSIONS_ERROR = new ProcessingError("You cannot perform this action on this user.", UserCrudPermissionsUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_ROLE_ASSIGNMENT_ERROR = new ProcessingError(UserCreationFailedUserMessage, CannotAssignRoleToUserUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_WITH_EXISTING_USERNAME_ERROR = new ProcessingError(UserCreationFailedUserMessage, UserNameAlreadyExistsUserHelp, false, true);
        public static readonly ProcessingError CREATE_USER_NOT_IN_COMPANY_ERROR = new ProcessingError(UserCreationFailedUserMessage, CreateUserNotInCompanyUserHelp, false, true);
        public static readonly ProcessingError USER_CREATION_FAILED_ERROR = new ProcessingError(UserCreationFailedUserMessage, ContactSupportUserHelp, true);

        public static readonly ProcessingError GET_USER_WITHOUT_ID_ERROR = new ProcessingError("Cannot get user without an Id.", "Please provide a user id and try again", false);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_USER_ERROR = new ProcessingError("Failed to find user in company.", "Could not find this user in this company.  If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_USERS_ERROR = new ProcessingError("Failed to find any users in company.", "Could not find any users in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_COULD_NOT_FIND_MANAGERS_ERROR = new ProcessingError("Failed to find any managers in company.", "Could not find and managers in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError USER_UPDATE_PERMISSIONS_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "You do not have permission to edit this user.  If you think this is a mistake, please contact an Administrator.", false, true);
        public static readonly ProcessingError UPDATE_USER_INVALID_USER_ROLE_ID_PROVIDED_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "Could not update user because the RoleId provided was invalid.", false, true);
        public static readonly ProcessingError GENERIC_UPDATE_USER_FAILED_ERROR = new ProcessingError(UserUpdateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_UPDATE_ERROR = new ProcessingError(UserUpdateFailedUserMessage, UserDoesNotExistInCompanyUserHelp, false, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_BEGIN_ORDER_ERROR = new ProcessingError("Order cannot be started.", "Failed to find User to begin order with. If the problem persists, please contact support.", true);
        public static readonly ProcessingError UPDATE_USER_WITH_INVALID_ROW_VERSION_ERROR = new ProcessingError(UserUpdateFailedUserMessage, "This user was edited by someone else while you were editing them.  Please refresh the page and make the desired changes.", false, true);
        public static readonly ProcessingError CANNOT_DELETE_USER_ASSIGNED_AS_MANAGER = new ProcessingError("User cannot be deleted.", UserDeleteFailedDueToManagerAssignmentUserHelp, false);
        public static readonly ProcessingError INVALID_ROLE_ASSIGNMENT_ERROR = new ProcessingError("Error creating/editing user because Role was invalid.", "An error occurred because you attempted to add an invalid role to the user. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GET_LOGGED_IN_USER_INFO_ERROR = new ProcessingError("An error occurred while getting logged in user's info.", "An error occurred while retrieving your info. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_VALIDATE_USER_PERMISSIONS_ERROR = new ProcessingError("An error occurred while validating user permissions.", "An error occurred while trying to ensure you had permission to perform this action. If the problem persists, please contact support.", false);

        public static readonly ProcessingError GENERIC_DELETE_USER_FAILED_ERROR = new ProcessingError(UserDeleteFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError COULD_NOT_FIND_USER_TO_DELETE_ERROR = new ProcessingError(UserDeleteFailedUserMessage, UserDoesNotExistInCompanyUserHelp, false, true);
        public static readonly ProcessingError USER_DELETE_PERMISSIONS_ERROR = new ProcessingError(UserDeleteFailedUserMessage, "You do not have permission to delete this user. If you think this is a mistake, please contact an Administrator", false, true);
        public static readonly ProcessingError USER_ROLE_UPDATE_FAILED_ERROR = new ProcessingError("Failed to update user's role.", ContactSupportUserHelp, true);

        public static readonly ProcessingError GENERIC_SALES_GROUP_PERMISSIONS_ERROR = new ProcessingError("Failed to complete operation for sales group.", "You do not have permission to complete this operation. If you think this is a mistake, please contact an Administrator.", false);
        public static readonly ProcessingError GENERIC_ADD_SALES_GROUP_ERROR = new ProcessingError(SalesGroupCreateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError ADD_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesGroupCreateFailedUserMessage, "You are attempting to assign this sales group to a parent sales group that does not exist within your company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_GET_SALES_GROUP_WITH_MANAGER_IN_TREE_ERROR = new ProcessingError("Failed to get sales group with manager in tree.", ContactSupportUserHelp, true);
        public static readonly ProcessingError GENERIC_GET_SALES_GROUP_IN_COMPANY_ERROR = new ProcessingError(SalesGroupGetFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError GENERIC_GET_ALL_SALES_GROUPS_IN_COMPANY_ERROR = new ProcessingError("Failed to find any sales groups in this company.", "Could not find any sales groups in this company. If you think this is a mistake, please contact support.", true);
        public static readonly ProcessingError GET_SALES_GROUP_WITHOUT_ID_ERROR = new ProcessingError(SalesGroupGetFailedUserMessage, "Failed to get sales group because no Id was provided. Please try again.", false, true);
        public static readonly ProcessingError GENERIC_UPDATE_SALES_GROUP_ERROR = new ProcessingError(SalesGroupUpdateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError UPDATE_SALES_GROUP_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesGroupUpdateFailedUserMessage, "You are attempting to edit a sales group that does not belong to your company.  If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError UPDATE_SALES_GROUP_PARENT_SALES_GROUP_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesGroupUpdateFailedUserMessage, "You are attempting to assign this sales group to a parent sales group that does not exist within your company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_DELETE_SALES_GROUP_ERROR = new ProcessingError(SalesGroupDeleteFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError DELETE_SALES_GROUP_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesGroupDeleteFailedUserMessage, "You are attempting to delete a sales group that does not belong to your company.  If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError DELETE_SALES_GROUP_WITHOUT_ID_ERROR = new ProcessingError(SalesGroupDeleteFailedUserMessage, "Failed to delete sales group because no Id was provided. Please try again.", false, true);
        public static readonly ProcessingError COULD_NOT_FIND_SALES_GROUP_IN_COMPANY_ERROR = new ProcessingError("Failed to find sales group in company.", "Could not find this sales group in this company. If you think this is a mistake, please contact support.", false, true);
        
        public static readonly ProcessingError SALES_TEAM_CRUD_PERMISSIONS_ERROR = new ProcessingError("You cannot perform this action on a sales team.", SalesTeamCrudPermissionsUserHelp, false, true);
        public static readonly ProcessingError CREATE_SALES_TEAM_INVALID_STATE_ERROR = new ProcessingError(SalesTeamCreateFailedUserMessage, InvalidStateUserHelp, false, true);
        public static readonly ProcessingError CREATE_SALES_TEAM_MANAGER_PERMISSION_ERROR = new ProcessingError(SalesTeamCreateFailedUserMessage, ManageSalesTeamUserPermissionUserHelp, false, true);
        public static readonly ProcessingError CREATE_SALES_TEAM_SALES_GROUP_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesTeamCreateFailedUserMessage, ModifySalesTeamSalesGroupNotInCompanyUserHelp, false, true);
        public static readonly ProcessingError GENERIC_ADD_SALES_TEAM_ERROR = new ProcessingError(SalesTeamCreateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError GENERIC_GET_SALES_TEAM_IN_COMPANY_ERROR = new ProcessingError("Failed to get sales team in company.", ContactSupportUserHelp, true);
        public static readonly ProcessingError GET_SALES_TEAM_WITHOUT_ID_ERROR = new ProcessingError(SalesGroupGetFailedUserMessage, "Failed to get sales group because no Id was provided. Please try again.", false, true);
        public static readonly ProcessingError COULD_NOT_FIND_SALES_TEAM_IN_COMPANY_ERROR = new ProcessingError("Failed to find sales team in company.", "Could not find this sales team in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError COULD_NOT_FIND_SALES_TEAMS_IN_COMPANY_ERROR = new ProcessingError("Failed to find any sales teams in company.", "Could not find any sales teams in this company. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError UPDATE_SALES_TEAM_INVALID_STATE_ERROR = new ProcessingError(SalesTeamUpdateFailedUserMessage, InvalidStateUserHelp, false, true);       
        public static readonly ProcessingError GENERIC_UPDATE_SALES_TEAM_ERROR = new ProcessingError(SalesTeamUpdateFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError GENERIC_UPDATE_SALES_TEAM_COMMISSIONS_ERROR = new ProcessingError("Failed to update sales team commissions.", ContactSupportUserHelp, true);
        public static readonly ProcessingError UPDATE_SALES_TEAM_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesTeamUpdateFailedUserMessage, "You are attempting to edit a sales team that does not belong to your company.  If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError UPDATE_SALES_TEAM_SALES_GROUP_NOT_IN_COMPANY  = new ProcessingError(SalesTeamUpdateFailedUserMessage, "You are attempting to assign this sales team to a sales group that does not exist within your company. If you think this is a mistake, please contact support", false, true);
        public static readonly ProcessingError GENERIC_DELETE_SALES_TEAM_ERROR = new ProcessingError(SalesTeamDeleteFailedUserMessage, ContactSupportUserHelp, true);
        public static readonly ProcessingError DELETE_SALES_TEAM_NOT_IN_COMPANY_ERROR = new ProcessingError(SalesTeamDeleteFailedUserMessage, DeleteSalesTeamNotInCompanyUserHelp, false, true);
        public static readonly ProcessingError DELETE_SALES_TEAM_WITHOUT_ID_ERROR = new ProcessingError(SalesTeamDeleteFailedUserMessage, "Please provide the Id of the sales team you would like to delete.", false, true);
        public static readonly ProcessingError MANAGER_DOESNT_OWN_SALES_TEAM_ERROR = new ProcessingError(SalesTeamAssignmentInvalidUserMessage, ManagerDoesntOwnSalesTeamUserHelp, false, true);
        public static readonly ProcessingError INVALID_ROLE_FOR_SALES_TEAM_ASSIGNMENT_ERROR = new ProcessingError(SalesTeamAssignmentInvalidUserMessage, InvalidRoleForSalesTeamAssignmentUserHelp, false, true);
        public static readonly ProcessingError SALES_TEAM_ASSIGNMENT_VALIDATION_FAILED_ERROR = new ProcessingError("Failed to validate sales team assignment.", ContactSupportUserHelp, true);
        public static readonly ProcessingError SALES_TEAM_COMMISSION_CREATION_ERROR = new ProcessingError(SalesTeamCommissionFailedUserMessage, SalesTeamCommissionFailedUserHelp, true);

        public static readonly ProcessingError GENERIC_SEARCH_WITHOUT_CRITERIA_ERROR = new ProcessingError("Failed to search.", "Search could not be completed because no criteria was provided.", false, true);
        public static readonly ProcessingError GENERIC_SEARCH_ERROR = new ProcessingError("An error occurred while completing search.", ContactSupportUserHelp, true);

        public static readonly ProcessingError GENERIC_EXTERNAL_API_RESPONSE_ERROR = new ProcessingError("Failed to determine a valid external API.", "An error occurred while processing your request. If the problem persists, please contact support.", true);
        public static readonly ProcessingError NO_VALID_EXTERNAL_API_FOUND = new ProcessingError("Failed to determine a valid external API", "An error occurred while processing your request. If the problem persists, please contact support.", true);

        public static readonly ProcessingError CANNOT_BEGIN_ORDER_WITH_INACTIVE_USER_ERROR = new ProcessingError(CannotBeginOrderUserMessage, "Unable to begin order because your account is not active. Please contact an administrator.", false);
        public static readonly ProcessingError ZIP_CODE_NOT_VALID_OR_NOT_FOUND_ERROR = new ProcessingError("An error occurred while looking up ZipCode.", "The Zip Code you provided was not found, please try another. If you think this is a mistake, please contact support.", false, true);
        public static readonly ProcessingError GENERIC_GET_COMPLIANCE_STATEMENTS_ERROR = new ProcessingError("An error occurred while looking up Compliance Statements.", "An error occurred while retrieving compliance statements. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_PROOF_DOCUMENT_TYPES_ERROR = new ProcessingError("An error occurred while looking up ProofDocumentTypes.", "An error occurred while retrieving proof document types. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_LIFELINE_PROGRAMS_ERROR = new ProcessingError("An error occurred while looking up LifelinePrograms.", "An error occurred while retrieving lifeline programs. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_STATE_AGREEMENTS_ERROR = new ProcessingError("An error occurred while looking up State Agreements.", "An error occurred while retrieving state agreements. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_COMPETITORS_ERROR = new ProcessingError("An error occurred while looking up Competitors.", "An error occurred while retrieving your company's competitors. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_BASE_INCOME_LEVELS_ERROR = new ProcessingError("An error occurred while looking up BaseIncomeLevels.", "An error occurred while retrieving base income levels for your state. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_STATE_SETTINGS_ERROR = new ProcessingError("An error occurred while looking up State Settings.", "An error occurred while retrieving state settings for your order. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_COMPANY_ERROR = new ProcessingError("An error occurred while looking up Company.", "An error occurred while retrieving company info. If the problem persists, please contact support.", true);
        public static readonly ProcessingError GENERIC_GET_STATE_PROGRAMS_ERROR = new ProcessingError("An error occurred while looking up State Programs.", "An error occurred while retrieving state programs. If the problem persists, please contact support.", true);
        public static readonly ProcessingError SALES_TEAM_REP_OR_MANAGER_SHOULD_HAVE_SALES_TEAM_ASSIGNED = new ProcessingError("Could not get Sales Team for user to choose for Order.", "There is an error with your account. If the problem persists, please contact an Administrator.", false);

        public static readonly ProcessingError GENERIC_ADD_TEMP_ORDER_ERROR = new ProcessingError("An error occurred while creating new entry in TempOrders table.", "An error occurred while writing order data to table. If the problem persists, please contact support.", true);

        public static readonly ProcessingError CANNOT_RESET_PASSWORD_FOR_USER_ROLE_ERROR = new ProcessingError("Could not reset password because you don't have the proper permissions.", "Your request could not be processed because you cannot perform this operation on a user with this role.", false);

        public static readonly ProcessingError IMAGE_HAS_NOT_YET_BEEN_UPLOADED_ERROR = new ProcessingError("The image has not yet been uploaded through the mobile app.", "This image has not been uploaded. The order cannot be completed until you have done so. Please upload the image and try again.", false, true);

        //NLAD
        public static readonly ProcessingError NO_VALID_SAC_NUMBER_ENTRY_FOR_COMPANY = new ProcessingError("No valid SAC number was found to submit this order.", "The order cannot be submitted because a valid SAC number was not found", true);
        public static readonly ProcessingError NO_VALID_ELIGIBILITY_CODE = new ProcessingError("No valid eligibility code was found to submit this order under the selected lifeline program.", "The order cannot be submitted because an eligibility code was not found for the selected lifeline program", true);
        public static readonly ProcessingError GENERIC_NLAD_PHONE_NUMBER_GENERATION_ERROR = new ProcessingError(NladPhoneNumberGenericError, NladPhoneNumberGenericError, true, true);
        //PuertoRico
        public static readonly ProcessingError GENERIC_PuertoRico_PHONE_NUMBER_GENERATION_ERROR = new ProcessingError(PuertoRicoPhoneNumberGenericError, PuertoRicoPhoneNumberGenericError, true, true);

        //Orders
        public static readonly ProcessingError CANNOT_GET_ORDERS = new ProcessingError("Problem retrieving orders, please contact support", "Problem retrieving orders, please contact support", true);
        public static readonly ProcessingError NOLIFELINE_PROGRAMS_AVAILABLE_ERROR = new ProcessingError("There are no Life Line Programs Available for this Zip Code.", "There are no LifeLine Programs Available for this Zip Code.",true);
        public static readonly ProcessingError PEMISSION_DENIED_ERROR = new ProcessingError("User does not have permission to create an order.", "User does not have permission to create an order.", true);
        //systems
        public static readonly ProcessingError CANNOT_GET_SYTEMS = new ProcessingError("Problem retrieving systems status, please contact support", "Problem retrieving systems status, please contact support", true);
        public static readonly ProcessingError GENERIC_UPDATE_SystemStatus_FAILED_ERROR= new ProcessingError("Failed to edit system status.", "Failed to edit system status.", true);
        public static readonly ProcessingError COULD_NOT_FIND_SYSTEM_TO_UPDATE_ERROR=new ProcessingError("Could not find user to update.", "Could not find user to update.", true);
        public static readonly ProcessingError COULD_DELETE_SYSTEMSTATUS_ERROR = new ProcessingError("Could not remove system status.", "Could not remove system status.", true);
        public static readonly ProcessingError GENERIC_SYSTEM_CREATE_ERROR = new ProcessingError("A new system status failed to be created.", "A new system status failed to be created.", true);
        public static readonly ProcessingError GENERIC_DELETE_SYSTEM_ERROR= new ProcessingError("Failed to delete system.", "Failed to delete system.", true);
        public static readonly ProcessingError COULD_NOT_FIND_CompaySystem_TO_UPDATE_ERROR= new ProcessingError("Could not find company system to update.", "Could not find company system to update.", true);
        public static readonly ProcessingError GENERIC_UPDATE_Company_FAILED_ERROR = new ProcessingError("Failed to edit company.", "Failed to edit company.", true);
        public static readonly ProcessingError GENERIC_COMPANY_CREATE_ERROR = new ProcessingError("A new company failed to be created.", "A new company failed to be created.", true);
        public static readonly ProcessingError GENERIC_DELETE_COMPANY_ERROR = new ProcessingError("Failed to remove company.", "Failed to remove company.", true);
        public static readonly ProcessingError DELETE_Company_WITHOUT_ID_ERROR = new ProcessingError("Please provide the Id of the company you would like to delete.", "Please provide the Id of the company you would like to delete.", false, true);
        public static readonly ProcessingError UPDATE_COMPANY_PARENT_SYSTEMSTATUS_NOT_IN_COMPANY_ERROR=new ProcessingError("Failed to update company system status.", "Failed to update company system status.",true);
        public static readonly ProcessingError Commission_Exceeds_Limit= new ProcessingError("Commission exceeds limits.", "Commission exceeds limits.", true);
        public static readonly ProcessingError PERMISSION_ERROR = new ProcessingError("You do not have the permission to execute this function.", "You do not have the permissions to execute this function.", true);
        // Resource URL
        public static readonly ProcessingError STORAGE_CREDENTIAL_ERROR = new ProcessingError("Failed to retrieve storage credentials. Could not create document.", "Failed to retrieve storage credentials.", true);
    }
}
