using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.domain.Entities;
public class Profile
{
    public Guid UserId { get; set; }
    public string? Bio { get; private set; }
    public List<Language>? Languages { get; private set; }
    public Guid GetUserId() => UserId;
    public List<Language>? GetLanguages() => Languages;
    public void UpdateBio(string? bio)
    {
        if(bio is not null)
        {
            Bio = bio;
        }
    }
    public void UpdateLanguages(List<Language>? languages)
    {
        if(languages is null)
        {
            return;
        }

        if(languages.Count == 0)
        {
            throw new InvalidOperationException("Languages cannot be empty.");
        }

        Languages = languages;
    }
}
