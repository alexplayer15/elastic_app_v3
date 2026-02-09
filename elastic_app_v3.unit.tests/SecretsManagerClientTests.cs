using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using elastic_app_v3.SecretsManager;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace elastic_app_v3.unit.tests
{
    public class SecretsManagerClientTests
    {
        private readonly IAmazonSecretsManager _mockAmazonSecretsManager = Substitute.For<IAmazonSecretsManager>();
        private readonly ISecretsManagerClient _secretsManagerClient;
        public SecretsManagerClientTests()
        {
            _secretsManagerClient = new SecretsManagerClient(_mockAmazonSecretsManager);
        }

        [Fact]
        public async Task GivenExistingSecretString_WhenGetSecretString_ThenReturnSecretString()
        {
            //Arrange
            var expectedSecretString = @"
            {
                ""PrivateKey"": ""BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA"",
                ""ExpirationInMinutes"": 60,
                ""Issuer"": ""PINEAPPLE"",
                ""Audience"": ""MANGO""
            }";

            _mockAmazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
            .Returns(new GetSecretValueResponse
            {
                SecretString = expectedSecretString
            });

            var secretName = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA";

            //Act
            var secretStringResponse = await _secretsManagerClient.GetSecretString(secretName);

            //Assert
            Assert.NotNull(secretStringResponse);
            Assert.Equal(expectedSecretString, secretStringResponse);
        }

        [Fact]
        public async Task GivenSecretContainsSecretBinary_WhenGetSecretString_ThenReturnException()
        {
            //Arrange
            _mockAmazonSecretsManager.GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
            .Returns(new GetSecretValueResponse
            {
                SecretBinary = new MemoryStream()
            });

            var secretName = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA";

            //Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _secretsManagerClient.GetSecretString(secretName)
            );

            Assert.Equal(
                "A secret binary is present where only a secret string is expected",
                ex.Message
            );
        }

        [Fact]
        public async Task GivenSecretExistsButContainsNoStringValue_WhenGetSecretString_ThenReturnException()
        {
            //Arrange
            _mockAmazonSecretsManager
                .GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
                .Returns(new GetSecretValueResponse
                {
                    SecretString = null
                });

            var secretName = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _secretsManagerClient.GetSecretString(secretName)
            );

            Assert.Equal(
                $"{secretName} was found but does not contain a string value",
                exception.Message
            );
        }

        [Fact]
        public async Task GivenSecretsManagerThrowsAnException_WhenGetSecretString_ThenReturnException()
        {
            //Arrange
            _mockAmazonSecretsManager
                .GetSecretValueAsync(Arg.Any<GetSecretValueRequest>())
                .Throws(new ResourceNotFoundException());

            var secretName = "BANANA-MANGO-PINEAPPLE-CHERRY-GUAVA";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
                () => _secretsManagerClient.GetSecretString(secretName)
            );
        }
    }
}
