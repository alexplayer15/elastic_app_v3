using elastic_app_v3.domain.ValueObjects;

namespace elastic_app_v3.domain.Models;
public class ProfileUpdate
{
    public Guid UserId { get; set; }
    public string? Bio { get; set; } = string.Empty;
    public List<Language>? Languages { get; set; } = [];
}
