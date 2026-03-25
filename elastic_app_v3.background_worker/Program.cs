using elastic_app_v3.background_worker.OutboxWorker;
using elastic_app_v3.background_worker.Producer;
using elastic_app_v3.background_worker.Repository;
using elastic_app_v3.background_worker.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.Configure<ElasticDatabaseSettings>(builder.Configuration.GetSection(ElasticDatabaseSettings.ElasticDatabaseSettingsName));

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(KafkaSettings.KafkaSettingsName));

builder.Services.AddScoped<IOutboxEventsRepository, OutboxEventsRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddHostedService<OutboxWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
