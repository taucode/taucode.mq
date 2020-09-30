using NUnit.Framework;
using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq.Exceptions;
using TauCode.Working.Exceptions;

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
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();

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
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher(connectionString);

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
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();

            // Act
            messagePublisher.ConnectionString = connectionString;

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.EqualTo(connectionString));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("host=localhost")]
        public void ConnectionString_StoppedThenStarted_CanBeSet(string connectionString)
        {
            // Arrange
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();
            messagePublisher.ConnectionString = "host=localhost";
            messagePublisher.Start();
            messagePublisher.Stop();

            // Act
            messagePublisher.ConnectionString = connectionString;

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.EqualTo(connectionString));
        }

        [Test]
        public void ConnectionString_StartedThenSet_ThrowsMqException()
        {
            // Arrange
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();
            messagePublisher.ConnectionString = "host=localhost";
            messagePublisher.Start();

            // Act
            var connectionString = messagePublisher.ConnectionString;

            var ex = Assert.Throws<MqException>(() => messagePublisher.ConnectionString = "host=127.0.0.1");

            // Assert
            Assert.That(connectionString, Is.EqualTo("host=localhost"));
            Assert.That(ex, Has.Message.EqualTo("Cannot set connection string while publisher is running."));
        }

        [Test]
        public void ConnectionString_DisposedThenSet_ThrowsObjectDisposedException()
        {
            // Arrange
            using IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();
            messagePublisher.ConnectionString = "host=localhost";
            messagePublisher.Name = "my-publisher";

            // Act
            messagePublisher.Dispose();
            var connectionString = messagePublisher.ConnectionString;

            var ex = Assert.Throws<ObjectDisposedException>(() => messagePublisher.ConnectionString = "host=127.0.0.1");

            // Assert
            Assert.That(connectionString, Is.EqualTo("host=localhost"));
            Assert.That(ex, Has.Message.StartWith("Cannot set connection string for disposed publisher."));
            Assert.That(ex.ObjectName, Is.EqualTo("my-publisher"));
        }

        #endregion

        #region Publish(IMessage)

        [Test]
        public void PublishIMessage_ValidStateAndArgument_PublishesAndProperSubscriberHandles()
        {
            // Arrange

            // Act

            // todo - happy path, publishes; subscription with no topic fires, subscriptions without topics don't fire.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessage_ArgumentIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - arg is null, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessage_ArgumentIsNotClass_ThrowsArgumentException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher();
            publisher.ConnectionString = "host=localhost";
            publisher.Start();

            // Act
            var ex = Assert.Throws<ArgumentException>(() => publisher.Publish(new StructMessage()));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"Cannot publish instance of '{typeof(StructMessage).FullName}'. Message type must be a class."));
            Assert.That(ex.ParamName, Is.EqualTo("message"));
        }

        [Test]
        public void PublishIMessage_ArgumentPropertyThrows_Todo()
        {
            // Arrange

            // Act

            // todo - arg throws when props queried, will EasyNetQ handle this?

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessage_NotStarted_ThrowsMqException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher();
            publisher.ConnectionString = "host=localhost";

            // Act
            var ex = Assert.Throws<MqException>(() => publisher.Publish(new HelloMessage()));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Publisher not started."));
        }

        [Test]
        public void PublishIMessage_Disposed_ThrowsObjectDisposedException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
                Name = "my-publisher"
            };

            publisher.Dispose();

            // Act
            var ex = Assert.Throws<ObjectDisposedException>(() => publisher.Publish(new HelloMessage()));

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("my-publisher"));
        }

        #endregion

        #region Publish(IMessage, string)

        [Test]
        public void PublishIMessageString_ValidStateAndArguments_PublishesAndProperSubscriberHandles()
        {
            // Arrange

            // Act

            // todo - happy path, publishes, subscription fired with proper topic; with other topic - not fired; with no topic - not fired.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_MessageIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - message is null, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_MessageIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - message is abstract class, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_MessageIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - message is not class, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_TopicIsNullOrEmpty_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - topic is null or empty, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_NotStarted_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - not started, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessageString_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region Name

        [Test]
        public void Name_NotDisposed_IsChangedAndCanBeRead()
        {
            // Arrange

            // Act

            // todo - when set, reflects, can be any value

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Name_Disposed_CanOnlyBeRead()
        {
            // Arrange

            // Act

            // todo - after disposed, name cannot be set.
            // todo - after disposed, name still can be read

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region State

        [Test]
        public void State_JustCreated_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - just created, eq. to 'stopped'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_Started_EqualsToStarted()
        {
            // Arrange

            // Act
            // todo - started, eq. to 'started'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_Stopped_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - stopped, eq. to 'stopped'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_DisposedJustAfterCreation_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - disposed just after creation, eq. to 'stopped'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_DisposedAfterStarted_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - disposed after started, eq. to 'stopped'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_DisposedAfterStopped_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - disposed after stopped, eq. to 'stopped'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void State_DisposedAfterDisposed_EqualsToStopped()
        {
            // Arrange

            // Act
            // todo - disposed after disposed, eq. to 'disposed'

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region IsDisposed

        [Test]
        public void IsDisposed_JustCreated_EqualsToFalse()
        {
            // Arrange

            // Act
            // todo - just created, eq. to 'false'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_Started_EqualsToFalse()
        {
            // Arrange

            // Act
            // todo - started, eq. to 'false'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_Stopped_EqualsToFalse()
        {
            // Arrange

            // Act
            // todo - stopped, eq. to 'false'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_DisposedJustAfterCreation_EqualsToTrue()
        {
            // Arrange

            // Act
            // todo - disposed just after creation, eq. to 'true'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_DisposedAfterStarted_EqualsToTrue()
        {
            // Arrange

            // Act
            // todo - disposed after started, eq. to 'true'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_DisposedAfterStopped_EqualsToTrue()
        {
            // Arrange

            // Act
            // todo - disposed after stopped, eq. to 'true'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void IsDisposed_DisposedAfterDisposed_EqualsToTrue()
        {
            // Arrange

            // Act
            // todo - disposed after disposed, eq. to 'true'

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region Start()

        [Test]
        public void Start_JustCreated_Starts()
        {
            // Arrange

            // Act
            // todo - just created, starts

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Start_ConnectionStringIsNullOrEmpty_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - con str is null or empty, throws.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Start_Started_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - started, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Start_Stopped_Starts()
        {
            // Arrange

            // Act
            // todo - stopped, starts

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Start_Disposed_ThrowsObjectDisposedException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
                Name = "my-publisher"
            };
            publisher.Dispose();

            // Act
            var ex = Assert.Throws<ObjectDisposedException>(() => publisher.Start());

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("my-publisher"));
        }

        #endregion

        #region Stop()

        [Test]
        public void Stop_JustCreated_ThrowsInappropriateWorkerStateException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
                Name = "my-publisher"
            };

            // Act
            var ex = Assert.Throws<InappropriateWorkerStateException>(() => publisher.Stop());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Inappropriate worker state (Stopped)."));
        }

        [Test]
        public void Stop_Started_Stops()
        {
            // Arrange

            // Act
            // todo - started, stops

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Stop_Stopped_ThrowsInappropriateWorkerStateException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
                Name = "my-publisher"
            };

            publisher.Start();
            publisher.Stop();

            // Act
            var ex = Assert.Throws<InappropriateWorkerStateException>(() => publisher.Stop());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Inappropriate worker state (Stopped)."));
        }

        [Test]
        public void Stop_Disposed_ThrowsObjectDisposedException()
        {
            // Arrange
            using var publisher = new EasyNetQMessagePublisher
            {
                ConnectionString = "host=localhost",
                Name = "my-publisher"
            };
            publisher.Dispose();

            // Act
            var ex = Assert.Throws<ObjectDisposedException>(() => publisher.Stop());

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("my-publisher"));
        }

        #endregion

        #region Dispose()

        [Test]
        public void Dispose_JustCreated_Disposes()
        {
            // Arrange

            // Act
            // todo - just created, disposes

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Dispose_Started_Disposes()
        {
            // Arrange

            // Act
            // todo - started, disposes

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Disposes_Stopped_Disposes()
        {
            // Arrange

            // Act
            // todo - stopped, disposes

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Disposes_Disposed_DoesNothing()
        {
            // Arrange

            // Act
            // todo - disposed, does nothing.

            // Assert
            throw new NotImplementedException();
        }

        #endregion
    }
}
