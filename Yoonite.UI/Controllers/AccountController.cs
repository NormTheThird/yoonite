using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {

        public ActionResult MyProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetAccount(GetAccountRequest request)
        {
            request.AccountId = this._user.Id;
            var response = this.AccountService.GetAccountWithSkills(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetSkills(GetSkillsRequest request)
        {
            var response = this.SkillService.GetSkills(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SaveAccount(SaveAccountRequest request)
        {
            var response = this.AccountService.SaveAccount(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SaveAccountImage(SaveStorageRequest request)
        {
            var saveStorageResponse = this.StorageService.SaveStorage(request);
            if (!saveStorageResponse.IsSuccess)
                return Json(saveStorageResponse);

            var saveAccountProfileImageRequest = new SaveAccountProfileImageRequest
            {
                AccountId = this._user.Id,
                ProfileImageStorageId = saveStorageResponse.StorageId
            };
            var saveAccountProfileImageResponse = this.AccountService.SaveAccountProfileImage(saveAccountProfileImageRequest);
            return Json(saveAccountProfileImageResponse);
        }
    }
}