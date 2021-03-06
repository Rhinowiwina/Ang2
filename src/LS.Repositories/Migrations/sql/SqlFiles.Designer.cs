//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LS.Repositories.Migrations.sql {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SqlFiles {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SqlFiles() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LS.Repositories.Migrations.sql.SqlFiles", typeof(SqlFiles).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE PROCEDURE [dbo].[usp_GetOrders]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@CustomerName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT O.*, U.FirstName AS EmployeeFirstName, U.LastName AS EmployeeLastName, T.ExternalDisplayName AS SalesTeamExternalDisplayName, T.Name AS SalesTeamName
        ///	FROM Orders (NOLOCK) O
        ///		LEFT JOIN AspNetUsers (NOLOCK) U ON U.Id=@UserId
        ///		LEFT JOIN AspN [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _20151020_usp_GetOrdersUpdate {
            get {
                return ResourceManager.GetString("_20151020_usp_GetOrdersUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE PROCEDURE [dbo].[usp_GetOrders]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@CustomerName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT O.*, EU.FirstName AS EmployeeFirstName, EU.LastName AS EmployeeLastName, T.ExternalDisplayName AS SalesTeamExternalDisplayName, T.Name AS SalesTeamName
        ///	FROM Orders (NOLOCK) O
        ///		LEFT JOIN AspNetUsers (NOLOCK) U ON U.Id=@UserId
        ///		LEFT JOIN As [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _20151111_usp_GetOrdersUpdate {
            get {
                return ResourceManager.GetString("_20151111_usp_GetOrdersUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE [dbo].[usp_GetUsers]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@FilterUserName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT U.Id, U.FirstName, U.LastName, U.IsActive,U.UserName, R.Name, R.Rank
        ///	FROM AspNetUsers (NOLOCK) U
        ///		-- Returned user&apos;s info
        ///		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
        ///		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
        ///		
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string _20151111_usp_GetUsersUpdate {
            get {
                return ResourceManager.GetString("_20151111_usp_GetUsersUpdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER PROCEDURE [dbo].[Order_Insert]
        ///    @Id [nvarchar](128),
        ///    @CompanyId [nvarchar](128),
        ///    @SalesTeamId [nvarchar](128),
        ///    @UserId [nvarchar](128),
        ///    @HouseholdReceivesLifelineBenefits [bit],
        ///    @CustomerReceivesLifelineBenefits [bit],
        ///    @QBFirstName [nvarchar](50),
        ///    @QBLastName [nvarchar](50),
        ///    @UnencryptedQbSsn [varchar](max),
        ///    @UnencryptedQbDob [varchar](max),
        ///    @CurrentLifelinePhoneNumber [nvarchar](20),
        ///    @LifelineProgramId [nvarchar](max),
        ///    @LPProofTypeId [nv [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DeviceDetails_Order_Insert {
            get {
                return ResourceManager.GetString("DeviceDetails_Order_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ALTER PROCEDURE [dbo].[Temp_Order_Insert]
        ///    @Id [nvarchar](128),
        ///    @CompanyId [nvarchar](128),
        ///    @SalesTeamId [nvarchar](128),
        ///    @UserId [nvarchar](128),
        ///    @HouseholdReceivesLifelineBenefits [bit],
        ///    @CustomerReceivesLifelineBenefits [bit],
        ///    @QBFirstName [nvarchar](50),
        ///    @QBLastName [nvarchar](50),
        ///    @UnencryptedQbSsn [varchar](max),
        ///    @UnencryptedQbDob [varchar](max),
        ///    @CurrentLifelinePhoneNumber [nvarchar](20),
        ///    @LifelineProgramId [nvarchar](max),
        ///    @LPProofTypeI [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string DeviceDetails_Temp_Order_Insert {
            get {
                return ResourceManager.GetString("DeviceDetails_Temp_Order_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE VIEW [dbo].[v_UserTeams] AS
        ///	SELECT U.Id AS UserID, U.FirstName, U.LastName, T.*
        ///	FROM AspNetUsers (NOLOCK) U
        ///		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
        ///		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
        ///		LEFT JOIN Level1SalesGroupApplicationUser (NOLOCK) G1 ON G1.ApplicationUser_Id=U.Id
        ///		LEFT JOIN Level2SalesGroupApplicationUser (NOLOCK) G2 ON G2.ApplicationUser_Id=U.Id
        ///		LEFT JOIN Level3SalesGroupApplicationUser (NOLOCK) G3 ON G3.ApplicationUser_Id=U.Id
        ///		LEFT JOIN Leve [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initail_vUserTeams {
            get {
                return ResourceManager.GetString("Initail_vUserTeams", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[BaseIncomeLevels] ([Id], [StateCode], [Base1Person], [Base2Person], [Base3Person], [Base4Person], [Base5Person], [Base6Person], [Base7Person], [Base8Person], [BaseAdditional], [DateActive], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;CA&apos;, &apos;25500&apos;, &apos;29700&apos;, &apos;35900&apos;, &apos;42100&apos;, &apos;48300&apos;, &apos;54500&apos;, &apos;60700&apos;, &apos;66900&apos;, &apos;6200&apos;, GETDATE(), GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;AL&apos;, &apos;11770&apos;, &apos;15930&apos;, &apos;20090&apos;, &apos;24250&apos;, &apos;28410&apos;, &apos;32570&apos;, &apos;36730&apos;, &apos;40890&apos;, &apos;4160&apos;, GETDATE(), GETDATE(), [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_BaseIncomeLevels {
            get {
                return ResourceManager.GetString("Initial_BaseIncomeLevels", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE TABLE [dbo].[Budget_LifelinePrograms](
        ///	[ProgramName] [varchar](250) NOT NULL,
        ///	[BudgetCodeID] [int] NOT NULL
        ///) ON [PRIMARY]
        ///
        ///INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N&apos;Aid to Families with Dependent Children&apos;, 47)
        ///INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N&apos;Annual Income&apos;, 6)
        ///INSERT [dbo].[Budget_LifelinePrograms] ([ProgramName], [BudgetCodeID]) VALUES (N&apos;BadgerCare&apos;, 30)
        ///INSERT [dbo].[Budget_LifelinePrograms] ([Pro [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_BudgetLifelinePrograms {
            get {
                return ResourceManager.GetString("Initial_BudgetLifelinePrograms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT [dbo].[Carriers] ([Id], [Name], [NetworkType], [SortOrder], [DateCreated], [DateModified], [IsDeleted]) VALUES (N&apos;3df3e85d-105f-4936-98a7-f9417a87d917&apos;, N&apos;Sprint&apos;, N&apos;CDMA&apos;, N&apos;2&apos;, CAST(N&apos;2015-08-24 00:00:00.000&apos; AS DateTime), CAST(N&apos;2015-08-24 00:00:00.000&apos; AS DateTime), 0)
        ///INSERT [dbo].[Carriers] ([Id], [Name], [NetworkType], [SortOrder], [DateCreated], [DateModified], [IsDeleted]) VALUES (N&apos;63be6e98-4ea5-4fb7-81e3-97ea150e1d01&apos;, N&apos;Verizon&apos;,N&apos;CDMA&apos;, N&apos;1&apos;,  CAST(N&apos;2015-08-24 00:00:00.000&apos; AS DateTime [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_Carriers {
            get {
                return ResourceManager.GetString("Initial_Carriers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SELECT * FROM LifelinePrograms P
        ///LEFT JOIN Budget_LifelinePrograms BP ON P.ProgramName=BP.ProgramName
        ///WHERE BP.ProgramName IS NULL
        ///DECLARE @BudgetID VARCHAR(MAX)  
        ///SET @BudgetID = (SELECT Id FROM Companies WHERE Name=&apos;Budget Mobile&apos;);
        ///DELETE FROM CompanyTranslations WHERE CompanyID=@BudgetID AND Type=&apos;LifelineProgram&apos;;
        ///
        ///INSERT INTO CompanyTranslations (ID, CompanyID, LSID, TranslatedID, [Type])
        ///SELECT CONVERT(NVARCHAR(50), NEWID()), @BudgetID AS CompanyID, P.Id AS LSID, BP.BudgetCodeID AS Translated [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_CompanyTranslations {
            get {
                return ResourceManager.GetString("Initial_CompanyTranslations", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- This script contains competitors for Budget Mobile. Valid CompanyId is required
        ///INSERT INTO [dbo].[Competitors] ([Id], [Name], [CompanyId], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES 
        ///(NEWID(), &apos;Assurance/Virign Mobile&apos;, &apos;65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c&apos;, &apos;AL&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;Bellsouth Telecommunications&apos;, &apos;65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c&apos;, &apos;AL&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;Centurylink Centurytel of Alabama&apos;, &apos;65eab0c7-c7b8-496b-9325-dd8 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_Competitors {
            get {
                return ResourceManager.GetString("Initial_Competitors", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- This script includes ComplianceStatements for BudgetMobile. A valid CompanyId is required.
        ///
        ///INSERT INTO [dbo].[ComplianceStatements] ([Id], [CompanyId], [StateCode], [Statement], [DateCreated], [DateModified], [IsDeleted]) VALUES 
        ///  (NEWID(), &apos;65eab0c7-c7b8-496b-9325-dd8c9ba8ce1c&apos;, &apos;CA&apos;, &apos;Does the customer certify that they or someone else in their household (any individual or group of individuals who are living together at the same address as one economic unit. May include related and unrelated perso [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_ComplianceStatements {
            get {
                return ResourceManager.GetString("Initial_ComplianceStatements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE [dbo].[usp_GetUsers]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@FilterUserName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT U.Id, U.FirstName, U.LastName, U.IsActive,U.UserName, R.Name, R.Rank
        ///	FROM AspNetUsers (NOLOCK) U
        ///		-- Returned user&apos;s info
        ///		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
        ///		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
        ///		
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_GetUsers {
            get {
                return ResourceManager.GetString("Initial_GetUsers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- Certain LifelinePrograms will require additional StateProgram verification
        ///-- StatePrograms will have to be inserted first to populate those FKs.
        ///-- Info for which LifelinePrograms have this requirement can be found in the TableData Excel file.
        ///
        ///INSERT INTO [dbo].[LifelinePrograms] ([Id], [ProgramName], [StateCode], [RequiresAccountNumber], [RequiredStateProgramId], [RequiredSecondaryStateProgramId], [DateCreated], [DateModified], [IsDeleted], [NladEligibilityCode]) VALUES 
        ///  (NEWID(), &apos;Annual Incom [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_LifelinePrograms {
            get {
                return ResourceManager.GetString("Initial_LifelinePrograms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT [dbo].[Orders]([Id], [CompanyId], [SalesTeamId], [UserId], [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits], [QBFirstName], 
        ///[QBLastName], [QBSsn], [QBDateOfBirth], [CurrentLifelinePhoneNumber], [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageFileName], [StateProgramId], [StateProgramNumber], 
        ///[SecondaryStateProgramId], [SecondaryStateProgramNumber], [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber], [IPProofTy [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_Order_Insert {
            get {
                return ResourceManager.GetString("Initial_Order_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- ProofType of &apos;id&apos; apply to every state which is why their value for StateCode is null (Except in the case of California). 
        ///-- Queries intending to include the program ProofDocumentTypes for a State as well as the id ProofDocumentTypes (except CA) should query &quot;WHERE StateCode = &apos;EX&apos; OR StateCode = null&quot;.
        ///
        ///INSERT INTO [dbo].[ProofDocumentTypes] ([Id], [ProofType], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;program&apos;, &apos;Bureau of Indian Affairs General Assistance (B [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_ProofDocumentTypes {
            get {
                return ResourceManager.GetString("Initial_ProofDocumentTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///CREATE PROCEDURE [dbo].[usp_SetCompanyId]
        ///	-- Add the parameters for the stored procedure here
        ///	@companyid [nvarchar](128),
        ///	@id [nvarchar](128)
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from
        ///	-- interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///    
        ///	update dbo.AspNetUsers
        ///	SET [CompanyId]=@companyid 
        ///	WHERE [Id]=@id;
        ///END
        ///
        ///
        ///
        ///.
        /// </summary>
        internal static string Initial_setCompanyId {
            get {
                return ResourceManager.GetString("Initial_setCompanyId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[StateAgreements] ([Id], [StateCode], [Agreement], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;AL&apos;, &apos;Have you fully read and understood the requirements and restrictions listed on the Lifeline Certification List?&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;AL&apos;, &apos;The information contained within this application is true and correct. I acknowledge that providing false or fraudulent documentation in order to demonstrate eligibility for the Lifeline program is punishable by fine [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_StateAgreements {
            get {
                return ResourceManager.GetString("Initial_StateAgreements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[StatePrograms] ([Id], [Name], [StateCode], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;DSHS&apos;, &apos;WA&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;DCN&apos;, &apos;MO&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;Control&apos;, &apos;PR&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;Certificate&apos;, &apos;PR&apos;, GETDATE(), GETDATE(), 0)
        ///.
        /// </summary>
        internal static string Initial_StatePrograms {
            get {
                return ResourceManager.GetString("Initial_StatePrograms", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[StateSettings] ([Id], [StateCode], [SsnType], [IncomeLevel], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;NE&apos;, &apos;Full&apos;, &apos;135&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;PA&apos;, &apos;Full&apos;, &apos;135&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;PR&apos;, &apos;Full&apos;, &apos;135&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;UT&apos;, &apos;Full&apos;, &apos;135&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;WA&apos;, &apos;Full&apos;, &apos;135&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;AZ&apos;, &apos;Last4&apos;, &apos;150&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;CA&apos;, &apos;Last4&apos;, &apos; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_StateSettings {
            get {
                return ResourceManager.GetString("Initial_StateSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to     INSERT [dbo].[TempOrders]([Id], [CompanyId], [SalesTeamId], [HouseholdReceivesLifelineBenefits], [CustomerReceivesLifelineBenefits], [QBFirstName],
        ///    [QBLastName], [QBSsn], [QBDateOfBirth], [CurrentLifelinePhoneNumber], [LifelineProgramId], [LPProofTypeId], [LPProofNumber], [LPImageFileName], [StateProgramId], [StateProgramNumber], 
        ///    [SecondaryStateProgramId], [SecondaryStateProgramNumber], [FirstName], [MiddleInitial], [LastName], [Ssn], [DateOfBirth], [EmailAddress], [ContactPhoneNumber], [IPPr [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_Temp_Order_Insert {
            get {
                return ResourceManager.GetString("Initial_Temp_Order_Insert", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[TpivProofDocumentTypes] ([Id], [Type], [Name], [LifelineSystem], [LifelineSystemId], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;SSN&apos;, &apos;Current Income Statement from and Employer, Paycheck Stub, or W-2&apos;, &apos;NLAD&apos;, &apos;T3&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;SSN&apos;, &apos;Government Assistance Program Document which includes name and date of birth&apos;, &apos;NLAD&apos;, &apos;T14&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;SSN&apos;, &apos;Military Discharge Documentation&apos;, &apos;NLAD&apos;, &apos;T12&apos;, GETDATE(), GETDATE(), [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_TpivProofDocumentTypes {
            get {
                return ResourceManager.GetString("Initial_TpivProofDocumentTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Jacob Rieger
        ///-- Create date: 6/2/2015
        ///-- Description:	A stored procedure to retrieve a valid Cali phone number by returning the value and
        ///-- incrementing the existing value by 1
        ///-- =============================================
        ///SET NOCOUNT ON;
        ///UPDATE dbo.CaliPhoneNumbers
        ///SET Number = 
        ///	CASE WHEN (Number &gt;= 9999999)
        ///		THEN 0
        ///	ELSE
        ///		Number + 1
        ///	END
        ///OUTPUT CONVERT(varchar(3), INSERTED.AreaCode) + CONVERT(varchar(7), INSERTED.Number)
        ///WH [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_usp_GetNextValidCaliPhoneNumber {
            get {
                return ResourceManager.GetString("Initial_usp_GetNextValidCaliPhoneNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Jacob Rieger
        ///-- Create date: 6/2/2015
        ///-- Description:	A stored procedure to retrieve a valid Nlad phone number by returning the value and
        ///-- settings IsCurrentlyInUse to true
        ///-- =============================================
        ///SET NOCOUNT ON;
        ///
        ///-- Insert statements for procedure here
        ///UPDATE TOP(1) dbo.NladPhoneNumbers
        ///SET IsCurrentlyInUse = 1, DateModified = GETUTCDATE()
        ///OUTPUT INSERTED.Number
        ///WHERE IsCurrentlyInUse = 0 AND CompanyId = @co [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_usp_GetNextValidNladPhoneNumber {
            get {
                return ResourceManager.GetString("Initial_usp_GetNextValidNladPhoneNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- =============================================
        ///-- Author:		Jacob Rieger
        ///-- Create date: 6/2/2015
        ///-- Description:	A stored procedure to retrieve a valid Nlad phone number by returning the value of
        ///-- the oldest in use number and updating it&apos;s date modified value
        ///-- =============================================
        ///SET NOCOUNT ON;
        ///UPDATE TOP(1) dbo.NladPhoneNumbers
        ///SET DateModified = GETUTCDATE()
        ///OUTPUT INSERTED.Number
        ///WHERE CompanyId = @companyId AND ID = (SELECT TOP(1) Id FROM dbo.NladPhoneNumbers O [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_usp_GetOldestInUseNladPhoneNumber {
            get {
                return ResourceManager.GetString("Initial_usp_GetOldestInUseNladPhoneNumber", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW [dbo].[v_UserActiveTeams] AS
        ///	SELECT *
        ///	FROM v_UserTeams
        ///	WHERE IsDeleted=0 AND Level1IsDeleted=0 AND Level2IsDeleted=0 AND Level3IsDeleted=0
        ///.
        /// </summary>
        internal static string Initial_v_UserActiveTeams {
            get {
                return ResourceManager.GetString("Initial_v_UserActiveTeams", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE [dbo].[usp_GetUsers]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@FilterUserName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT U.Id, U.FirstName, U.LastName, U.IsActive,U.UserName, R.Name, R.Rank
        ///	FROM AspNetUsers (NOLOCK) U
        ///		-- Returned user&apos;s info
        ///		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
        ///		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
        ///		
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_View_GetOrder_ProviderTranslation {
            get {
                return ResourceManager.GetString("Initial_View_GetOrder_ProviderTranslation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to INSERT INTO [dbo].[ZipCodes] ([Id], [PostalCode], [State], [StateAbbreviation], [CountyFips], [City], [DateCreated], [DateModified], [IsDeleted]) VALUES (NEWID(), &apos;82604&apos;, &apos;Wyoming&apos;, &apos;WY&apos;, &apos;56025&apos;, &apos;Casper&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;82229&apos;, &apos;Wyoming&apos;, &apos;WY&apos;, &apos;56009&apos;, &apos;Shawnee&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;82329&apos;, &apos;Wyoming&apos;, &apos;WY&apos;, &apos;56007&apos;, &apos;Medicine Bow&apos;, GETDATE(), GETDATE(), 0)
        ///, (NEWID(), &apos;82190&apos;, &apos;Wyoming&apos;, &apos;WY&apos;, &apos;56029&apos;, &apos;Yellowstone National Park&apos;, GETDATE(), GETDATE(), 0)
        ///, ( [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Initial_ZipCodes {
            get {
                return ResourceManager.GetString("Initial_ZipCodes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE PROCEDURE [dbo].[usp_GetOrders]
        ///	@UserId [nvarchar](128) = &apos;ThisUserIDWontExist&apos;, 
        ///	@CustomerName [nvarchar](500) = NULL
        ///AS
        ///BEGIN
        ///	-- SET NOCOUNT ON added to prevent extra result sets from interfering with SELECT statements.
        ///	SET NOCOUNT ON;
        ///
        ///	SELECT *
        ///	FROM Orders (NOLOCK) O
        ///		LEFT JOIN AspNetUsers (NOLOCK) U ON U.Id=@UserId
        ///		LEFT JOIN AspNetUserRoles (NOLOCK) UR ON UR.UserId=U.Id
        ///		LEFT JOIN AspNetRoles (NOLOCK) R ON R.Id=UR.RoleID
        ///	WHERE 1=1
        ///		AND O.CompanyId=U.CompanyId
        ///		AND (
        ///	 [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TenantAccountFulfillmentLog_usp_GetOrders {
            get {
                return ResourceManager.GetString("TenantAccountFulfillmentLog_usp_GetOrders", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to CREATE VIEW [dbo].[v_FullOrderInfo] AS
        ///SELECT O.*
        ///	, T.ExternalDisplayName, T.Name AS SalesTeam, T.PayPalEmail AS TeamPaypalEmail
        ///	, U.FirstName AS Emp_FName, U.LastName AS Emp_LName, U.PayPalEmail AS UserPayPalEmail
        ///	, BLP.TranslatedID AS BLPID
        ///	, LPP.Name AS ProgramProofType, IDP.Name AS IDProofType
        ///	, SP.Name AS StateProgramType, SP2.Name AS SecondaryStateProgramType
        ///	, SSNP.Name AS TPIVBypassSSNDocType, SSNP.LifelineSystemID AS TPIVBypassSSNTCode, DOBP.Name AS TPIVBypassDOBDocType, DOBP.LifelineS [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TenantAccountFulfillmentLog_v_FullOrderInfo {
            get {
                return ResourceManager.GetString("TenantAccountFulfillmentLog_v_FullOrderInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to UPDATE LP SET RequiredStateProgramID=SP.Id 
        ///FROM LifelinePrograms LP 
        ///LEFT JOIN StatePrograms SP ON SP.Name=&apos;DCN&apos; 
        ///WHERE LP.StateCode=&apos;MO&apos; 
        ///     AND LP.ProgramName NOT IN (&apos;Federal Housing Assistance (Section 8)&apos;, &apos;National School Lunch (free program only)&apos;, &apos;Supplemental Security Income&apos;, &apos;Annual Income&apos;) 
        /// 
        ///UPDATE LP SET RequiredStateProgramID=SP.Id 
        ///FROM LifelinePrograms LP 
        ///LEFT JOIN StatePrograms SP ON SP.Name=&apos;DSHS&apos; 
        ///WHERE LP.StateCode=&apos;WA&apos; 
        ///     AND LP.ProgramName IN (&apos;Food Stamps&apos;,&apos;Supplem [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string update_required_state_programs {
            get {
                return ResourceManager.GetString("update_required_state_programs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE [dbo].[usp_GetOrders]
        ///
        ///.
        /// </summary>
        internal static string usp_GetOrdersDrop {
            get {
                return ResourceManager.GetString("usp_GetOrdersDrop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to DROP PROCEDURE [dbo].[usp_GetOrders]
        ///
        ///.
        /// </summary>
        internal static string usp_GetUsersDrop {
            get {
                return ResourceManager.GetString("usp_GetUsersDrop", resourceCulture);
            }
        }

        internal static string Order_Insert
        {
            get
            {
                return ResourceManager.GetString("Order_Insert", resourceCulture);
            }
        }
        internal static string Temp_Order_Insert
        {
            get
            {
                return ResourceManager.GetString("Temp_Order_Insert", resourceCulture);
            }
        }
    }
}
