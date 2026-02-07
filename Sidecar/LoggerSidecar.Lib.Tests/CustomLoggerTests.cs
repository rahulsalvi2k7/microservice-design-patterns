using NUnit.Framework;
using LoggerSidecar.Lib;

namespace LoggerSidecar.Tests
{
    [TestFixture]
    public class CustomLoggerTests
    {
        private CustomLogger _customLogger;
        private LogMessageStore _logMessageStore;

        [SetUp]
        public void SetUp()
        {
            _logMessageStore = new LogMessageStore();
            _customLogger = new CustomLogger(_logMessageStore);
        }

        [TearDown]
        public void TearDown()
        {
            _logMessageStore = null;
            _customLogger = null;
        }

        [Test]
        public async Task Info_EnqueuesMessageWithInfoLogLevel()
        {
            // Arrange
            var serviceName = "TestService";
            var message = "Test info message";

            // Act
            await _customLogger.Info(serviceName, message);

            // Assert
            Assert.That(_logMessageStore.LogMessages.Count, Is.EqualTo(1));
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Is.EqualTo(message));
            Assert.That(logMessage.ServiceName, Is.EqualTo(serviceName));
            Assert.That(logMessage.LogLevel, Is.EqualTo(LogLevel.Info));
        }

        [Test]
        public async Task Error_EnqueuesMessageWithErrorLogLevel()
        {
            // Arrange
            var serviceName = "TestService";
            var message = "Test error message";

            // Act
            await _customLogger.Error(serviceName, message);

            // Assert
            Assert.That(_logMessageStore.LogMessages.Count, Is.EqualTo(1));
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Is.EqualTo(message));
            Assert.That(logMessage.ServiceName, Is.EqualTo(serviceName));
            Assert.That(logMessage.LogLevel, Is.EqualTo(LogLevel.Error));
        }

        [Test]
        public async Task Warn_EnqueuesMessageWithWarnLogLevel()
        {
            // Arrange
            var serviceName = "TestService";
            var message = "Test warning message";

            // Act
            await _customLogger.Warn(serviceName, message);

            // Assert
            Assert.That(_logMessageStore.LogMessages.Count, Is.EqualTo(1));
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Is.EqualTo(message));
            Assert.That(logMessage.ServiceName, Is.EqualTo(serviceName));
            Assert.That(logMessage.LogLevel, Is.EqualTo(LogLevel.Warn));
        }

        [Test]
        public async Task LogMethods_SetDateTimeUtcNow()
        {
            // Arrange
            var beforeTime = DateTime.UtcNow;

            // Act
            await _customLogger.Info("TestService", "Test message");
            var afterTime = DateTime.UtcNow;

            // Assert
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.DateTime, Is.GreaterThanOrEqualTo(beforeTime));
            Assert.That(logMessage.DateTime, Is.LessThanOrEqualTo(afterTime));
        }

        [Test]
        [TestCase("ServiceA", LogLevel.Info)]
        [TestCase("ServiceB", LogLevel.Error)]
        [TestCase("ServiceC", LogLevel.Warn)]
        public async Task LogMethods_EnqueueMessageWithCorrectProperties(string serviceName, LogLevel expectedLevel)
        {
            // Arrange
            var message = "Test message";

            // Act
            switch (expectedLevel)
            {
                case LogLevel.Info:
                    await _customLogger.Info(serviceName, message);
                    break;
                case LogLevel.Error:
                    await _customLogger.Error(serviceName, message);
                    break;
                case LogLevel.Warn:
                    await _customLogger.Warn(serviceName, message);
                    break;
            }

            // Assert
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.ServiceName, Is.EqualTo(serviceName));
            Assert.That(logMessage.LogLevel, Is.EqualTo(expectedLevel));
            Assert.That(logMessage.Message, Is.EqualTo(message));
        }

        [Test]
        public async Task MultipleLogCalls_EnqueueMessagesInOrder()
        {
            // Arrange
            var messages = new[] { "Message 1", "Message 2", "Message 3" };

            // Act
            await _customLogger.Info("Service1", messages[0]);
            await _customLogger.Error("Service2", messages[1]);
            await _customLogger.Warn("Service3", messages[2]);

            // Assert
            Assert.That(_logMessageStore.LogMessages.Count, Is.EqualTo(3));
            
            _logMessageStore.LogMessages.TryDequeue(out var msg1);
            Assert.That(msg1.Message, Is.EqualTo(messages[0]));
            
            _logMessageStore.LogMessages.TryDequeue(out var msg2);
            Assert.That(msg2.Message, Is.EqualTo(messages[1]));
            
            _logMessageStore.LogMessages.TryDequeue(out var msg3);
            Assert.That(msg3.Message, Is.EqualTo(messages[2]));
        }

        [Test]
        public async Task Info_ReturnsCompletedTask()
        {
            // Arrange & Act
            var task = _customLogger.Info("TestService", "Test message");

            // Assert
            Assert.That(task.IsCompleted, Is.True);
        }

        [Test]
        [TestCase("")]
        [TestCase("Short")]
        [TestCase("This is a very long message that contains many characters and should still work correctly")]
        public async Task Info_HandlesVariousMessageLengths(string message)
        {
            // Arrange & Act
            await _customLogger.Info("TestService", message);

            // Assert
            _logMessageStore.LogMessages.TryDequeue(out var logMessage);
            Assert.That(logMessage.Message, Is.EqualTo(message));
        }

        [Test]
        public async Task ConcurrentCalls_MaintainQueueIntegrity()
        {
            // Arrange
            var tasks = new Task[10];

            // Act
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks[i] = _customLogger.Info($"Service{index}", $"Message {index}");
            }
            await Task.WhenAll(tasks);

            // Assert
            Assert.That(_logMessageStore.LogMessages.Count, Is.EqualTo(10));
        }
    }
}
