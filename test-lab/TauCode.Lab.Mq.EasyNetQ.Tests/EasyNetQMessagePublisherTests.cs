﻿using NUnit.Framework;
using System;

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

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("host=some-host")]
        public void ConnectionString_StoppedThenStarted_CanBeSet(string connectionString)
        {
            // Arrange
            IEasyNetQMessagePublisher messagePublisher = new EasyNetQMessagePublisher();
            messagePublisher.Start();
            messagePublisher.Stop();

            // Act
            messagePublisher.ConnectionString = connectionString;

            // Assert
            Assert.That(messagePublisher.ConnectionString, Is.EqualTo(connectionString));
        }

        [Test]
        public void ConnectionString_StartedThenSet_ThrowsTodo()
        {
            // Arrange
            
            // Act

            // todo: when started, can be read, but not set.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ConnectionString_DisposedThenSet_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - when disposed, cannot be set, but can be read.

            // Assert
            throw new NotImplementedException();
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
        public void PublishIMessage_ArgumentIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - arg is abstract class, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessage_ArgumentIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - arg is struct, throws

            // Assert
            throw new NotImplementedException();
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
        public void PublishIMessage_NotStarted_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - not started, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void PublishIMessage_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act

            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
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
        public void Start_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region Stop()

        [Test]
        public void Stop_JustCreated_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - just created, throws

            // Assert
            throw new NotImplementedException();
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
        public void Stop_Stopped_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - stopped, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Stop_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
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
