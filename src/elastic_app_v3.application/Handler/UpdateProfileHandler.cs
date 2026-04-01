using elastic_app_v3.application.DTOs.Profile;
using elastic_app_v3.application.Errors;
using elastic_app_v3.application.Mapping;
using elastic_app_v3.application.Services.Profiles;
using FluentResults;
using FluentValidation;
using MediatR;

namespace elastic_app_v3.application.Handler;
public class UpdateProfileHandler(
    IValidator<UpdateProfileRequest> updateProfileRequestValidator,
    IProfileService profileService) : IRequestHandler<UpdateProfileRequest, Result<UpdateProfileResponse>>
{
    private readonly IValidator<UpdateProfileRequest> _updateProfileRequestValidator = updateProfileRequestValidator;
    private readonly IProfileService _profileService = profileService;
    public async Task<Result<UpdateProfileResponse>> Handle(
        UpdateProfileRequest request, 
        CancellationToken cancellationToken)
    {
        var validationResult = _updateProfileRequestValidator.Validate(request);

        if (!validationResult.IsValid)
        {
            var errorDescription = string.Join("; ",
                validationResult.Errors.Select(e => e.ErrorMessage));

            return Result.Fail(new ValidationError(errorDescription));
        }

        var profileUpdate = request.ToDomainModel();
        return await _profileService.UpdateProfile(profileUpdate, cancellationToken);
    }
}
