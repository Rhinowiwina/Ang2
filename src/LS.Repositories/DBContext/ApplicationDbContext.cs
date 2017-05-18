using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using LS.Domain;
using LS.Domain.ExternalApiIntegration.CaliforniaDap;
using LS.Domain.ExternalApiIntegration.Nlad;
using LS.Repositories.EFConfigs;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LS.Repositories.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, ApplicationUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public static ApplicationDbContext Create()
        
           {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Configurations.Add(new ApiLogEFConfig());
            modelBuilder.Configurations.Add(new ApplicationUserEFConfig());
            modelBuilder.Configurations.Add(new BaseIncomeLevelsEFConfig());
            modelBuilder.Configurations.Add(new CaliPhoneNumberEFConfig());
            modelBuilder.Configurations.Add(new CompanyEFConfig());
            modelBuilder.Configurations.Add(new CompetitorEFConfig());
            modelBuilder.Configurations.Add(new ComplianceStatementEFConfig());
        
            modelBuilder.Configurations.Add(new ExternalStorageCredentialsEFConfig());
            modelBuilder.Configurations.Add(new ImageUploadEFConfig());
        
            modelBuilder.Configurations.Add(new Level1SalesGroupEFConfig());
            modelBuilder.Configurations.Add(new Level2SalesGroupEFConfig());
            modelBuilder.Configurations.Add(new Level3SalesGroupEFConfig());
            modelBuilder.Configurations.Add(new LifelineProgramEFConfig());
            modelBuilder.Configurations.Add(new LoginInfoEFConfig());
          
            modelBuilder.Configurations.Add(new OrderEFConfig());
            modelBuilder.Configurations.Add(new PlanEFConfig());
            modelBuilder.Configurations.Add(new ProofDocumentTypeEFConfig());
       
            modelBuilder.Configurations.Add(new SalesTeamEFConfig());
            modelBuilder.Configurations.Add(new StateAgreementEFConfig());
            modelBuilder.Configurations.Add(new StateProgramEFConfig());
            modelBuilder.Configurations.Add(new StateSettingsEFConfig());
            modelBuilder.Configurations.Add(new TempOrderEFConfig());
            modelBuilder.Configurations.Add(new TpivProofDocumentTypeEFConfig());
            modelBuilder.Configurations.Add(new CommissionLogEFConfig());
            modelBuilder.Configurations.Add(new WebApplicationLogEntryEFConfig());
            modelBuilder.Configurations.Add(new ZipCodeEFConfig());
            modelBuilder.Configurations.Add(new DevDataEFConfig());
            modelBuilder.Configurations.Add(new CompanyTranslationsEFConfig());
            modelBuilder.Configurations.Add(new OrdersTaxesEFConfig ());
           
            modelBuilder.Configurations.Add(new OrderNoteEFConfig());
          
            modelBuilder.Configurations.Add(new ProductCommissionEFConfig());
         
            modelBuilder.Configurations.Add(new ResourceCategoryEFConfig());
            modelBuilder.Configurations.Add(new ResourcesEFConfig());
    
            modelBuilder.Configurations.Add(new PaymentTransactionLogEFConfig());
            modelBuilder.Configurations.Add(new TransactionLogEFConfig());
            modelBuilder.Configurations.Add(new ResponseLogsCGMEHDBDetailsEFConfig());
            modelBuilder.Configurations.Add(new ResponseLogsCGMEHDBEFConfig());
            modelBuilder.Configurations.Add(new LoginMsgEFConfig());
            modelBuilder.Configurations.Add(new AddressProofDocumentTypeEFConfig());

            modelBuilder.Configurations.Add(new UserOnBoardDataEFConfig());
            modelBuilder.Configurations.Add(new CompanyStatesEFConfig());
            modelBuilder.Configurations.Add(new UserSignUpsEFConfig());

            modelBuilder.Configurations.Add(new AddressValidationEFConfig());
            modelBuilder.Configurations.Add(new AuditLogEFConfig());

            modelBuilder.Configurations.Add(new DeviceOrderEFConfig());
            modelBuilder.Configurations.Add(new DeviceOrderDeviceEFConfig());
        
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApiLogEntry> ApiLogEntries { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<ComplianceStatement> ComplianceStatements { get; set; }
        public DbSet<ExternalStorageCredentials> ExternalStorageCredentials { get; set; }
      
        public DbSet<Level1SalesGroup> Level1SalesGroups { get; set; }
        public DbSet<Level2SalesGroup> Level2TSalesGroups { get; set; }
        public DbSet<Level3SalesGroup> Level3SalesGroups { get; set; }
        public DbSet<LifelineProgram> LifelinePrograms { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<ProofDocumentType> ProofDocumentTypes { get; set; }
        public DbSet<SalesTeam> SalesTeams { get; set; }
        public DbSet<StateAgreement> StateAgreements { get; set; }
        public DbSet<StateProgram> StatePrograms { get; set; }
        public DbSet<StateSettings> StateSettings { get; set; }
        public DbSet<WebApplicationLogEntry> WebApplicationLogEntries { get; set; }
        public DbSet<NladPhoneNumber> NladPhoneNumbers { get; set; }
        public DbSet<CaliPhoneNumber> CaliPhoneNumbers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DevData> DevData { get; set; }
        public DbSet<TempOrder> TempOrders { get; set; }
        public DbSet<CompanyTranslations> CompanyTranslations { get; set; }
        public DbSet<OrdersTaxes> Taxes { get; set; }
        public DbSet<Payment> Payment { get; set; }    
        public DbSet<PaymentTransactionLog> PaymentTransactionLog { get; set; }
        public DbSet<CommissionLog> CommissionLog { get; set; }
        public DbSet<TransactionLog> TransactionLog { get; set; }
        public DbSet<ResponseLogsCGMEHDB> EhdbResponseLog { get; set; }
        public DbSet<ResponseLogsCGMEHDBDetails> EhdbResponseLogDetails { get; set; }
        public DbSet<LoginMsg>LoginMsg { get; set; }
        public DbSet<AddressProofDocumentType> AddressProofDocumentType { get; set; }
        public DbSet<UserOnBoardData> UserOnBoardData { get; set; }
        public DbSet<CompanyStates> CompanyStates { get; set; }
        public DbSet<UserSignUps> UserSignUps { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        }
}
