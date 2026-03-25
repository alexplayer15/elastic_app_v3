using elastic_app_v3.dashboard;
using elastic_app_v3.dashboard.Consumer;
using elastic_app_v3.dashboard.Kafka;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.KafkaSettingsName));

builder.Services.AddScoped<IKafkaTopicManager, KafkaTopicManager>();
builder.Services.AddHostedService<EventConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Run();
