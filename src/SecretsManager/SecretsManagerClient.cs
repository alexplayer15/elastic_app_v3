using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace elastic_app_v3.SecretsManager
{
    public class SecretsManagerClient(IAmazonSecretsManager secretsManagerClient) : ISecretsManagerClient
    {
        private readonly IAmazonSecretsManager _secretsManagerClient = secretsManagerClient;
        public async Task<string> GetSecretString(string secretName)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT",
            };

            try
            {
                var response = await _secretsManagerClient.GetSecretValueAsync(request);

                if(response.SecretBinary is not null)
                {
                    throw new Exception();
                }

                if(!string.IsNullOrEmpty(response.SecretString))
                {
                    return response.SecretString;
                }

                throw new InvalidOperationException($"{secretName} was found but does not contain a string value");

            }
            catch (AmazonSecretsManagerException)
            {
                throw;
            }
        }
    }
}
