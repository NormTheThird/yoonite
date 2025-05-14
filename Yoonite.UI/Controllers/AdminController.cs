using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        #region Accounts

        public ActionResult Accounts()
        {
            return View("Accounts/Index");
        }

        [HttpPost]
        public ActionResult GetAccounts(GetAccountsRequest request)
        {
            var response = this.AccountService.GetAccounts(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult ChangeAccountStatus(ChangeAccountStatusRequest request)
        {
            var response = this.AccountService.ChangeAccountStatus(request);
            return Json(response);
        }

        #endregion

        #region Projects

        public ActionResult Projects()
        {
            return View("Projects/Index");
        }

        [HttpPost]
        public ActionResult GetProjects(GetProjectsRequest request)
        {
            request.GetActiveAndInactive = true;
            var response = this.ProjectService.GetProjects(request);
            return Json(response);
        }

        #endregion

        #region Skills

        public ActionResult Skills()
        {
            return View("Skills/Index");
        }

        [HttpPost]
        public ActionResult GetSkills(GetSkillsRequest request)
        {
            var response = this.SkillService.GetSkills(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SaveSkill(SaveSkillRequest request)
        {
            var response = this.SkillService.SaveSkill(request);
            return Json(response);
        }

        #endregion
    }
}