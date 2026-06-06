using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.ValueObjects;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using elastic_app_v3.application.Errors.Profile;
using elastic_app_v3.domain.Models;
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
    
    //to do: split this to follow SRP
    public async Task<Result<Profile>> UpdateProfile(
        ProfileUpdate profileUpdate,
        CancellationToken cancellationToken)
    {
        return await _resiliencePipeline.ExecuteAsync(
            async token =>
            {
                await using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(token);

                await using var transaction = await connection.BeginTransactionAsync(token);

                var updateProfileCommand = new CommandDefinition(
                    ProfileSqlConstants.UpdateBio,
                     new
                     {
                         profileUpdate.UserId,
                         profileUpdate.Bio
                     },
                    transaction,
                    cancellationToken: token
                );

                var updatedBio = await connection.QuerySingleOrDefaultAsync<string>(updateProfileCommand);

                if (!string.IsNullOrEmpty(profileUpdate.Bio) && updatedBio == null)
                {
                    return Result.Fail<Profile>(new UpdateBioError()); 
                }

                var languages = profileUpdate.Languages;
                var updatedLanguages = new List<Language>();
                if (languages is not null)
                {
                    var deleteLanguagesCommand = new CommandDefinition(
                        ProfileSqlConstants.DeleteProfileLanguages,
                        new { UserId = profileUpdate.UserId },
                        transaction,
                        cancellationToken: token
                    );

                    await connection.ExecuteAsync(deleteLanguagesCommand);
                    
                    foreach (Language language in languages)
                    {
                        var addLanguagesCommand = new CommandDefinition(
                            ProfileSqlConstants.AddProfileLanguages,
                            new
                            {
                                profileUpdate.UserId,
                                language.Type,
                                language.Proficiency
                            },
                            transaction,
                            cancellationToken: token
                        );
                        var updatedLanguage = await connection.QuerySingleAsync<Language>(addLanguagesCommand);
                        updatedLanguages.Add(updatedLanguage);
                    }
                }

                await transaction.CommitAsync(token); //no test checks if the data is actually in the db

                return Result.Ok(new Profile()
                {
                    UserId = profileUpdate.UserId,
                    Languages = updatedLanguages,
                    Bio = updatedBio
                });
            }, cancellationToken); //need a try/catch to rollback transactions?
    }
}
