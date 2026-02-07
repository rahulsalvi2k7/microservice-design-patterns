using NUnit.Framework;
using LoggerSidecar.Lib;

namespace LoggerSidecar.Tests
{
    [TestFixture]
    public class LogMessageTests
    {
        [Test]
        public void LogMessage_HasDateTimeProperty()
        {
            // Arrange & Act
            var now = DateTime.UtcNow;
            var message = new LogMessage { DateTime = now };

            // Assert
            Assert.That(message.DateTime, Is.EqualTo(now));
        }

        [Test]
        public void LogMessage_HasLogLevelProperty()
        {
            // Arrange & Act
            var message = new LogMessage { LogLevel = LogLevel.Info };

            // Assert
            Assert.That(message.LogLevel, Is.EqualTo(LogLevel.Info));
        }

        [Test]
        public void LogMessage_HasServiceNameProperty()
        {
            // Arrange & Act
            var serviceName = "TestService";
            var message = new LogMessage { ServiceName = serviceName };

            // Assert
            Assert.That(message.ServiceName, Is.EqualTo(serviceName));
        }

        [Test]
        public void LogMessage_HasMessageProperty()
        {
            // Arrange & Act
            var messageText = "Test message";
            var message = new LogMessage { Message = messageText };

            // Assert
            Assert.That(message.Message, Is.EqualTo(messageText));
        }

        [Test]
        public void LogMessage_CanBeCreatedWithAllProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var serviceName = "TestService";
            var messageText = "Test message";
            var logLevel = LogLevel.Error;

            // Act
            var message = new LogMessage 
            { 
                DateTime = now,
                ServiceName = serviceName,
                Message = messageText,
                LogLevel = logLevel
            };

            // Assert
            Assert.That(message.DateTime, Is.EqualTo(now));
            Assert.That(message.ServiceName, Is.EqualTo(serviceName));
            Assert.That(message.Message, Is.EqualTo(messageText));
            Assert.That(message.LogLevel, Is.EqualTo(logLevel));
        }

        [Test]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        public void LogMessage_SupportsAllLogLevels(LogLevel logLevel)
        {
            // Arrange & Act
            var message = new LogMessage { LogLevel = logLevel };

            // Assert
            Assert.That(message.LogLevel, Is.EqualTo(logLevel));
        }

        [Test]
        public void LogMessage_CanHaveNullProperties()
        {
            // Arrange & Act
            var message = new LogMessage 
            { 
                ServiceName = null,
                Message = null
            };

            // Assert
            Assert.That(message.ServiceName, Is.Null);
            Assert.That(message.Message, Is.Null);
        }

        [Test]
        public void LogMessage_IsARecord()
        {
            // Arrange & Act
            var message1 = new LogMessage 
            { 
                DateTime = DateTime.UtcNow,
                ServiceName = "Test",
                Message = "Test",
                LogLevel = LogLevel.Info
            };
            var message2 = new LogMessage 
            { 
                DateTime = message1.DateTime,
                ServiceName = "Test",
                Message = "Test",
                LogLevel = LogLevel.Info
            };

            // Assert
            Assert.That(message1, Is.EqualTo(message2));
        }
    }
}
