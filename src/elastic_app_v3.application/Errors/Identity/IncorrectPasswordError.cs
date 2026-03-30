using FluentResults;

namespace elastic_app_v3.application.Errors.Identity
{
    public sealed class IncorrectPasswordError() : Error("Incorrect password");
}
