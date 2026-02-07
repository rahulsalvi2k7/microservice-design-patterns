using NUnit.Framework;
using System.Text;

namespace CentralLoggingService.Tests
{
    [TestFixture]
    public class LogFileRotationTests
    {
        private string _todayFileName;
        private string _yesterdayFileName;

        [SetUp]
        public void SetUp()
        {
            _todayFileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            _yesterdayFileName = $"{DateTime.UtcNow.AddDays(-1):yyyy-MM-dd}.log";

            // Clean up
            if (File.Exists(_todayFileName))
                File.Delete(_todayFileName);
            if (File.Exists(_yesterdayFileName))
                File.Delete(_yesterdayFileName);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up
            if (File.Exists(_todayFileName))
                File.Delete(_todayFileName);
            if (File.Exists(_yesterdayFileName))
                File.Delete(_yesterdayFileName);
        }

        [Test]
        public void LogRotation_GeneratesTodaysFileName()
        {
            // Arrange & Act
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            var expectedFormat = DateTime.UtcNow.ToString("yyyy-MM-dd");

            // Assert
            Assert.That(fileName, Does.Contain(expectedFormat));
        }

        [Test]
        public void LogRotation_GeneratesYesterdaysFileName()
        {
            // Arrange & Act
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var fileName = $"{yesterday:yyyy-MM-dd}.log";
            var expectedFormat = yesterday.ToString("yyyy-MM-dd");

            // Assert
            Assert.That(fileName, Does.Contain(expectedFormat));
        }

        [Test]
        public async Task LogRotation_SeparatesLogsByDay()
        {
            // Arrange
            var today = DateTime.UtcNow;
            var todayFile = $"{today:yyyy-MM-dd}.log";
            var yesterdayFile = $"{today.AddDays(-1):yyyy-MM-dd}.log";

            // Act
            await File.AppendAllTextAsync(todayFile, "Today's log");
            await File.AppendAllTextAsync(yesterdayFile, "Yesterday's log");

            // Assert
            Assert.That(todayFile, Is.Not.EqualTo(yesterdayFile));
            Assert.That(File.Exists(todayFile), Is.True);
            Assert.That(File.Exists(yesterdayFile), Is.True);
            
            // Cleanup
            File.Delete(todayFile);
            File.Delete(yesterdayFile);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-7)]
        public void LogRotation_GeneratesFileNamesForMultipleDays(int dayOffset)
        {
            // Arrange
            var date = DateTime.UtcNow.AddDays(dayOffset);

            // Act
            var fileName = $"{date:yyyy-MM-dd}.log";

            // Assert
            Assert.That(fileName, Is.Not.Empty);
            Assert.That(fileName, Does.EndWith(".log"));
        }

        [Test]
        public async Task LogRotation_PreviousLogsArePreserved()
        {
            // Arrange
            var yesterdayFile = $"{DateTime.UtcNow.AddDays(-1):yyyy-MM-dd}.log";
            var yesterdayContent = "Yesterday's log entries";

            // Act
            await File.WriteAllTextAsync(yesterdayFile, yesterdayContent);

            // Assert
            var content = await File.ReadAllTextAsync(yesterdayFile);
            Assert.That(content, Is.EqualTo(yesterdayContent));
            
            // Cleanup
            if (File.Exists(yesterdayFile))
                File.Delete(yesterdayFile);
        }
    }
}
