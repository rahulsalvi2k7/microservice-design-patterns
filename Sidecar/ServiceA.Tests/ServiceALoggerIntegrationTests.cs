using NUnit.Framework;
using LoggerSidecar.Lib;
using Moq;

namespace ServiceA.Tests
{
    [TestFixture]
    public class ServiceALoggerIntegrationTests
    {
        private CustomLogger _logger;
        private LogMessageStore _logStore;

        [SetUp]
        public void SetUp()
        {
            _logStore = new LogMessageStore();
            _logger = new CustomLogger(_logStore);
        }

        [TearDown]
        public void TearDown()
        {
            _logger = null;
            _logStore = null;
        }

        [Test]
        public async Task ServiceALogger_LogsInfoMessages()
        {
            // Arrange
            var message = "ServiceA operation completed";

            // Act
            await _logger.Info("serviceA", message);

            // Assert
            var result = _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(result, Is.True);
            Assert.That(logMessage.ServiceName, Is.EqualTo("serviceA"));
            Assert.That(logMessage.LogLevel, Is.EqualTo(LoggerSidecar.Lib.LogLevel.Info));
        }

        [Test]
        public async Task ServiceALogger_MultipleUpdates_AllLogged()
        {
            // Arrange
            var updates = new[] { "Update 1", "Update 2", "Update 3" };

            // Act
            foreach (var update in updates)
            {
                await _logger.Info("serviceA", update);
            }

            // Assert
            Assert.That(_logStore.LogMessages.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task ServiceALogger_RequestBodyLogging()
        {
            // Arrange
            var requestBody = "{\"id\": 1, \"name\": \"Item\", \"quantity\": 5}";

            // Act
            await _logger.Info("serviceA", requestBody);

            // Assert
            _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Contains.Substring("id"));
            Assert.That(logMessage.Message, Contains.Substring("name"));
        }

        [Test]
        public async Task ServiceALogger_LogMessageTimestamp()
        {
            // Arrange
            var beforeTime = DateTime.UtcNow;

            // Act
            await _logger.Info("serviceA", "Test message");

            // Assert
            var afterTime = DateTime.UtcNow;
            _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.DateTime, Is.GreaterThanOrEqualTo(beforeTime));
            Assert.That(logMessage.DateTime, Is.LessThanOrEqualTo(afterTime));
        }
    }
}
