using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LS.WebApp.Controllers.mvc {
    public class UserSignupController : Controller {
        public async Task<ActionResult> Index() {
            return Redirect("https://users.spinlifeserv.com");
        }

        public ActionResult Error() {
            return View();
        }
    }
}