using NUnit.Framework;
using LoggerSidecar.Lib;
using Moq;

namespace ServiceB.Tests
{
    [TestFixture]
    public class ServiceBLoggerIntegrationTests
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
        public async Task ServiceBLogger_LogsErrorMessages()
        {
            // Arrange
            var message = "ServiceB error occurred";

            // Act
            await _logger.Error("serviceB", message);

            // Assert
            var result = _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(result, Is.True);
            Assert.That(logMessage.ServiceName, Is.EqualTo("serviceB"));
            Assert.That(logMessage.LogLevel, Is.EqualTo(LoggerSidecar.Lib.LogLevel.Error));
        }

        [Test]
        public async Task ServiceBLogger_MultipleCreations_AllLogged()
        {
            // Arrange
            var creations = new[] { "Creation 1", "Creation 2", "Creation 3" };

            // Act
            foreach (var creation in creations)
            {
                await _logger.Error("serviceB", creation);
            }

            // Assert
            Assert.That(_logStore.LogMessages.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task ServiceBLogger_RequestBodyLogging()
        {
            // Arrange
            var requestBody = "{\"id\": \"new-123\", \"status\": \"created\", \"timestamp\": \"2024-02-07\"}";

            // Act
            await _logger.Error("serviceB", requestBody);

            // Assert
            _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Contains.Substring("id"));
            Assert.That(logMessage.Message, Contains.Substring("status"));
        }

        [Test]
        public async Task ServiceBLogger_LogMessageTimestamp()
        {
            // Arrange
            var beforeTime = DateTime.UtcNow;

            // Act
            await _logger.Error("serviceB", "Test message");

            // Assert
            var afterTime = DateTime.UtcNow;
            _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.DateTime, Is.GreaterThanOrEqualTo(beforeTime));
            Assert.That(logMessage.DateTime, Is.LessThanOrEqualTo(afterTime));
        }

        [Test]
        public async Task ServiceBLogger_CanLogWarnings()
        {
            // Arrange
            var message = "ServiceB warning";

            // Act
            await _logger.Warn("serviceB", message);

            // Assert
            _logStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.LogLevel, Is.EqualTo(LoggerSidecar.Lib.LogLevel.Warn));
        }

        [Test]
        public async Task ServiceBLogger_ErrorsAndWarnings_MaintainOrder()
        {
            // Arrange
            var errorMsg = "This is an error";
            var warnMsg = "This is a warning";

            // Act
            await _logger.Error("serviceB", errorMsg);
            await _logger.Warn("serviceB", warnMsg);

            // Assert
            _logStore.LogMessages.TryDequeue(out var first);
            Assert.That(first.LogLevel, Is.EqualTo(LoggerSidecar.Lib.LogLevel.Error));
            
            _logStore.LogMessages.TryDequeue(out var second);
            Assert.That(second.LogLevel, Is.EqualTo(LoggerSidecar.Lib.LogLevel.Warn));
        }
    }
}
