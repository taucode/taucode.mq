using NUnit.Framework;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessagePublisherTests
    {
        #region ctor

        [Test]
        public void Constructor_NoArguments_RunsOk()
        {
            // Arrange

            // Act
            IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.Null);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("host=some-host")]
        public void Constructor_ConnectionString_RunsOk(string connectionString)
        {
            // Arrange

            // Act
            IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher(connectionString);

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.EqualTo(connectionString));
        }

        #endregion

        #region ConnectionString

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("host=some-host")]
        public void ConnectionString_NotStarted_CanBeSet(string connectionString)
        {
            // Arrange
            IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();

            // Act
            messagePublisher.ConnectionString = connectionString;

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.EqualTo(connectionString));
        }

        // todo - when set, changes, null is also ok.
        // todo - when running, cannot be set, but can be read.
        // todo - when disposed, cannot be set, but can be read.

        #endregion

        #region Publish(IMessage)

        // todo - happy path, publishes; subscription with no topic fires, subscriptions without topics don't fire.
        // todo - arg is null, throws
        // todo - arg is abstract class, throws
        // todo - arg is struct, throws
        // todo - not started, throws
        // todo - disposed, throws


        #endregion

        #region Publish(IMessage, string)

        // todo - happy path, publishes, subscription fired with proper topic; with other topic - not fired; with no topic - not fired.
        // todo - arg is null, throws
        // todo - arg is abstract class, throws
        // todo - arg is struct, throws
        // todo - topic is null or empty, throws
        // todo - not started, throws
        // todo - disposed, throws

        #endregion

        #region Name

        // todo - when set, reflects, can be any value
        // todo - after disposed, name cannot be set.
        // todo - after disposed, name still can be read

        #endregion

        #region State

        // todo - just created, eq. to 'stopped'
        // todo - started, eq. to 'started'
        // todo - stopped, eq. to 'stopped'
        // todo - disposed just after creation, eq. to 'stopped'
        // todo - disposed after started, eq. to 'stopped'
        // todo - disposed after stopped, eq. to 'stopped'
        // todo - disposed after disposed, eq. to 'disposed'

        #endregion

        #region IsDisposed

        // todo - just created, eq. to 'false'
        // todo - started, eq. to 'false'
        // todo - stopped, eq. to 'false'
        // todo - disposed just after creation, eq. to 'true'
        // todo - disposed after started, eq. to 'true'
        // todo - disposed after stopped, eq. to 'true'
        // todo - disposed after disposed, eq. to 'true'

        #endregion

        #region Start()

        // todo - just created, starts
        // todo - con str is null or empty, throws.
        // todo - started, throws
        // todo - stopped, starts
        // todo - disposed, throws

        #endregion

        #region Stop()

        // todo - just created, throws
        // todo - started, stops
        // todo - stopped, throws
        // todo - disposed, throws


        #endregion

        #region Dispose()

        // todo - just created, disposes
        // todo - started, disposes
        // todo - stopped, disposes
        // todo - disposed, does nothing.

        #endregion
    }
}
