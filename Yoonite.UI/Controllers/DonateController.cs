using System.Web.Mvc;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class DonateController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}