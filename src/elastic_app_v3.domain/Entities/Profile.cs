using CSharpFunctionalExtensions;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Profile;
using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.domain.Entities;
public class Profile(Guid userId) : Entity<Guid>(userId)
{
    public string? Bio { get; private set; } = string.Empty;
    public IReadOnlyList<Language> Languages { get; private set; } = [];
    public IReadOnlyList<string> Hobbies { get; private set; } = [];
    private Profile(
        Guid userId, 
        string? bio, 
        IReadOnlyList<Language> languages,
        IReadOnlyList<string> hobbies
    ) : this(userId)
    {
        Bio = bio ?? string.Empty;
        Languages = languages;
        Hobbies = hobbies;
    }
    public static Profile Rehydrate(Guid userId, string? bio, IReadOnlyList<Language> languages, IReadOnlyList<string> hobbies)
        => new(userId, bio, languages, hobbies);

    public UnitResult<ProfileError> UpdateBio(string bio)
    {
        if (string.IsNullOrWhiteSpace(bio))
        {
            return UnitResult.Failure<ProfileError>(new MissingBioError());
        }
        if (bio.Length > 500)
        {
            return UnitResult.Failure<ProfileError>(new BioTooLongError());
        }
        
        Bio = bio;
        
        return UnitResult.Success<ProfileError>();
    }
    public UnitResult<ProfileError> UpdateLanguages(List<Language>? languages)
    {
        if(languages is null || languages.Count == 0)
        {
            return UnitResult.Failure<ProfileError>(new UpdateLanguagesError());
        }
        
        Languages = languages;
        
        return UnitResult.Success<ProfileError>();
    }

    public UnitResult<ProfileError> UpdateHobbies(IReadOnlyList<string> hobbies)
    {
        if (hobbies.Count > 10)
        {
            return UnitResult.Failure<ProfileError>(new TooManyHobbiesError());
        }
        
        Hobbies  = hobbies;
        return UnitResult.Success<ProfileError>();
    }
}