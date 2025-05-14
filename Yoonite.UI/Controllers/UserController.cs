using System.Linq;
using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUsers(GetAccountsRequest request)
        {
            var response = this.AccountService.GetAccountsWithSkills(request);
            response.Accounts.Remove(response.Accounts.FirstOrDefault(_ => _.Id.Equals(this._user.Id)));
            return Json(response);
        }

        [HttpPost]
        public ActionResult ContactUser(SaveMessageRequest request)
        {
            request.Message.FromAccountId = this._user.Id;
            var response = this.MessagingService.SaveMessage(request);
            if (response.IsSuccess)
            {
                // TODO: TREY: 8/10/2019 SEND EMAIL TO USER THAT RECEIVES EMAIL
            }
            return Json(response);
        }
    }
}