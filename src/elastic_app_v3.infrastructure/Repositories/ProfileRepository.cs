using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.ValueObjects;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using elastic_app_v3.application.Errors.Profile;
using FluentResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;

namespace elastic_app_v3.infrastructure.Repositories;
public class ProfileRepository(
    IOptions<ElasticDatabaseSettings> elasticAppDatabaseSettings,
    ResiliencePipelineProvider<string> resiliencePipelineProvider) : IProfileRepository
{
    private readonly string _connectionString = elasticAppDatabaseSettings.Value.GetConnectionString();
    private readonly ResiliencePipeline _resiliencePipeline
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.ElasticAppDatabaseResiliencePolicyKey);

    public async Task<Result<Profile>> GetProfileByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await _resiliencePipeline.ExecuteAsync(
            async token =>
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(token);

                var command = new CommandDefinition(
                    ProfileSqlConstants.GetProfileByUserId,
                    new { UserId = userId },
                    cancellationToken: token
                );

                return await connection.QuerySingleOrDefaultAsync<Profile>(command);
            }, cancellationToken
        );

        if (profile is null || profile.UserId == Guid.Empty)
        {
            return Result.Fail(new NoProfileFoundError(userId));
        }

        return profile; //can return Profile directly because of implicit operator in FluentResult package
    }

    //to do: split this to follow SRP
    public async Task<Result<Profile>> UpdateProfile(
        Profile profile,
        CancellationToken cancellationToken)
    {
        return await _resiliencePipeline.ExecuteAsync(
            async token =>
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(token);

                using var transaction = await connection.BeginTransactionAsync(token);

                var updateProfileCommand = new CommandDefinition(
                    ProfileSqlConstants.UpdateProfile,
                     new
                     {
                         profile.UserId,
                         profile.Bio
                     },
                    transaction,
                    cancellationToken: token
                );

                var updatedBio = await connection.QuerySingleOrDefaultAsync<string>(updateProfileCommand);

                if (!string.IsNullOrEmpty(profile.Bio) && updatedBio == null)
                {
                    return Result.Fail<Profile>(new UpdateBioError());
                }

                profile.UpdateBio(updatedBio);

                var languages = profile.GetLanguages();
                if (languages is not null)
                {
                    var deleteLanguagesCommand = new CommandDefinition(
                        ProfileSqlConstants.DeleteProfileLanguages,
                        new { UserId = profile.GetUserId() },
                        transaction,
                        cancellationToken: token
                    );

                    await connection.ExecuteAsync(deleteLanguagesCommand);

                    var insertedLanguages = new List<Language>();
                    foreach (Language language in languages)
                    {
                        var addLanguagesCommand = new CommandDefinition(
                            ProfileSqlConstants.AddProfileLanguages,
                            new
                            {
                                profile.UserId,
                                language.Type,
                                language.Proficiency
                            },
                            transaction,
                            cancellationToken: token
                        );
                        var insertedLanguage = await connection.QuerySingleAsync<Language>(addLanguagesCommand);
                        insertedLanguages.Add(insertedLanguage);
                    }

                    profile.UpdateLanguages(insertedLanguages);
                }

                await transaction.CommitAsync(token); //no test checks if the data is actually in the db

                return profile;

            }, cancellationToken); //need a try/catch to rollback transactions?
    }
}
