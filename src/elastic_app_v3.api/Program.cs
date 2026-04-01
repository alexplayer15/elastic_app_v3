using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using elastic_app_v3.api.Routing;
using elastic_app_v3.api.Exceptions;
using elastic_app_v3.api;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition =
        JsonIgnoreCondition.WhenWritingNull;

    options.SerializerOptions.Converters.Add(
    new JsonStringEnumConverter());
});

builder.Services.ConfigureAppServices(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<CustomValidationResultFactory>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowWeb",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                            .WithHeaders("content-type")
                            .WithMethods(HttpMethod.Post.ToString());
                      });
}); // to do: implement reverse proxy

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseCors("AllowWeb");

app.UseAuthentication();
app.UseAuthorization();

RoutingBase.Map(app);

app.Run();


