    INSERT [dbo].[TempOrders]([Id], [CompanyId], [SalesTeamId], [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits], [QBFirstName],
    [QBLastName], [QBSsn], [QBDateOfBirth], [CurrentLifelinePhoneNumber], [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageFileName], [StateProgramId], [StateProgramNumber], 
    [SecondaryStateProgramId], [SecondaryStateProgramNumber], [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber], [IPProofTypeId],
    [IPImageFileName], [ServiceAddressStreet1], [ServiceAddressStreet2], [ServiceAddressCity], [ServiceAddressState], [ServiceAddressZip], [ServiceAddressIsPermanent], 
    [BillingAddressStreet1], [BillingAddressStreet2], [BillingAddressCity], [BillingAddressState], [BillingAddressZip], [ShippingAddressStreet1],
    [ShippingAddressStreet2], [ShippingAddressCity], [ShippingAddressState], [ShippingAddressZip], [HohSpouse], [HohAdultsParent], [HohAdultsChild], [HohAdultsRelative], 
    [HohAdultsRoommate], [HohAdultsOther], [HohAdultsOtherText], [HohExpenses], [HohShareLifeline], [HohShareLifelineNames], [HohAgreeMultiHouse], [HohAgreeViolation], 
    [Signature], [HasDevice], [CarrierId], [DeviceId], [DeviceIdentifier], [SimIdentifier], [PlanId], [TpivBypass], [TpivBypassSignature], [TpivBypassSsnProofTypeId],
    [TpivBypassSsnCardLastFour], [TpivBypassDobProofTypeId], [TpivBypassDobCardLastFour], [LatitudeCoordinate], [LongitudeCoordinate], 
    [PaymentType], [CreditCardReference], [CreditCardSuccess], [CreditCardTransactionId], [LifelineEnrollmentId], [LifelineEnrollmentType],
    [AIInitials], [AIFrequency], [AIAvgIncome], [AINumHousehold], [DateCreated], [DateModified], [IsDeleted])
    
    VALUES (@Id, @CompanyId, @SalesTeamId, @HouseholdReceivesLifelineBenefits, @CustomerReceivesLifelineBenefits, @QBFirstName, @QBLastName, 
    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbSsn),
    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedQbDob),
    @CurrentLifelinePhoneNumber, 
    @LifelineProgramId, @LPProofTypeId, @LPProofNumber, @LPImageFileName, @StateProgramId, @StateProgramNumber, @SecondaryStateProgramId, @SecondaryStateProgramNumber, @FirstName, @MiddleInitial, @LastName,
    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedSsn),
    ENCRYPTBYPASSPHRASE(@Passphrase, @UnencryptedDob),
    @EmailAddress, @ContactPhoneNumber, @IPProofTypeId, @IPImageFileName, @ServiceAddressStreet1, @ServiceAddressStreet2, @ServiceAddressCity, @ServiceAddressState, @ServiceAddressZip, @ServiceAddressIsPermanent,
    @BillingAddressStreet1, @BillingAddressStreet2, @BillingAddressCity, @BillingAddressState, @BillingAddressZip, @ShippingAddressStreet1, @ShippingAddressStreet2, @ShippingAddressCity, 
    @ShippingAddressState, @ShippingAddressZip, @HohSpouse, @HohAdultsParent, @HohAdultsChild, @HohAdultsRelative, @HohAdultsRoommate, @HohAdultsOther, @HohAdultsOtherText, @HohExpenses, @HohShareLifeline, @HohShareLifelineNames,
    @HohAgreeMultiHouse, @HohAgreeViolation, @Signature, @HasDevice, @CarrierId, @DeviceId, @DeviceIdentifier, @SimIdentifier, @PlanId, @TpivBypass, @TpivBypassSignature, @TpivBypassSsnProofTypeId, @TpivBypassSsnCardLastFour,
    @TpivBypassDobProofTypeId, @TpivBypassDobCardLastFour, @LatitudeCoordinate, @LongitudeCoordinate, @PaymentType, @CreditCardReference, @CreditCardSuccess, @CreditCardTransactionId,
    @LifelineEnrollmentId, @LifelineEnrollmentType, @AIInitials, @AIFrequency, @AIAvgIncome, @AINumHousehold, @DateCreated, @DateModified, @IsDeleted)
    
    SELECT * FROM TempOrders WHERE Id = @Id