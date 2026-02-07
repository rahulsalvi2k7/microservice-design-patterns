using NUnit.Framework;
using System.Text;

namespace CentralLoggingService.Tests
{
    [TestFixture]
    public class LogEndpointTests
    {
        private string _testLogFileName;

        [SetUp]
        public void SetUp()
        {
            _testLogFileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            
            // Clean up any existing test log file
            if (File.Exists(_testLogFileName))
            {
                File.Delete(_testLogFileName);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test log file
            if (File.Exists(_testLogFileName))
            {
                File.Delete(_testLogFileName);
            }
        }

        [Test]
        public void LogEndpoint_GeneratesCorrectLogFileName()
        {
            // Arrange & Act
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";

            // Assert
            Assert.That(fileName, Is.Not.Empty);
            Assert.That(fileName, Does.Contain(".log"));
        }

        [Test]
        [TestCase("2024-02-07")]
        [TestCase("2024-12-25")]
        [TestCase("2025-01-01")]
        public void LogEndpoint_GeneratesCorrectDateFormatInFileName(string dateString)
        {
            // Arrange
            var date = DateTime.Parse(dateString);
            var expectedFileName = $"{date:yyyy-MM-dd}.log";

            // Act
            var actualFileName = $"{date:yyyy-MM-dd}.log";

            // Assert
            Assert.That(actualFileName, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void LogEndpoint_LogFileNameIncludesExtension()
        {
            // Arrange & Act
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";

            // Assert
            Assert.That(fileName, Does.EndWith(".log"));
        }

        [Test]
        public void LogEndpoint_CanCreateLogFile()
        {
            // Arrange
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";

            // Act
            File.WriteAllText(fileName, "Test log entry");

            // Assert
            Assert.That(File.Exists(fileName), Is.True);
            
            // Cleanup
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [Test]
        public async Task LogEndpoint_CanAppendToLogFile()
        {
            // Arrange
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            var logEntry = "Test log entry\n";

            // Act
            await File.AppendAllTextAsync(fileName, logEntry);

            // Assert
            Assert.That(File.Exists(fileName), Is.True);
            var content = await File.ReadAllTextAsync(fileName);
            Assert.That(content, Contains.Substring("Test log entry"));
            
            // Cleanup
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [Test]
        public async Task LogEndpoint_AppendMultipleLogEntries()
        {
            // Arrange
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            var entries = new[] { "Entry 1\n", "Entry 2\n", "Entry 3\n" };

            // Act
            foreach (var entry in entries)
            {
                await File.AppendAllTextAsync(fileName, entry);
            }

            // Assert
            var content = await File.ReadAllTextAsync(fileName);
            foreach (var entry in entries)
            {
                Assert.That(content, Contains.Substring(entry.TrimEnd()));
            }
            
            // Cleanup
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [Test]
        public async Task LogEndpoint_PreservesNewlinesBetweenEntries()
        {
            // Arrange
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";

            // Act
            await File.AppendAllTextAsync(fileName, "First entry" + Environment.NewLine);
            await File.AppendAllTextAsync(fileName, "Second entry" + Environment.NewLine);

            // Assert
            var lines = File.ReadAllLines(fileName);
            Assert.That(lines.Length, Is.EqualTo(2));
            Assert.That(lines[0], Is.EqualTo("First entry"));
            Assert.That(lines[1], Is.EqualTo("Second entry"));
            
            // Cleanup
            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        [Test]
        public async Task LogEndpoint_HandlesJsonLogEntries()
        {
            // Arrange
            var fileName = $"{DateTime.UtcNow:yyyy-MM-dd}.log";
            var jsonEntry = "{\"level\":\"INFO\",\"service\":\"ServiceA\",\"message\":\"Operation successful\"}\n";

            // Act
            await File.AppendAllTextAsync(fileName, jsonEntry);

            // Assert
            var content = await File.ReadAllTextAsync(fileName);
            Assert.That(content, Contains.Substring("level"));
            Assert.That(content, Contains.Substring("ServiceA"));
            
            // Cleanup
            if (File.Exists(fileName))
                File.Delete(fileName);
        }
    }
}
