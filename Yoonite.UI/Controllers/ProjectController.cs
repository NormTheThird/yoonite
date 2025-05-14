using System.Linq;
using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class ProjectController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyProjects()
        {
            return View();
        }

        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMyProjects(GetUserProjectsRequest request)
        {
            request.AccountId = this._user.Id;
            var response = this.ProjectService.GetUserProjects(request);
            return Json(response);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult GetProjects(GetProjectsRequest request)
        {
            var response = this.ProjectService.GetProjects(request);
            if(this._user == null)
                return Json(response);
            var myProjects = response.Projects.Where(_ => _.AccountId.Equals(this._user.Id)).ToList();
            foreach (var project in myProjects)
                response.Projects.Remove(project);
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetProjectOwner(GetAccountRequest request)
        {
            var response = this.AccountService.GetAccount(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetSkills(GetSkillsRequest request)
        {
            var response = this.SkillService.GetSkills(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SaveProject(SaveProjectRequest request)
        {
            request.Project.AccountId = this._user.Id;
            var response = this.ProjectService.SaveProject(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteProject(DeleteProjectRequest request)
        {
            var response = this.ProjectService.DeleteProject(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult ContactOwner(SaveMessageRequest request)
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