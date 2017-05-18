using System;
using System.Collections.Generic;
using LS.Domain;
using LS.Domain.ExternalApiIntegration;
using LS.Domain.ExternalApiIntegration.CGM;
using LS.Domain.ExternalApiIntegration.Interfaces;
using LS.Core.Interfaces;
using System.Data.SqlClient;

namespace LS.ApiBindingModels {
    public static class BindingModelsConverter {
        public static Level1SalesGroup ToLevel1SalesGroup(this SalesGroupAddBindingModel model, string loggedInUserId, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level1SalesGroup {
                Name = model.Name,
                CompanyId = companyId,
                CreatedByUserId = loggedInUserId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static Level1SalesGroup ToLevel1SalesGroup(this SalesGroupEditBindingModel model, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level1SalesGroup {
                Id = model.Id,
                Name = model.Name,
                CompanyId = companyId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static Level2SalesGroup ToLevel2SalesGroup(this SalesGroupAddBindingModel model, string loggedInUserId, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level2SalesGroup {
                Name = model.Name,
                CompanyId = companyId,
                ParentSalesGroupId = model.ParentSalesGroupId,
                CreatedByUserId = loggedInUserId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static Level2SalesGroup ToLevel2SalesGroup(this SalesGroupEditBindingModel model, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level2SalesGroup {
                Id = model.Id,
                Name = model.Name,
                CompanyId = companyId,
                ParentSalesGroupId = model.ParentSalesGroupId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static Level3SalesGroup ToLevel3SalesGroup(this SalesGroupAddBindingModel model, string loggedInUserId, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level3SalesGroup {
                Name = model.Name,
                CompanyId = companyId,
                ParentSalesGroupId = model.ParentSalesGroupId,
                CreatedByUserId = loggedInUserId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static Level3SalesGroup ToLevel3SalesGroup(this SalesGroupEditBindingModel model, string companyId) {
            var managers = new List<ApplicationUser>();
            if (model.ManagerIds != null) {
                model.ManagerIds.ForEach(id => managers.Add(new ApplicationUser { Id = id }));
            }
            return new Level3SalesGroup {
                Id = model.Id,
                Name = model.Name,
                CompanyId = companyId,
                ParentSalesGroupId = model.ParentSalesGroupId,
                Managers = managers,
                IsDeleted = false
            };
        }

        public static SalesTeam ToSalesTeam(this SalesTeamCreationBindingModel model, string companyId, string createdByUserId) {
            return new SalesTeam {
                ExternalPrimaryId = model.ExternalPrimaryId,
                ExternalDisplayName = model.ExternalDisplayName,
                Name = model.Name,
                SigType = model.SigType,
                Level3SalesGroupId = model.Level3SalesGroupId,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                State = model.State,
                Zip = model.Zip,
                Phone = model.Phone,
                TaxId = model.TaxId,
                PayPalEmail = model.PayPalEmail,
                CycleCountTypeDevice = model.CycleCountTypeDevice,
                CycleCountTypeSim = model.CycleCountTypeSim,
                CompanyId = companyId,
                CreatedByUserId = createdByUserId,
                IsActive = model.IsActive
            };
        }

        public static SalesTeam ToSalesTeam(this SalesTeamEditBindingModel model, string companyId) {
            return new SalesTeam {
                Id = model.Id,
                Name = model.Name,
                SigType = model.SigType,
                Level3SalesGroupId = model.Level3SalesGroupId,
                Address1 = model.Address1,
                Address2 = model.Address2,
                City = model.City,
                State = model.State,
                Zip = model.Zip,
                Phone = model.Phone,
                TaxId = model.TaxId,
                PayPalEmail = model.PayPalEmail,
                CycleCountTypeDevice = model.CycleCountTypeDevice,
                CycleCountTypeSim = model.CycleCountTypeSim,
                CompanyId = companyId,
                IsActive = model.IsActive,
                IsDeleted=model.IsDeleted
            };
        }

        public static ApplicationUser ToApplicationUser(this UserCreationBindingModel model, string companyId, string createdByUserId) {
            var utcNow = DateTime.UtcNow;
            return new ApplicationUser {
                UserName = model.Email,
                Email = model.Email,
                PayPalEmail = model.PayPalEmail,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ExternalUserID=model.ExternalUserID,
                IsExternalUserIDActive = model.ExternalUserIDActive,
                SalesTeamId = model.SalesTeamId,
                IsActive = model.IsActive.Value, 
                AdditionalDataNeeded=model.AdditionalDataNeeded,          
                PermissionsBypassTpiv = model.PermissionsAllowTpivBypass.Value,
                PermissionsAccountOrder = model.PermissionsAccountOrder,
                CompanyId = companyId,
                CreatedByUserId = createdByUserId,
                DateCreated = utcNow,
                DateModified = utcNow
            };
        }
        public static LoginMsg ToLoginMsg(this LoginMsgBindingModel model)
            {
            return new LoginMsg()
                {
                Id = model.Id,
                Title = model.Title,
                Msg = model.Msg,
                BeginDate = model.BeginDate,
                ExpirationDate = model.ExpirationDate,
                Active = model.Active,
                MsgLevel = model.MsgLevel,
                DateModified = DateTime.UtcNow,
                };
            }
        public static LoginMsg ToNewLoginMsg(this LoginMsgBindingModel model)
            {
            return new LoginMsg()
                {
                Id = Guid.NewGuid().ToString(),
               Title = model.Title,
                Msg = model.Msg,
                BeginDate = model.BeginDate,
                ExpirationDate = model.ExpirationDate,
                Active=model.Active,
                MsgLevel = model.MsgLevel,
                DateModified = DateTime.UtcNow,
                };
            }
        public static ApplicationUser ToApplicationUser(this UserUpdateBindingModel model, string companyId) {

            return new ApplicationUser {
                Id = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ExternalUserID=model.ExternalUserID,
                IsExternalUserIDActive = model.ExternalUserIDActive,
                Email = model.Email,
                PayPalEmail = model.PayPalEmail,
                CompanyId = companyId,
                SalesTeamId = model.SalesTeamId,
                IsActive = model.IsActive.Value,
                AdditionalDataNeeded = model.AdditionalDataNeeded,
                PermissionsBypassTpiv = model.PermissionsAllowTpivBypass.Value,
                PermissionsAccountOrder = model.PermissionsAccountOrder,
                PermissionsLifelineCA = model.PermissionsLifelineCA,
                RowVersion = Convert.FromBase64String(model.RowVersion),
                UserName = model.UserName
            };
        }

        public static ApplicationUser ToApplicationUser(this UserCreationAPIBindingModel model, string companyId, string createdByUserId) {
            var utcNow = DateTime.UtcNow;
            return new ApplicationUser {
                UserName = model.Email,
                Email = model.Email,
                PayPalEmail = model.PayPalEmail,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ExternalUserID=model.ExternalUserID,
                SalesTeamId = model.SalesTeamId,
                IsActive = model.IsActive.Value,
                IsExternalUserIDActive = model.ExternalUserIDActive,
                PermissionsBypassTpiv = model.PermissionsAllowTpivBypass.Value,
                PermissionsAccountOrder = model.PermissionsAccountOrder,
                CompanyId = companyId,
                CreatedByUserId = createdByUserId,
                DateCreated = utcNow,
                DateModified = utcNow
            };
        }

        public static TempOrder ToTempOrder(this SubmitOrderBindingModel model, string companyId, ApplicationUser user) {
            return new TempOrder {
                ParentOrderId=model.ParentOrderId,
                TransactionId=model.TransactionId,
                CompanyId = companyId,
                UserId = user.Id,
                SalesTeamId = model.SalesTeamId,
                HouseholdReceivesLifelineBenefits = model.HouseholdReceivesLifelineBenefits,
                CustomerReceivesLifelineBenefits = model.CustomerReceivesLifelineBenefits,
                QBFirstName = model.QBFirstName,
                QBLastName = model.QBLastName,
                UnencryptedQBDateOfBirth = model.QBDateOfBirth,
                UnencryptedQBSsn = model.QBSsn,
                Language = model.Language,
                CommunicationPreference=model.CommunicationPreference,
                CurrentLifelinePhoneNumber = model.CurrentLifelinePhoneNumber,
                LifelineProgramId = model.LifelineProgramId,
                LPProofTypeId = model.LPProofTypeId,
                LPProofNumber = model.LPProofNumber,
                LPProofImageID= model.LPProofImageUploadId,
                LPProofImageFilename = model.LPProofImageUploadFilename,
                StateProgramId = model.StateProgramId,
                StateProgramNumber = model.StateProgramNumber,
                SecondaryStateProgramId = model.SecondaryStateProgramId,
                SecondaryStateProgramNumber = model.SecondaryStateProgramNumber,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                Gender=model.Gender,
                UnencryptedSsn = model.Ssn,
                UnencryptedDateOfBirth = model.DateOfBirth,
                EmailAddress = model.EmailAddress,
                ContactPhoneNumber = model.ContactPhoneNumber,
                IDProofTypeID = model.IPProofTypeId,
                IDProofImageID = model.IPProofImageUploadId,
                IDProofImageFilename = model.IPProofImageUploadFilename,
                IDProofImageID2 = model.IPProofImageUploadId2,
                IDProofImageFilename2 = model.IPProofImageUploadFilename2,
                ServiceAddressStreet1 = model.ServiceAddressStreet1,
                ServiceAddressStreet2 = model.ServiceAddressStreet2,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressZip = model.ServiceAddressZip,
                ServiceAddressIsPermanent = model.ServiceAddressIsPermanent,
                BillingAddressStreet1 = "",
                BillingAddressStreet2 = "",
                BillingAddressCity = "",
                BillingAddressState = "",
                BillingAddressZip = "",
                ShippingAddressStreet1 = model.ShippingAddressStreet1,
                ShippingAddressStreet2 = model.ShippingAddressStreet2,
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ShippingAddressState,
                ShippingAddressZip = model.ShippingAddressZip,
                HohSpouse = model.HohSpouse,
                HohAdultsParent = model.HohAdultsParent,
                HohAdultsChild = model.HohAdultsChild,
                HohAdultsRelative = model.HohAdultsRelative,
                HohAdultsRoommate = model.HohAdultsRoommate,
                HohAdultsOther = model.HohAdultsOther,
                HohAdultsOtherText = model.HohAdultsOtherText,
                HohExpenses = model.HohExpenses,
                HohShareLifeline = model.HohShareLifeline,
                HohPuertoRicoAgreeViolation=model.HohPuertoRicoAgreeViolation,
                // HohShareLifelineNames = todo do we still need this field?
                HohAgreeMultiHouse = model.HohAgreeMultihouse,
                HohAgreeViolation = model.HohAgreeViolation,
                AgreementStatements = model.AgreementStatements,
                Initials=model.Initials,
                Signature = model.Signature,
                SignatureType=model.SignatureType,
                LatitudeCoordinate = model.LatitudeCoordinate,
                LongitudeCoordinate = model.LongitudeCoordinate,
                AIAvgIncome = model.LifelineProgramAnnualIncome.AverageIncomeAmount,
                AIFrequency = model.LifelineProgramAnnualIncome.AverageIncomeDuration,
                AIInitials = model.LifelineProgramAnnualIncome.AnnualIncomeCustomerInitials,
                AINumHousehold = (model.LifelineProgramAnnualIncome.NumHouseAdult + model.LifelineProgramAnnualIncome.NumHouseChildren),
                AINumHouseAdult= model.LifelineProgramAnnualIncome.NumHouseAdult,
                AINumHouseChildren= model.LifelineProgramAnnualIncome.NumHouseChildren,
                ServiceAddressBypass =model.HasServiceAddressBypass,
                ServiceAddressBypassProofTypeID=model.ServiceAddressByPassDocumentProofId,
                ServiceAddressBypassSignature=model.ServiceAddressBypassSignature,
                ServiceAddressBypassProofImageID=model.ServiceAddressByPassImageID,
                TpivBypass = model.TpivBypass,
                TpivBypassSignature = model.TpivBypassSignature,
                TpivBypassSsnProofTypeId = model.TpivBypassSsnDocumentId,
                TpivBypassSsnProofImageFilename = model.TpivSsnImageUploadId,
                TpivBypassSsnProofNumber = model.TpivBypassSsnLast4Digits,
                TpivBypassDobProofNumber = model.TpivBypassDobLast4Digits,
                TpivBypassDobProofTypeID = model.TpivBypassDobDocumentId,
                FulfillmentType = model.FulfillmentType,
                TpivCode=model.TpivCode,
                TpivNasScore=model.TpivNasScore,
                TpivRiskIndicators=model.TpivRiskIndicators,
                TpivTransactionID=model.TpivTransactionID,
                DeviceId = model.DeviceId,
                DeviceModel=model.DeviceModel,
                DeviceCompatibility=model.DeviceCompatibility,          
                ByopCarrier=model.ByopCarrier
                
            };
        }

        public static TempOrder ToTempOrder(this SubmitOrderBindingModel model, string companyId) {
            return new TempOrder {
                TransactionId = model.TransactionId,
                CompanyId = companyId,
                UserId = "",
                SalesTeamId = model.SalesTeamId,
                HouseholdReceivesLifelineBenefits = model.HouseholdReceivesLifelineBenefits,
                CustomerReceivesLifelineBenefits = model.CustomerReceivesLifelineBenefits,

                QBFirstName = model.QBFirstName,
                QBLastName = model.QBLastName,
                UnencryptedQBDateOfBirth = model.QBDateOfBirth,
                UnencryptedQBSsn = model.QBSsn,

                CurrentLifelinePhoneNumber = model.CurrentLifelinePhoneNumber,
                LifelineProgramId = model.LifelineProgramId,
                LPProofTypeId = model.LPProofTypeId,
                LPProofNumber = model.LPProofNumber,
                LPProofImageID = model.LPProofImageUploadId,
                LPProofImageFilename = model.LPProofImageUploadFilename,
                StateProgramId = model.StateProgramId,
                StateProgramNumber = model.StateProgramNumber,
                SecondaryStateProgramId = model.SecondaryStateProgramId,
                SecondaryStateProgramNumber = model.SecondaryStateProgramNumber,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                UnencryptedSsn = model.Ssn,
                UnencryptedDateOfBirth = model.DateOfBirth,
                EmailAddress = model.EmailAddress,
                ContactPhoneNumber = model.ContactPhoneNumber,
                IDProofTypeID = model.IPProofTypeId,
                IDProofImageID = model.IPProofImageUploadId,
                IDProofImageFilename = model.IPProofImageUploadFilename,
                ServiceAddressStreet1 = model.ServiceAddressStreet1,
                ServiceAddressStreet2 = model.ServiceAddressStreet2,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressZip = model.ServiceAddressZip,
                ServiceAddressIsPermanent = model.ServiceAddressIsPermanent,
                BillingAddressStreet1 = model.BillingAddressStreet1,
                BillingAddressStreet2 = model.BillingAddressStreet2,
                BillingAddressCity = model.BillingAddressCity,
                BillingAddressState = model.BillingAddressState,
                BillingAddressZip = model.BillingAddressZip,
                ShippingAddressStreet1 = model.ShippingAddressStreet1,
                ShippingAddressStreet2 = model.ShippingAddressStreet2,
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ShippingAddressState,
                ShippingAddressZip = model.ShippingAddressZip,
                HohSpouse = model.HohSpouse,
                HohAdultsParent = model.HohAdultsParent,
                HohAdultsChild = model.HohAdultsChild,
                HohAdultsRelative = model.HohAdultsRelative,
                HohAdultsRoommate = model.HohAdultsRoommate,
                HohAdultsOther = model.HohAdultsOther,
                HohAdultsOtherText = model.HohAdultsOtherText,
                HohExpenses = model.HohExpenses,
                HohShareLifeline = model.HohShareLifeline,
                // HohShareLifelineNames = todo do we still need this field?
                HohAgreeMultiHouse = model.HohAgreeMultihouse,
                HohAgreeViolation = model.HohAgreeViolation,
                AgreementStatements = model.AgreementStatements,
                Signature = model.Signature,
                SignatureType = model.SignatureType,
                LatitudeCoordinate = model.LatitudeCoordinate,
                LongitudeCoordinate = model.LongitudeCoordinate,
                AIAvgIncome = model.LifelineProgramAnnualIncome.AverageIncomeAmount,
                AIFrequency = model.LifelineProgramAnnualIncome.AverageIncomeDuration,
                AIInitials = model.LifelineProgramAnnualIncome.AnnualIncomeCustomerInitials,
                AINumHousehold = model.LifelineProgramAnnualIncome.HouseholdSized,
                TpivBypass = model.TpivBypass,
                TpivCode = model.TpivCode,
                TpivNasScore = model.TpivNasScore,
                TpivRiskIndicators = model.TpivRiskIndicators,
                TpivTransactionID = model.TpivTransactionID,
                TpivBypassSignature = model.TpivBypassSignature,
                TpivBypassSsnProofTypeId = model.TpivBypassSsnDocumentId,
                TpivBypassSsnProofNumber = model.TpivBypassSsnLast4Digits,
                TpivBypassDobProofNumber = model.TpivBypassDobLast4Digits,
                TpivBypassDobProofTypeID = model.TpivBypassDobDocumentId,
                FulfillmentType = model.FulfillmentType,
                DeviceId = model.DeviceId
            };
        }
        public static NevadaSolixEligibilityRequestData ToSolixRequestData(this SubmitOrderBindingModel model)
        {
            //TEST CASES
            //ELIGIBLE
            //return new NevadaSolixEligibilityRequestData {
            //    LastName = "Doe",
            //    DateOfBirth = "01/01/1970",
            //    Last4Ssn = "4321"
            //};

            //NOT ELIGIBLE
            //return new NevadaSolixEligibilityRequestData {
            //    LastName = "Doe",
            //    DateOfBirth = "01/01/1970",
            //    Last4Ssn = "4322"
            //};

            return new NevadaSolixEligibilityRequestData {
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Last4Ssn = model.Ssn.Substring(0, 4)//in the event the ssn is full
            };
        }
        
        public static CheckStatusRequestData ToCheckStatusRequestData(this SubmitOrderBindingModel model) {
            //var isHohFlag = model.HohAdultsParent || model.HohAdultsChild || model.HohAdultsRelative ||
            //                model.HohAdultsRoommate || model.HohAdultsOther;
            //if (isHohFlag) {
            //    if (model.HohExpenses != null) {
            //        isHohFlag = (bool)model.HohExpenses;
            //    } else {
            //        isHohFlag = false;
            //    }
            //}

            var isHohFlag = true;

            return new CheckStatusRequestData {
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                LifelineProgramId = model.LifelineProgramId,
                CompanyId = model.CompanyID,
                HasServiceAddressBypass=model.HasServiceAddressBypass,
                ServiceAddressByPassDocumentProofId=model.ServiceAddressByPassDocumentProofId,
                ServiceAddressBypassSignature=model.ServiceAddressBypassSignature,
                ServiceAddress1 = model.ServiceAddressStreet1,
                ServiceAddress2 = model.ServiceAddressStreet2,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressZip5 = model.ServiceAddressZip,
                ServiceType = "Voice",
                Ssn4 = model.Ssn,
                DateOfBirth = model.DateOfBirth,
                QualifyingBeneficiaryDateOfBirth = model.QBDateOfBirth,
                QualifyingBeneficiaryFirstName = model.QBFirstName,
                QualifyingBeneficiaryLastName = model.QBLastName,
                QualifyingBeneficiaryLast4Ssn = model.QBSsn,
                IsHohFlag = isHohFlag,
                ExternalVelocityCheck=model.ExternalVelocityCheck,
                AssignedTelephoneNumber = model.AssignedPhoneNumber,
                TpivBypass = model.TpivBypass,
                TpivBypassDobDocument = model.TpivBypassDobDocumentId,
                TpivBypassDobLast4Digits = model.TpivBypassDobLast4Digits,
                TpivBypassSignature = model.TpivBypassSignature,
                TpivBypassSsnDocument = model.TpivBypassSsnDocumentId,
                TpivBypassSsnLast4Digits = model.TpivBypassSsnLast4Digits,
                PriorULTSTelephoneNumber = model.CurrentLifelinePhoneNumber
            };
        }

        public static CheckStatusRequestData ToCaliforniaPrecheckCheckStatusRequestData(this CaliforniaPrecheckBindingModel model) {
            return new CheckStatusRequestData {
                FirstName = model.Firstname,
                MiddleInitial = model.MI,
                LastName = model.Lastname,
                CompanyId = model.CompanyID,
                ServiceAddress1 = model.Address,
                ServiceAddress2 = model.Address2,
                ServiceAddressCity = model.City,
                ServiceAddressState = model.State,
                ServiceAddressZip5 = model.Zipcode,
                Ssn4 = model.SSN,
                DateOfBirth = model.Dob,
                IsHohFlag = false,
                PriorULTSTelephoneNumber = model.CurrentLifelinePhoneNUmber
            };
        }

        public static Order ToOrderUpdate(this OrderUpdateBindingModel model, string OrderId) {
            return new Order {
                Id = OrderId,
                SalesTeamId = model.SalesTeamId,
                CustomerReceivesLifelineBenefits = model.CustomerReceivesLifelineBenefits,
                HouseholdReceivesLifelineBenefits = model.HouseholdReceivesLifelineBenefits,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressIsPermanent = model.ServiceAddressIsPermanent,
                ServiceAddressIsRural = false,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressStreet1 = model.ServiceAddressStreet1,
                ServiceAddressStreet2 = model.ServiceAddressStreet2,
                ServiceAddressZip = model.ServiceAddressZip,
                BillingAddressCity = model.BillingAddressCity,
                BillingAddressState = model.BillingAddressState,
                BillingAddressStreet1 = model.BillingAddressStreet1,
                BillingAddressStreet2 = model.BillingAddressStreet2,
                BillingAddressZip = model.BillingAddressZip,
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ServiceAddressState,
                ShippingAddressStreet1 = model.ShippingAddressStreet1,
                ShippingAddressStreet2 = model.ShippingAddressStreet2,
                ShippingAddressZip = model.ShippingAddressZip,
                ContactPhoneNumber = model.ContactPhoneNumber,
                EmailAddress = model.EmailAddress,
                //                Credit Card Fields?
                CurrentLifelinePhoneNumber = model.CurrentLifelinePhoneNumber,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                UnencryptedSsn = model.Ssn,
                UnencryptedDateOfBirth = model.DateOfBirth,
                UnencryptedQBDateOfBirth = model.QBDateOfBirth,
                UnencryptedQBSsn = model.QBSsn,
                HohAdultsChild = model.HohAdultsChild,
                HohAdultsOther = model.HohAdultsOther,
                HohAdultsOtherText = model.HohAdultOtherText,
                HohAdultsParent = model.HohAdultsParent,
                HohAdultsRelative = model.HohAdultsRelative,
                HohAdultsRoommate = model.HohAdultsRoommate,
                HohAgreeMultiHouse = model.HohAgreeMultihouse,
                HohAgreeViolation = model.HohAgreeViolation,
                HohExpenses = model.HohExpenses,
                HohShareLifeline = model.HohShareLifeline,
                //                HohShareLifelineNames = model
                HohSpouse = model.HohSpouse,
                IDProofImageID = model.IPProofImageUploadId,
                IDProofImageFilename = model.IPProofImageUploadId,
                IDProofTypeID = model.IPProofTypeId,
                LPProofImageID = model.LPProofImageUploadId,
                LPProofImageFilename = model.LPProofImageUploadId,
                LPProofTypeId = model.LPProofTypeId,
                LPProofNumber = model.LPProofNumber,
                LatitudeCoordinate = model.LatitudeCoordinate,
                LongitudeCoordinate = model.LongitudeCoordinate,
                LifelineProgramId = model.LifelineProgramId,
                QBFirstName = model.QBFirstName,
                QBLastName = model.QBLastName,
                StateProgramId = model.StateProgramId,
                StateProgramNumber = model.StateProgramNumber,
                SecondaryStateProgramId = model.SecondaryStateProgramId,
                SecondaryStateProgramNumber = model.SecondaryStateProgramNumber,
                Signature = model.Signature,
                SignatureType=model.SignatureType,
                TpivBypass = model.TpivBypass,
                TpivBypassSignature = model.TpivBypassSignature,
                TPIVBypassSSNProofNumber = model.TpivBypassDobLast4Digits,
                TpivBypassDobProofTypeId = model.TpivBypassDobDocumentId,
                TpivBypassDobProofNumber = model.TpivBypassSsnLast4Digits,
                TPIVBypassSSNProofTypeID = model.TpivBypassSsnDocumentId,
                UserId = model.UserID,
                CompanyId = model.CompanyID,
                LifelineEnrollmentType = model.LifelineEnrollmentType,
                TenantAddressId = model.TenantAddressID,
                TenantReferenceId = model.TenantReferenceId.ToString(),
               
                PricePlan = model.PricePlan,
                PriceTotal = model.PriceTotal,

                FulfillmentType = model.FulfillmentType,
                DeviceModel = model.DeviceModel,
                DeviceId = model.DeviceId,

                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };
        }

      

        public static ProductCommissions ToProductCommission(this ProductCommissionsCreateBindingModel model) {
            return new ProductCommissions {
                Id = model.Id,
                ProductType = model.ProductType,
                Amount = model.Amount,
                SalesTeamID = model.SalesTeamID,
                RecipientType = model.RecipientType,
                RecipientUserId = model.RecipientUserId,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                IsDeleted = model.IsDeleted
            };
        }

        public static ProductCommissions ToProductCommission(this ProductCommissionsUpdateBindingModel model) {
            return new ProductCommissions {
                Id = model.Id,
                ProductType = model.ProductType,
                Amount = model.Amount,
                SalesTeamID = model.SalesTeamID,
                RecipientType = model.RecipientType,
                RecipientUserId = model.RecipientUserId,
                DateCreated = model.DateCreated,
                DateModified = model.DateModified,
                IsDeleted = model.IsDeleted
            };
        }
        public static SubmitApplicationRequestData ToSubmitApplicationRequestData(this SubmitOrderBindingModel model, ICheckStatusResponse checkStatusResponse,
            string companyId) {
            var isHohFlag = model.HohAdultsParent || model.HohAdultsChild || model.HohAdultsRelative ||
                            model.HohAdultsRoommate || model.HohAdultsOther;
            if (isHohFlag) {
                if (model.HohExpenses != null) {
                    isHohFlag = (bool)model.HohExpenses;
                } else {
                    isHohFlag = false;
                }
            }
            //model.AssignedPhoneNumber = checkStatusResponse.AssignedPhoneNumber;
            return new SubmitApplicationRequestData {
                EnrollmentType = checkStatusResponse.EnrollmentType,
                AssignedPhoneNumber = model.AssignedPhoneNumber,

                // current lifeline phone number of customer

                //TODO: change variable below to current lifeline phonenumber
                AssignedTelephoneNumber = model.AssignedPhoneNumber,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                Ssn4 = model.Ssn,
                DateOfBirth = model.DateOfBirth,
                ServiceAddress1 = model.ServiceAddressStreet1,
                ServiceAddress2 = model.ServiceAddressStreet2,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressZip5 = model.ServiceAddressZip,
                ShippingAddress1 = model.ShippingAddressStreet1,
                ShippingAddress2 = model.ShippingAddressStreet2,
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ShippingAddressState,
                ShippingAddressZip5 = model.ShippingAddressZip,
                BillingFirstName = model.FirstName,
                BillingMiddleInitial = model.MiddleInitial,
                BillingLastName = model.LastName,
                BillingAddress1 = model.BillingAddressStreet1,
                BillingAddress2 = model.BillingAddressStreet2,
                BillingCity = model.BillingAddressCity,
                BillingState = model.BillingAddressState,
                BillingZip5 = model.BillingAddressZip,
                ContactPhoneNumber = model.ContactPhoneNumber,
                IsHohFlag = isHohFlag,
                TpivBypass = model.TpivBypass,
                TpivBypassSignature = model.TpivBypassSignature,
                TpivBypassDobLast4Digits = model.TpivBypassDobLast4Digits,
                TpivBypassSsnLast4Digits = model.TpivBypassSsnLast4Digits,
                LifelineProgramId = model.LifelineProgramId,
                CompanyId = companyId,
                PriorULTSTelephoneNumber = model.CurrentLifelinePhoneNumber
            };
        }

        public static ValidationOrderBindingModel ToValidation(this SubmitOrderBindingModel model,ICheckStatusResponse checkStatusResponse, EnrollmentType enrollmentType,string userId, string companyId, string loggedInUserFname, string loggedInUserLname) {
            return new ValidationOrderBindingModel {
                OrderId = model.OrderId,
                SalesTeamId = model.SalesTeamId,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressIsPermanent = model.ServiceAddressIsPermanent,
                ServiceAddressIsRural = checkStatusResponse.ServiceAddressIsRural,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressStreet1 = model.ServiceAddressStreet1,
                ServiceAddressStreet2 = model.ServiceAddressStreet2,
                ServiceAddressZip = model.ServiceAddressZip,
                BillingAddressCity = model.BillingAddressCity,
                BillingAddressState = model.BillingAddressState,
                BillingAddressStreet1 = model.BillingAddressStreet1,
                BillingAddressStreet2 = model.BillingAddressStreet2,
                BillingAddressZip = model.BillingAddressZip,
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ServiceAddressState,
                ShippingAddressStreet1 = model.ShippingAddressStreet1,
                ShippingAddressStreet2 = model.ShippingAddressStreet2,
                ShippingAddressZip = model.ShippingAddressZip,
                ContactPhoneNumber = model.ContactPhoneNumber,
                EmailAddress = model.EmailAddress,
                //                Credit Card Fields?
                CurrentLifelinePhoneNumber = model.CurrentLifelinePhoneNumber,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                Gender=model.Gender,
                Ssn = model.Ssn,
                DateOfBirth = model.DateOfBirth,
                QBDateOfBirth = model.QBDateOfBirth,
                QBSsn = model.QBSsn,
                IsHoh = model.IsHoh,
                HohAdultsChild = model.HohAdultsChild,
                HohAdultsOther = model.HohAdultsOther,
                HohAdultOtherText = model.HohAdultsOtherText,
                HohAdultsParent = model.HohAdultsParent,
                HohAdultsRelative = model.HohAdultsRelative,
                HohAdultsRoommate = model.HohAdultsRoommate,
                HohAgreeMultihouse = model.HohAgreeMultihouse,
                HohAgreeViolation = model.HohAgreeViolation,
                HohExpenses = model.HohExpenses,
                HohShareLifeline = model.HohShareLifeline,
                //                HohShareLifelineNames = model
                HohSpouse = model.HohSpouse,
                IPProofImageUploadId = model.IPProofImageUploadId,
                IPProofTypeId = model.IPProofTypeId,
                LPProofImageUploadId = model.LPProofImageUploadId,
                LPProofTypeId = model.LPProofTypeId,
                LPProofNumber = model.LPProofNumber,
                LatitudeCoordinate = model.LatitudeCoordinate,
                LongitudeCoordinate = model.LongitudeCoordinate,
                LifelineProgramId = model.LifelineProgramId,
                QBFirstName = model.QBFirstName,
                QBLastName = model.QBLastName,
                StateProgramId = model.StateProgramId,
                StateProgramNumber = model.StateProgramNumber,
                SecondaryStateProgramId = model.SecondaryStateProgramId,
                SecondaryStateProgramNumber = model.SecondaryStateProgramNumber,
                Signature = model.Signature,
                SignatureType=model.SignatureType,
                TpivBypass = model.TpivBypass,
                TpivBypassDobLast4Digits = model.TpivBypassDobLast4Digits,
                TpivBypassDobDocumentId = model.TpivBypassDobDocumentId,
                TpivBypassSsnLast4Digits = model.TpivBypassSsnLast4Digits,
                TpivBypassSsnDocumentId = model.TpivBypassSsnDocumentId,
                UserID = userId,
                CompanyID = companyId,
                LifelineEnrollmentType = enrollmentType.ToString(),
                TenantAddressID = model.TenantAddressID,
                LoggedInUserFname = loggedInUserFname,
                LoggedInUserLname = loggedInUserLname
            };
        }

        public static Order ToFinalOrder(this SubmitOrderBindingModel model, ICheckStatusResponse checkStatusResponse, ApplicationUser user, string companyId) {
            return new Order {
              
                ParentOrderId = model.ParentOrderId,
                TransactionId = model.TransactionId,
                SalesTeamId = model.SalesTeamId,
                CustomerReceivesLifelineBenefits = model.CustomerReceivesLifelineBenefits,
                HouseholdReceivesLifelineBenefits = model.HouseholdReceivesLifelineBenefits,
                ServiceAddressCity = model.ServiceAddressCity,
                ServiceAddressIsPermanent = model.ServiceAddressIsPermanent,
                ServiceAddressIsRural = checkStatusResponse.ServiceAddressIsRural,
                ServiceAddressState = model.ServiceAddressState,
                ServiceAddressStreet1 = model.ServiceAddressStreet1,
                ServiceAddressStreet2 = model.ServiceAddressStreet2,
                ServiceAddressZip = model.ServiceAddressZip,
                BillingAddressCity = "",
                BillingAddressState = "",
                BillingAddressStreet1 = "",
                BillingAddressStreet2 = "",
                BillingAddressZip = "",
                ShippingAddressCity = model.ShippingAddressCity,
                ShippingAddressState = model.ServiceAddressState,
                ShippingAddressStreet1 = model.ShippingAddressStreet1,
                ShippingAddressStreet2 = model.ShippingAddressStreet2,
                ShippingAddressZip = model.ShippingAddressZip,
                ContactPhoneNumber = model.ContactPhoneNumber,
                EmailAddress = model.EmailAddress,
                //                Credit Card Fields?
                CurrentLifelinePhoneNumber = model.CurrentLifelinePhoneNumber,
                Language=model.Language,
                CommunicationPreference=model.CommunicationPreference,
                FirstName = model.FirstName,
                MiddleInitial = model.MiddleInitial,
                LastName = model.LastName,
                Gender = model.Gender,
                UnencryptedSsn = model.Ssn,
                UnencryptedDateOfBirth = model.DateOfBirth,
                UnencryptedQBDateOfBirth = model.QBDateOfBirth,
                UnencryptedQBSsn = model.QBSsn,
                HohAdultsChild = model.HohAdultsChild,
                HohAdultsOther = model.HohAdultsOther,
                HohAdultsOtherText = model.HohAdultsOtherText,
                HohAdultsParent = model.HohAdultsParent,
                HohAdultsRelative = model.HohAdultsRelative,
                HohAdultsRoommate = model.HohAdultsRoommate,
                HohAgreeMultiHouse = model.HohAgreeMultihouse,
                HohAgreeViolation = model.HohAgreeViolation,
                HohExpenses = model.HohExpenses,
                HohShareLifeline = model.HohShareLifeline,
                HohPuertoRicoAgreeViolation = model.HohPuertoRicoAgreeViolation,
                HohSpouse = model.HohSpouse,
                IDProofImageID = model.IPProofImageUploadId,
                IDProofImageFilename = model.IPProofImageUploadFilename,
                IDProofTypeID = model.IPProofTypeId,
                IDProofImageFilename2 = model.IPProofImageUploadFilename2,
                IDProofImageID2 = model.IPProofImageUploadId2,
                LPProofImageID = model.LPProofImageUploadId,
                LPProofImageFilename = model.LPProofImageUploadFilename,
                LPProofTypeId = model.LPProofTypeId,
                LPProofNumber = model.LPProofNumber,
                LatitudeCoordinate = model.LatitudeCoordinate,
                LongitudeCoordinate = model.LongitudeCoordinate,
                LifelineProgramId = model.LifelineProgramId,
                QBFirstName = model.QBFirstName,
                QBLastName = model.QBLastName,
                StateProgramId = model.StateProgramId,
                StateProgramNumber = model.StateProgramNumber,
                SecondaryStateProgramId = model.SecondaryStateProgramId,
                SecondaryStateProgramNumber = model.SecondaryStateProgramNumber,
                Signature = model.Signature,
                SignatureType = model.SignatureType,
                SigFileName = model.SignatureImageFilename,
                Initials=model.Initials,
                InitialsFileName= model.InitialsFileName,
                ServiceAddressBypass = model.HasServiceAddressBypass ,
                ServiceAddressBypassProofTypeID=model.ServiceAddressByPassDocumentProofId,
                ServiceAddressBypassProofImageID = model.ServiceAddressByPassImageID,
                ServiceAddressBypassProofImageFilename = model.ServiceAddressByPassImageFileName,
                ServiceAddressBypassSignature=model.ServiceAddressBypassSignature,
                TpivBypass = model.TpivBypass,
                TpivBypassSignature = model.TpivBypassSignature,
                TPIVBypassSSNProofImageFilename = model.TpivSsnImageUploadFilename,
                TPIVBypassSSNProofImageID = model.TpivSsnImageUploadId,
                TpivBypassDobProofNumber = model.TpivBypassDobLast4Digits,
                TpivBypassDobProofTypeId = model.TpivBypassDobDocumentId,
                TPIVBypassSSNProofNumber = model.TpivBypassSsnLast4Digits,
                TPIVBypassSSNProofTypeID = model.TpivBypassSsnDocumentId,
                UserId = user.Id,
                CompanyId = companyId,
                LifelineEnrollmentType = checkStatusResponse.EnrollmentType.ToString(),
                TenantAddressId = model.TenantAddressID,
                TenantReferenceId = model.TenantReferenceId.ToString(),
                AINumHouseAdult=model.LifelineProgramAnnualIncome.NumHouseAdult,
                AINumHouseChildren=model.LifelineProgramAnnualIncome.NumHouseChildren,
                AIAvgIncome = model.LifelineProgramAnnualIncome.AverageIncomeAmount,
                AIFrequency = model.LifelineProgramAnnualIncome.AverageIncomeDuration,
                AIInitials = model.LifelineProgramAnnualIncome.AnnualIncomeCustomerInitials,
                AINumHousehold = (model.LifelineProgramAnnualIncome.NumHouseAdult + model.LifelineProgramAnnualIncome.NumHouseChildren),
                ExternalVelocityCheck=model.ExternalVelocityCheck,
                PricePlan = model.PricePlan,
                PriceTotal = model.PriceTotal,
                FulfillmentType = model.FulfillmentType,
                TpivCode=model.TpivCode,
                TpivRiskIndicators=model.TpivRiskIndicators,
                TpivNasScore=model.TpivNasScore,
                TpivTransactionID=model.TpivTransactionID,
                DeviceId = model.DeviceId,
                DeviceCompatibility=model.DeviceCompatibility,
                ByopCarrier =model.ByopCarrier,
                DeviceModel=model.DeviceModel,
                DateCreated = DateTime.UtcNow,
                DateModified = DateTime.UtcNow
            };
        }

        public static AccountBalanceBindingModel ToAccountBalanceBindingModel(ICheckVerizonBalanceResult balanceResult) {
            return new AccountBalanceBindingModel {
                IsError = false,
                PlanVoice = balanceResult.PlanBalance_Voice.ToString(),
                PlanText = balanceResult.PlanBalance_Text.ToString(),
                PlanMMS = balanceResult.PlanBalance_MMS.ToString(),
                PlanCombo = balanceResult.PlanBalance_Combo_VS.ToString(),
                PlanData = balanceResult.PlanBalance_Data.ToString(),
                TopUpVoice = balanceResult.TopUpBalance_Voice.ToString(),
                TopUpText = balanceResult.TopUpBalance_Text.ToString(),
                TopUpCombo = balanceResult.TopUpBalance_Combo_VS.ToString(),
                TopUpData = balanceResult.TopUpBalance_Data,
                TopUpMMS = balanceResult.TopUpBalance_MMS.ToString(),
                TopUpExpire = balanceResult.TopUpExpiration,
                TopUpExpirationSet = balanceResult.TopUpExpirationSet,
                ServiceEndDate = balanceResult.ServiceEndDate
            };
        }

        public static AccountBalanceBindingModel ToAccountBalanceBindingModel(ICheckTmobileBalanceResult balanceResult) {
            return new AccountBalanceBindingModel {
                IsError = false,
                PlanVoice = balanceResult.planBalance_Voice.ToString(),
                PlanText = balanceResult.planBalance_Text.ToString(),
                PlanMMS = "",
                PlanCombo = "",
                PlanData = "",
                TopUpVoice = balanceResult.topUpBalance_Voice.ToString(),
                TopUpText = balanceResult.topUpBalance_Text.ToString(),
                TopUpCombo = "",
                TopUpData = balanceResult.topUpBalance_Data,
                TopUpMMS = "",
                TopUpExpire = balanceResult.TopUpExpiration,
                TopUpExpirationSet = balanceResult.TopUpExpirationSet,
                ServiceEndDate = balanceResult.ServiceEndDate
            };
        }

        public static AccountBalanceBindingModel ToAccountBalanceBindingModel(IRetrVoiceandTextBalanceResult balanceResult) {
            return new AccountBalanceBindingModel {
                IsError = false,
                PlanVoice = balanceResult.planBalance_Voice.ToString(),
                PlanText = balanceResult.planBalance_Text.ToString(),
                PlanMMS = "",
                PlanCombo = balanceResult.planBalance_Combo_VS.ToString(),
                PlanData = balanceResult.planBalance_Data.ToString(),
                TopUpVoice = balanceResult.topUpBalance_Voice.ToString(),
                TopUpText = balanceResult.topUpBalance_Text.ToString(),
                TopUpCombo = balanceResult.TopUpBalance_Combo_VS.ToString(),
                TopUpData = balanceResult.topUpBalance_Data,
                TopUpMMS = "",
                TopUpExpire = balanceResult.TopUpExpiration,
                TopUpExpirationSet = balanceResult.TopUpExpirationSet,
                ServiceEndDate = balanceResult.ServiceEndDate
            };
        }

        public static CGMCheckRequest ToCGMCheckRequestModel(CaliforniaPrecheckBindingModel model)
        {
            return new CGMCheckRequest
            {
                Token ="",
                SubscriberFirstName=model.Firstname,
                SubscriberLastName=model.Lastname,
                SubscriberMiddleNameInital=model.MI,
                SubscriberSecondLastName="",
                SubscriberLast4ssn=model.SSN,
                SubscriberPrimaryAddress1=model.Address,
                SubscriberPrimaryAddress2=model.Address2,
                SubscriberPrimaryCity=model.City,
                SubscriberPrimaryState=model.State,
                SubscriberPrimaryZipCode=model.Zipcode,
                SubscriberDob=model.Dob,
                AgentFirstName="",
                AgentLastName="",
                AgentDob="",
                AgentLast4ssn="",
                AgentDeviceId=""
            };
        }

        public static CGMCheckRequest ToCGMCheckRequestModel(SubmitOrderBindingModel model)
        {
            return new CGMCheckRequest
            {
                Token = "",
                SubscriberFirstName = model.FirstName,
                SubscriberLastName = model.LastName,
                SubscriberMiddleNameInital = model.MiddleInitial,
                SubscriberSecondLastName = "",
                SubscriberLast4ssn = model.Ssn,
                SubscriberPrimaryAddress1 = model.ServiceAddressStreet1,
                SubscriberPrimaryAddress2 = model.ServiceAddressStreet2,
                SubscriberPrimaryCity = model.ServiceAddressCity,
                SubscriberPrimaryState = model.ServiceAddressState,
                SubscriberPrimaryZipCode = model.ServiceAddressZip,
                SubscriberDob = model.DateOfBirth,
                AgentFirstName = model.AgentFirstName,
                AgentLastName =model.AgentLastName,
                AgentDob = "",
                AgentLast4ssn = "",
                AgentDeviceId = ""
            };
        }

        public static CGMEnrollRequest ToCGMEnrollModel(CGMCheckRequest model){
            return new CGMEnrollRequest
            {
                Token = "",
                SubscriberEnrollmentDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                SubscriberFirstName = model.SubscriberFirstName,
                SubscriberLastName = model.SubscriberLastName,
                SubscriberMiddleNameInital = model.SubscriberMiddleNameInital,
                SubscriberSecondLastName = "",
                SubscriberLast4ssn = model.SubscriberLast4ssn,
                SubscriberPrimaryAddress1 = model.SubscriberPrimaryAddress1,
                SubscriberPrimaryAddress2 = model.SubscriberPrimaryAddress2,
                SubscriberPrimaryCity = model.SubscriberPrimaryCity,
                SubscriberPrimaryState = model.SubscriberPrimaryState,
                SubscriberPrimaryZipCode = model.SubscriberPrimaryZipCode,
                SubscriberDob = model.SubscriberDob,
                AgentFirstName = model.AgentFirstName,
                AgentLastName = model.AgentLastName,
                AgentDob = "",
                AgentLast4ssn = "",
                AgentDeviceId = "",
                EnrollmentType="",
                TransactionId=""
            };
        }

        public static SolixAPICreateCustomerRequest ToSolixCreateCustomerModel(this Order model, string ExternalUserID, string TranslatedLifelineProgram, string EligibilityType, string DocumentImage1, string DocumentFileName1, string DocumentFileDescription1, string DocumentImage2, string DocumentFileName2, string DocumentFileDescription2, string DocumentImage3, string DocumentFileName3, string DocumentFileDescription3,string DocumentImage4,string DocumentFileName4,string DocumentFileDescription4,string DocumentImage5,string DocumentFileName5,string DocumentFileDescription5,string SignatureImage, string CustomerInitialsImage,string IsByop,string IsRequalification,string RequalificationAppId,string RequalificationMDN,string IsFreePhoneEligible) {
            var FieldActivation = "N";
            //if (model.FulfillmentType == "store") { FieldActivation = "Y"; }
            if (model.FulfillmentType == "store" || model.DeviceId == "CELL") { FieldActivation = "Y"; } //Only allow field activation when it is a free phone, chosen for "store" fulfillment.  For now...

            var IsTempAddress = "N";
            if (model.ServiceAddressIsPermanent) { IsTempAddress = "Y"; }

            var Language = "1";

            var ProgramList = TranslatedLifelineProgram;
            var AlreadyEnrolled = "";
            if (model.CustomerReceivesLifelineBenefits) {
                AlreadyEnrolled = "Y";
            } else {
                AlreadyEnrolled = "N";
            }

            var Eligibility = EligibilityType;
            var HouseholdSize = "";
            var NumHouseholdAdult = "";
            var NumHouseholdChildren = "";
            var AnnualHouseholdIncome = "";

            if (Eligibility == "I") {
                HouseholdSize = model.AINumHousehold.ToString();
                NumHouseholdAdult = model.AINumHouseAdult.ToString();
                NumHouseholdChildren = model.AINumHouseChildren.ToString();
                AnnualHouseholdIncome = model.AIAvgIncome.ToString();
                ProgramList = "";
            }

            var WorksheetCA_Q1 = "N";
            if (model.HohSpouse) {WorksheetCA_Q1 = "Y";}

            var WorksheetCA_Q2 = "N";
            if (model.HohAdultsRelative == true || model.HohAdultsChild == true || model.HohAdultsRelative == true || model.HohAdultsRoommate == true || model.HohAdultsOther == true || model.HohShareLifeline == true) { WorksheetCA_Q2 = "Y"; }

            var WorksheetCA_Q2a = "N";
            if (model.HohAdultsRelative) {WorksheetCA_Q2a = "Y";}

            var WorksheetCA_Q2b = "N";
            if (model.HohAdultsChild) {WorksheetCA_Q2b = "Y";}

            var WorksheetCA_Q2c = "N";
            if (model.HohAdultsRelative) {WorksheetCA_Q2c = "Y";}

            var WorksheetCA_Q2d = "N";
            if (model.HohAdultsRoommate) {WorksheetCA_Q2d = "Y";}

            var WorksheetCA_Q2e = "N";
            if (model.HohAdultsOther) { WorksheetCA_Q2e = "Y"; }

            var WorksheetCA_Q2other = "";
            if (model.HohAdultsOther) {WorksheetCA_Q2other = model.HohAdultsOtherText;}

            var WorksheetCA_Q3 = "N";
            if (model.HohShareLifeline == true) {WorksheetCA_Q3 = "Y";}

            var WorksheetCA_CertificationF = "N";
            if (model.HohAgreeViolation == true) {WorksheetCA_CertificationF = "Y";}

            var WorksheetCA_CertificationG = "N";
            if (model.HohAgreeMultiHouse == true) {WorksheetCA_CertificationG = "Y";}


            return new SolixAPICreateCustomerRequest {
                CreateUserID = ExternalUserID,
                FieldActivation = FieldActivation,
                FirstName = model.FirstName,
                Mi = model.MiddleInitial,
                LastName = model.LastName,
                ResidentialAddress1 = model.ServiceAddressStreet1,
                ResidentialAddress2 = model.ServiceAddressStreet2,
                ResidentialCity = model.ServiceAddressCity,
                ResidentialState = model.ServiceAddressState,
                ResidentialZip = model.ServiceAddressZip,
                IsTempAddress = IsTempAddress,
                ShippingAddress1 = model.ShippingAddressStreet1,
                ShippingAddress2 = model.ShippingAddressStreet2,
                ShippingCity = model.ShippingAddressCity,
                ShippingState = model.ShippingAddressState,
                ShippingZip = model.ShippingAddressZip,
                Email = model.EmailAddress,
                ContactPhone = model.ContactPhoneNumber,
                DOB = model.UnencryptedDateOfBirth,
                SSNLast4 = model.UnencryptedSsn,
                DocumentImage1 = DocumentImage1,
                DocumentImage2 = DocumentImage2,
                DocumentImage3 = DocumentImage3,
                DocumentImage4 = DocumentImage4,
                DocumentImage5 = DocumentImage5,
                DocumentFileName1 = DocumentFileName1,
                DocumentFileName2 = DocumentFileName2,
                DocumentFileName3 = DocumentFileName3,
                DocumentFileName4 = DocumentFileName4,
                DocumentFileName5 = DocumentFileName5,
                DocumentFileDescription1 = DocumentFileDescription1,
                DocumentFileDescription2 = DocumentFileDescription2,
                DocumentFileDescription3 = DocumentFileDescription3,
                DocumentFileDescription4 = DocumentFileDescription4,
                DocumentFileDescription5 = DocumentFileDescription5,
                LxnxID = model.TpivCode,
                LxnxNasScore = model.TpivNasScore,
                LxnxRiskIndicators = model.TpivRiskIndicators,
                LxnxTransactionID = model.TpivTransactionID,
                WorksheetCA_Q1 = WorksheetCA_Q1,
                WorksheetCA_Q2 = WorksheetCA_Q2,
                WorksheetCA_Q2a = WorksheetCA_Q2a,
                WorksheetCA_Q2b = WorksheetCA_Q2b,
                WorksheetCA_Q2c = WorksheetCA_Q2c,
                WorksheetCA_Q2d = WorksheetCA_Q2d,
                WorksheetCA_Q2e = WorksheetCA_Q2e,
                WorksheetCA_Q2other = WorksheetCA_Q2other,
                WorksheetCA_Q3 = WorksheetCA_Q3,
                WorksheetCA_CertificationF = WorksheetCA_CertificationF,
                WorksheetCA_CertificationG = WorksheetCA_CertificationG,
                Signature = SignatureImage,
                CustomerInitials = CustomerInitialsImage,
                PrintType = model.CommunicationPreference,
                SignedLegalGuardian = "N",
                Language = Language,
                Eligibility = Eligibility,
                ProgramList = ProgramList,
                HouseholdSize = HouseholdSize,
                AnnualHouseholdIncome = AnnualHouseholdIncome,
                HouseholdNumberAdult = NumHouseholdAdult,
                HouseholdNumberKids = NumHouseholdChildren,
                AlreadyEnrolled = AlreadyEnrolled,
                PrivacyLaw = "N",
                Longitude = model.LongitudeCoordinate.ToString(),
                Latitude = model.LatitudeCoordinate.ToString(),
                IsBYOP = IsByop,
                IsRequalification = IsRequalification,
                RequalificationMDN = RequalificationMDN,
                RequalificationAppId = RequalificationAppId,
                IsFreePhoneEligible = IsFreePhoneEligible,
                Device_type=model.DeviceId,
                Byop_Carrier = model.ByopCarrier == null ? "": model.ByopCarrier
                };
        }
    }
}
