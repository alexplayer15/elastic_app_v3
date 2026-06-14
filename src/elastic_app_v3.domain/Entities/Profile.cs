using CSharpFunctionalExtensions;
using elastic_app_v3.domain.ValueObjects;
using Result = FluentResults.Result;

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

    public Result UpdateBio(string bio)
    {
        if (string.IsNullOrWhiteSpace(bio))
        {
            return Result.Fail("Bio cannot be empty if it is being updated.");
        }
        if (bio.Length > 500)
        {
            return Result.Fail("Bio cannot exceed 500 characters.");
        }
        
        Bio = bio;
        
        return Result.Ok();
    }
    public Result UpdateLanguages(List<Language>? languages)
    {
        if(languages is null || languages.Count == 0)
        {
            return Result.Fail("Languages cannot be empty if they are being updated.");
        }
        
        Languages = languages;
        
        return Result.Ok();
    }

    public Result UpdateHobbies(IReadOnlyList<string> hobbies)
    {
        if (hobbies.Count > 10)
        {
            return Result.Fail("You cannot have more than 10 hobbies. No-one has that much time.");
        }
        
        Hobbies  = hobbies;
        return Result.Ok();
    }
}