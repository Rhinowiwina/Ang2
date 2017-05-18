namespace LS.Repositories.Migrations {
    using System;
    using System.Data.Entity.Migrations;

    public partial class Data_Procedures_Views : DbMigration {
        public override void Up() {
            this.Sql("INSERT INTO Companies (Id, Name, MinToChangeTeam, MaxCommission,DateCreated, DateModified, IsDeleted, PrimaryColorHex, SecondaryColorHex, CompanyLogoURL) VALUES ('65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'Arrow', '2', '500.00', '2016-05-16 00:00:00.000','2016-05-16 00:00:00.000', 'false', 'ce181e', '939598', 'Arrow-Sales-Group-Logo.png')");

            this.Sql(@"INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES ('116f684-1ade-4b19-bcb6-ec45ce7e4a89', 0, 'Super Administrator');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('538940e1-b769-4d53-8710-30a20eac8256', 3, 'Level 2 Manager');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('577db571-458f-4bfc-81f3-1a415758b8f6', 1, 'Administrator');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('853ddcca-3369-41f2-97f4-d063f8ce94e5', 6, 'Sales Rep');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('96a84c56-c15e-4707-90c3-755fbcf5c33c', 4, 'Level 3 Manager');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('ef77e3e0-fedd-4284-85de-f9fb411714bb', 5, 'Sales Team Manager');
                INSERT INTO ASPNetRoles(Id, Rank, Name) VALUES('f1c85c97-2c50-4165-9bb8-248518a109f2', 2, 'Level 1 Manager');
            ");

            this.Sql(@"
                INSERT INTO ASPNetUsers(Id, FirstName, LastName, PayPalEmail, CompanyId, Language, IsActive, IsDeleted, DateCreated, DateModified, Email, EmailConfirmed, PasswordHash, SecurityStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Username,PermissionsLifelineCA,PermissionsBypassTpiv,PermissionsAccountOrder,RequiresTraining,ExternalUserId)
                VALUES('b0d345d3-cfba-4ff4-89d2-5952c65652f9', 'SA', '305', 'kevin@305spin.com', '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'EN', 1, 0, getdate(), getdate(), 'kevin@305spin.com', 1, 'AJYTsJ1Zs2l7rC3qSPBz4b7Cu5baGs2+2OPyJTBFRrCoz+2069a9wy/tPb/oVvRBKQ==', 'ed50e3db-32c4-4119-9ffa-cb531d72b755', 1, 0, 0, 0, 'SA',1,1,1,0,'11');
                INSERT INTO ASPNetUserRoles(UserID, RoleID) VALUES('b0d345d3-cfba-4ff4-89d2-5952c65652f9', '116f684-1ade-4b19-bcb6-ec45ce7e4a89');
            ");
            this.Sql(@"
                CREATE PROCEDURE [dbo].[usp_GetUsers]
                 @UserId [nvarchar](128) = 'ThisUserIDWontExist', 
                 @FilterUserName [nvarchar](500) = NULL
                AS
                BEGIN
                 -- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
                 SET NOCOUNT ON;

                 SELECT U.Id, U.FirstName, U.LastName, U.IsActive,U.UserName, R.Name, R.Rank
                 FROM AspNetUsers (NOLOCK) U
                  -- Returned user's info
                  LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
                  LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID

                  -- Groups that returned user manages
                  LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.ApplicationUser_Id=U.Id
                  LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.ApplicationUser_Id=U.Id
                  LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) UG3 ON UG3.ApplicationUser_Id=U.Id

                  --Logged in user's rank
                  LEFT JOIN AspNetUsers (NOLOCK) LIU ON LIU.Id=@UserId
                  LEFT JOIN AspNetUserRoles (NOLOCK) LIUR ON LIUR.UserId=LIU.Id
                  LEFT JOIN AspNetRoles (NOLOCK) LIR ON LIR.Id=LIUR.RoleID

                  --Logged in user's teams
                  LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) G1LIU ON G1LIU.ApplicationUser_Id=@UserId
                  LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) G2LIU ON G2LIU.ApplicationUser_Id=@UserId
                  LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) G3LIU ON G3LIU.ApplicationUser_Id=@UserId
                  LEFT JOIN Level2SalesGroup (NOLOCK) AG2 ON AG2.ParentSalesGroupId=G1LIU.Level1SalesGroup_Id OR AG2.Id=G2LIU.Level2SalesGroup_Id
                  LEFT JOIN Level3SalesGroup (NOLOCK) AG3 ON AG3.ParentSalesGroupId=AG2.Id OR AG3.Id=G3LIU.Level3SalesGroup_Id
                  LEFT JOIN SalesTeams (NOLOCK) T ON T.Level3SalesGroupId=AG3.Id OR T.Id=LIU.SalesTeamId
                 WHERE 1=1
                  AND U.CompanyId=LIU.CompanyId
                  AND U.IsDeleted=0
                  AND (
                   LIR.Rank<=1 --Logged in user is an admin or above
                   OR UG1.Level1SalesGroup_Id=G1LIU.Level1SalesGroup_Id --User is a manager for one of the Logged in user's Level 1 Groups
                   OR UG2.Level2SalesGroup_Id=AG2.Id --User is a manager for one of the Logged in user's Level 2 Groups
                   OR UG3.Level3SalesGroup_Id=AG3.Id --User is a manager for one of the Logged in user's Level 3 Groups
                   OR U.SalesTeamID=T.ID --User is assigned to one of the Logged in user's teams
                  )
                  AND LIR.Rank<R.Rank --User is a lower rank than the Logged in user
                  AND (
                   U.FirstName LIKE '%'+@FilterUserName+'%' OR U.LastName LIKE '%'+@FilterUserName+'%' OR U.FirstName+' '+U.LastName LIKE '%'+@FilterUserName+'%'
                   OR U.Username LIKE '%'+@FilterUserName+'%'
                   OR U.Email LIKE '%'+@FilterUserName+'%'
                   OR @FilterUserName IS NULL
                  )
                 GROUP BY U.Id, U.FirstName, U.LastName, U.IsActive, U.Username, R.Name, R.Rank
                 ORDER BY U.FirstName, U.LastName, U.Username
                END
            ");


            this.Sql(@"CREATE VIEW[dbo].[v_GroupsManaged] AS SELECT U.Id As UserID, G.Name AS GroupName FROM AspNetUsers(NOLOCK) U INNER JOIN Level3SalesGroupApplicationUser(NOLOCK) AU ON U.Id = AU.ApplicationUser_Id INNER JOIN Level3SalesGroup(NOLOCK) G ON AU.Level3SalesGroup_Id = G.Id where U.IsDeleted = 0 AND G.IsDeleted = 0 Union SELECT U.Id As UserID, G.Name AS GroupName FROM AspNetUsers(NOLOCK) U INNER JOIN Level2SalesGroupApplicationUser(NOLOCK) AU  ON U.Id = AU.ApplicationUser_Id INNER JOIN Level2SalesGroup(NOLOCK) G ON AU.Level2SalesGroup_Id = G.Id where U.IsDeleted = 0 AND G.IsDeleted = 0 Union SELECT U.Id As UserID, G.Name AS GroupName FROM AspNetUsers(NOLOCK) U INNER JOIN                         Level1SalesGroupApplicationUser(NOLOCK) AU  ON U.Id = AU.ApplicationUser_Id INNER JOIN Level1SalesGroup(NOLOCK) G ON AU.Level1SalesGroup_Id = G.Id where U.IsDeleted = 0 AND G.IsDeleted = 0");
            this.Sql(@"
                       CREATE VIEW [dbo].[v_UserTeams] AS
                    SELECT T.Id, T.Name, T.CompanyId, T.ExternalPrimaryId, T.ExternalDisplayName, T.Address1, T.Address2, T.City, T.State, T.Zip, T.Phone, T.TaxId, T.PayPalEmail, T.Level3SalesGroupId, T.CreatedByUserId, T.IsActive, T.IsDeleted, T.DateCreated, T.DateModified, T.CycleCountTypeDevice, T.CycleCountTypeSim, T.SigType
            , U.Id AS UserID, U.FirstName, U.LastName
                     , TG3.Id AS Level3Id, TG3.Name AS Level3Name, TG3.IsDeleted AS Level3IsDeleted, TG2.Id AS Level2Id, TG2.Name AS Level2Name, TG2.IsDeleted AS Level2IsDeleted, TG1.Id AS Level1Id, TG1.Name AS Level1Name, TG1.IsDeleted AS Level1IsDeleted
                    FROM SalesTeams (NOLOCK) T
                     LEFT JOIN Level3SalesGroup (NOLOCK) TG3 ON TG3.Id=T.Level3SalesGroupId
                     LEFT JOIN Level2SalesGroup (NOLOCK) TG2 ON TG2.Id=TG3.ParentSalesGroupId
                     LEFT JOIN Level1SalesGroup (NOLOCK) TG1 ON TG1.Id=TG2.ParentSalesGroupId
                     LEFT JOIN 
                      (
                       SELECT U.CompanyId, U.Id, U.FirstName, U.LastName, U.SalesTeamId, R.Rank
                       FROM AspNetUsers (NOLOCK) U
                        LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
                        LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
                      ) U ON U.CompanyId=T.CompanyId 
                     LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.Level1SalesGroup_Id=TG1.Id AND UG1.ApplicationUser_Id=U.Id
                     LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.Level2SalesGroup_Id=TG2.Id AND UG2.ApplicationUser_Id=U.Id
                     LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) UG3 ON UG3.Level3SalesGroup_Id=TG3.Id AND UG3.ApplicationUser_Id=U.Id
                     WHERE (U.Rank<=1 OR UG1.Level1SalesGroup_Id IS NOT NULL OR UG2.Level2SalesGroup_Id IS NOT NULL OR UG3.Level3SalesGroup_Id IS NOT NULL OR U.SalesTeamId=T.Id)
                   ");
            this.Sql(@"
                       CREATE VIEW[dbo].[v_UserActiveTeams] AS
                       SELECT *
                       FROM v_UserTeams
                       WHERE IsDeleted = 0  AND IsActive = 1 AND Level1IsDeleted = 0 AND Level2IsDeleted = 0 AND Level3IsDeleted = 0
                   ");

            this.Sql(@"
                       CREATE VIEW [dbo].[v_UserGroups] AS
                           SELECT U.Id AS UserID, TG3.Id AS Level3Id, TG3.Name AS Level3Name, TG2.Id AS Level2Id, TG2.Name AS Level2Name, TG1.Id AS Level1Id, TG1.Name AS Level1Name
                           FROM Level3SalesGroup (NOLOCK) TG3
                            LEFT JOIN Level2SalesGroup (NOLOCK) TG2 ON TG2.Id=TG3.ParentSalesGroupId
                            LEFT JOIN Level1SalesGroup (NOLOCK) TG1 ON TG1.Id=TG2.ParentSalesGroupId
                            LEFT JOIN 
                             (
                              SELECT U.CompanyId, U.Id, U.FirstName, U.LastName, U.SalesTeamId, R.Rank
                              FROM AspNetUsers (NOLOCK) U
                               LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
                               LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
                             ) U ON U.CompanyId=TG3.CompanyId 
                            LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.Level1SalesGroup_Id=TG1.Id AND UG1.ApplicationUser_Id=U.Id
                            LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.Level2SalesGroup_Id=TG2.Id AND UG2.ApplicationUser_Id=U.Id
                            LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) UG3 ON UG3.Level3SalesGroup_Id=TG3.Id AND UG3.ApplicationUser_Id=U.Id
                            WHERE (U.Rank<=1 OR UG1.Level1SalesGroup_Id IS NOT NULL OR UG2.Level2SalesGroup_Id IS NOT NULL OR UG3.Level3SalesGroup_Id IS NOT NULL)
                             AND TG1.IsDeleted = 0 AND TG2.IsDeleted = 0 AND TG3.IsDeleted = 0
                            GROUP BY TG3.Id, TG3.Name, TG2.Id, TG2.Name, TG1.Id, TG1.Name, U.ID
                           UNION SELECT U.Id AS UserID, NULL AS Level3Id, NULL AS Level3Name, TG2.Id AS Level2Id, TG2.Name AS Level2Name, TG1.Id AS Level1Id, TG1.Name AS Level1Name
                           FROM Level2SalesGroup (NOLOCK) TG2
                            LEFT JOIN Level1SalesGroup (NOLOCK) TG1 ON TG1.Id=TG2.ParentSalesGroupId
                            LEFT JOIN 
                             (
                              SELECT U.CompanyId, U.Id, U.FirstName, U.LastName, U.SalesTeamId, R.Rank
                              FROM AspNetUsers (NOLOCK) U
                               LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
                               LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
                             ) U ON U.CompanyId=TG2.CompanyId 
                            LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.Level1SalesGroup_Id=TG1.Id AND UG1.ApplicationUser_Id=U.Id
                            LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) UG2 ON UG2.Level2SalesGroup_Id=TG2.Id AND UG2.ApplicationUser_Id=U.Id
                            --LEFT JOIN Level3SalesGroup (NOLOCK) TG3 ON TG3.ParentSalesGroupId = TG2.Id
                            WHERE (U.Rank<=1 OR UG1.Level1SalesGroup_Id IS NOT NULL OR UG2.Level2SalesGroup_Id IS NOT NULL)
                             AND TG1.IsDeleted = 0 AND TG2.IsDeleted = 0-- AND COALESCE(TG3.IsDeleted, 1)=1
                            GROUP BY TG2.Id, TG2.Name, TG1.Id, TG1.Name, U.ID
                           UNION SELECT U.Id AS UserID, NULL AS Level3Id, NULL AS Level3Name, NULL AS Level2Id, NULL AS Level2Name, TG1.Id AS Level1Id, TG1.Name AS Level1Name
                           FROM Level1SalesGroup (NOLOCK) TG1
                            LEFT JOIN 
                             (
                              SELECT U.CompanyId, U.Id, U.FirstName, U.LastName, U.SalesTeamId, R.Rank
                              FROM AspNetUsers (NOLOCK) U
                               LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
                               LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
                             ) U ON U.CompanyId=TG1.CompanyId 
                            LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) UG1 ON UG1.Level1SalesGroup_Id=TG1.Id AND UG1.ApplicationUser_Id=U.Id
                            --LEFT JOIN Level2SalesGroup (NOLOCK) TG2 ON TG2.ParentSalesGroupId = TG1.Id
                            WHERE (U.Rank<=1 OR UG1.Level1SalesGroup_Id IS NOT NULL)
                             AND TG1.IsDeleted = 0-- AND COALESCE(TG2.IsDeleted, 1)=1
                            GROUP BY TG1.Id, TG1.Name, U.ID
                   ");


            this.Sql(@"
                           CREATE PROCEDURE[dbo].[Temp_Order_Insert]
                           @Id[nvarchar](128),
                           @CompanyId[nvarchar](128),
                           @SalesTeamId[nvarchar](128),
                           @UserId[nvarchar](128),
                           @HouseholdReceivesLifelineBenefits[bit],
                           @CustomerReceivesLifelineBenefits[bit],
                           @QBFirstName[nvarchar](50),
                           @QBLastName[nvarchar](50),
                           @UnencryptedQbSsn[varchar](max),
                           @UnencryptedQbDob[varchar](max),
                           @CurrentLifelinePhoneNumber[nvarchar](20),
                           @LifelineProgramId[nvarchar](max),
                           @LPProofTypeId[nvarchar](max),
                           @LPProofNumber[nvarchar](50),
                           @LPImageFileName[nvarchar](100),
                           @StateProgramId[nvarchar](max),
                           @StateProgramNumber[nvarchar](max),
                           @SecondaryStateProgramId[nvarchar](max),
                           @SecondaryStateProgramNumber[nvarchar](max),
                           @FirstName[nvarchar](50),
                           @MiddleInitial[nvarchar](1),
                           @LastName[nvarchar](50),
                           @UnencryptedSsn[varchar](max),
                           @UnencryptedDob[varchar](max),
                           @EmailAddress[nvarchar](50),
                           @ContactPhoneNumber[nvarchar](max),
                           @IPProofTypeId[nvarchar](max),
                           @IPImageFileName[nvarchar](100),
                           @ServiceAddressStreet1[nvarchar](100),
                           @ServiceAddressStreet2[nvarchar](100),
                           @ServiceAddressCity[nvarchar](50),
                           @ServiceAddressState[nvarchar](2),
                           @ServiceAddressZip[nvarchar](10),
                           @ServiceAddressIsPermanent[bit],
                           @BillingAddressStreet1[nvarchar](100),
                           @BillingAddressStreet2[nvarchar](100),
                           @BillingAddressCity[nvarchar](50),
                           @BillingAddressState[nvarchar](2),
                           @BillingAddressZip[nvarchar](10),
                           @ShippingAddressStreet1[nvarchar](100),
                           @ShippingAddressStreet2[nvarchar](100),
                           @ShippingAddressCity[nvarchar](50),
                           @ShippingAddressState[nvarchar](2),
                           @ShippingAddressZip[nvarchar](max),
                           @HohSpouse[bit],
                           @HohAdultsParent[bit],
                           @HohAdultsChild[bit],
                           @HohAdultsRelative[bit],
                           @HohAdultsRoommate[bit],
                           @HohAdultsOther[bit],
                           @HohAdultsOtherText[nvarchar](50),
                           @HohExpenses[bit],
                           @HohShareLifeline[bit],
                           @HohShareLifelineNames[nvarchar](500),
                           @HohAgreeMultiHouse[bit],
                           @HohAgreeViolation[bit],
                           @Signature[nvarchar](max),
                           @SignatureType[nvarchar](max),
                           @HasDevice[bit],
                           @CarrierId[nvarchar](max),
                           @DeviceId[nvarchar](128),
                           @DeviceIdentifier[nvarchar](50),
                           @SimIdentifier[nvarchar](50),
                           @PlanId[nvarchar](128),
                           @TpivBypass[bit],
                           @TpivBypassSignature[nvarchar](100),
                           @TpivBypassSsnProofTypeId[nvarchar](max),
                           @TpivBypassSsnCardLastFour[nvarchar](4),
                           @TpivBypassDobProofTypeId[nvarchar](max),
                           @TpivBypassDobCardLastFour[nvarchar](4),
                           @LatitudeCoordinate[real],
                           @LongitudeCoordinate[real],
                           @PaymentType[nvarchar](10),
                           @CreditCardReference[nvarchar](100),
                           @CreditCardSuccess[bit],
                           @CreditCardTransactionId[nvarchar](100),
                           @LifelineEnrollmentId[nvarchar](50),
                           @LifelineEnrollmentType[nvarchar](50),
                           @AIInitials[nvarchar](max),
                           @AIFrequency[nvarchar](max),
                           @AIAvgIncome int,
                           @AINumHousehold int,
                           @FulfillmentType[varchar](20),
                           @DateCreated[datetime],
                           @DateModified[datetime],
                           @IsDeleted[bit],
                           @Passphrase[nvarchar](max),
                           @TransactionId[nvarchar](max),
                           @ParentOrderId[nvarchar](128),
                           @HohPuertoRicoAgreeViolation[bit],
                           @Gender[nvarchar](6),
            @ServiceAddressBypassDocumentProofId [nvarchar](128),
                       	@ServiceAddressBypassImageFileName [nvarchar](128),
                        @ServiceAddressBypassSignature[nvarchar](100),
                        @ServiceAddressBypass[bit],
                           @TpivBypassSsnImageFileName [nvarchar](100),
                           @Initials [nvarchar](max),
                           @Language [nvarchar](2),
                           @CommunicationPreference [nvarchar](50)
                       AS
                       BEGIN
                           INSERT[dbo].[TempOrders](
                                     [Id], [CompanyId], [SalesTeamId], [UserId]
                                     , [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits]
                                     , [QBFirstName], [QBLastName], [QBSsn], [QBDateOfBirth]
                                     , [CurrentLifelinePhoneNumber]
                                     , [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageFileName]
                                     , [StateProgramId], [StateProgramNumber], [SecondaryStateProgramId], [SecondaryStateProgramNumber]
                                     , [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber]
                                     , [IPProofTypeId], [IPImageFileName]
                                     , [ServiceAddressStreet1], [ServiceAddressStreet2], [ServiceAddressCity], [ServiceAddressState], [ServiceAddressZip], [ServiceAddressIsPermanent]
                                     , [BillingAddressStreet1], [BillingAddressStreet2], [BillingAddressCity], [BillingAddressState], [BillingAddressZip]
                                     , [ShippingAddressStreet1], [ShippingAddressStreet2], [ShippingAddressCity], [ShippingAddressState], [ShippingAddressZip]
                                     , [HohSpouse], [HohAdultsParent], [HohAdultsChild], [HohAdultsRelative], [HohAdultsRoommate], [HohAdultsOther], [HohAdultsOtherText], [HohExpenses], [HohShareLifeline], [HohShareLifelineNames], [HohAgreeMultiHouse], [HohAgreeViolation]
                                     , [Signature],[SignatureType]
                                     , [HasDevice], [CarrierId], [DeviceId], [DeviceIdentifier], [SimIdentifier], [PlanId]
                                     , [TpivBypass], [TpivBypassSignature], [TpivBypassSsnProofTypeId], [TpivBypassSsnCardLastFour], [TpivBypassDobProofTypeId], [TpivBypassDobCardLastFour]
                                     , [LatitudeCoordinate], [LongitudeCoordinate]
                                     , [PaymentType], [CreditCardReference], [CreditCardSuccess], [CreditCardTransactionId]
                                     , [LifelineEnrollmentId], [LifelineEnrollmentType]
                                     , [AIInitials], [AIFrequency], [AIAvgIncome], [AINumHousehold], [FulfillmentType],[TransactionId]
                                     , [DateCreated], [DateModified], [IsDeleted],[ParentOrderId],[HohPuertoRicoAgreeViolation],[Gender],
            		  [ServiceAddressBypassDocumentProofId],[ServiceAddressBypassImageFileName],[ServiceAddressBypassSignature],[ServiceAddressBypass], [TpivBypassSsnImageFileName],[Initials],[Language],[CommunicationPreference])

                           VALUES(
                                     @Id, @CompanyId, @SalesTeamId, @UserId
                                     , @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits
                                     , @QBFirstName, @QBLastName, ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbSsn), ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbDob)
                                     , @CurrentLifelinePhoneNumber
                                     , @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPImageFileName
                                     , @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber
                                     , @FirstName, @MiddleInitial, @LastName, ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedSsn), ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedDob), @EmailAddress, @ContactPhoneNumber
                                     , @IPProofTypeId, @IPImageFileName
                                     , @ServiceAddressStreet1, @ServiceAddressStreet2, @ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent
                                     , @BillingAddressStreet1, @BillingAddressStreet2, @BillingAddressCity, @BillingAddressState, @BillingAddressZip
                                     , @ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, @ShippingAddressState, @ShippingAddressZip
                                     , @HohSpouse, @HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate, @HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames, @HohAgreeMultiHouse, @HohAgreeViolation
                                     , @Signature, @SignatureType
                                     , @HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier, @PlanId
                                     , @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnCardLastFour, @TpivBypassDobProofTypeId, @TpivBypassDobCardLastFour
                                     , @LatitudeCoordinate, @LongitudeCoordinate
                                     , @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId
                                     , @LifelineEnrollmentId, @LifelineEnrollmentType
                                     , @AIInitials, @AIFrequency, @AIAvgIncome, @AINumHousehold, @FulfillmentType, @TransactionId
                                     , @DateCreated, @DateModified, @IsDeleted, @ParentOrderId, @HohPuertoRicoAgreeViolation,@Gender,@ServiceAddressBypassDocumentProofId,@ServiceAddressBypassImageFileName      ,@ServiceAddressBypassSignature,@ServiceAddressBypass,@TpivBypassSsnImageFileName,@Initials,@Language,@CommunicationPreference)

                           SELECT* FROM TempOrders WHERE Id = @Id
                       END
                       ");

            this.Sql(@"
                                            CREATE PROCEDURE [dbo].[Order_Insert]
                       @Id [nvarchar](128),
                       @CompanyId [nvarchar](128),
                       @SalesTeamId [nvarchar](128),
                       @UserId [nvarchar](128),
                       @HouseholdReceivesLifelineBenefits [bit],
                       @CustomerReceivesLifelineBenefits [bit],
                       @QBFirstName [nvarchar](50),
                       @QBLastName [nvarchar](50),
                       @UnencryptedQbSsn [varchar](max),
                       @UnencryptedQbDob [varchar](max),
                       @CurrentLifelinePhoneNumber [nvarchar](20),
                       @LifelineProgramId [nvarchar](max),
                       @LPProofTypeId [nvarchar](max),
                       @LPProofNumber [nvarchar](50),
                       @LPImageFileName [nvarchar](100),
                       @StateProgramId [nvarchar](max),
                       @StateProgramNumber [nvarchar](max),
                       @SecondaryStateProgramId [nvarchar](max),
                       @SecondaryStateProgramNumber [nvarchar](max),
                       @FirstName [nvarchar](50),
                       @MiddleInitial [nvarchar](1),
                       @LastName [nvarchar](50),
                       @UnencryptedSsn [varchar](max),
                       @UnencryptedDob [varchar](max),
                       @EmailAddress [nvarchar](50),
                       @ContactPhoneNumber [nvarchar](max),
                       @IPProofTypeId [nvarchar](max),
                       @IPImageFileName [nvarchar](100),
                       @ServiceAddressStreet1 [nvarchar](100),
                       @ServiceAddressStreet2 [nvarchar](100),
                       @ServiceAddressCity [nvarchar](50),
                       @ServiceAddressState [nvarchar](2),
                       @ServiceAddressZip [nvarchar](10),
                       @ServiceAddressIsPermanent [bit],
                       @ServiceAddressIsRural [bit],
                       @BillingAddressStreet1 [nvarchar](100),
                       @BillingAddressStreet2 [nvarchar](100),
                       @BillingAddressCity [nvarchar](50),
                       @BillingAddressState [nvarchar](2),
                       @BillingAddressZip [nvarchar](10),
                       @ShippingAddressStreet1 [nvarchar](100),
                       @ShippingAddressStreet2 [nvarchar](100),
                       @ShippingAddressCity [nvarchar](50),
                       @ShippingAddressState [nvarchar](2),
                       @ShippingAddressZip [nvarchar](max),
                       @HohSpouse [bit],
                       @HohAdultsParent [bit],
                       @HohAdultsChild [bit],
                       @HohAdultsRelative [bit],
                       @HohAdultsRoommate [bit],
                       @HohAdultsOther [bit],
                       @HohAdultsOtherText [nvarchar](50),
                       @HohExpenses [bit],
                       @HohShareLifeline [bit],
                       @HohShareLifelineNames [nvarchar](500),
                       @HohAgreeMultiHouse [bit],
                       @HohAgreeViolation [bit],
                       @Signature [nvarchar](max),
                       @SignatureType [nvarchar](max),
                       @SigFileName [nvarchar](max),
                       @HasDevice [bit],
                       @CarrierId [nvarchar](max),
                       @DeviceId [nvarchar](128),
                       @DeviceIdentifier [nvarchar](50),
                       @SimIdentifier [nvarchar](50),
                       @PlanId [nvarchar](128),
                       @TpivBypass [bit],
                       @TpivBypassSignature [nvarchar](100),
                       @TpivBypassSsnProofTypeId [nvarchar](max),
                       @TpivBypassSsnCardLastFour [nvarchar](4),
                       @TpivBypassDobProofTypeId [nvarchar](max),
                       @TpivBypassDobCardLastFour [nvarchar](4),
                       @TpivCode [nvarchar](max),
                       @TpivBypassMessage [nvarchar](max),
                       @LatitudeCoordinate [real],
                       @LongitudeCoordinate [real],
                       @PaymentType [nvarchar](10),
                       @CreditCardReference [nvarchar](100),
                       @CreditCardSuccess [bit],
                       @CreditCardTransactionId [nvarchar](100),
                       @LifelineEnrollmentId [nvarchar](50),
                       @LifelineEnrollmentType [nvarchar](50),
                       @AIInitials [nvarchar](max),
                       @AIFrequency [nvarchar](max),
                       @AIAvgIncome [int],
                       @AINumHousehold [int],
                    @FulfillmentType [varchar](20),
                    @DeviceModel [varchar](50),
                    @PricePlan [float],
                    @PriceTotal [float],
                       @TenantReferenceId [nvarchar](max),
                       @TenantAccountId [nvarchar](max),
                       @TenantAddressId [nvarchar](max),
                       @DateCreated [datetime],
                       @DateModified [datetime],
                       @IsDeleted [bit],
                       @Passphrase [nvarchar](max),
                    @ExternalVelocityCheck [nvarchar](max),
                    @TransactionId [nvarchar](max),
                    @ParentOrderId [nvarchar](128),
                    @HohPuertoRicoAgreeViolation [bit],
                    @Gender[nvarchar](6),
                    @ServiceAddressBypassDocumentProofId [nvarchar](128),
                       @ServiceAddressBypassImageFileName [nvarchar](128),
                    @ServiceAddressBypassSignature[nvarchar](100),
                       @ServiceAddressBypass[bit],
                    @TpivBypassSsnImageFileName[nvarchar](100),
                       @Initials [nvarchar] (max),
                       @InitialsFileName [nvarchar](75),
                       @Language [nvarchar] (2),
                       @CommunicationPreference [nvarchar](50)

                   AS
                   BEGIN
                       INSERT [dbo].[Orders]([Id], [CompanyId], [SalesTeamId], [UserId], [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits], [QBFirstName], 
                       [QBLastName], [QBSsn], [QBDateOfBirth], [CurrentLifelinePhoneNumber], [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageFileName], [StateProgramId], [StateProgramNumber], 
                       [SecondaryStateProgramId], [SecondaryStateProgramNumber], [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber], [IPProofTypeId], 
                       [IPImageFileName], [ServiceAddressStreet1], [ServiceAddressStreet2], [ServiceAddressCity], [ServiceAddressState], [ServiceAddressZip], [ServiceAddressIsPermanent], 
                       [ServiceAddressIsRural], [BillingAddressStreet1], [BillingAddressStreet2], [BillingAddressCity], [BillingAddressState], [BillingAddressZip], [ShippingAddressStreet1], 
                       [ShippingAddressStreet2], [ShippingAddressCity], [ShippingAddressState], [ShippingAddressZip], [HohSpouse], [HohAdultsParent], [HohAdultsChild], [HohAdultsRelative], 
                       [HohAdultsRoommate], [HohAdultsOther], [HohAdultsOtherText], [HohExpenses], [HohShareLifeline], [HohShareLifelineNames], [HohAgreeMultiHouse], [HohAgreeViolation], 
                       [Signature], [SignatureType], [SigFileName], [HasDevice], [CarrierId], [DeviceId], [DeviceIdentifier], [SimIdentifier], [PlanId], [TpivBypass], [TpivBypassSignature], [TpivBypassSsnProofTypeId], 
                       [TpivBypassSsnCardLastFour], [TpivBypassDobProofTypeId], [TpivBypassDobCardLastFour], [TpivCode], [TpivBypassMessage], [LatitudeCoordinate], [LongitudeCoordinate], 
                       [PaymentType], [CreditCardReference], [CreditCardSuccess], [CreditCardTransactionId], [LifelineEnrollmentId], [LifelineEnrollmentType], 
                       [AIInitials], [AIFrequency], [AIAvgIncome], [AINumHousehold], [FulfillmentType], [DeviceModel], [PricePlan], [PriceTotal], [TenantReferenceId], [TenantAccountId], [TenantAddressId],
                     [DateCreated], [DateModified], [IsDeleted],[ExternalVelocityCheck],[TransactionId],[ParentOrderId],[HohPuertoRicoAgreeViolation],[Gender],
                      [ServiceAddressBypassDocumentProofId],[ServiceAddressBypassImageFileName],[ServiceAddressBypassSignature],[ServiceAddressBypass],
                      [TpivBypassSsnImageFileName],[Initials],[InitialsFileName],[Language],[CommunicationPreference]) 

                       VALUES (@Id, @CompanyId, @SalesTeamId, @UserId, @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits, @QBFirstName, @QBLastName, 
                       ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbSsn), 
                       ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbDob), 
                       @CurrentLifelinePhoneNumber, 
                       @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPImageFileName, @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber, @FirstName, @MiddleInitial, @LastName, 
                       ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedSsn), 
                       ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedDob), 
                       @EmailAddress, @ContactPhoneNumber, @IPProofTypeId, @IPImageFileName, @ServiceAddressStreet1, @ServiceAddressStreet2, @ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent, 
                       @ServiceAddressIsRural, @BillingAddressStreet1, @BillingAddressStreet2, @BillingAddressCity, @BillingAddressState, @BillingAddressZip, @ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, 
                       @ShippingAddressState, @ShippingAddressZip, @HohSpouse, @HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate, @HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames, 
                       @HohAgreeMultiHouse, @HohAgreeViolation, @Signature, @SignatureType, @SigFileName, @HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier, @PlanId, @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnCardLastFour, 
                       @TpivBypassDobProofTypeId, @TpivBypassDobCardLastFour, @TpivCode, @TpivBypassMessage, @LatitudeCoordinate, @LongitudeCoordinate, @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId, 
                       @LifelineEnrollmentId, @LifelineEnrollmentType, @AIInitials, @AIFrequency, @AIAvgIncome, @AINumHousehold, @FulfillmentType, @DeviceModel, @PricePlan, @PriceTotal,
                    @TenantReferenceId, @TenantAccountId, @TenantAddressId,@DateCreated, @DateModified, @IsDeleted,@ExternalVelocityCheck,@TransactionId,@ParentOrderId,@HohPuertoRicoAgreeViolation,@Gender,@ServiceAddressBypassDocumentProofId,@ServiceAddressBypassImageFileName,@ServiceAddressBypassSignature,@ServiceAddressBypass,@TpivBypassSsnImageFileName,@Initials,@InitialsFileName,@Language,@CommunicationPreference) 

                       SELECT * FROM Orders WHERE Id = @Id 
                                   END
                                       ");
            //       //Base Income Levels
            this.Sql(@"INSERT INTO [dbo].[BaseIncomeLevels] ([Id], [StateCode], [Base1Person], [Base2Person], [Base3Person], [Base4Person], [Base5Person], [Base6Person], [Base7Person], [Base8Person], [BaseAdditional], [DateActive], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'CA', '25500', '29700', '35900', '42100', '48300', '54500', '60700', '66900', '6200', GETDATE(), GETDATE(), GETDATE(), 0)
                   ,(NEWID(),'AL','11770','15930','20090','24250','28410','32570','36730','40890','4160',GETDATE(),GETDATE(),GETDATE(),0)
                   , (NEWID(), 'AK', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'AZ', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'AR', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'CO', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'CT', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DE', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'FL', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'GA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'HI', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'ID', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'IL', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'IN', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'IA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'KS', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'KY', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'LA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'ME', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MD', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MI', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MN', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MS', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MO', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'MT', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NE', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NV', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NH', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NJ', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NM', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NY', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'NC', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'ND', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'OH', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'OK', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'OR', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'PA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'RI', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SC', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SD', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'TN', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'TX', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'UT', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'VT', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'VA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'WA', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'WV', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'WI', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'WY', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'PR', '11770', '15930', '20090', '24250', '28410', '32570', '36730', '40890', '4160', GETDATE(), GETDATE(), GETDATE(), 0)");


            ////Compliance Statements

            this.Sql(@"
                           INSERT INTO [dbo].[ComplianceStatements] ([Id], [CompanyId], [StateCode], [Statement], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'CA', 'Does the customer certify that they or someone else in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons) receives the California LifeLine discount for wireless or home phone services?<br><br>The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'AL', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'AK', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'AZ', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'AR', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'CO', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'CT', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'DE', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'FL', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'GA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'HI', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'ID', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'IL', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'IN', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'IA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'KS', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'KY', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'LA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'ME', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MD', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MI', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MN', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MS', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MO', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'MT', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NE', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NV', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NH', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NJ', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NM', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NY', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'NC', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'ND', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'OH', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'OK', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'OR', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'PA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'RI', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'SC', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'SD', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'TN', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'TX', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'UT', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'VT', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'VA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'WA', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'WV', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'WI', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'WY', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), '65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c', 'PR', 'Is the customer, or anyone in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated persons), currently receiving Lifeline benefits for wireless or home phone services?  The leading providers of Lifeline service are listed below.', GETDATE(), GETDATE(), 0)
                           ");

            //Life Line Programs

            this.Sql(@"INSERT INTO [dbo].[LifelinePrograms] ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES (NEWID(), 'Annual Income', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'AL', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'AL', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'AL', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'AL', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'AL', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'AL', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'AR', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'AR', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'AR', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'AR', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'AR', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'AR', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'AZ', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'AZ', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Food Stamps', 'AZ', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'AZ', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'AZ', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'AZ', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'AZ', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Telephone Assistance Program for the Medically Needy', 'AZ', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'AZ', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Annual Income', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Medicaid/Medi-Cal', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, '')
                       , (NEWID(), 'Supplemental Nutrition Assistance Program (SNAP)/Cal-Fresh', 'CA', 1, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, '')
                       , (NEWID(), 'Temporary Assistance for Needy Families (TANF)', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'CalWORKS', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'StanWORKS', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Welfare to Work', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'GAIN', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Head Start Program (income based only)', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, '')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'CA', 0, null, null, GETDATE(), GETDATE(), 0, 'E11')
                       , (NEWID(), 'Women, Infants and Children Program ', 'CA', 1, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Annual Income', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, '')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'CO', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'CO', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'CO', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'CO', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'CO', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'CO', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'GA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'GA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'GA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'GA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Senior Citizen Low-Income Discount Plan', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'GA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'GA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'HI', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'HI', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'HI', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'HI', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'HI', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'HI', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'IA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'IA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'IA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'IA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'IA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'IA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'ID', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'ID', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'ID', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'ID', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'ID', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'ID', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'IN', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'IN', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'IN', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'IN', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'IN', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'IN', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'General Public Assistance', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Medicaid', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'KS', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally Administered Free Lunch Program (income based only)', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'KS', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Annual Income', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'KY', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'KY', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'KY', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'KY', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'KY', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'KY', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'LA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'LA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'LA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'LA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'LA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'LA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Emergency Aid to the Elderly, Disabled and Children', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'MassHealth', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'Medicaid', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'National School Lunch (free program only)', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Transitional Aid to Families with Dependent Children', 'MA', 1, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Tribally Administered Free Lunch Program (income based only)', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'MA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Annual Income', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Electrical Universal Service Program', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Maryland Energy Assistance Program', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'Medicaid', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Medical Assistance', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'National School Lunch (free program only)', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Public Assistance to Adults', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Nutrition Assistance Program', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Security Income', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Temporary Cash Assistance', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Temporary Disability Assistance Program', 'MD', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'MD', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Annual Income', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Emergency Assistance Program', 'ME', 1, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Home Energy Assistance Program', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Maine Care', 'ME', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'Medicaid', 'ME', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'National School Lunch (free program only)', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Nutrition Assistance Program', 'ME', 1, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'ME', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'ME', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'MI', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'MI', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'MI', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'MI', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'MI', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'MI', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Food Stamps', 'MN', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'MN', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'Minnesota Family Investment Program', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'National School Lunch (free program only)', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Security Income', 'MN', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'MN', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Annual Income', 'MO', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'MO', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Food Stamps', 'MO', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'MO', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'MO HealthNet', 'MO', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'MO', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Security Income', 'MO', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'MO', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Annual Income', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'ND', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'ND', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'ND', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'ND', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'ND', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'ND', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'NE', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'NE', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'NE', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'NE', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Annual Income', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'NV', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'NV', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'NV', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'NV', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'NV', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'NV', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Food Stamps', 'OH', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'General/Disability Assistance', 'OH', 1, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Medicaid', 'OH', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'SSI-Blind and Disabled', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'OH', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'OH', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Aid to Families with Dependent Children', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Certified by the OK Tax Commission to participate in or receive assistance or benefits pursuant to the Sales Tax Relief Act, 68 O.S. § 5011 et seq', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Certified program by the State Dept. of Rehabilitation, providing vocational rehabilitation assistance or benefits, including hearing impaired', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Head Start Programs (under income qualifying eligibility provision only)', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Medicaid', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'OK', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'OK', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'PA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'PA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'PA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'PA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'PA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'PA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Medicaid', 'PR', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Puerto Rico Nutritional Assistance Program (PAN)', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'PR', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'PR', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Annual Income', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Family Independence Program', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'General Public Assistance', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Medicaid', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Rhode Island Medical Assistance Program', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Rhode Island Pharmaceutical Assistance to Elderly', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'RI Works', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Nutrition Assistance Program', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Security Income', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'RI', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'RI', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'SC', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'SC', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'SC', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'SC', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'SC', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'SC', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'SD', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'SD', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'SD', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'SD', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'SD', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'SD', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'TX', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'TX', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'TX', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'TX', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'TX', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'TX', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'UT', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'UT', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'UT', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'UT', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'UT', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Head Start Program (income based only)', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'UT', 0, null, null, GETDATE(), GETDATE(), 0, 'E11')
                       , (NEWID(), 'Annual Income', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'VT', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'VT', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'VT', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'VT', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'VT', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'VT', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Annual Income', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'WA', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'WA', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'WA', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'WA', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'WA', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Head Start Program (income based only)', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'WA', 0, null, null, GETDATE(), GETDATE(), 0, 'E11')
                       , (NEWID(), 'Annual Income', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'BadgerCare', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'Medical Assistance', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'National School Lunch (free program only)', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Supplemental Security Income', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'WI Homestead Tax Credit (Schedule H)', 'WI', 0, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Wisconsin Works (W2)', 'WI', 1, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Annual Income', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Emergency Assistance', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'School Clothing Allowance', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Supplemental Security Income', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'WV', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       , (NEWID(), 'WV Childrens Health Insurance Program (WV CHIP)', 'WV', 1, null, null, GETDATE(), GETDATE(), 0, 'E9')
                       , (NEWID(), 'Annual Income', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E12')
                       , (NEWID(), 'Bureau of Indian Affairs General Assistance', 'WY', 1, null, null, GETDATE(), GETDATE(), 0, 'E13')
                       , (NEWID(), 'Federal Housing Assistance (Section 8)', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E8')
                       , (NEWID(), 'Food Distribution Program on Indian Reservations', 'WY', 1, null, null, GETDATE(), GETDATE(), 0, 'E4')
                       , (NEWID(), 'Food Stamps', 'WY', 1, null, null, GETDATE(), GETDATE(), 0, 'E10')
                       , (NEWID(), 'Low Income Home Energy Assistance Program', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E2')
                       , (NEWID(), 'Medicaid', 'WY', 1, null, null, GETDATE(), GETDATE(), 0, 'E5')
                       , (NEWID(), 'National School Lunch (free program only)', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E1')
                       , (NEWID(), 'Supplemental Security Income', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E7')
                       , (NEWID(), 'Temporary Assistance for Needy Families', 'WY', 1, null, null, GETDATE(), GETDATE(), 0, 'E3')
                       , (NEWID(), 'Tribally-Administered Temporary Assistance for Needy Families', 'WY', 0, null, null, GETDATE(), GETDATE(), 0, 'E6')
                       ");
            this.Sql(@"INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'AR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Arizona Low-Income Telephone Assistance Program /Telephone Assistance Program for the Medically Needy (ALITAP) award letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'State Children’s Health Insurance Plan (SCHIP) or KidsCare (Medicaid)', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'AZ', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', '1099 Distribution', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bank Statement with SSI direct deposit', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Certificate of Eligbility for Exchange Vistor status (only if Section 5 is completed)', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'General/Disability Assistance award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 consecutive paycheck stubs', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid/Medi-Cal card/awards letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Passport to Services (CAL-Fresh and CAL-Works Programs only)', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SNAP/Cal-Fresh card/letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'CalWORKS award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'StanWORKS award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Welfare to Work award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'GAIN award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribal Head Start Programs (under income qualifying eligibility provision only) award letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Women, Infants and Children Program card/awards letter/voucher', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Worker''s Compensation Appeals Board Award Letter', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'CO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Utility Bill with Senior Discount / Senior Citizen Low-income Discount plan letter', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'GA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 consecutive months paycheck stubs', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'HI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'IA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'ID', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'IN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'General Public Assistance (GPA) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Kansas Lifeline Service Program (KLSP) Database', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally Administered Heard Start Program award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'KS', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'KY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'LA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', ' Disabled and Children (EAEDC) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Emergency Aid to the Elderly', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'MassHealth card/award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Transitional Aid to Families with Dependent Children (TAFDC) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally Administered Heard Start Program award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'MA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Electrical Universal Service Program (EUSP) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Maryland Energy Assistance Program (MEAP) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medical Assistance (MA) card/award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Public Assistance to Adults (PAA) awards letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tel-Life Database', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Cash Assistance (TCA) awards letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Disability Assistance Program (TDAP) awards letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'MD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Emergency Assistance Program award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Home Energy Assistance Program (HEAP) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Maine Care award letter/card', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'ME', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 consecutive paycheck stubs', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Michigan Lifeline Eligibility Database', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'MI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Minnesota Family Investment Program (MFIP) award letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'MN', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'MO Health', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'MO', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'ND', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'E-MAC award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Kids Connection card/award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'MAC card/award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SAM card/award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'NE', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Department of Health and Human Services DSHS Database', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'NV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'General/Disability Assistance award letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI - Blind and Disabled (SSDI) award letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'OH', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Aid to Families with Dependent Children award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Certified by the OK Tax Commission to participate in or receive assistance or benefits pursuant to the Sales Tax Relief Act, 68 O.S. Â§ 5011 et seq.', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Certified program by the State Dept. of Rehabilitation, providing vocational rehabilitation assistance or benefits, including hearing impaired)', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Head Start Programs (under income qualifying eligibility provision only) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'OK', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Pennsylvania DPW Lifeline Database', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'PA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Puerto Rico Nutritional Assistance Program(PAN) award letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'PR', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Family Independence Program (FIP) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'General Public Assistance (GPA) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Rhode Island Medical Assistance Program (MAP) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Rhode Island Pharmaceutical Assistance to Elderly (RIPAE) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Rhode Island Works (Formerly known as AFDC) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'RI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'SC', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'SD', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Lite-UP Texas Eligibility System Database (Solix)', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program Benfits Letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Texas Affidavit for Indigent Consumer', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Texas Annual Income Affidavit For Individuals Exiting a Texas Correctional Facility', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'TX', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Head Start (Income based qualification only) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'UT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Vermont Department for Children and Families Database', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'VT', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'BVS/WTAP Database', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally Administered Heard Start Program award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'WA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'BadgerCare card/award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medical Assistance (MA) card/award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'WI Homestead Tax Credit (Schedule H) award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Wisconsin Cares Database', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Wisconsin Works (W2) statement/award letter', 'WI', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Emergency Assistance Program award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'School Clothing Allowance (SCA) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'WV Children''s Health Insurance Program (WV CHIP) card/award letter', 'WV', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Bureau of Indian Affairs General Assistance (BIA) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Divorce decree or child support document', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Federal Housing Assistance/Section 8 Housing Awards Letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food Distribution Program on Indian Reservations (FDPIR) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Food stamp/SNAP card/letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Last 3 months of consecutive paycheck stubs', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Low Income Home Energy Assistance Program (LIHEAP) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Medicaid card/letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'National Administered Free Lunch Program', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Prior year''s state, federal or tribal tax return (W2)', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Retirement/pension benefit statement', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Social Security benefits statement', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'SSI (Supplement Security Income) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Temporary Assistance to Needy Families (TANF) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Tribally-Administered Temporary Assistance for Needy Families (TTANF) award letter', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Unemployment/Workers Compensation benefits statement', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'program', 'Veterans Administration benefits statement', 'WY', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Unexpired United States Driver''s License', '', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Unexpired UNITED STATES Government, Military, State, or Tribal Issued ID', '', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Unexpired Permanent Resident Card and/or Unexpired Permanent Resident Alien Card', '', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Unexpired United States Passport', '', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Prison Issued Identification Cards', '', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'U.S. Driver License', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Certificate of U.S. Citizenship', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Certificate of Naturalization or Citizenship', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'U.S. Passport/U.S. Territory Passport', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Foreign Passport', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'U.S. government, military, state of Tribal issued-ID, which includes DOB and/or SSN and/or Tribal ID', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'U.S. Military ID cards (active or reserve duty, dependent of a military member, retired member, discharged from service, medical/religious personnel)', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Common Access Card (only if designated as Active Military or Active Reserve or Active Selected Reserve)', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Military Discharge documentation, which includes DOB and/or SSN and/or Tribal ID', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Northern Mariana Card', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Permanent Resident Card', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Permanent Resident Alien Card', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Permanent Resident Re-Entry Permit', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Temporary Resident Identification Card', 'CA', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'id', 'Employment Authorization Card', 'CA', GETDATE(), GETDATE(), 0)");

            ////StateAgreements
            this.Sql(@"INSERT INTO [dbo].[StateAgreements] ([Id], [StateCode], [Agreement], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'AL', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'The Lifeline program is a government benefit program and that willfully making false statements to obtain the benefit can result in fines, imprisonment, de-enrollment or being barred from the program. I meet the eligibility criteria for the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Only one Lifeline service is available per household. A household is defined, for purposes of the Lifeline program, as any individual or group of individuals who live together at the same address and share income and expenses. My household will receive only one Lifeline service and, to the best of my knowledge, my household is not already receiving a Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'A household is not permitted to receive Lifeline benefits from multiple providers. Violation of the one-per-household limitation constitutes a violation of rules and will result in the subscriber''s de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Lifeline is a non-transferable benefit and the subscriber may not transfer his or her benefit to any other person.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.     I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'I consent to providing my name, telephone number and address to the Universal Service Administrative Company for the purpose of verifying I do not receive more than one Lifeline benefit. I also consent to sharing my account information with the Federal Communications Commission and Missouri Public Service Commission who oversee and administer the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'I acknowledge the obligation to re-certify my continued eligibility for Lifeline benefits at any time and failure to re-certify my continued eligibility will result in de-enrollment and the termination of Lifeline benefits. I will provide notification to my voice service provider within 30 days if for any reasons I no longer satisfy the criteria for receiving Lifeline including, as relevant, if I no longer meet the income-based or program-based criteria for receiving Lifeline support, I receive more than one Lifeline benefit, or another member of my household is receiving a Lifeline benefit.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'If I move to a new address I will provide that new address to my voice service provider within 30 days. If I have a temporary residential address then I will be required to verify my address with my voice service provider every 90 days.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'I will be de-enrolled from the Lifeline program and my service deactivated if my service fails to be used for a 60-day time period. Using the service includes completion of an outbound call, purchase of additional usage, or answering an incoming call from a party not affiliated with this company. I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', '                Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment and also that you shall be responsible to re-pay the benefits received from the Lifeline program?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I authorize DSHS to disclose or give access to confidential information about me for 90 days from the date of this application  for the purpose of determining my eligibility for LifeLine assistance.  I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company’s 60-day non-usage reminder. I authorize the Budget Prepay to verify my eligibility for the Lifeline assistance through the DSHS Benefit Verification System for 90 days from the date of this application.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'I give permission to the Department of Health Services to verify to Budget PrePay Inc. whether I participate in a low-income assistance program that would let me qualify for Lifeline. Budget PrePay Inc. shall maintain the information in this form and any information received about me from the Department as confidential customer account information.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I understand that Lifeline is a federal government benefit program and that only qualified persons may participate in the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', '<b>I understand that Lifeline is only available for one phone line per household, whether landline or wireless.</b> Other Lifeline Providers Include: %%COMPETITORS%%. To the best of my knowledge no one in my household is receiving Lifeline service. A household is defined, for purpose of the Lifeline program, as any individuals who live together at the same address and share income and expenses.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I certify that I am at least 18 years of age and not currently receiving a Lifeline telephone service from any other landline or wireless telephone company. I will only receive Lifeline from Budget PrePay and no other landline or wireless telephone company. Any violation of the one phone line per household limitation will result in de-enrollment from the Lifeline program and may be punished by fine or imprisonment.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I will not transfer my service to any other individual, including another eligible low-income consumer.    I acknowledge that if eligible for a transfer of benefits, from my existing wireless or wirleline carrier, and with my consent of the benefit transfer, I will lose my Lifeline Program benefit with my existing wireless or wireline carrier and it will be transferred to Budget Mobile.    I acknowledge that Budget Mobile has explained to me that I may not have multiple Lifeline Program benefits with the same or different wireless or wireline carriers.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I authorize Budget PrePay to access any records required to verify my eligibility for Lifeline service. I also authorize Budget PrePay to release any of my records required for the administration of the Lifeline program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I understand that I will be required to verify my continued eligibility for Budget PrePay''s Lifeline service at least annually, and that I may be required to verify my continued eligibility at anytime, and that failure to do so will result in termination of Lifeline benefits. I will notify Budget PrePay immediately if I no longer qualify for Lifeline, or if I have a question as to whether I would still qualify.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I will notify Budget PrePay within thirty (30) days if my home address changes. If the address I have provided is a temporary address, I understand that I must verify my address every ninety (90) days. Failure to provide such notification or verification may result in de-enrollment from the program.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I authorize Budget PrePay to contact me by interactive voice response (IVR), or other means, to notify me of annual Lifeline re-verification and the company''s 60-day non-usage reminder.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'I understand that completion of this application does not constitute immediate approval for Lifeline service.', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'Do you understand that the Lifeline benefit is available only one per household and that a household is defined as any individual or group of individuals who are living together at the same address as one economic unit?', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'Do you acknowledge under penalty of perjury, that all the information you have provided is accurate and true and that providing false or fraudulent information to receive a Lifeline benefit is punishable by law up to and including imprisonment?', GETDATE(), GETDATE(), 0)
                       ");
            this.Sql(@"INSERT INTO [dbo].[StatePrograms] ([Id], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'DSHS', 'WA', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DCN', 'MO', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'Control', 'PR', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'Certificate', 'PR', GETDATE(), GETDATE(), 0)");

            this.Sql(@"INSERT INTO [dbo].[StateSettings] ([Id], [StateCode], [SsnType], [IncomeLevel], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'NE', 'Full', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PA', 'Full', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'PR', 'Full', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'UT', 'Full', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WA', 'Full', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AZ', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CA', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KS', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MI', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OH', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TX', 'Last4', '150', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NV', 'Last4', '175', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AL', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AK', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'AR', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CO', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'CT', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'DE', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'FL', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'GA', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'HI', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ID', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IL', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IN', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'IA', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'KY', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'LA', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ME', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MD', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MA', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MN', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MS', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MO', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'MT', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NH', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NJ', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NM', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NY', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'NC', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'ND', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OK', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'OR', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'RI', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SC', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'SD', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'TN', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'VT', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'VA', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WV', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WI', 'Last4', '135', GETDATE(), GETDATE(), 0)
                       , (NEWID(), 'WY', 'Last4', '135', GETDATE(), GETDATE(), 0)");

            this.Sql(@"INSERT INTO [dbo].[TpivProofDocumentTypes] ([Id], [Type], [Name], [LifelineSystem], [LifelineSystemId], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), 'SSN', 'W-2', 'NLAD', 'T3', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Government Assistance Program Document which includes DOB', 'NLAD', 'T14', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Military Discharge Documentation which includes DOB', 'NLAD', 'T12', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Prior Years State, Federal or Tribal Tax Return', 'NLAD', 'T4', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Social Security Card or SSA-1099 (Social Security Benefit Statement)', 'NLAD', 'T5', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Statement of Benefits from a Qualifying Program which includes DOB', 'NLAD', 'T15', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Unemployment/Workers Compensation Statement which includes DOB', 'NLAD', 'T16', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Unexpired US Government, Military, State, or Tribal Issued ID which includes DOB', 'NLAD', 'T8', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'SSN', 'Unexpired Weapons Permit which includes DOB', 'NLAD', 'T13', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Birth Certificate', 'NLAD', 'T2', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Certificate of Naturalization or Certificate of U.S. Citizenship', 'NLAD', 'T6', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unexpired Drivers License', 'NLAD', 'T1', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Government Assistance Program Document which includes DOB', 'NLAD', 'T14', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Military Discharge Documentation which includes DOB', 'NLAD', 'T12', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unexpired Passport', 'NLAD', 'T10', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unexpired Permanent Resident Card or Permanent Resident Alien Card', 'NLAD', 'T7', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Statement of Benefits from a Qualifying Program which includes SSN', 'NLAD', 'T15', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unemployment/Workers Compensation Statement which includes SSN', 'NLAD', 'T16', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unexpired US Government, Military, State, or Tribal Issued ID which includes DOB', 'NLAD', 'T8', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'DOB', 'Unexpired Weapons Permit which includes SSN', 'NLAD', 'T13', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Birth Certificate', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Certificate of Naturalization or Certificate of U.S. Citizenship', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Current Income Statement from and Employer, Paycheck Stub, or W-2', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Drivers License', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Government Assistance Program Document which includes name and date of birth', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Military Discharge Documentation', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Passport', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Permanent Resident Card or Permanent Resident Alien Card', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Prior Years State, Federal or Tribal Tax Return', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Social Security Card', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Statement of Benefits from a Qualifying Program which contains name and date of birth', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Unemployment/Workers Compensation Statement', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'US Government, Military, State, or Tribal Issued ID', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Weapons Permit', 'DAP', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Birth Certificate', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Certificate of Naturalization or Certificate of U.S. Citizenship', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Current Income Statement from and Employer, Paycheck Stub, or W-2', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Drivers License', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Government Assistance Program Document which includes name and date of birth', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Military Discharge Documentation', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Passport', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Permanent Resident Card or Permanent Resident Alien Card', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Prior Years State, Federal or Tribal Tax Return', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Social Security Card', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Statement of Benefits from a Qualifying Program which contains name and date of birth', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Unemployment/Workers Compensation Statement', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'US Government, Military, State, or Tribal Issued ID', 'TEXAS', '', GETDATE(), GETDATE(), 0)
                   , (NEWID(), 'All', 'Weapons Permit', 'TEXAS', '', GETDATE(), GETDATE(), 0)");

            this.Sql(@"CREATE TABLE [dbo].[Budget_LifelinePrograms](
                         [ProgramName] [varchar](250) NOT NULL,
                         [BudgetCodeID] [int] NOT NULL
                            ) ON [PRIMARY]");

            //insert data into Budget_LifelinePrograms
            this.Sql(@"
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Aid to Families with Dependent Children', 47)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Annual Income', 6)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'BadgerCare', 30)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Bureau of Indian Affairs General Assistance', 28)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Certified by the OK Tax Commission to participate in or receive assistance or benefits pursuant to the Sales Tax Relief Act, 68 O.S. § 5011 et seq', 46)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Certified program by the State Dept. of Rehabilitation, providing vocational rehabilitation assistance or benefits, including hearing impaired', 45)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Electrical Universal Service Program', 22)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Emergency Aid to the Elderly, Disabled and Children', 64)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Emergency Assistance', 42)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Emergency Assistance Program', 55)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Family Independence Program', 25)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Federal Housing Assistance (Section 8)', 3)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Food Distribution Program on Indian Reservations', 27)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Food Stamps', 2)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'General Public Assistance', 23)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'General/Disability Assistance', 56)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Head Start Programs (under income qualifying eligibility provision only)', 48)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Home Energy Assistance Program', 54)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Low Income Home Energy Assistance Program', 8)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Maine Care', 53)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Maryland Energy Assistance Program', 21)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'MassHealth', 66)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Medicaid', 1)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Medicaid/Medi-Cal', 1)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Medical Assistance', 16)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Minnesota Family Investment Program', 63)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'MO HealthNet', 67)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'National School Lunch (free program only)', 5)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Public Assistance to Adults', 19)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Puerto Rico Nutritional Assistance Program (PAN)', 68)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Rhode Island Medical Assistance Program', 26)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Rhode Island Pharmaceutical Assistance to Elderly', 24)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'RI Works', 33)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'School Clothing Allowance', 44)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Senior Citizen Low-Income Discount Plan', 58)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'SSI-Blind and Disabled', 57)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Supplemental Nutrition Assistance Program', 17)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Supplemental Nutrition Assistance Program (SNAP)/Cal-Fresh', 17)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Supplemental Security Income', 7)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Telephone Assistance Program for the Medically Needy', 62)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Temporary Assistance for Needy Families', 4)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Temporary Cash Assistance', 18)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Temporary Disability Assistance Program', 20)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Transitional Aid to Families with Dependent Children', 65)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Tribally Administered Free Lunch Program (income based only)', 41)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Tribally-Administered Head Start Program (income based only)', 40)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Tribally-Administered Temporary Assistance for Needy Families', 29)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'WI Homestead Tax Credit (Schedule H)', 31)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Wisconsin Works (W2)', 32)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Women, Infants and Children Program ', 69)
                       
                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'WV Childrens Health Insurance Program (WV CHIP)', 43)

                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Temporary Assistance for Needy Families (TANF)', 4)

                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'CalWORKS', 4)

                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'StanWORKS', 4)

                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'Welfare to Work', 4)

                       INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N'GAIN', 4)
                       
                       ");

            this.Sql(@"
                    DECLARE @ArrowID VARCHAR(MAX)  
                    SET @ArrowID = (SELECT Id FROM Companies WHERE Name='Arrow');
                    DELETE FROM CompanyTranslations WHERE CompanyID=@ArrowID AND Type='LifelineProgram';

                    INSERT INTO CompanyTranslations (ID, CompanyID, LSID, TranslatedID, [Type],IsDeleted,DateCreated,DateModified)
                    SELECT CONVERT(NVARCHAR(50), NEWID()), @ArrowID AS CompanyID, P.Id AS LSID, BP.BudgetCodeID AS TranslatedID, 'LifelineProgram' AS [Type],0,GETDATE(),GETDATE()
                    FROM LifelinePrograms P
                    LEFT JOIN Budget_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
                    WHERE BP.BudgetCodeID IS NOT NULL AND BP.BudgetCodeID!=''
                ");
                this.Sql(@"INSERT [dbo].[Level1SalesGroup] ([CompanyId], [Name],[CreatedByUserId],[DateCreated],[DateModified],[Id],[IsDeleted]) VALUES ('65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c','Level1','b0d345d3-cfba-4ff4-89d2-5952c65652f9',GETDATE(),GETDATE(),'1b707ba4-ed6f-4667-9510-94b63cff1bb2',0)");

                this.Sql(@"INSERT [dbo].[ExternalStorageCredentials] ([Id], [AccessKey],[SecretKey],[Type],[System],[Path],[MaxImageSize],[CompanyId],[DateCreated],[DateModified],[IsDeleted]) VALUES ('fa8ec697-0728-4d00-bb52-903f8c87d57f','AKIAJCRW4XAU3QCZO7NA','3BULiY8RiPyFGWJZ3+gdrOT4k+z487n1vdC6+iUQ','Docs','AWS','lifeserv.dev.budgetmobile.docs',640,'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GETDATE(),GETDATE(),0)

                INSERT[dbo].[ExternalStorageCredentials]([Id], [AccessKey],[SecretKey],[Type],[System],[Path],[MaxImageSize],[CompanyId],[DateCreated],[DateModified],[IsDeleted])
                VALUES('289272df-d686-49bc-9bf9-150f7c56dfc','AKIAJMXY2WVIV6AWQTOA','/FaxMKvVV9gQkSlQZ4ANDLk/mmYH4NZnY9xDXFFR','Signatures','AWS','lifeserv.dev.budgetmobile.signatures',640,'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GETDATE(),GETDATE(),0)

                INSERT [dbo].[ExternalStorageCredentials] ([Id], [AccessKey],[SecretKey],[Type],[System],[Path],[MaxImageSize],[CompanyId],[DateCreated],[DateModified],[IsDeleted]) VALUES ('289272df-d686-49bc-9bf9-e150f7c56dfa','AKIAIVOMYVXYLQFREYEA','xdyw7bQ9zXpQgop//itcbKWPARv0IZXSdliQDGQQ','Proof','AWS','lifeserv.dev.budgetmobile.proofs',640,'65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c',GETDATE(),GETDATE(),0)"
                );
                }

        public override void Down() {
            }
        }
    }
