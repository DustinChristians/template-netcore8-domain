using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompanyName.ProjectName.Repository.Data;
using CompanyName.ProjectName.Repository.Repositories.Logging;
using CompanyName.ProjectName.Scheduler.Constants;
using CompanyName.ProjectName.Scheduler.Tasks.Logging;
using CompanyName.ProjectName.TestUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace CompanyName.ProjectName.UnitTests.ScheduledTasks
{
    /// <summary>
    /// The EventLog table is not an Entity. It is created and written to by Serilog.
    /// That is why raw SQL commands are being used to create the tables and the records.
    /// </summary>
    [TestFixture]
    public class DatabaseEventLogCleanupTaskTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task DeleteOldEventLogsAsync_DeletesLogsOlderThanThreshold()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { ConfigurationKeys.DeleteDatabaseLogsOlderThanDays, "30" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Capture current time and compute expected threshold
            var now = DateTimeOffset.Now;
            var expectedThreshold = now.AddDays(-30);

            // Seed the database with EventLog records
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Create the EventLog table manually
                context.Database.ExecuteSqlRaw("CREATE TABLE EventLog (Id INTEGER PRIMARY KEY AUTOINCREMENT, TimeStamp TEXT NOT NULL);");

                // Insert an old log and a recent log.
                var oldLogTimestamp = now.AddDays(-31);
                var recentLogTimestamp = now.AddDays(-10);
                context.Database.ExecuteSqlRaw("INSERT INTO EventLog (TimeStamp) VALUES ({0})", oldLogTimestamp);
                context.Database.ExecuteSqlRaw("INSERT INTO EventLog (TimeStamp) VALUES ({0})", recentLogTimestamp);
            }

            // Act: run the cleanup task
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var eventLogRepository = new EventLogRepository(context);
                var cleanupTask = new DatabaseEventLogCleanupTask(configuration, eventLogRepository);
                await cleanupTask.DeleteOldEventLogsAsync();
            }

            // Assert: Query the EventLog table using ADO.NET
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TimeStamp FROM EventLog";
                    var timestamps = new List<DateTimeOffset>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var ts = DateTimeOffset.Parse(reader.GetString(0));
                            timestamps.Add(ts);
                        }
                    }

                    Assert.That(timestamps.Count, Is.EqualTo(1));
                    foreach (var ts in timestamps)
                    {
                        Assert.That(ts, Is.GreaterThanOrEqualTo(expectedThreshold));
                    }
                }
            }
        }

        [Test]
        public async Task DeleteOldEventLogsAsync_NoLogsOlderThanThreshold_LeavesAllLogs()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { ConfigurationKeys.DeleteDatabaseLogsOlderThanDays, "30" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Capture current time and compute expected threshold
            var now = DateTimeOffset.Now;
            var expectedThreshold = now.AddDays(-30);

            // Seed the database with only recent logs.
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Create the EventLog table manually
                context.Database.ExecuteSqlRaw("CREATE TABLE EventLog (Id INTEGER PRIMARY KEY AUTOINCREMENT, TimeStamp TEXT NOT NULL);");

                var recentLogTimestamp1 = now.AddDays(-5);
                var recentLogTimestamp2 = now.AddDays(-10);
                context.Database.ExecuteSqlRaw("INSERT INTO EventLog (TimeStamp) VALUES ({0})", recentLogTimestamp1);
                context.Database.ExecuteSqlRaw("INSERT INTO EventLog (TimeStamp) VALUES ({0})", recentLogTimestamp2);
            }

            // Act: run the cleanup task
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var eventLogRepository = new EventLogRepository(context);
                var cleanupTask = new DatabaseEventLogCleanupTask(configuration, eventLogRepository);
                await cleanupTask.DeleteOldEventLogsAsync();
            }

            // Assert: Query the EventLog table using ADO.NET
            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var connection = context.Database.GetDbConnection();
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT TimeStamp FROM EventLog";
                    var timestamps = new List<DateTimeOffset>();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var ts = DateTimeOffset.Parse(reader.GetString(0));
                            timestamps.Add(ts);
                        }
                    }

                    Assert.That(timestamps.Count, Is.EqualTo(2));
                    foreach (var ts in timestamps)
                    {
                        Assert.That(ts, Is.GreaterThanOrEqualTo(expectedThreshold));
                    }
                }
            }
        }
    }
}
