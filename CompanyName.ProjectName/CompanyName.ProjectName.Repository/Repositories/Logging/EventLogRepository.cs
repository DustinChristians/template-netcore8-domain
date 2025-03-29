using System;
using System.Threading.Tasks;
using CompanyName.ProjectName.Core.Abstractions.Repositories.Logging;
using CompanyName.ProjectName.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.ProjectName.Repository.Repositories.Logging
{
    public class EventLogRepository : IEventLogRepository
    {
        protected CompanyNameProjectNameContext context;

        public EventLogRepository(CompanyNameProjectNameContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task DeleteLogsOlderThanDateTimeOffsetAsync(DateTimeOffset datetimeoffset)
        {
            return context.Database.ExecuteSqlRawAsync("DELETE FROM EventLog WHERE TimeStamp < {0}", datetimeoffset);
        }
    }
}
