using System;
using System.Threading.Tasks;

namespace CompanyName.ProjectName.Core.Abstractions.Repositories.Logging
{
    public interface IEventLogRepository
    {
        Task DeleteLogsOlderThanDateTimeOffsetAsync(DateTimeOffset datetimeoffset);
    }
}