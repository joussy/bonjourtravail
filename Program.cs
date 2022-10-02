using bonjourtravail_api.Services;
using bonjourtravail_api.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<bonjourtravail_api.Settings.MongoDatabaseSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.AddSingleton<IMongoClient>(c =>
{
    var mongoSettings = c.GetService<IOptions<bonjourtravail_api.Settings.MongoDatabaseSettings>>().Value;

    var mongoClient = new MongoClient(
    mongoSettings.ConnectionString);

    return mongoClient;
});

builder.Services.Configure<PoleEmploiSettings>(
    builder.Configuration.GetSection("PoleEmploi"));

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IJobService, JobService>();
builder.Services.AddTransient<IPoleEmploiService, PoleEmploiService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseAuthorization();

app.MapControllers();

app.Run();
