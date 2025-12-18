using elastic_app_v3.DTOs;
using elastic_app_v3.Errors;

namespace elastic_app_v3.Services
{
    public interface IUserService
    {
        Result<SignUpResponse> SignUp(SignUpRequest request);
        Result<GetUserResponse> GetUserById(Guid userId);
    }
}
