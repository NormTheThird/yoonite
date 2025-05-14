using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Yoonite.Common.Models;
using Yoonite.UI.SecurityModels;

namespace Yoonite.UI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            var formsAuthCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (string.IsNullOrEmpty(formsAuthCookie?.Value)) return;

            var formsAuthTicket = FormsAuthentication.Decrypt(formsAuthCookie.Value);
            if (formsAuthTicket == null) return;
            if (string.IsNullOrEmpty(formsAuthTicket.UserData)) return;

            var securityModelModel = new JavaScriptSerializer().Deserialize<SecurityModel>(formsAuthTicket.UserData);
            var customPrincipal = new CustomPrincipal(new CustomIdentity(securityModelModel));
            HttpContext.Current.User = customPrincipal;

            formsAuthCookie.Expires = DateTime.Now.AddDays(30);
            HttpContext.Current.Response.Cookies.Set(formsAuthCookie);
        }
    }
}
