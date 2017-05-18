using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Configuration;
using LS.Repositories.DBContext;
using LS.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using LS.Domain;
using Exceptionless;
using Exceptionless.Models;
namespace LS.WebApp {
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser, string> {
        private static readonly int MinimumPasswordLength = 8;
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store) {
            PasswordValidator = new MinimumLengthValidator(MinimumPasswordLength);
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) {

            var manager = new ApplicationUserManager(new ApplicationUserStore(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager) {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            manager.EmailService = new EmailService();

            manager.UserLockoutEnabledByDefault = false;
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null) {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    public class EmailService : IIdentityMessageService {
        public Task SendAsync(IdentityMessage message) {
            var emailHelper = new EmailHelper();

            var CCAddresses = "";
            string isDev = ConfigurationManager.AppSettings["Environment"];
            if (isDev == "DEV") {
                CCAddresses = "randy@305spin.com";
            }
            var sendEmailResult = new EmailHelper().SendEmail(message.Subject, message.Destination, CCAddresses, message.Body, null);

            return Task.FromResult(0);

        }
    }

    public class ApplicationUserStore :
        UserStore<ApplicationUser, ApplicationRole, string, IdentityUserLogin, ApplicationUserRole, IdentityUserClaim> {
        public ApplicationUserStore(ApplicationDbContext context) : base(context) {
        }
    }

    public class ApplicationRoleManager : RoleManager<ApplicationRole> {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> store) : base(store) {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context) {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }

    public class ApplicationRoleStore : RoleStore<ApplicationRole, string, ApplicationUserRole> {
        public ApplicationRoleStore(ApplicationDbContext context)
            : base(context) {
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string> {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager) {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user) {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager, DefaultAuthenticationTypes.ApplicationCookie);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context) {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
