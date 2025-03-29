using CompanyName.ProjectName.Core.Abstractions.Tasks.Logging;
using CompanyName.ProjectName.Scheduler.Abstractions;
using CompanyName.ProjectName.Scheduler.Constants;
using CompanyName.ProjectName.Scheduler.Tasks.Logging;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace CompanyName.ProjectName.Scheduler
{
    public class TaskScheduler : ITaskScheduler
    {
        private readonly IConfiguration configuration;
        private readonly IDatabaseEventLogCleanupTask databaseEventLogCleanupTask;

        public TaskScheduler(IConfiguration configuration, IDatabaseEventLogCleanupTask databaseEventLogCleanupTask)
        {
            this.configuration = configuration;
            this.databaseEventLogCleanupTask = databaseEventLogCleanupTask;
        }

        public void ScheduleRecurringTasks()
        {
            RecurringJob.RemoveIfExists(nameof(DatabaseEventLogCleanupTask.DeleteOldEventLogsAsync));
            RecurringJob.AddOrUpdate<IDatabaseEventLogCleanupTask>(
                nameof(DatabaseEventLogCleanupTask),
                task => task.DeleteOldEventLogsAsync(),
                configuration.GetSection(ConfigurationKeys.DatabaseEventLogCleanupTaskCronExpression).Value);

            // Schedule more tasks here
        }
    }
}
