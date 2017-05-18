namespace LS.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceAddressBypassImageID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "ServiceAddressByPassImageID", c => c.String());
            AddColumn("dbo.TempOrders", "ServiceAddressByPassImageID", c => c.String());
            this.Sql("DROP PROCEDURE [dbo].[Temp_Order_Insert]");
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
                    @LPImageID [nvarchar](100),
				    @LPProofImageUploadFilename [nvarchar](100),
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
                    @IPImageID [nvarchar](100),
					@IPProofImageUploadFilename [nvarchar](100),
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
					@ServiceAddressBypassImageID [nvarchar](128),
	                @ServiceAddressBypassSignature[nvarchar](100),
	                @ServiceAddressBypass[bit],
                    @TpivBypassSsnImageId [nvarchar](100),
	                @TpivSsnImageUploadFilename[nvarchar](100),
                    @Initials [nvarchar](max),
                    @Language [nvarchar](2),
                    @CommunicationPreference [nvarchar](50),
					@TpivCode [nvarchar] (128),
					@TpivNasScore [nvarchar](100),
					@TpivTransactionID [nvarchar](128),
					@TpivRiskIndicators [nvarchar](200),
					@AINumHouseAdult [int],
					@AINumHouseChildren [int]
                AS
                BEGIN
                    INSERT[dbo].[TempOrders](
                              [Id], [CompanyId], [SalesTeamId], [UserId]
                              , [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits]
                              , [QBFirstName], [QBLastName], [QBSsn], [QBDateOfBirth]
                              , [CurrentLifelinePhoneNumber]
                              , [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageID],[LPProofImageUploadFilename]
                              , [StateProgramId], [StateProgramNumber], [SecondaryStateProgramId], [SecondaryStateProgramNumber]
                              , [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber]
                              , [IPProofTypeId], [IPImageID],[IPProofImageUploadFilename]
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
                              , [DateCreated], [DateModified], [IsDeleted],[ParentOrderId],[HohPuertoRicoAgreeViolation],[Gender]
							  , [ServiceAddressBypassDocumentProofId],[ServiceAddressBypassImageFileName],[ServiceAddressBypassImageID],[ServiceAddressBypassSignature],[ServiceAddressBypass], [TpivSsnImageUploadFilename],[TpivBypassSsnImageId],[Initials],[Language],[CommunicationPreference]
							  , [TpivCode], [TpivNasScore], [TpivTransactionID], [TpivRiskIndicators],[AINumHouseAdult],[AINumHouseChildren]
							  )
							 
                    VALUES(
                              @Id, @CompanyId, @SalesTeamId, @UserId
                              , @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits
                              , @QBFirstName, @QBLastName, ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbSsn), ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbDob)
                              , @CurrentLifelinePhoneNumber
                              , @LifelineProgramId, @LPProofTypeId, @LPProofNumber,  @LPImageID, @LPProofImageUploadFilename
                              , @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber
                              , @FirstName, @MiddleInitial, @LastName, ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedSsn), ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedDob), @EmailAddress, @ContactPhoneNumber
                              , @IPProofTypeId, @IPImageID, @IPProofImageUploadFilename
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
                              , @DateCreated, @DateModified, @IsDeleted, @ParentOrderId, @HohPuertoRicoAgreeViolation,@Gender,@ServiceAddressBypassDocumentProofId,@ServiceAddressBypassImageFileName, @ServiceAddressBypassImageID,@ServiceAddressBypassSignature,@ServiceAddressBypass,@TpivSsnImageUploadFilename,@TpivBypassSsnImageId,@Initials,@Language,@CommunicationPreference
							  , @TpivCode, @TpivNasScore, @TpivTransactionID,@TpivRiskIndicators, @AINumHouseAdult,@AINumHouseChildren)
                    SELECT* FROM TempOrders WHERE Id = @Id
                END
            ");
            this.Sql("DROP PROCEDURE [dbo].[Order_Insert]");
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
                    @LPImageId [nvarchar](100),
				    @LPProofImageUploadFilename [nvarchar](100),
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
                    @IPImageID [nvarchar](100),
					@IPProofImageUploadFilename [nvarchar](100),
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
					@ServiceAddressBypassImageID [nvarchar](128),
	                @ServiceAddressBypassSignature[nvarchar](100),
                    @ServiceAddressBypass[bit],
					@TpivBypassSsnImageId [nvarchar](100),
	                @TpivSsnImageUploadFilename[nvarchar](100),
                    @Initials [nvarchar] (max),
                    @InitialsFileName [nvarchar](75),
                    @Language [nvarchar] (2),
                    @CommunicationPreference [nvarchar](50),
	                @TpivNasScore [nvarchar](100),
	                @TpivTransactionID [nvarchar](128),
	                @TpivRiskIndicators [nvarchar](200),
                    @RTR_Name [nvarchar](100),
                    @RTR_Date [datetime],
                    @RTR_Notes [nvarchar](300),
					@RTR_RejectCode [nvarchar](50),
	                @AINumHouseAdult [int],
                    @AINumHouseChildren [int],
					@OrderCode [nvarchar](50)
                AS
                BEGIN
                    INSERT [dbo].[Orders]([Id], [CompanyId], [SalesTeamId], [UserId], [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits], [QBFirstName], 
                    [QBLastName], [QBSsn], [QBDateOfBirth], [CurrentLifelinePhoneNumber], [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageID],[LPProofImageUploadFilename], [StateProgramId], [StateProgramNumber], 
                    [SecondaryStateProgramId], [SecondaryStateProgramNumber], [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber], [IPProofTypeId], 
                    [IPImageID],[IPProofImageUploadFilename], [ServiceAddressStreet1], [ServiceAddressStreet2], [ServiceAddressCity], [ServiceAddressState], [ServiceAddressZip], [ServiceAddressIsPermanent], 
                    [ServiceAddressIsRural], [BillingAddressStreet1], [BillingAddressStreet2], [BillingAddressCity], [BillingAddressState], [BillingAddressZip], [ShippingAddressStreet1], 
                    [ShippingAddressStreet2], [ShippingAddressCity], [ShippingAddressState], [ShippingAddressZip], [HohSpouse], [HohAdultsParent], [HohAdultsChild], [HohAdultsRelative], 
                    [HohAdultsRoommate], [HohAdultsOther], [HohAdultsOtherText], [HohExpenses], [HohShareLifeline], [HohShareLifelineNames], [HohAgreeMultiHouse], [HohAgreeViolation], 
                    [Signature], [SignatureType], [SigFileName], [HasDevice], [CarrierId], [DeviceId], [DeviceIdentifier], [SimIdentifier], [PlanId], [TpivBypass], [TpivBypassSignature], [TpivBypassSsnProofTypeId], 
                    [TpivBypassSsnCardLastFour], [TpivBypassDobProofTypeId], [TpivBypassDobCardLastFour], [TpivCode], [TpivBypassMessage], [LatitudeCoordinate], [LongitudeCoordinate], 
                    [PaymentType], [CreditCardReference], [CreditCardSuccess], [CreditCardTransactionId], [LifelineEnrollmentId], [LifelineEnrollmentType], 
                    [AIInitials], [AIFrequency], [AIAvgIncome], [AINumHousehold], [FulfillmentType], [DeviceModel], [PricePlan], [PriceTotal], [TenantReferenceId], [TenantAccountId], [TenantAddressId],
	                [DateCreated], [DateModified], [IsDeleted],[ExternalVelocityCheck],[TransactionId],[ParentOrderId],[HohPuertoRicoAgreeViolation],[Gender],
	                [ServiceAddressBypassDocumentProofId],[ServiceAddressBypassImageFileName],[ServiceAddressBypassImageID],[ServiceAddressBypassSignature],[ServiceAddressBypass],
	                [TpivSsnImageUploadFilename],[TpivBypassSsnImageId],[Initials],[InitialsFileName],[Language],[CommunicationPreference],
	                [TpivNasScore], [TpivTransactionID], [TpivRiskIndicators],[RTR_Name],[RTR_Date],[RTR_Notes],[RTR_RejectCode],[AINumHouseAdult],[AINumHouseChildren],[OrderCode]) 
    
                    VALUES (@Id, @CompanyId, @SalesTeamId, @UserId, @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits, @QBFirstName, @QBLastName, 
                    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbSsn), 
                    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbDob), 
                    @CurrentLifelinePhoneNumber, 
                    @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPImageID, @LPProofImageUploadFilename, @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber, @FirstName, @MiddleInitial, @LastName, 
                    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedSsn), 
                    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedDob), 
                    @EmailAddress, @ContactPhoneNumber, @IPProofTypeId, @IPImageID, @IPProofImageUploadFilename, @ServiceAddressStreet1, @ServiceAddressStreet2, @ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent, 
                    @ServiceAddressIsRural, @BillingAddressStreet1, @BillingAddressStreet2, @BillingAddressCity, @BillingAddressState, @BillingAddressZip, @ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, 
                    @ShippingAddressState, @ShippingAddressZip, @HohSpouse, @HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate, @HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames, 
                    @HohAgreeMultiHouse, @HohAgreeViolation, @Signature, @SignatureType, @SigFileName, @HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier, @PlanId, @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnCardLastFour, 
                    @TpivBypassDobProofTypeId, @TpivBypassDobCardLastFour, @TpivCode, @TpivBypassMessage, @LatitudeCoordinate, @LongitudeCoordinate, @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId, 
                    @LifelineEnrollmentId, @LifelineEnrollmentType, @AIInitials, @AIFrequency, @AIAvgIncome, @AINumHousehold, @FulfillmentType, @DeviceModel, @PricePlan, @PriceTotal,
	                @TenantReferenceId, @TenantAccountId, @TenantAddressId,@DateCreated, @DateModified, @IsDeleted,@ExternalVelocityCheck,@TransactionId,@ParentOrderId,@HohPuertoRicoAgreeViolation,@Gender,@ServiceAddressBypassDocumentProofId,@ServiceAddressBypassImageFileName,@ServiceAddressBypassImageID,@ServiceAddressBypassSignature,@ServiceAddressBypass,@TpivSsnImageUploadFilename,@TpivBypassSsnImageId,@Initials,@InitialsFileName,@Language,@CommunicationPreference,
	                @TpivNasScore, @TpivTransactionID, @TpivRiskIndicators, @RTR_Name, @RTR_Date, @RTR_Notes, @RTR_RejectCode,@AINumHouseAdult,@AINumHouseChildren,@OrderCode) 
                    SELECT * FROM Orders WHERE Id = @Id 
                END
            ");

            this.Sql(@"
                UPDATE Orders SET LPProofImageUploadFilename=LPImageID + '.jpg' WHERE COALESCE(LPProofImageUploadFilename,'')='' AND COALESCE(LPImageID,'')!=''
                UPDATE Orders SET IPProofImageUploadFilename=IPImageID + '.jpg' WHERE COALESCE(IPProofImageUploadFilename,'')='' AND COALESCE(IPImageID,'')!=''
                UPDATE Orders SET TpivSsnImageUploadFileName=TpivBypassSsnImageID + '.jpg' WHERE COALESCE(TpivSsnImageUploadFileName,'')='' AND COALESCE(TpivBypassSsnImageID,'')!=''
                UPDATE Orders SET ServiceAddressByPassImageFileName=ServiceAddressByPassImageID + '.jpg' WHERE COALESCE(ServiceAddressByPassImageFileName,'')='' AND COALESCE(ServiceAddressByPassImageID,'')!=''
                UPDATE Orders SET SigFileName=SigFileName + '.jpg' WHERE SigFileName NOT LIKE '%.jpg' AND SigFileName NOT LIKE '%.png'
                UPDATE Orders SET InitialsFileName=InitialsFileName + '.jpg' WHERE InitialsFileName NOT LIKE '%.jpg' AND InitialsFileName NOT LIKE '%.png'
            ");
        }
        
        public override void Down()
        {
            DropColumn("dbo.TempOrders", "ServiceAddressByPassImageID");
            DropColumn("dbo.Orders", "ServiceAddressByPassImageID");
        }
    }
}
