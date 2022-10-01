using bonjourtravail_api.Services;
using bonjourtravail_api.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDatabaseSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.Configure<PoleEmploiSettings>(
    builder.Configuration.GetSection("PoleEmploi"));

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<JobService>();
builder.Services.AddTransient<IPoleEmploiService, PoleEmploiService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();
app.Run();
