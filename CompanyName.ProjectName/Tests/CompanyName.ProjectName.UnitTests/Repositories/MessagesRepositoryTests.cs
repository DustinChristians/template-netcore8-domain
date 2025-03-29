using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CompanyName.ProjectName.Core.Models.Domain;
using CompanyName.ProjectName.Core.Models.ResourceParameters;
using CompanyName.ProjectName.Repository.Data;
using CompanyName.ProjectName.Repository.Repositories;
using CompanyName.ProjectName.TestUtilities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CompanyName.ProjectName.UnitTests.Repositories
{
    [TestFixture]
    public class MessagesRepositoryTests
    {
        public static IEnumerable<TestCaseData> NotFoundSearchTestCases
        {
            get
            {
                yield return new TestCaseData(new MessagesResourceParameters
                {
                    ChannelId = 3,
                    SearchQuery = null
                });
                yield return new TestCaseData(new MessagesResourceParameters
                {
                    ChannelId = 1,
                    SearchQuery = "Three"
                });
            }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetMessagesAsync_NullParameters_ReturnsArgumentNullException()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                // Assert
                Assert.ThrowsAsync<ArgumentNullException>(async () => await messagesRepository.GetMessagesAsync(null));
            }
        }

        [Test]
        public async Task GetMessagesAsync_AllPropertiesNull_ReturnsAllMessages()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var user = new User()
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var messageOne = new Message()
            {
                Text = "Test Message One",
                ChannelId = 1,
            };
            var messageTwo = new Message()
            {
                Text = "Test Message Two",
                ChannelId = 1,
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add a user because we need a UserId foreign key for the messages
                var usersRepository = new UsersRepository(context, MapperUtilities.GetTestMapper());
                await usersRepository.CreateAsync(user);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                messageOne.UserId = user.Id;
                messageTwo.UserId = user.Id;

                // Add the messages
                await messagesRepository.CreateAsync(messageOne);
                await messagesRepository.CreateAsync(messageTwo);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var parameters = new MessagesResourceParameters { ChannelId = 0, SearchQuery = null };

                // Get messages with null properties for parameters
                var results = await messagesRepository.GetMessagesAsync(parameters);

                // Assert
                Assert.That(results.Count(), Is.EqualTo(2));
                Assert.That(results.FirstOrDefault(x => x.Text == messageOne.Text), Is.Not.Null);
                Assert.That(results.FirstOrDefault(x => x.Text == messageTwo.Text), Is.Not.Null);
            }
        }

        [Test]
        public async Task GetMessagesAsync_ChannelId1_ReturnsMessage()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var user = new User()
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var messageOne = new Message()
            {
                Text = "Test Message One",
                ChannelId = 1,
            };
            var messageTwo = new Message()
            {
                Text = "Test Message Two",
                ChannelId = 2,
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add a user because we need a UserId foreign key for the messages
                var usersRepository = new UsersRepository(context, MapperUtilities.GetTestMapper());
                await usersRepository.CreateAsync(user);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                messageOne.UserId = user.Id;
                messageTwo.UserId = user.Id;

                // Add the messages
                await messagesRepository.CreateAsync(messageOne);
                await messagesRepository.CreateAsync(messageTwo);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var parameters = new MessagesResourceParameters { ChannelId = 1, SearchQuery = null };

                // Get messages with null properties for parameters
                var results = await messagesRepository.GetMessagesAsync(parameters);

                // Assert
                Assert.That(results.Count(), Is.EqualTo(1));
                Assert.That(results.FirstOrDefault(x => x.Text == messageOne.Text), Is.Not.Null);
            }
        }

        [Test]
        public async Task GetMessagesAsync_SearchQueryTwo_ReturnsMessage()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var user = new User()
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var messageOne = new Message()
            {
                Text = "Test Message One",
                ChannelId = 1,
            };
            var messageTwo = new Message()
            {
                Text = "Test Message Two",
                ChannelId = 2,
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add a user because we need a UserId foreign key for the messages
                var usersRepository = new UsersRepository(context, MapperUtilities.GetTestMapper());
                await usersRepository.CreateAsync(user);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                messageOne.UserId = user.Id;
                messageTwo.UserId = user.Id;

                // Add the messages
                await messagesRepository.CreateAsync(messageOne);
                await messagesRepository.CreateAsync(messageTwo);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var parameters = new MessagesResourceParameters { ChannelId = 0, SearchQuery = "Two" };

                // Get messages with null properties for parameters
                var results = await messagesRepository.GetMessagesAsync(parameters);

                // Assert
                Assert.That(results.Count(), Is.EqualTo(1));
                Assert.That(results.FirstOrDefault(x => x.Text == messageTwo.Text), Is.Not.Null);
            }
        }

        [TestCaseSource(nameof(NotFoundSearchTestCases))]
        public async Task GetMessagesAsync_ChannelId3_ReturnsEmptyList(MessagesResourceParameters messagesResourceParameters)
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var user = new User()
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var messageOne = new Message()
            {
                Text = "Test Message One",
                ChannelId = 1,
            };
            var messageTwo = new Message()
            {
                Text = "Test Message Two",
                ChannelId = 2,
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                // Add a user because we need a UserId foreign key for the messages
                var usersRepository = new UsersRepository(context, MapperUtilities.GetTestMapper());
                await usersRepository.CreateAsync(user);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                messageOne.UserId = user.Id;
                messageTwo.UserId = user.Id;

                // Add the messages
                await messagesRepository.CreateAsync(messageOne);
                await messagesRepository.CreateAsync(messageTwo);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var messagesRepository = new MessagesRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var results = await messagesRepository.GetMessagesAsync(messagesResourceParameters);

                // Assert
                Assert.That(results.Count(), Is.EqualTo(0));
            }
        }
    }
}
