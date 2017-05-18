using Microsoft.AspNet.Identity.EntityFramework;

namespace LS.Domain
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationRole Role { get; set; }
    }
}
