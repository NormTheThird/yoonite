using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Service.Services;

namespace Yoonite.UI.Controllers
{
    public class SecurityController : BaseController
    {
        public ISecurityService SecurityService { get; private set; } = new SecurityService();

        public ActionResult Index(string resetId)
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            @ViewBag.ResetId = resetId;
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterAccountRequest request)
        {
            var registerAccountResponse = SecurityService.RegisterAccount(request);
            if (!registerAccountResponse.IsSuccess)
                return Json(registerAccountResponse);

            var getSecurityModelResponse = SecurityService.GetSecurityModel(new GetSecurityModelRequest { AccountId = registerAccountResponse.NewAccountId });
            if (!getSecurityModelResponse.IsSuccess)
                return Json(getSecurityModelResponse);

            var userData = new JavaScriptSerializer().Serialize(getSecurityModelResponse.SecurityModel);
            var formsAuthTicket = new FormsAuthenticationTicket(1, request.Email, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData);
            var encryptedFormsAuthonTicket = FormsAuthentication.Encrypt(formsAuthTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedFormsAuthonTicket) { HttpOnly = true, Secure = Request.IsSecureConnection };
            Response.Cookies.Set(cookie);
            return Json(getSecurityModelResponse);
        }

        [HttpPost]
        public ActionResult ValidateAccount(ValidateAccountRequest request)
        {
            //TODO: TREY: 11/25/2018 Remove after testing
            //request = new ValidateAccountRequest { Email = "WilliamRNorman@Hotmail.com", Password = "password" };

            var validateAccountResponse = this.SecurityService.ValidateAccount(request);
            if (!validateAccountResponse.IsSuccess) return Json(validateAccountResponse);
            var securityModel = validateAccountResponse.SecurityModel;
            var userData = new JavaScriptSerializer().Serialize(securityModel);
            var formsAuthTicket = new FormsAuthenticationTicket(1, request.Email, DateTime.Now, DateTime.Now.AddDays(30), false, userData);
            var encryptedFormsAuthonTicket = FormsAuthentication.Encrypt(formsAuthTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedFormsAuthonTicket) { HttpOnly = true, Secure = Request.IsSecureConnection };
            Response.Cookies.Set(cookie);
            return Json(validateAccountResponse);
        }

        [HttpPost]
        public ActionResult ForgotPassword(PasswordResetRequest request)
        {
            var passwordResetResponse = this.SecurityService.PasswordReset(request);
            if (!passwordResetResponse.IsSuccess)
                return Json(passwordResetResponse);

            var parameters = new Dictionary<string, string>();
            parameters.Add("url", $"{request.BaseUrl}/Security/Index?resetId={passwordResetResponse.ResetId}");

            var sendResetEmailRequest = new SendEmailRequest
            {
                FromName = "Yoonite",
                FromEmail = "no-reply@unieksoftware.com",
                Recipients = new List<string> { request.Email },
                Parameters = parameters,
                EmailType = EmailType.ResetPassword
            };
            var sendResetEmailResponse = this.MessagingService.SendEmail(sendResetEmailRequest);
            return Json(sendResetEmailResponse);
        }

        [HttpPost]
        public ActionResult ResetPassword(ValidatePasswordResetRequest request)
        {
            var response = SecurityService.ValidatePasswordReset(request);
            return Json(response);
        }

        [HttpPost]
        [Authorize]
        public ActionResult GetSecurityModel()
        {
            return Json(this._user);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Security");
        }
    }
}