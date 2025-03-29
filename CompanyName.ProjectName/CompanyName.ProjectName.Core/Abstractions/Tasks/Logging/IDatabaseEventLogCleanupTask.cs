using System.Threading.Tasks;

namespace CompanyName.ProjectName.Core.Abstractions.Tasks.Logging
{
    public interface IDatabaseEventLogCleanupTask
    {
        Task DeleteOldEventLogsAsync();
    }
}