using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LS.Core.Interfaces;

namespace LS.Domain
{
    public class CompletedOrder
    {

        public string Id { get; set; }
        public string CompanyId { get; set; }


        public string SalesTeamId { get; set; }

        public string UserId { get; set; }

        // Customer Info
        public string FirstName { get; set; }
        public string MiddleInitial { get; set; }
        public string LastName { get; set; }

        // Service Address
        public string ServiceAddressStreet1 { get; set; }
        public string ServiceAddressStreet2 { get; set; }
        public string ServiceAddressCity { get; set; }
        public string ServiceAddressState { get; set; }
        public string ServiceAddressZip { get; set; }
        public bool ServiceAddressIsPermanent { get; set; }
        public bool ServiceAddressIsRural { get; set; }

        // Billing Address
        public string BillingAddressStreet1 { get; set; }
        public string BillingAddressStreet2 { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressZip { get; set; }

        // Shipping Address
        public string ShippingAddressStreet1 { get; set; }
        public string ShippingAddressStreet2 { get; set; }
        public string ShippingAddressCity { get; set; }
        public string ShippingAddressState { get; set; }
        public string ShippingAddressZip { get; set; }

      
        public string DeviceId { get; set; }
        public string DeviceIdentifier { get; set; }
        public string SimIdentifier { get; set; }
        public string PlanId { get; set; } // Set to -1 by default

        //Company Info
        public string TenantReferenceId { get; set; }
        public string TenantAccountId { get; set; }
        public string TenantAddressId { get; set; }
        public string FulfillmentType { get; set; }
        public string DeviceModel { get; set; }

        public string SalesTeamExternalDisplayName { get; set; }
        public string SalesTeamName { get; set; }

        public string EmployeeFirstName { get; set; }
        public string EmployeeLastName { get; set; }

        // IEntity stuff
        public DateTime DateCreated { get; set; }
        public string DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
