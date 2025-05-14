using System.Web.Mvc;
using Yoonite.Common.RequestAndResponses;

namespace Yoonite.UI.Controllers
{
    [Authorize]
    public class MessageController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserMessages(GetUserMessagesRequest request)
        {
            request.UserId = this._user.Id;
            var response = this.MessagingService.GetUserMessages(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult SendResponseMessage(SaveMessageRequest request)
        {
            request.Message.FromAccountId = this._user.Id;
            var response = this.MessagingService.SaveMessage(request);
            if(response.IsSuccess)
            {
                // TODO: TREY: 8/10/2019 SEND EMAIL TO USER THAT RECEIVES EMAIL

                var getMessageResponse = this.MessagingService.GetMessage(new GetMessageRequest { MessageId = response.MessageId });
                if(getMessageResponse.IsSuccess)
                    return Json(getMessageResponse);
            }
            return Json(response);
        }

        [HttpPost]
        public ActionResult DeleteMessage(DeleteMessageRequest request)
        {
            var response = this.MessagingService.DeleteMessage(request);
            return Json(response);
        }

        [HttpPost]
        public ActionResult MarkMessageAsRead(MarkMessageAsReadRequest request)
        {
            request.UserId = this._user.Id;
            var response = this.MessagingService.MarkMessageAsRead(request);
            return Json(response);
        }

    }
}