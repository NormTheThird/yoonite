using System.Web.Mvc;

namespace Yoonite.UI.Controllers
{
    public class LegalController : Controller
    {
        public ActionResult TermsConditions()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }
    }
}