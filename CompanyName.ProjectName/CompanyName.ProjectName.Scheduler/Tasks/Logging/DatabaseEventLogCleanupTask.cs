using System;
using System.Threading.Tasks;
using CompanyName.ProjectName.Core.Abstractions.Repositories.Logging;
using CompanyName.ProjectName.Core.Abstractions.Tasks.Logging;
using CompanyName.ProjectName.Scheduler.Constants;
using Microsoft.Extensions.Configuration;

namespace CompanyName.ProjectName.Scheduler.Tasks.Logging
{
    public class DatabaseEventLogCleanupTask : IDatabaseEventLogCleanupTask
    {
        private readonly IConfiguration configuration;
        private readonly IEventLogRepository eventLogRepository;

        public DatabaseEventLogCleanupTask(IConfiguration configuration, IEventLogRepository eventLogRepository)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.eventLogRepository = eventLogRepository ?? throw new ArgumentNullException(nameof(eventLogRepository));
        }

        public async Task DeleteOldEventLogsAsync()
        {
            int days = configuration.GetValue<int>(ConfigurationKeys.DeleteDatabaseLogsOlderThanDays);
            await eventLogRepository.DeleteLogsOlderThanDateTimeOffsetAsync(DateTimeOffset.Now.AddDays(-days));
        }
    }
}
