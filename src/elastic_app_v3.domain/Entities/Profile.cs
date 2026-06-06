using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.domain.Entities;
public class Profile
{
    public Guid UserId { get; set; }
    public string? Bio { get; set; }
    public List<Language>? Languages { get; set; }
}
