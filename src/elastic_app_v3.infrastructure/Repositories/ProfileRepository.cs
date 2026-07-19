using Dapper;
using elastic_app_v3.domain.Abstractions;
using elastic_app_v3.domain.Entities;
using elastic_app_v3.domain.ValueObjects;
using elastic_app_v3.infrastructure.Config;
using elastic_app_v3.infrastructure.SqlQueryConstants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using CSharpFunctionalExtensions;
using elastic_app_v3.domain.DTOs;
using elastic_app_v3.domain.Errors;
using elastic_app_v3.domain.Errors.Profile;

namespace elastic_app_v3.infrastructure.Repositories;
public class ProfileRepository(
    IOptions<ElasticDatabaseSettings> elasticAppDatabaseSettings,
    ResiliencePipelineProvider<string> resiliencePipelineProvider
) : IProfileRepository
{
    private readonly string _connectionString = elasticAppDatabaseSettings.Value.GetConnectionString();
    private readonly ResiliencePipeline _resiliencePipeline
            = resiliencePipelineProvider.GetPipeline(ResiliencePolicy.ElasticAppDatabaseResiliencePolicyKey);
    
    public async Task<Result<Profile, ProfileError>> GetProfileByUserId(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await _resiliencePipeline.ExecuteAsync(async token =>
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(token);

                var profileCommand = new CommandDefinition(
                    ProfileSqlConstants.GetProfileByUserId,
                    new { UserId = userId },
                    cancellationToken: token);

                var profileRow = await connection.QuerySingleOrDefaultAsync<ProfileRow>(profileCommand);

                if (profileRow is null)
                {
                    return Result.Failure<Profile, ProfileError>(new NoProfileFoundError(userId));
                }

                var languagesCommand = new CommandDefinition(
                    ProfileSqlConstants.GetLanguagesByUserId,
                    new { UserId = userId },
                    cancellationToken: token);

                var languages = (await connection.QueryAsync<Language>(languagesCommand)).ToList();

                var hobbiesCommand = new CommandDefinition(
                    ProfileSqlConstants.GetHobbiesByUserId,
                    new { UserId = userId },
                    cancellationToken: token);

                var hobbies = (await connection.QueryAsync<string>(hobbiesCommand)).ToList();

                return Result.Success<Profile, ProfileError>(Profile.Rehydrate(profileRow.UserId, profileRow.Bio, languages, hobbies));
            }, cancellationToken);
        }
        catch (SqlException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    //to do: split this to follow SRP
    public async Task<Result<Profile, ProfileError>> UpdateProfile(Profile profile,
        CancellationToken cancellationToken)
    {
        try
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
                            UserId = profile.Id,
                            profile.Bio
                        },
                        transaction,
                        cancellationToken: token
                    );

                    await connection.QuerySingleOrDefaultAsync<string>(updateProfileCommand);

                    var languages = profile.Languages;
                    if (languages.Count > 0)
                    {
                        var deleteLanguagesCommand = new CommandDefinition(
                            ProfileSqlConstants.DeleteProfileLanguages,
                            new { UserId = profile.Id },
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
                                    UserId = profile.Id,
                                    language.Type,
                                    language.Proficiency
                                },
                                transaction,
                                cancellationToken: token
                            );
                            await connection.QuerySingleAsync<Language>(addLanguagesCommand);
                        }
                    }

                    await transaction.CommitAsync(token); //no test checks if the data is actually in the db

                    return Result.Success<Profile, ProfileError>(profile);
                }, cancellationToken);
        }
        catch(Exception ex)
        {
            throw ex; //to do: add logger and meaningful exception
        }
    }
    
    //Does this belong here?
    public async Task<UnitResult<ProfileError>> SaveProfilePicture(Guid userId,
        string objectUrl,
        CancellationToken cancellationToken)
    {
        try
        {
            await _resiliencePipeline.ExecuteAsync(
                async token =>
                {
                    await using var connection = new SqlConnection(_connectionString);

                    await connection.OpenAsync(token);

                    var command = new CommandDefinition(
                        ProfileSqlConstants.UpdateProfilePicture,
                        new
                        {
                            ProfilePictureUrl = objectUrl,
                            UserId = userId
                        },
                        cancellationToken: token
                    );

                    return await connection.QuerySingleOrDefaultAsync(command);
                },
                cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        
        return UnitResult.Success<ProfileError>();
    }
}
