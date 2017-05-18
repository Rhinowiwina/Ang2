namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial_create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AddressProofDocumentTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.String(maxLength: 5),
                        Name = c.String(maxLength: 125),
                        LifelineSystem = c.String(maxLength: 10),
                        LifelineSystemId = c.String(maxLength: 10),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApiLogEntries",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Api = c.String(maxLength: 100, unicode: false),
                        Function = c.String(maxLength: 250, unicode: false),
                        Input = c.String(unicode: false, storeType: "text"),
                        Response = c.String(unicode: false, storeType: "text"),
                        JsonImported = c.Boolean(nullable: false,defaultValue:false),
                        DateStarted = c.DateTime(nullable: false),
                        DateEnded = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CaliPhoneNumbers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        AreaCode = c.Int(nullable: false),
                        Number = c.Int(nullable: false),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100),
                        CompanyLogoUrl = c.String(),
                        PrimaryColorHex = c.String(),
                        SecondaryColorHex = c.String(),
                        CompanySupportUrl = c.String(),
                        MinToChangeTeam = c.Int(nullable: false),
                        MaxCommission = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Notes = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 75),
                        LastName = c.String(maxLength: 75),
                        PayPalEmail = c.String(maxLength: 100),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        SalesTeamId = c.String(maxLength: 128),
                        Language = c.String(nullable: false, maxLength: 4),
                        ExternalUserID = c.String(maxLength:100),
                        PermissionsLifelineCA = c.Boolean(nullable: false),
                        PermissionsBypassTpiv = c.Boolean(nullable: false),
                        PermissionsAccountOrder = c.Boolean(nullable: false,defaultValue:true),
                        RequiresTraining = c.Boolean(nullable:true),
                        TrainingExpirationDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false,defaultValue: true),
                        IsDeleted = c.Boolean(nullable: false,defaultValue: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(maxLength: 20),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.SalesTeams", t => t.SalesTeamId)
                .Index(t => t.CompanyId)
                .Index(t => t.SalesTeamId)
                .Index(t => t.CreatedByUserId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Rank = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.SalesTeams",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 250),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        ExternalPrimaryId = c.String(maxLength: 50),
                        ExternalDisplayName = c.String(maxLength: 50),
                        Address1 = c.String(maxLength: 100),
                        Address2 = c.String(maxLength: 100),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        Zip = c.String(maxLength: 10),
                        Phone = c.String(maxLength: 100),
                        TaxId = c.String(),
                        PayPalEmail = c.String(),
                        CycleCountTypeDevice = c.String(maxLength: 10),
                        CycleCountTypeSim = c.String(maxLength: 10),
                        Level3SalesGroupId = c.String(maxLength: 128),
                        SigType = c.String(),
                        CreatedByUserId = c.String(maxLength: 128),
                        ModifiedByUserId = c.String(maxLength:128),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.Level3SalesGroup", t => t.Level3SalesGroupId)
                .Index(t => t.CompanyId)
                .Index(t => t.Level3SalesGroupId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.Level3SalesGroup",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        ParentSalesGroupId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedByUserId = c.String(nullable: false, maxLength: 128),
                        ModifiedByUserId = c.String(maxLength:128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.Level2SalesGroup", t => t.ParentSalesGroupId)
                .Index(t => t.CompanyId)
                .Index(t => t.ParentSalesGroupId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.Level2SalesGroup",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        ParentSalesGroupId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedByUserId = c.String(nullable: false, maxLength: 128),
                        ModifiedByUserId = c.String(maxLength:128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.Level1SalesGroup", t => t.ParentSalesGroupId)
                .Index(t => t.CompanyId)
                .Index(t => t.ParentSalesGroupId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.Level1SalesGroup",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        IsDeleted = c.Boolean(nullable: false),
                        CreatedByUserId = c.String(nullable: false, maxLength: 128),
                        ModifiedByUserId = c.String(maxLength:128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .Index(t => t.CompanyId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.SacEntries",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(),
                        SacNumber = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Company_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.Company_Id)
                .Index(t => t.Company_Id);
            
            CreateTable(
                "dbo.CommissionLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrderId = c.String(nullable: false, maxLength: 128),
                        RecipientUserId = c.String(maxLength: 128),
                        SalesTeamId = c.String(maxLength: 128),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        PaymentID = c.String(maxLength: 128),
                        ProcessID = c.String(),
                        OrderType = c.String(nullable: false),
                        RecipientType = c.String(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Payments", t => t.PaymentID)
                .ForeignKey("dbo.SalesTeams", t => t.SalesTeamId)
                .ForeignKey("dbo.AspNetUsers", t => t.RecipientUserId)
                .Index(t => t.RecipientUserId)
                .Index(t => t.SalesTeamId)
                .Index(t => t.PaymentID);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Email = c.String(maxLength:100),
                        TransactionID = c.String(),
                        DatePaid = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CompanyTranslations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyID = c.String(maxLength:128),
                        LSID = c.String(maxLength:128),
                        TranslatedID = c.Int(nullable: false),
                        Type = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable:true),
                        DateModified = c.DateTime(nullable: true),
                        IsDeleted = c.Boolean(nullable:true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Competitors",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ComplianceStatements",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2),
                        Statement = c.String(),
                        Statement_ES = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.DevDatas",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DateSubmitted = c.DateTime(nullable: false),
                        OrigState = c.String(maxLength: 100),
                        E_Prg = c.String(maxLength: 100),
                        Num_HouseHold = c.String(maxLength: 4),
                        AuthCode = c.String(maxLength: 100),
                        PreQual = c.String(maxLength: 100),
                        Fname = c.String(maxLength: 50),
                        MI = c.String(maxLength: 50),
                        Lname = c.String(maxLength: 50),
                        SSN_Decrypted = c.String(maxLength: 50),
                        DOB_Decrypted = c.String(maxLength: 50),
                        Address = c.String(maxLength: 100),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 2),
                        Zip = c.String(),
                        BillAsInstall = c.Boolean(nullable: false),
                        Bill_Address = c.String(maxLength: 100),
                        Bill_City = c.String(maxLength: 50),
                        Bill_State = c.String(maxLength: 2),
                        Bill_Zip = c.String(),
                        DayPhone = c.String(maxLength: 20),
                        Avg_Income = c.String(maxLength: 100),
                        Income_Frequency = c.String(maxLength: 50),
                        Qualifying_Beneficiary = c.String(maxLength: 2),
                        Beneficiary_FirstName = c.String(maxLength: 30),
                        Beneficiary_LastName = c.String(maxLength: 30),
                        Beneficiary_SSN_Decrypted = c.String(),
                        Beneficiary_DOB_Decrypted = c.String(),
                        HOH_Spouse = c.String(maxLength: 10),
                        HOH_Adults_Parent = c.String(maxLength: 10),
                        HOH_Adults_Child = c.String(maxLength: 10),
                        HOH_Adults_Relative = c.String(maxLength: 10),
                        HOH_Adults_Roommate = c.String(maxLength: 10),
                        HOH_Adults_Other = c.String(maxLength: 10),
                        HOH_Adults_Other_Text = c.String(maxLength: 200),
                        HOH_Expenses = c.String(maxLength: 100),
                        HOH_Share_Lifeline = c.String(maxLength: 50),
                        HOH_Share_Lifeline_Names = c.String(maxLength: 50),
                        HOH_Agree_MultHoues = c.String(maxLength: 50),
                        HOH_Agree_Violation = c.String(maxLength: 50),
                        DocumentType = c.String(maxLength: 100),
                        DocumentNumber = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResponseLogsCGMEHDBs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TransactionId = c.String(),
                        APILogEntriesID = c.String(),
                        LogDate = c.DateTime(),
                        Blacklist = c.Boolean(nullable: false),
                        Status = c.String(),
                        Message = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResponseLogsCGMEHDBDetails",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ResponseLogsCGMEHDBId = c.String(maxLength: 128),
                        Type = c.String(),
                        PeriodDays = c.Int(nullable: false),
                        Matches = c.Int(),
                        IsMatched = c.Boolean(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ResponseLogsCGMEHDBs", t => t.ResponseLogsCGMEHDBId)
                .Index(t => t.ResponseLogsCGMEHDBId);
            
            CreateTable(
                "dbo.ExternalStorageCredentials",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        AccessKey = c.String(nullable: false),
                        SecretKey = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        System = c.String(nullable: false),
                        Path = c.String(),
                        MaxImageSize = c.Int(nullable: false),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.LifelinePrograms",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProgramName = c.String(maxLength: 150),
                        StateCode = c.String(maxLength: 2),
                        RequiresAccountNumber = c.Boolean(nullable: false),
                        RequiredStateProgramId = c.String(maxLength: 128),
                        RequiredSecondaryStateProgramId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        NladEligibilityCode = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StatePrograms", t => t.RequiredSecondaryStateProgramId)
                .ForeignKey("dbo.StatePrograms", t => t.RequiredStateProgramId)
                .Index(t => t.RequiredStateProgramId)
                .Index(t => t.RequiredSecondaryStateProgramId);
            
            CreateTable(
                "dbo.StatePrograms",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100),
                        StateCode = c.String(maxLength: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LoginMsgs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Title = c.String(),
                        Msg = c.String(),
                        BeginDate = c.DateTime(nullable: false),
                        ExpirationDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        MsgLevel = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.NladPhoneNumbers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Number = c.Long(nullable: false),
                        CompanyId = c.String(maxLength: 128),
                        IsCurrentlyInUse = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.String(maxLength: 128),
                        ParentOrderId = c.String(maxLength: 128),
                        SalesTeamId = c.String(maxLength: 128),
                        UserId = c.String(maxLength: 128),
                        HouseholdReceivesLifelineBenefits = c.Boolean(nullable: false),
                        CustomerReceivesLifelineBenefits = c.Boolean(nullable: false),
                        QBFirstName = c.String(maxLength: 50),
                        QBLastName = c.String(maxLength: 50),
                        QBSsn = c.Binary(),
                        QBDateOfBirth = c.Binary(),
                        CurrentLifelinePhoneNumber = c.String(maxLength: 20),
                        LifelineProgramId = c.String(maxLength: 128),
                        LPProofTypeId = c.String(maxLength: 128),
                        LPProofNumber = c.String(maxLength: 50),
                        LPImageFileName = c.String(maxLength: 100),
                        StateProgramId = c.String(maxLength: 128),
                        StateProgramNumber = c.String(maxLength: 50),
                        SecondaryStateProgramId = c.String(maxLength: 128),
                        SecondaryStateProgramNumber = c.String(maxLength: 50),
                        Language = c.String(),
                        CommunicationPreference = c.String(maxLength:50),
                        FirstName = c.String(maxLength: 50),
                        MiddleInitial = c.String(maxLength: 1),
                        LastName = c.String(maxLength: 50),
                        Gender = c.String(maxLength: 20),
                        Ssn = c.Binary(),
                        DateOfBirth = c.Binary(),
                        EmailAddress = c.String(maxLength: 50),
                        ContactPhoneNumber = c.String(maxLength: 20),
                        IPProofTypeId = c.String(maxLength: 128),
                        IPImageFileName = c.String(maxLength: 100),
                        ServiceAddressBypass = c.Boolean(nullable: false),
                        ServiceAddressBypassSignature = c.String(),
                        ServiceAddressByPassDocumentProofId = c.String(),
                        ServiceAddressByPassImageFileName = c.String(),
                        ServiceAddressStreet1 = c.String(maxLength: 100),
                        ServiceAddressStreet2 = c.String(maxLength: 100),
                        ServiceAddressCity = c.String(maxLength: 50),
                        ServiceAddressState = c.String(maxLength: 2),
                        ServiceAddressZip = c.String(maxLength: 10),
                        ServiceAddressIsPermanent = c.Boolean(nullable: false),
                        ServiceAddressIsRural = c.Boolean(nullable: false),
                        BillingAddressStreet1 = c.String(maxLength: 100),
                        BillingAddressStreet2 = c.String(maxLength: 100),
                        BillingAddressCity = c.String(maxLength: 50),
                        BillingAddressState = c.String(maxLength: 2),
                        BillingAddressZip = c.String(maxLength: 10),
                        ShippingAddressStreet1 = c.String(maxLength: 100),
                        ShippingAddressStreet2 = c.String(maxLength: 100),
                        ShippingAddressCity = c.String(maxLength: 50),
                        ShippingAddressState = c.String(maxLength: 2),
                        ShippingAddressZip = c.String(maxLength: 10),
                        HohSpouse = c.Boolean(nullable: false),
                        HohAdultsParent = c.Boolean(nullable: false),
                        HohAdultsChild = c.Boolean(nullable: false),
                        HohAdultsRelative = c.Boolean(nullable: false),
                        HohAdultsRoommate = c.Boolean(nullable: false),
                        HohAdultsOther = c.Boolean(nullable: false),
                        HohAdultsOtherText = c.String(maxLength: 50),
                        HohExpenses = c.Boolean(),
                        HohShareLifeline = c.Boolean(),
                        HohShareLifelineNames = c.String(maxLength: 500),
                        HohAgreeMultiHouse = c.Boolean(),
                        HohAgreeViolation = c.Boolean(nullable: false),
                        HohPuertoRicoAgreeViolation = c.Boolean(),
                        Signature = c.String(),
                        Initials = c.String(),
                        InitialsFileName = c.String(),
                        SignatureType = c.String(maxLength: 20),
                        SigFileName = c.String(maxLength: 500),
                        HasDevice = c.Boolean(nullable: false),
                        CarrierId = c.String(maxLength: 128),
                        DeviceId = c.String(maxLength: 50),
                        DeviceIdentifier = c.String(maxLength: 50),
                        SimIdentifier = c.String(maxLength: 50),
                        PlanId = c.String(maxLength: 128),
                        FullFillmentDate = c.DateTime(),
                        TpivBypass = c.Boolean(nullable: false),
                        TpivBypassSignature = c.String(maxLength: 100),
                        TpivBypassSsnProofTypeId = c.String(maxLength: 128),
                        TpivBypassSsnImageFileName = c.String(),
                        TpivBypassSsnCardLastFour = c.String(maxLength: 4),
                        TpivBypassDobProofTypeId = c.String(maxLength: 128),
                        TpivBypassDobCardLastFour = c.String(maxLength: 4),
                        TpivCode = c.String(maxLength: 50),
                        TpivBypassMessage = c.String(),
                        LatitudeCoordinate = c.Single(nullable: false),
                        LongitudeCoordinate = c.Single(nullable: false),
                        PaymentType = c.String(maxLength: 10),
                        CreditCardReference = c.String(maxLength: 100),
                        CreditCardSuccess = c.Boolean(nullable: false),
                        CreditCardTransactionId = c.String(maxLength: 100),
                        LifelineEnrollmentId = c.String(maxLength: 50),
                        LifelineEnrollmentType = c.String(maxLength: 50),
                        AIInitials = c.String(maxLength: 5),
                        AIFrequency = c.String(maxLength: 20),
                        AIAvgIncome = c.Int(nullable: false),
                        AINumHousehold = c.Int(nullable: false),
                        TenantReferenceId = c.String(),
                        TenantAccountId = c.String(),
                        TenantAddressId = c.Int(nullable: false),
                        PricePlan = c.Double(nullable: false),
                        PriceTotal = c.Double(nullable: false),
                        FulfillmentType = c.String(maxLength: 20),
                        DeviceModel = c.String(maxLength: 50),
                        ExternalVelocityCheck = c.String(maxLength: 10),
                        TransactionId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Plans", t => t.PlanId)
                .ForeignKey("dbo.SalesTeams", t => t.SalesTeamId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CompanyId)
                .Index(t => t.SalesTeamId)
                .Index(t => t.UserId)
                .Index(t => t.PlanId);
            
            CreateTable(
                "dbo.StateAgreements",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2, unicode: false),
                        Agreement = c.String(),
                        Agreement_ES = c.String(),
                        StateAgreementParentId = c.String(maxLength:128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.TempOrders",
                c => new
                    {
                    Id = c.String(nullable: false,maxLength: 128),
                    UserId = c.String(maxLength: 300),
                    CompanyId = c.String(maxLength: 128),
                    ParentOrderId = c.String(),
                    SalesTeamId = c.String(maxLength: 128),
                    HouseholdReceivesLifelineBenefits = c.Boolean(nullable: false),
                    CustomerReceivesLifelineBenefits = c.Boolean(nullable: false),
                    QBFirstName = c.String(maxLength: 50),
                    QBLastName = c.String(maxLength: 50),
                    QBSsn = c.Binary(),
                    QBDateOfBirth = c.Binary(),
                    CurrentLifelinePhoneNumber = c.String(maxLength: 20),
                    LifelineProgramId = c.String(maxLength: 128),
                    LPProofTypeId = c.String(maxLength: 128),
                    LPProofNumber = c.String(maxLength: 50),
                    LPImageFileName = c.String(maxLength: 100),
                    StateProgramId = c.String(),
                    StateProgramNumber = c.String(),
                    SecondaryStateProgramId = c.String(),
                    SecondaryStateProgramNumber = c.String(),
                    Language = c.String(),
                    CommunicationPreference = c.String(maxLength:50),
                        FirstName = c.String(maxLength: 50),
                        MiddleInitial = c.String(maxLength: 1),
                        LastName = c.String(maxLength: 50),
                        Ssn = c.Binary(),
                        Gender = c.String(),
                        DateOfBirth = c.Binary(),
                        EmailAddress = c.String(maxLength: 50),
                        ContactPhoneNumber = c.String(),
                        IPProofTypeId = c.String(),
                        IPImageFileName = c.String(maxLength: 100),
                        ServiceAddressBypass = c.Boolean(nullable: false),
                        ServiceAddressBypassSignature = c.String(),
                        ServiceAddressByPassDocumentProofId = c.String(),
                        ServiceAddressByPassImageFileName = c.String(),
                        ServiceAddressStreet1 = c.String(maxLength: 100),
                        ServiceAddressStreet2 = c.String(maxLength: 100),
                        ServiceAddressCity = c.String(maxLength: 50),
                        ServiceAddressState = c.String(maxLength: 2),
                        ServiceAddressZip = c.String(maxLength: 10),
                        ServiceAddressIsPermanent = c.Boolean(nullable: false),
                        BillingAddressStreet1 = c.String(maxLength: 100),
                        BillingAddressStreet2 = c.String(maxLength: 100),
                        BillingAddressCity = c.String(maxLength: 50),
                        BillingAddressState = c.String(maxLength: 2),
                        BillingAddressZip = c.String(maxLength: 10),
                        ShippingAddressStreet1 = c.String(maxLength: 100),
                        ShippingAddressStreet2 = c.String(maxLength: 100),
                        ShippingAddressCity = c.String(maxLength: 50),
                        ShippingAddressState = c.String(maxLength: 2),
                        ShippingAddressZip = c.String(),
                        HohSpouse = c.Boolean(nullable: false),
                        HohAdultsParent = c.Boolean(nullable: false),
                        HohAdultsChild = c.Boolean(nullable: false),
                        HohAdultsRelative = c.Boolean(nullable: false),
                        HohAdultsRoommate = c.Boolean(nullable: false),
                        HohAdultsOther = c.Boolean(nullable: false),
                        HohAdultsOtherText = c.String(maxLength: 50),
                        HohExpenses = c.Boolean(),
                        HohShareLifeline = c.Boolean(),
                        HohShareLifelineNames = c.String(maxLength: 500),
                        HohAgreeMultiHouse = c.Boolean(),
                        HohAgreeViolation = c.Boolean(nullable: false),
                        HohPuertoRicoAgreeViolation = c.Boolean(),
                        Signature = c.String(),
                        Initials = c.String(),
                        SignatureType = c.String(),
                        HasDevice = c.Boolean(nullable: false),
                        CarrierId = c.String(),
                        DeviceId = c.String(),
                        DeviceIdentifier = c.String(maxLength: 50),
                        SimIdentifier = c.String(maxLength: 50),
                        PlanId = c.String(maxLength: 128),
                        TpivBypass = c.Boolean(nullable: false),
                        TpivBypassSignature = c.String(maxLength: 100),
                        TpivBypassSsnProofTypeId = c.String(),
                        TpivBypassSsnImageFileName = c.String(),
                        TpivBypassSsnCardLastFour = c.String(maxLength: 4),
                        TpivBypassDobProofTypeId = c.String(),
                        TpivBypassDobCardLastFour = c.String(maxLength: 4),
                        LatitudeCoordinate = c.Single(nullable: false),
                        LongitudeCoordinate = c.Single(nullable: false),
                        PaymentType = c.String(maxLength: 10),
                        CreditCardReference = c.String(maxLength: 100),
                        CreditCardSuccess = c.Boolean(nullable: false),
                        CreditCardTransactionId = c.String(maxLength: 100),
                        LifelineEnrollmentId = c.String(maxLength: 50),
                        LifelineEnrollmentType = c.String(maxLength: 50),
                        AIInitials = c.String(maxLength: 5),
                        AIFrequency = c.String(maxLength: 20),
                        AIAvgIncome = c.Int(nullable: false),
                        AINumHousehold = c.Int(nullable: false),
                        FulfillmentType = c.String(maxLength: 20),
                        TransactionId = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Plans", t => t.PlanId)
                .ForeignKey("dbo.SalesTeams", t => t.SalesTeamId)
                .Index(t => t.CompanyId)
                .Index(t => t.SalesTeamId)
                .Index(t => t.PlanId);
            
            CreateTable(
                "dbo.Plans",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(maxLength: 100),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2),
                        Price = c.Decimal(nullable: false, precision: 9, scale: 2),
                        Minutes = c.Int(nullable: false),
                        Texts = c.Int(nullable: false),
                        Data = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.PaymentTransactionLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        TransactionID = c.Int(nullable: false, identity: true),
                        CompanyID = c.String(maxLength: 128),
                        TransactionType = c.String(maxLength: 50),
                        OrderType = c.String(maxLength: 50),
                        MerchantID = c.String(maxLength: 128),
                        ReferenceTransaction = c.String(maxLength: 128),
                        Amount = c.Double(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.ProofDocumentTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProofType = c.String(maxLength: 20, unicode: false),
                        Name = c.String(maxLength: 175, unicode: false),
                        StateCode = c.String(maxLength: 2, unicode: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StateSettings",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2),
                        SsnType = c.String(maxLength: 10),
                        IncomeLevel = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrdersTaxes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrderID = c.String(maxLength: 128),
                        OrderType = c.String(maxLength: 10),
                        Amount = c.Double(nullable: false),
                        Description = c.String(maxLength: 500),
                        Type = c.String(maxLength: 100),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TransactionLogs",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        SalesTeamId = c.String(),
                        UserId = c.String(),
                        Type = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WebApplicationLog",
                c => new
                    {
                        Id = c.Guid(nullable: false, identity: true),
                        TimeStamp = c.DateTime(),
                        Level = c.String(),
                        Message = c.String(),
                        Exception = c.String(),
                        CallSite = c.String(),
                        ProcessId = c.String(),
                        ThreadId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BaseIncomeLevels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        StateCode = c.String(maxLength: 2),
                        Base1Person = c.Int(nullable: false),
                        Base2Person = c.Int(nullable: false),
                        Base3Person = c.Int(nullable: false),
                        Base4Person = c.Int(nullable: false),
                        Base5Person = c.Int(nullable: false),
                        Base6Person = c.Int(nullable: false),
                        Base7Person = c.Int(nullable: false),
                        Base8Person = c.Int(nullable: false),
                        BaseAdditional = c.Int(nullable: false),
                        DateActive = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImageUploads",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ImageCode = c.String(nullable: false, maxLength: 20),
                        MaxImageSize = c.Int(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.String(nullable: false, maxLength: 128),
                        StorageCredentialsId = c.String(nullable: false, maxLength: 128),
                        UploadType = c.String(maxLength: 20),
                        DeviceDetails = c.String(),
                        HasBeenUploaded = c.Boolean(nullable: false),
                        DateUploaded = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.ExternalStorageCredentials", t => t.StorageCredentialsId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CompanyId)
                .Index(t => t.StorageCredentialsId);
            
            CreateTable(
                "dbo.LoginInfo",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserIsLoggedIn = c.Boolean(nullable: false),
                        SessionId = c.String(nullable: false, maxLength: 128),
                        Latitude = c.Single(nullable: false),
                        Longitude = c.Single(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TpivProofDocumentTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Type = c.String(maxLength: 5),
                        Name = c.String(maxLength: 100),
                        LifelineSystem = c.String(maxLength: 10),
                        LifelineSystemId = c.String(maxLength: 10),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ZipCodes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        PostalCode = c.String(maxLength: 10),
                        State = c.String(maxLength: 50),
                        StateAbbreviation = c.String(maxLength: 2),
                        CountyFips = c.String(maxLength: 10),
                        City = c.String(maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderNotes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrderID = c.String(maxLength: 50),
                        Note = c.String(),
                        AddedBy = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductCommissions",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ProductType = c.String(maxLength: 100),
                        Amount = c.Decimal(precision: 18, scale: 2),
                        SalesTeamID = c.String(maxLength: 128),
                        RecipientType = c.String(maxLength: 100),
                        RecipientUserId = c.String(maxLength: 128),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.RecipientUserId)
                .Index(t => t.RecipientUserId);
            
            CreateTable(
                "dbo.ResourceCategories",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ResourceCategoryId = c.String(maxLength: 128),
                        CompanyId = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Name = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ResourceCategories", t => t.ResourceCategoryId)
                .Index(t => t.ResourceCategoryId);
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CompanyId = c.String(),
                        ResourceCategoryId = c.String(maxLength: 128),
                        SortOrder = c.Int(nullable: false),
                        Name = c.String(),
                        AlternateName = c.String(),
                        Type = c.String(),
                        Resource = c.String(),
                        AlternateResource = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ResourceCategories", t => t.ResourceCategoryId)
                .Index(t => t.ResourceCategoryId);
            
            CreateTable(
                "dbo.Level3SalesGroupApplicationUser",
                c => new
                    {
                        Level3SalesGroup_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Level3SalesGroup_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Level3SalesGroup", t => t.Level3SalesGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Level3SalesGroup_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Level2SalesGroupApplicationUser",
                c => new
                    {
                        Level2SalesGroup_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Level2SalesGroup_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Level2SalesGroup", t => t.Level2SalesGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Level2SalesGroup_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.Level1SalesGroupApplicationUser",
                c => new
                    {
                        Level1SalesGroup_Id = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Level1SalesGroup_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Level1SalesGroup", t => t.Level1SalesGroup_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Level1SalesGroup_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.TempOrderStateAgreements",
                c => new
                    {
                        TempOrder_Id = c.String(nullable: false, maxLength: 128),
                        StateAgreement_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.TempOrder_Id, t.StateAgreement_Id })
                .ForeignKey("dbo.TempOrders", t => t.TempOrder_Id, cascadeDelete: true)
                .ForeignKey("dbo.StateAgreements", t => t.StateAgreement_Id, cascadeDelete: true)
                .Index(t => t.TempOrder_Id)
                .Index(t => t.StateAgreement_Id);
            
            CreateTable(
                "dbo.OrderStateAgreements",
                c => new
                    {
                        Order_Id = c.String(nullable: false, maxLength: 128),
                        StateAgreement_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Order_Id, t.StateAgreement_Id })
                .ForeignKey("dbo.Orders", t => t.Order_Id, cascadeDelete: true)
                .ForeignKey("dbo.StateAgreements", t => t.StateAgreement_Id, cascadeDelete: true)
                .Index(t => t.Order_Id)
                .Index(t => t.StateAgreement_Id);
           

            this.Sql(@"
                CREATE UNIQUE NONCLUSTERED INDEX EmailIndex
                ON AspNetUsers([Email]);

                CREATE UNIQUE NONCLUSTERED INDEX ExternalUserIDIndex
                ON AspNetUsers([ExternalUserID])
                WHERE ExternalUserID IS NOT NULL AND ExternalUserID != '';
            ");
            this.Sql(@"
                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_StateAgreements_FE2D5760EC62416CED3C' AND object_id = OBJECT_ID('StateAgreements')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_StateAgreements_FE2D5760EC62416CED3C] ON [dbo].[StateAgreements]
                    (
	                    [StateCode] ASC,
	                    [IsDeleted] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_StateAgreements_FE2D5760EC62416CED3C' AND object_id = OBJECT_ID('StateAgreements')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_StateAgreements_FE2D5760EC62416CED3C] ON [dbo].[StateAgreements]
                    (
	                    [StateCode] ASC,
	                    [IsDeleted] ASC
                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_LoginInfo_FD3C9CD2B404EBFA68E4' AND object_id = OBJECT_ID('LoginInfo')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_LoginInfo_FD3C9CD2B404EBFA68E4] ON [dbo].[LoginInfo]
                    (
	                    [UserId] ASC,
	                    [UserIsLoggedIn] ASC,
	                    [IsDeleted] ASC
                    )
                    INCLUDE ( 	[DateCreated],
	                    [DateModified],
	                    [Id],
	                    [Latitude],
	                    [Longitude],
	                    [SessionId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_Orders_14D650235145FFDB8404' AND object_id = OBJECT_ID('Orders')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_Orders_14D650235145FFDB8404] ON [dbo].[Orders]
                    (
	                    [SalesTeamId] ASC,
	                    [DateCreated] ASC
                    )
                    INCLUDE ( 	[AIAvgIncome],
	                    [AIFrequency],
	                    [AIInitials],
	                    [AINumHousehold],
	                    [BillingAddressCity],
	                    [BillingAddressState],
	                    [BillingAddressStreet1],
	                    [BillingAddressStreet2],
	                    [BillingAddressZip],
	                    [CarrierId],
	                    [CompanyId],
	                    [ContactPhoneNumber],
	                    [CreditCardReference],
	                    [CreditCardSuccess],
	                    [CreditCardTransactionId],
	                    [CurrentLifelinePhoneNumber],
	                    [CustomerReceivesLifelineBenefits],
	                    [DateModified],
	                    [DateOfBirth],
	                    [DeviceId],
	                    [DeviceIdentifier],
	                    [DeviceModel],
	                    [EmailAddress],
	                    [ExternalVelocityCheck],
	                    [FirstName],
	                    [FulfillmentType],
	                    [FullFillmentDate],
	                    [HasDevice],
	                    [HohAdultsChild],
	                    [HohAdultsOther],
	                    [HohAdultsOtherText],
	                    [HohAdultsParent],
	                    [HohAdultsRelative],
	                    [HohAdultsRoommate],
	                    [HohAgreeMultiHouse],
	                    [HohAgreeViolation],
	                    [HohExpenses],
	                    [HohPuertoRicoAgreeViolation],
	                    [HohShareLifeline],
	                    [HohShareLifelineNames],
	                    [HohSpouse],
	                    [HouseholdReceivesLifelineBenefits],
	                    [Id],
	                    [IPImageFileName],
	                    [IPProofTypeId],
	                    [IsDeleted],
	                    [LastName],
	                    [LatitudeCoordinate],
	                    [LifelineEnrollmentId],
	                    [LifelineEnrollmentType],
	                    [LifelineProgramId],
	                    [LongitudeCoordinate],
	                    [LPImageFileName],
	                    [LPProofNumber],
	                    [LPProofTypeId],
	                    [MiddleInitial],
	                    [PaymentType],
	                    [PlanId],
	                    [PricePlan],
	                    [PriceTotal],
	                    [QBDateOfBirth],
	                    [QBFirstName],
	                    [QBLastName],
	                    [QBSsn],
	                    [SecondaryStateProgramId],
	                    [SecondaryStateProgramNumber],
	                    [ServiceAddressCity],
	                    [ServiceAddressIsPermanent],
	                    [ServiceAddressIsRural],
	                    [ServiceAddressState],
	                    [ServiceAddressStreet1],
	                    [ServiceAddressStreet2],
	                    [ServiceAddressZip],
	                    [ShippingAddressCity],
	                    [ShippingAddressState],
	                    [ShippingAddressStreet1],
	                    [ShippingAddressStreet2],
	                    [ShippingAddressZip],
	                    [SigFileName],
	                    [Signature],
	                    [SignatureType],
	                    [SimIdentifier],
	                    [Ssn],
	                    [StateProgramId],
	                    [StateProgramNumber],
	                    [TenantAccountId],
	                    [TenantAddressId],
	                    [TenantReferenceId],
	                    [TpivBypass],
	                    [TpivBypassDobCardLastFour],
	                    [TpivBypassDobProofTypeId],
	                    [TpivBypassMessage],
	                    [TpivBypassSignature],
	                    [TpivBypassSsnCardLastFour],
	                    [TpivBypassSsnProofTypeId],
	                    [TpivCode],
	                    [TransactionId],
	                    [UserId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_Orders_26B81AFDE02CD32EE687' AND object_id = OBJECT_ID('ImageUploads')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_Orders_26B81AFDE02CD32EE687] ON [dbo].[Orders]
                    (
	                    [IsDeleted] ASC,
	                    [DateCreated] ASC
                    )
                    INCLUDE ( 	[CompanyId],
	                    [DeviceIdentifier],
	                    [Id],
	                    [LifelineProgramId],
	                    [SimIdentifier],
	                    [TenantAccountId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_Orders_3BD94B119788C056280B' AND object_id = OBJECT_ID('Orders')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_Orders_3BD94B119788C056280B] ON [dbo].[Orders]
                    (
	                    [IsDeleted] ASC,
	                    [SalesTeamId] ASC,
	                    [DateCreated] ASC
                    )
                    INCLUDE ( 	[CompanyId],
	                    [DeviceIdentifier],
	                    [FirstName],
	                    [Id],
	                    [LastName],
	                    [LatitudeCoordinate],
	                    [LifelineProgramId],
	                    [LongitudeCoordinate],
	                    [SimIdentifier],
	                    [TenantAccountId],
	                    [UserId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_CommissionLogs_11152FA1A8D7F04D35DB' AND object_id = OBJECT_ID('CommissionLogs')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_CommissionLogs_11152FA1A8D7F04D35DB] ON [dbo].[CommissionLogs]
                    (
	                    [Amount] ASC
                    )
                    INCLUDE ( 	[OrderId],
	                    [OrderType],
	                    [PaymentID],
	                    [RecipientUserId],
	                    [SalesTeamId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END

                IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='nci_wi_CommissionLogs_11152FA1A8D7F04D35DB' AND object_id = OBJECT_ID('CommissionLogs')) 
                BEGIN
                    CREATE NONCLUSTERED INDEX [nci_wi_CommissionLogs_11152FA1A8D7F04D35DB] ON [dbo].[CommissionLogs]
                    (
	                    [Amount] ASC
                    )
                    INCLUDE ( 	[OrderId],
	                    [OrderType],
	                    [PaymentID],
	                    [RecipientUserId],
	                    [SalesTeamId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                END
            ");


            }
        
        public override void Down()
        {
            DropForeignKey("dbo.Resources", "ResourceCategoryId", "dbo.ResourceCategories");
            DropForeignKey("dbo.ResourceCategories", "ResourceCategoryId", "dbo.ResourceCategories");
            DropForeignKey("dbo.ProductCommissions", "RecipientUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ImageUploads", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ImageUploads", "StorageCredentialsId", "dbo.ExternalStorageCredentials");
            DropForeignKey("dbo.ImageUploads", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.PaymentTransactionLogs", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Orders", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "SalesTeamId", "dbo.SalesTeams");
            DropForeignKey("dbo.Orders", "PlanId", "dbo.Plans");
            DropForeignKey("dbo.Orders", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.OrderStateAgreements", "StateAgreement_Id", "dbo.StateAgreements");
            DropForeignKey("dbo.OrderStateAgreements", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.TempOrders", "SalesTeamId", "dbo.SalesTeams");
            DropForeignKey("dbo.TempOrders", "PlanId", "dbo.Plans");
            DropForeignKey("dbo.Plans", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.TempOrders", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.TempOrderStateAgreements", "StateAgreement_Id", "dbo.StateAgreements");
            DropForeignKey("dbo.TempOrderStateAgreements", "TempOrder_Id", "dbo.TempOrders");
            DropForeignKey("dbo.NladPhoneNumbers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.LifelinePrograms", "RequiredStateProgramId", "dbo.StatePrograms");
            DropForeignKey("dbo.LifelinePrograms", "RequiredSecondaryStateProgramId", "dbo.StatePrograms");
            DropForeignKey("dbo.ExternalStorageCredentials", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.ResponseLogsCGMEHDBDetails", "ResponseLogsCGMEHDBId", "dbo.ResponseLogsCGMEHDBs");
            DropForeignKey("dbo.ComplianceStatements", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Competitors", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.CommissionLogs", "RecipientUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommissionLogs", "SalesTeamId", "dbo.SalesTeams");
            DropForeignKey("dbo.CommissionLogs", "PaymentID", "dbo.Payments");
            DropForeignKey("dbo.CaliPhoneNumbers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.SacEntries", "Company_Id", "dbo.Companies");
            DropForeignKey("dbo.AspNetUsers", "SalesTeamId", "dbo.SalesTeams");
            DropForeignKey("dbo.SalesTeams", "Level3SalesGroupId", "dbo.Level3SalesGroup");
            DropForeignKey("dbo.Level3SalesGroup", "ParentSalesGroupId", "dbo.Level2SalesGroup");
            DropForeignKey("dbo.Level2SalesGroup", "ParentSalesGroupId", "dbo.Level1SalesGroup");
            DropForeignKey("dbo.Level1SalesGroupApplicationUser", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level1SalesGroupApplicationUser", "Level1SalesGroup_Id", "dbo.Level1SalesGroup");
            DropForeignKey("dbo.Level1SalesGroup", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level1SalesGroup", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Level2SalesGroupApplicationUser", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level2SalesGroupApplicationUser", "Level2SalesGroup_Id", "dbo.Level2SalesGroup");
            DropForeignKey("dbo.Level2SalesGroup", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level2SalesGroup", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Level3SalesGroupApplicationUser", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level3SalesGroupApplicationUser", "Level3SalesGroup_Id", "dbo.Level3SalesGroup");
            DropForeignKey("dbo.Level3SalesGroup", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Level3SalesGroup", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.SalesTeams", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SalesTeams", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.OrderStateAgreements", new[] { "StateAgreement_Id" });
            DropIndex("dbo.OrderStateAgreements", new[] { "Order_Id" });
            DropIndex("dbo.TempOrderStateAgreements", new[] { "StateAgreement_Id" });
            DropIndex("dbo.TempOrderStateAgreements", new[] { "TempOrder_Id" });
            DropIndex("dbo.Level1SalesGroupApplicationUser", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Level1SalesGroupApplicationUser", new[] { "Level1SalesGroup_Id" });
            DropIndex("dbo.Level2SalesGroupApplicationUser", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Level2SalesGroupApplicationUser", new[] { "Level2SalesGroup_Id" });
            DropIndex("dbo.Level3SalesGroupApplicationUser", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Level3SalesGroupApplicationUser", new[] { "Level3SalesGroup_Id" });
            DropIndex("dbo.Resources", new[] { "ResourceCategoryId" });
            DropIndex("dbo.ResourceCategories", new[] { "ResourceCategoryId" });
            DropIndex("dbo.ProductCommissions", new[] { "RecipientUserId" });
            DropIndex("dbo.ImageUploads", new[] { "StorageCredentialsId" });
            DropIndex("dbo.ImageUploads", new[] { "CompanyId" });
            DropIndex("dbo.ImageUploads", new[] { "UserId" });
            DropIndex("dbo.PaymentTransactionLogs", new[] { "CompanyID" });
            DropIndex("dbo.Plans", new[] { "CompanyId" });
            DropIndex("dbo.TempOrders", new[] { "PlanId" });
            DropIndex("dbo.TempOrders", new[] { "SalesTeamId" });
            DropIndex("dbo.TempOrders", new[] { "CompanyId" });
            DropIndex("dbo.Orders", new[] { "PlanId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "SalesTeamId" });
            DropIndex("dbo.Orders", new[] { "CompanyId" });
            DropIndex("dbo.NladPhoneNumbers", new[] { "CompanyId" });
            DropIndex("dbo.LifelinePrograms", new[] { "RequiredSecondaryStateProgramId" });
            DropIndex("dbo.LifelinePrograms", new[] { "RequiredStateProgramId" });
            DropIndex("dbo.ExternalStorageCredentials", new[] { "CompanyId" });
            DropIndex("dbo.ResponseLogsCGMEHDBDetails", new[] { "ResponseLogsCGMEHDBId" });
            DropIndex("dbo.ComplianceStatements", new[] { "CompanyId" });
            DropIndex("dbo.Competitors", new[] { "CompanyId" });
            DropIndex("dbo.CommissionLogs", new[] { "PaymentID" });
            DropIndex("dbo.CommissionLogs", new[] { "SalesTeamId" });
            DropIndex("dbo.CommissionLogs", new[] { "RecipientUserId" });
            DropIndex("dbo.SacEntries", new[] { "Company_Id" });
            DropIndex("dbo.Level1SalesGroup", new[] { "CreatedByUserId" });
            DropIndex("dbo.Level1SalesGroup", new[] { "CompanyId" });
            DropIndex("dbo.Level2SalesGroup", new[] { "CreatedByUserId" });
            DropIndex("dbo.Level2SalesGroup", new[] { "ParentSalesGroupId" });
            DropIndex("dbo.Level2SalesGroup", new[] { "CompanyId" });
            DropIndex("dbo.Level3SalesGroup", new[] { "CreatedByUserId" });
            DropIndex("dbo.Level3SalesGroup", new[] { "ParentSalesGroupId" });
            DropIndex("dbo.Level3SalesGroup", new[] { "CompanyId" });
            DropIndex("dbo.SalesTeams", new[] { "CreatedByUserId" });
            DropIndex("dbo.SalesTeams", new[] { "Level3SalesGroupId" });
            DropIndex("dbo.SalesTeams", new[] { "CompanyId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CreatedByUserId" });
            DropIndex("dbo.AspNetUsers", new[] { "SalesTeamId" });
            DropIndex("dbo.AspNetUsers", new[] { "CompanyId" });
            DropIndex("dbo.CaliPhoneNumbers", new[] { "CompanyId" });
            DropTable("dbo.OrderStateAgreements");
            DropTable("dbo.TempOrderStateAgreements");
            DropTable("dbo.Level1SalesGroupApplicationUser");
            DropTable("dbo.Level2SalesGroupApplicationUser");
            DropTable("dbo.Level3SalesGroupApplicationUser");
            DropTable("dbo.Resources");
            DropTable("dbo.ResourceCategories");
            DropTable("dbo.ProductCommissions");
            DropTable("dbo.OrderNotes");
            DropTable("dbo.ZipCodes");
            DropTable("dbo.TpivProofDocumentTypes");
            DropTable("dbo.LoginInfo");
            DropTable("dbo.ImageUploads");
            DropTable("dbo.BaseIncomeLevels");
            DropTable("dbo.WebApplicationLog");
            DropTable("dbo.TransactionLogs");
            DropTable("dbo.OrdersTaxes");
            DropTable("dbo.StateSettings");
            DropTable("dbo.ProofDocumentTypes");
            DropTable("dbo.PaymentTransactionLogs");
            DropTable("dbo.Plans");
            DropTable("dbo.TempOrders");
            DropTable("dbo.StateAgreements");
            DropTable("dbo.Orders");
            DropTable("dbo.NladPhoneNumbers");
            DropTable("dbo.LoginMsgs");
            DropTable("dbo.StatePrograms");
            DropTable("dbo.LifelinePrograms");
            DropTable("dbo.ExternalStorageCredentials");
            DropTable("dbo.ResponseLogsCGMEHDBDetails");
            DropTable("dbo.ResponseLogsCGMEHDBs");
            DropTable("dbo.DevDatas");
            DropTable("dbo.ComplianceStatements");
            DropTable("dbo.Competitors");
            DropTable("dbo.CompanyTranslations");
            DropTable("dbo.Payments");
            DropTable("dbo.CommissionLogs");
            DropTable("dbo.SacEntries");
            DropTable("dbo.Level1SalesGroup");
            DropTable("dbo.Level2SalesGroup");
            DropTable("dbo.Level3SalesGroup");
            DropTable("dbo.SalesTeams");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Companies");
            DropTable("dbo.CaliPhoneNumbers");
            DropTable("dbo.ApiLogEntries");
            DropTable("dbo.AddressProofDocumentTypes");
        }
    }
}
