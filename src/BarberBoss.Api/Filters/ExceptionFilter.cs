using BarberBoss.Communication.Responses;
using BarberBoss.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BarberBoss.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is BarberBossException barberBossException)
            HandleProjectException(barberBossException, context);
        else
            ThrowUnkowError(context);
    }

    private void HandleProjectException(BarberBossException barberBossException, ExceptionContext context)
    {
        if (context.Exception is ErrorOnValidationException errorOnValidationException)
        {
            var errorResponse = new ResponseErrorJson(errorOnValidationException.GetErrors());

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Result = new ObjectResult(errorResponse);
        }
    }

    private void ThrowUnkowError(ExceptionContext context)
    {
        var errorResponse = new ResponseErrorJson("Unknown error");

        context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Result = new ObjectResult(errorResponse);
    }
}
