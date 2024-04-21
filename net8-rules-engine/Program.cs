
using net8_rules_engine.Configs;
using net8_rules_engine.Contexts;
using net8_rules_engine.Services;
using Microsoft.EntityFrameworkCore;
using net8_rules_engine.Jobs;

var builder = WebApplication.CreateBuilder(args);

//Config Env
builder.Services.Configure<AppConfig>(builder.Configuration);
AppConfig? appConfig = builder.Configuration.Get<AppConfig>();

//Config DB Postgres
builder.Services.AddDbContext<DataDbContext>(options => {
    options.UseNpgsql(appConfig!.PostgreSqlConnectionString!);
});

//Config DI
builder.Services.AddSingleton<GlobalConfig>();
builder.Services.AddSingleton<IEngineSetup, EngineSetup>();
builder.Services.AddScoped<IEngineService, EngineService>();

//Config BgJob
builder.Services.AddHostedService<SetupEngineJob>();
builder.Services.AddHostedService<SyncEngineJob>();

//Config Controllers
builder.Services.AddControllers();

//Config Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

//Run Migration
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataDbContext>();
    db.Database.Migrate();
}

//Run Swagger
app.UseSwagger();
app.UseSwaggerUI();

//Run Https
app.UseHttpsRedirection();

//Run Authorization
app.UseAuthorization();

//Run Controllers
app.MapControllers();

//Run App
app.Run();
