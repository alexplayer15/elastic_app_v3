namespace elastic_app_v3.SecretsManager
{
    public interface ISecretsManagerClient
    {
        Task<string> GetSecretString(string secretName);
    }
}
