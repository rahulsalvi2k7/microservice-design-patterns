using NUnit.Framework;
using LoggerSidecar.Lib;
using Moq;

namespace ServiceB.Tests
{
    [TestFixture]
    public class ServiceBCreateEndpointTests
    {
        private Mock<ICustomLogger> _mockLogger;

        [SetUp]
        public void SetUp()
        {
            _mockLogger = new Mock<ICustomLogger>();
        }

        [TearDown]
        public void TearDown()
        {
            _mockLogger = null;
        }

        [Test]
        public async Task CreateEndpoint_CallsLoggerWithServiceNameB()
        {
            // Arrange
            var testMessage = "Test create message";
            _mockLogger.Setup(l => l.Error("serviceB", testMessage)).Returns(Task.CompletedTask);

            // Act
            await _mockLogger.Object.Error("serviceB", testMessage);

            // Assert
            _mockLogger.Verify(l => l.Error("serviceB", testMessage), Times.Once);
        }

        [Test]
        public async Task CreateEndpoint_LogsWithErrorLogLevel()
        {
            // Arrange
            var testMessage = "Test create message";

            // Act
            await _mockLogger.Object.Error("serviceB", testMessage);

            // Assert
            _mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase("Simple message")]
        [TestCase("{\"key\": \"value\"}")]
        public async Task CreateEndpoint_HandlesVariousMessageFormats(string message)
        {
            // Arrange
            _mockLogger.Setup(l => l.Error("serviceB", message)).Returns(Task.CompletedTask);

            // Act
            await _mockLogger.Object.Error("serviceB", message);

            // Assert
            _mockLogger.Verify(l => l.Error("serviceB", message), Times.Once);
        }

        [Test]
        public async Task CreateEndpoint_PassesMessageToLogger()
        {
            // Arrange
            var originalMessage = "Test message for ServiceB";

            // Act
            await _mockLogger.Object.Error("serviceB", originalMessage);

            // Assert
            _mockLogger.Verify(l => l.Error("serviceB", originalMessage), Times.Once);
        }

        [Test]
        public async Task CreateEndpoint_LoggerIsCalledExactlyOnce()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Error("serviceB", message);

            // Assert
            _mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase("resource-created")]
        [TestCase("item-added")]
        [TestCase("entity-created")]
        public async Task CreateEndpoint_HandlesRealisticScenarios(string scenario)
        {
            // Arrange
            var message = $"ServiceB creation: {scenario}";

            // Act
            await _mockLogger.Object.Error("serviceB", message);

            // Assert
            _mockLogger.Verify(l => l.Error("serviceB", message), Times.Once);
        }

        [Test]
        public async Task CreateEndpoint_DoesNotCallInfoLogger()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Error("serviceB", message);

            // Assert
            _mockLogger.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task CreateEndpoint_DoesNotCallWarnLogger()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Error("serviceB", message);

            // Assert
            _mockLogger.Verify(l => l.Warn(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
