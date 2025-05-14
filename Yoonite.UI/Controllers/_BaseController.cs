using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Yoonite.Common.Models;
using Yoonite.Service.Services;

namespace Yoonite.UI.Controllers
{
    public class BaseController : Controller
    {
        public IAccountService AccountService { get; } = new AccountService();
        public ILoggingService LoggingService { get; } = new LoggingService();
        public IMessagingService MessagingService { get; } = new MessagingService();
        public IProjectService ProjectService { get; } = new ProjectService();
        public ISkillService SkillService { get; } = new SkillService();
        public IStorageService StorageService { get; } = new StorageService();

        protected SecurityModel _user => GetUserSecurityModel();

        private SecurityModel GetUserSecurityModel()
        {
            var formsAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (string.IsNullOrEmpty(formsAuthCookie?.Value)) return null;

            var formsAuthTicket = FormsAuthentication.Decrypt(formsAuthCookie.Value);
            if (formsAuthTicket == null) return null;
            if (string.IsNullOrEmpty(formsAuthTicket.UserData)) return null;

            return new JavaScriptSerializer().Deserialize<SecurityModel>(formsAuthTicket.UserData);
        }
    }
}