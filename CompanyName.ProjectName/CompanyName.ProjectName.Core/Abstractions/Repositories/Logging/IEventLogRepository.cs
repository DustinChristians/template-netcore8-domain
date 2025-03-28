using System;

namespace CompanyName.ProjectName.Core.Abstractions.Repositories.Logging
{
    public interface IEventLogRepository
    {
        void DeleteLogsOlderThanDateTimeOffset(DateTimeOffset datetimeoffset);
    }
}