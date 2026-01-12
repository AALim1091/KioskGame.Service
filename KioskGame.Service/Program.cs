using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Web;
using KioskGame.Service;
using JsonAttribute = NLog.Layouts.JsonAttribute;

var builder = WebApplication.CreateBuilder(args);

#region NlogConfig
var nlogConfig = new LoggingConfiguration();

var jsonLayout = new JsonLayout
{
    Attributes =
    {
        new JsonAttribute("timestamp", "${longdate}"),
        new JsonAttribute("level", "${level:upperCase=true}"),
        new JsonAttribute("logger", "${logger}"),
        new JsonAttribute("message", "${message}"),
        new JsonAttribute("exception", "${exception:format=ToString}"),
        new JsonAttribute("threadId", "${threadid}"),
        new JsonAttribute("threadName", "${threadname}"),
        new JsonAttribute("processId", "${processid}"),
        new JsonAttribute("processName", "${processname}"),
        new JsonAttribute("machineName", "${machinename}")
    },
    IncludeEventProperties = false
};

var fileTarget = new FileTarget("file")
{
    FileName = Path.Combine(Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName, "KioskGame.Service-${shortdate}.tmslog"),
    Layout = jsonLayout,
    ArchiveEvery = FileArchivePeriod.None, 
    KeepFileOpen = false
};

nlogConfig.AddTarget(fileTarget);
nlogConfig.LoggingRules.Clear();
nlogConfig.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, fileTarget, "KioskGame.Service.*");

LogManager.Configuration = nlogConfig;
#endregion

var logger = LogManager.GetCurrentClassLogger();
logger.Info("NLog configured successfully.");
logger.Info("Kiosk Game Service is starting...");

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IEndpointConfiguration, EndpointConfiguration>();
builder.Services.AddSingleton<IClock, Clock>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<PrizeService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

app.UseCors();
app.MapRoutes();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

//TODO: Host on Render
//var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
//app.Urls.Add($"http://*:{port}");


await app.RunAsync().ConfigureAwait(false);
