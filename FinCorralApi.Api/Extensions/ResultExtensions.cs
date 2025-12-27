using FinCorralApi.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace FinCorralApi.Api.Extensions
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return new BadRequestObjectResult(result.Error);
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return new BadRequestObjectResult(result.Error);
        }
    }

}
