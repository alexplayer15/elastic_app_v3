using elastic_app_v3.DTOs;
using elastic_app_v3.Services;
using elastic_app_v3.Enums;
using Microsoft.AspNetCore.Mvc;

namespace elastic_app_v3.Routing
{
    public static class UserEndpoints
    {
        public static void Map(WebApplication app)
        {
            app.MapPost("/user/signup", IResult (
                [FromBody] SignUpRequest request,
                [FromServices] IUserService userService) =>
            {
                var result = userService.TrySignUp(request);

                return result.IsSuccess
                    ? TypedResults.Ok($"User with id: {result.Value.UserId} successfully created")
                    : result.Error.ErrorCategory switch
                    {
                        ErrorCategory.UserAlreadyExists => TypedResults.Conflict(result.Error),
                        _ => TypedResults.StatusCode(500)
                    };
            })
            .WithName("PostUserSignUp")
            .WithOpenApi();
        }
    }
}
