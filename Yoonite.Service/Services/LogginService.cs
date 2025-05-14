using System;
using System.Linq;
using Yoonite.Common.RequestAndResponses;
using Yoonite.Data;

namespace Yoonite.Service.Services
{
    public interface ILoggingService
    {
        LogErrorResponse LogError(LogErrorRequest request);
    }

    public class LoggingService : ILoggingService
    {
        public LogErrorResponse LogError(LogErrorRequest request)
        {
            try
            {
                var response = new LogErrorResponse();
                var innerExMessage = "";
                var entityError = "";
                if (request.ex.InnerException != null) innerExMessage = request.ex.InnerException.Message;
                if (request.ex.GetType().FullName.Equals("System.Data.Entity.Validation.DbEntityValidationException"))
                {
                    foreach (var entity in ((System.Data.Entity.Validation.DbEntityValidationException)request.ex)
                        .EntityValidationErrors)
                    {
                        entityError += "[Entity: " + entity.Entry.Entity.ToString() + " ";
                        entityError = entity.ValidationErrors.Aggregate(entityError,
                            (current, error) =>
                                current + ("[PropertyName: " + error.PropertyName + "][ErrorMessage: " +
                                           error.ErrorMessage + "]"));
                        entityError += "]";
                    }
                }

                var stackTrace = request.ex.StackTrace ?? "";
                if (stackTrace.Length > 3000)
                    stackTrace = stackTrace.Substring(0, 3000);

                using (var context = new YooniteEntities())
                {
                    var error = new ErrorLog
                    {
                        Id = Guid.NewGuid(),
                        ReviewedByAccountId = null,
                        HResult = request.ex.HResult,
                        Source = request.ex.Source ?? "NO SOURCE SPECIFIED",
                        ExceptionType = request.ex.GetType().Name,
                        ExceptionMessage = request.ex.Message + " " + entityError,
                        InnerExceptionMessage = innerExMessage,
                        StackTrace = stackTrace,
                        Parameters = "",
                        ReviewedComments = "",
                        IsReviewed = false,
                        DateReviwed = null,
                        DateCreated = DateTimeOffset.Now,

                    };

                    context.ErrorLogs.Add(error);
                    context.SaveChanges();
                    response.IsSuccess = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                return new LogErrorResponse { ErrorMessage = ex.Message };
            }
        }
    }
}