using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    //[Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ContactUs(ContactUsRequest request)
        {
            var response = this.MessagingService.ContactUs(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetUserUnreadMessageCount(GetUserUnreadMessageCountRequest request)
        {
            request.UserId = this._user.Id;
            var response = this.MessagingService.GetUserUnreadMessageCount(request);
            return Json(response);
        }
    }
}