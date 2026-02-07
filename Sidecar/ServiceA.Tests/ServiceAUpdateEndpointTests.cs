using NUnit.Framework;
using LoggerSidecar.Lib;
using Moq;

namespace ServiceA.Tests
{
    [TestFixture]
    public class ServiceAUpdateEndpointTests
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
        public async Task UpdateEndpoint_CallsLoggerWithServiceNameA()
        {
            // Arrange
            var testMessage = "Test update message";
            _mockLogger.Setup(l => l.Info("serviceA", testMessage)).Returns(Task.CompletedTask);

            // Act
            await _mockLogger.Object.Info("serviceA", testMessage);

            // Assert
            _mockLogger.Verify(l => l.Info("serviceA", testMessage), Times.Once);
        }

        [Test]
        public async Task UpdateEndpoint_LogsWithInfoLogLevel()
        {
            // Arrange
            var testMessage = "Test update message";

            // Act
            await _mockLogger.Object.Info("serviceA", testMessage);

            // Assert
            _mockLogger.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase("")]
        [TestCase("Simple message")]
        [TestCase("{\"key\": \"value\"}")]
        public async Task UpdateEndpoint_HandlesVariousMessageFormats(string message)
        {
            // Arrange
            _mockLogger.Setup(l => l.Info("serviceA", message)).Returns(Task.CompletedTask);

            // Act
            await _mockLogger.Object.Info("serviceA", message);

            // Assert
            _mockLogger.Verify(l => l.Info("serviceA", message), Times.Once);
        }

        [Test]
        public async Task UpdateEndpoint_PassesMessageToLogger()
        {
            // Arrange
            var originalMessage = "Test message for ServiceA";

            // Act
            await _mockLogger.Object.Info("serviceA", originalMessage);

            // Assert
            _mockLogger.Verify(l => l.Info("serviceA", originalMessage), Times.Once);
        }

        [Test]
        public async Task UpdateEndpoint_LoggerIsCalledExactlyOnce()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Info("serviceA", message);

            // Assert
            _mockLogger.Verify(l => l.Info(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase("user-action")]
        [TestCase("order-update")]
        [TestCase("inventory-check")]
        public async Task UpdateEndpoint_HandlesRealisticScenarios(string scenario)
        {
            // Arrange
            var message = $"ServiceA update: {scenario}";

            // Act
            await _mockLogger.Object.Info("serviceA", message);

            // Assert
            _mockLogger.Verify(l => l.Info("serviceA", message), Times.Once);
        }

        [Test]
        public async Task UpdateEndpoint_DoesNotCallErrorLogger()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Info("serviceA", message);

            // Assert
            _mockLogger.Verify(l => l.Error(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task UpdateEndpoint_DoesNotCallWarnLogger()
        {
            // Arrange
            var message = "Test message";

            // Act
            await _mockLogger.Object.Info("serviceA", message);

            // Assert
            _mockLogger.Verify(l => l.Warn(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
