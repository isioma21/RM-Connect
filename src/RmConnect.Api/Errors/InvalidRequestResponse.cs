using Microsoft.AspNetCore.Mvc;

namespace RmConnect.Api.Errors;

public static class InvalidRequestResponse
{
    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        return new BadRequestObjectResult(new { message = "One or more fields are invalid.", errors });
    }
}
