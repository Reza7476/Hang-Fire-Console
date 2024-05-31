using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;



var builder = WebApplication.CreateBuilder(args);
var connectionString = "server=.;Database=HangFireTest;Trusted_Connection=True;Encrypt=false;TrustServerCertificate=true;";

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(4),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(4),
        QueuePollInterval = TimeSpan.FromSeconds(10),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
    });
});


builder.Services.AddHangfireServer();
var app = builder.Build();
app.UseRouting();
app.UseHangfireDashboard("/reza-hangfire");
app.UseEndpoints(endpoint =>
{
    endpoint.MapHangfireDashboard();
});

var backgroundJob = app.Services.GetRequiredService<IBackgroundJobClient>();
//var backgroundJob = app.Services.GetRequiredService<ILogger<Program>>();

//یک job فوری

backgroundJob.Enqueue(() => ConsoleHelper.WriteMessage("reza", ConsoleColor.Green));




//یک
//job
//تکرار شونده
RecurringJob.AddOrUpdate("reza", () => ConsoleHelper.WriteMessage("salam reza", ConsoleColor.Red), Cron.Minutely);
//یک job
//زمان بندی شده
backgroundJob.Schedule(() => ConsoleHelper.WriteMessage("salam arsalan",ConsoleColor.Blue),TimeSpan.FromSeconds(50));
//یک job بعد از 
// اینکه یک job به پایان رسید
var parentJobId = backgroundJob.Enqueue(() =>ConsoleHelper.WriteMessage("salam fatehei",ConsoleColor.Green));
backgroundJob.ContinueJobWith(parentJobId, () => ConsoleHelper.WriteMessage("salam iran",ConsoleColor.Magenta));


app.Run();








public static class ConsoleHelper
{
    public static void WriteMessage(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}