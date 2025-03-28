using System;
using System.Linq;
using System.Threading;
using CompanyName.ProjectName.Repository.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CompanyName.ProjectName.Repository.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CompanyNameProjectNameContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            using (var semaphore = new Semaphore(initialCount: 1, maximumCount: 1, name: "Database Initialization"))
            {
                semaphore.WaitOne();
                try
                {
                    InitializeDatabase(context);
                }
                finally
                {
                    semaphore.Release();
                }
            }
        }

        private static void InitializeDatabase(CompanyNameProjectNameContext context)
        {
            try
            {
                context.Database.Migrate();
            }
            catch (SqlException exception) when (exception.Number == 1801)
            {
                // Exception number 1801 indicates that the database already exists.
                return;
            }

            System.Diagnostics.Debug.WriteLine("No Migrate Exception");

            if (!context.Users.Any())
            {
                var users = new UserEntity[]
                {
                    new UserEntity
                    {
                        Email = "bill.smith@test.com",
                        FirstName = "Bill",
                        LastName = "Smith",
                        IsActive = true,
                        Guid = Guid.NewGuid(),
                        CreatedBy = 1,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ModifiedBy = 1,
                        ModifiedOn = DateTimeOffset.UtcNow
                    },
                    new UserEntity
                    {
                        Email = "bob.jones@test.com",
                        FirstName = "Bob",
                        LastName = "Jones",
                        IsActive = true,
                        Guid = Guid.NewGuid(),
                        CreatedBy = 1,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ModifiedBy = 1,
                        ModifiedOn = DateTimeOffset.UtcNow
                    },
                };

                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Messages.Any())
            {
                var messages = new MessageEntity[]
                {
                    new MessageEntity
                    {
                        Text = "Hello, Bill!",
                        ChannelId = 1,
                        IsActive = true,
                        Guid = Guid.NewGuid(),
                        UserId = 1,
                        CreatedBy = 1,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ModifiedBy = 1,
                        ModifiedOn = DateTimeOffset.UtcNow
                    },
                    new MessageEntity
                    {
                        Text = "Hi, Bob!",
                        ChannelId = 1,
                        IsActive = true,
                        Guid = Guid.NewGuid(),
                        UserId = 2,
                        CreatedBy = 1,
                        CreatedOn = DateTimeOffset.UtcNow,
                        ModifiedBy = 1,
                        ModifiedOn = DateTimeOffset.UtcNow
                    },
                };

                context.Messages.AddRange(messages);
                context.SaveChanges();
            }

            System.Diagnostics.Debug.WriteLine("InitializeDatabase Complete");
        }
    }
}
