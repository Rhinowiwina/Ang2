using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace ApiBindingModels {

    public class UsersPortalLevel1Groups {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class UsersPortalSalesTeams {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ExternalPrimaryId { get; set; }
        public string ExternalDisplayName { get; set; }
    }

    public class UsersPortalCreateUsersRequest {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string SalesTeamID { get; set; }
        public string CompanyID { get; set; }
        public string ExternalUserID { get; set; }
    }

    public class ExternalUserIDValidationResponse {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public bool IsActive { get; set; }
    }
}