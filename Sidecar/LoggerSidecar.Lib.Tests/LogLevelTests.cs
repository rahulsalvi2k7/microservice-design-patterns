using NUnit.Framework;
using LoggerSidecar.Lib;

namespace LoggerSidecar.Tests
{
    [TestFixture]
    public class LogLevelTests
    {
        [Test]
        public void LogLevel_HasInfoValue()
        {
            // Arrange & Act & Assert
            Assert.That(LogLevel.Info, Is.EqualTo(LogLevel.Info));
        }

        [Test]
        public void LogLevel_HasWarnValue()
        {
            // Arrange & Act & Assert
            Assert.That(LogLevel.Warn, Is.EqualTo(LogLevel.Warn));
        }

        [Test]
        public void LogLevel_HasErrorValue()
        {
            // Arrange & Act & Assert
            Assert.That(LogLevel.Error, Is.EqualTo(LogLevel.Error));
        }

        [Test]
        public void LogLevel_Values_AreDifferent()
        {
            // Arrange & Act & Assert
            Assert.That(LogLevel.Info, Is.Not.EqualTo(LogLevel.Warn));
            Assert.That(LogLevel.Warn, Is.Not.EqualTo(LogLevel.Error));
            Assert.That(LogLevel.Info, Is.Not.EqualTo(LogLevel.Error));
        }

        [Test]
        [TestCase(LogLevel.Info)]
        [TestCase(LogLevel.Warn)]
        [TestCase(LogLevel.Error)]
        public void LogLevel_CanBeUsedInComparison(LogLevel level)
        {
            // Arrange & Act & Assert
            Assert.That(level, Is.Not.Null);
        }
    }
}
