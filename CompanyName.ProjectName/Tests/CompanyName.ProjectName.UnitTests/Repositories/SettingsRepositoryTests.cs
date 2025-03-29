using System.Threading.Tasks;
using CompanyName.ProjectName.Core.Models.Domain;
using CompanyName.ProjectName.Repository.Data;
using CompanyName.ProjectName.Repository.Repositories.Settings;
using CompanyName.ProjectName.TestUtilities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CompanyName.ProjectName.UnitTests.Repositories
{
    [TestFixture]
    public class SettingsRepositoryTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("")]
        [TestCase("KeyThatDoesNotExist")]
        [TestCase(null)]
        public async Task GetSettingValue_ReturnsDefaultValue(string key)
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var defaultValue = "default value";

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var value = await settingsRepository.GetSettingValue(key, defaultValue);

                // Assert
                Assert.That(value, Is.EqualTo(defaultValue));
            }
        }

        [Test]
        public async Task GetSettingValue_TestKeyString_ReturnsKeyValue()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var testSetting = new Setting()
            {
                Key = "TestKey",
                Value = "TestValue",
                Type = typeof(string).ToString(),
                DisplayName = "Test Key",
                Description = "For Testing GetSettingValue"
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());
                await settingsRepository.CreateAsync(testSetting);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var value = await settingsRepository.GetSettingValue("TestKey", string.Empty);

                // Assert
                Assert.That(value, Is.EqualTo(testSetting.Value));
            }
        }

        [TestCase("")]
        [TestCase("KeyThatDoesNotExist")]
        [TestCase(null)]
        public async Task TryGetSettingValue_ReturnsNullSuccessfulFalse(string key)
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var result = await settingsRepository.TryGetSettingValue<string>(key);

                // Assert
                Assert.That(result.Value, Is.Null);
                Assert.That(result.Successful, Is.False);
            }
        }

        [Test]
        public async Task TryGetSettingValue_TestKeyString_ReturnsKeyValue()
        {
            // Arrange
            var options = DatabaseUtilities.GetTestDbConextOptions<CompanyNameProjectNameContext>();

            var testSetting = new Setting()
            {
                Key = "TestKey",
                Value = "TestValue",
                Type = typeof(string).ToString(),
                DisplayName = "Test Key",
                Description = "For Testing GetSettingValue"
            };

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());
                await settingsRepository.CreateAsync(testSetting);
            }

            await using (var context = new CompanyNameProjectNameContext(options))
            {
                var settingsRepository = new SettingsRepository(context, MapperUtilities.GetTestMapper());

                // Act
                var result = await settingsRepository.TryGetSettingValue<string>("TestKey");

                // Assert
                Assert.That(result.Value, Is.EqualTo(testSetting.Value));
                Assert.That(result.Successful, Is.True);
            }
        }

        [Test]
        public void TryUpdateSettingValue()
        {
            Assert.Pass();
        }
    }
}
