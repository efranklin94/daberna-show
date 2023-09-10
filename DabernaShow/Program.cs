using DabernaShow;
using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var redisSettings = RedisSettings.FromConfiguration(builder.Configuration);
builder.Services.AddSingleton(redisSettings);
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse($"{redisSettings.Host}:{redisSettings.Port},password={redisSettings.Password}");
    return ConnectionMultiplexer.Connect(config);
});

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

app.Run();
