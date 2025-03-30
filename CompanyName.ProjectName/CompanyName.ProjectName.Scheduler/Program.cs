using System;
using CompanyName.ProjectName.Mapping;
using CompanyName.ProjectName.Scheduler;
using CompanyName.ProjectName.Scheduler.Abstractions;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog logger early.
Log.Logger = LoggerConfig.CreateLogger();
builder.Host.UseSerilog();

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("CompanyName.ProjectName.Repository"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add Hangfire server.
builder.Services.AddHangfireServer();

// Register task scheduler.
builder.Services.AddTransient<ITaskScheduler, TaskScheduler>();

// Register shared dependencies (Mapping project) and scheduler tasks.
CompanyName.ProjectName.Mapping.DependencyConfig.Register(builder.Services, builder.Configuration, System.Reflection.Assembly.GetEntryAssembly().GetName().Name);
CompanyName.ProjectName.Scheduler.DependencyConfig.Register(builder.Services);

var app = builder.Build();

// Seed databases (if necessary).
DatabaseConfig.SeedDatabases(app);

// Configure middleware.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
LoggerConfig.Configure(loggerFactory);

app.UseHangfireDashboard();

using (var scope = app.Services.CreateScope())
{
    var taskScheduler = scope.ServiceProvider.GetRequiredService<ITaskScheduler>();
    taskScheduler.ScheduleRecurringTasks();
}

app.UseRouting();

// Recreate logger after database seeding if needed.
Log.Logger = LoggerConfig.CreateLogger();

Log.Information("Starting Up");

app.Run();

// Ensure proper shutdown of Serilog.
Log.CloseAndFlush();
