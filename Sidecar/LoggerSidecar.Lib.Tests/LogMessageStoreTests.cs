using NUnit.Framework;
using LoggerSidecar.Lib;

namespace LoggerSidecar.Tests
{
    [TestFixture]
    public class LogMessageStoreTests
    {
        [Test]
        public void Constructor_CreatesEmptyQueue()
        {
            // Arrange & Act
            var store = new LogMessageStore();

            // Assert
            Assert.That(store.LogMessages.Count, Is.EqualTo(0));
        }

        [Test]
        public void LogMessages_IsNotNull()
        {
            // Arrange & Act
            var store = new LogMessageStore();

            // Assert
            Assert.That(store.LogMessages, Is.Not.Null);
        }

        [Test]
        public void LogMessages_CanEnqueueMessage()
        {
            // Arrange
            var store = new LogMessageStore();
            var message = new LogMessage 
            { 
                Message = "Test", 
                ServiceName = "TestService",
                LogLevel = LogLevel.Info,
                DateTime = DateTime.UtcNow
            };

            // Act
            store.LogMessages.Enqueue(message);

            // Assert
            Assert.That(store.LogMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void LogMessages_CanDequeueMessage()
        {
            // Arrange
            var store = new LogMessageStore();
            var message = new LogMessage 
            { 
                Message = "Test", 
                ServiceName = "TestService",
                LogLevel = LogLevel.Info,
                DateTime = DateTime.UtcNow
            };
            store.LogMessages.Enqueue(message);

            // Act
            var result = store.LogMessages.TryDequeue(out var dequeuedMessage);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(dequeuedMessage, Is.EqualTo(message));
            Assert.That(store.LogMessages.Count, Is.EqualTo(0));
        }

        [Test]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public void LogMessages_CanHandleMultipleMessages(int messageCount)
        {
            // Arrange
            var store = new LogMessageStore();

            // Act
            for (int i = 0; i < messageCount; i++)
            {
                store.LogMessages.Enqueue(new LogMessage 
                { 
                    Message = $"Message {i}",
                    ServiceName = "TestService",
                    LogLevel = LogLevel.Info,
                    DateTime = DateTime.UtcNow
                });
            }

            // Assert
            Assert.That(store.LogMessages.Count, Is.EqualTo(messageCount));
        }

        [Test]
        public void LogMessages_IsConcurrentQueue()
        {
            // Arrange
            var store = new LogMessageStore();

            // Act & Assert
            var queueType = store.LogMessages.GetType();
            Assert.That(queueType.Name, Is.EqualTo("ConcurrentQueue`1"));
        }

        [Test]
        public void LogMessages_IsThreadSafe()
        {
            // Arrange
            var store = new LogMessageStore();
            var tasks = new Task[10];

            // Act
            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    store.LogMessages.Enqueue(new LogMessage 
                    { 
                        Message = $"Message {index}",
                        ServiceName = "TestService",
                        LogLevel = LogLevel.Info,
                        DateTime = DateTime.UtcNow
                    });
                });
            }
            Task.WaitAll(tasks);

            // Assert
            Assert.That(store.LogMessages.Count, Is.EqualTo(10));
        }
    }
}
