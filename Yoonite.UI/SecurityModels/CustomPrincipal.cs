using System.Security.Principal;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Yoonite.Common.Models;

namespace Yoonite.UI.SecurityModels
{
    public class CustomPrincipal : IPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(CustomIdentity identity)
        {
            Identity = identity;
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public static bool IsSystemAdmin() => GetUserSecurityModel()?.IsSystemAdmin ?? false;

        public static SecurityModel GetUserSecurityModel()
        {
            var formsAuthCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (string.IsNullOrEmpty(formsAuthCookie?.Value)) return null;

            var formsAuthTicket = FormsAuthentication.Decrypt(formsAuthCookie.Value);
            if (formsAuthTicket == null) return null;
            if (string.IsNullOrEmpty(formsAuthTicket.UserData)) return null;

            return new JavaScriptSerializer().Deserialize<SecurityModel>(formsAuthTicket.UserData);
        }
    }
}