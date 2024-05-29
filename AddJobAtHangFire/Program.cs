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

//یک job فوری
backgroundJob.Enqueue(() => Console.WriteLine("hello reza"));
//یک
//job
//تکرار شونده
RecurringJob.AddOrUpdate("رضا دهقانی", () => Console.WriteLine("Recurring job executed"), Cron.Minutely);
//یک job
//زمان بندی شده
backgroundJob.Schedule(() => Console.WriteLine("Scheduled job executed"), TimeSpan.FromMinutes(5));
//یک job بعد از 
// اینکه یک job به پایان رسید
var parentJobId = backgroundJob.Enqueue(() => Console.WriteLine("Parent job executed"));
backgroundJob.ContinueWith(parentJobId, () => Console.WriteLine("Continuation job executed"));


app.Run();








