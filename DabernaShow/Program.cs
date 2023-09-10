using DabernaShow;
using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ScoreResetService>();

var redisSettings = RedisSettings.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(redisSettings);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse($"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}");
    return ConnectionMultiplexer.Connect(config);
});


builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseRedisStorage($"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}"));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var options = new BackgroundJobServerOptions
{
    WorkerCount = Environment.ProcessorCount * 5 
};
app.UseHangfireServer(options);
RecurringJob.AddOrUpdate<ScoreResetService>("reset-scores",
    x => x.ResetScores(),
    Cron.Weekly,
    //"*/ * * * * *", 
    TimeZoneInfo.Utc 
);


app.Run();
