using NUnit.Framework;
using System;
using TauCode.Lab.Mq.EasyNetQ.Tests.ContextFactory;
using TauCode.Mq.Exceptions;
using TauCode.Working;
using TauCode.Working.Exceptions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessageSubscriberTests
    {
        #region ctor

        [Test]
        public void ConstructorOneArgument_ValidArgument_RunsOk()
        {
            // Arrange

            // Act
            // todo - happy path, check name, con.str, status etc.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ConstructorOneArgument_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new EasyNetQMessageSubscriber(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("contextFactory"));
        }

        [Test]
        public void ConstructorTwoArguments_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            // todo - happy path, connection string equals to what we passed, null accepted also.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ConstructorTwoArguments_FactoryIsNull_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - factory is null, throws

            // Assert
            throw new NotImplementedException();
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
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory());

            // Act
            subscriber.ConnectionString = connectionString;

            // Assert
            Assert.That(subscriber.ConnectionString, Is.EqualTo(connectionString));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("host=some-host")]
        public void ConnectionString_StoppedThenStarted_CanBeSet(string connectionString)
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory());
            subscriber.ConnectionString = "host=localhost";
            subscriber.Start();
            subscriber.Stop();

            // Act
            subscriber.ConnectionString = connectionString;

            // Assert
            Assert.That(subscriber.ConnectionString, Is.EqualTo(connectionString));
        }

        [Test]
        public void ConnectionString_StartedThenSet_ThrowsMqException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory());
            subscriber.ConnectionString = "host=localhost";
            subscriber.Start();

            // Act
            var connectionString = subscriber.ConnectionString;

            var ex = Assert.Throws<MqException>(() => subscriber.ConnectionString = "host=127.0.0.1");

            // Assert
            Assert.That(connectionString, Is.EqualTo("host=localhost"));
            Assert.That(ex, Has.Message.EqualTo("Cannot set connection string while subscriber is running."));
        }

        [Test]
        public void ConnectionString_DisposedThenSet_ThrowsTodo()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost")
            {
                Name = "sub",
            };

            // Act
            subscriber.Dispose();

            var ex = Assert.Throws<ObjectDisposedException>(() => subscriber.ConnectionString = "host=127.0.0.1");

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("sub"));
            Assert.That(subscriber.Name, Is.EqualTo("sub"));
        }

        #endregion

        #region ContextFactory

        [Test]
        public void ContextFactory_ThrowsOnCreateContext_Todo()
        {
            // Arrange

            // Act
            // todo - see name of ut.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ReturnsNullOnCreateContext_Todo()
        {
            // Arrange

            // Act
            // todo - see name of ut.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ContextBeginThrows_Todo()
        {
            // Arrange

            // Act
            // todo - IMessageHandlerContext.Begin() throws => todo.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ContextEndThrows_Todo()
        {
            // Arrange

            // Act
            // todo - IMessageHandlerContext.End() throws => todo.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ContextGetServiceThrows_Todo()
        {
            // Arrange

            // Act
            // todo - IMessageHandlerContext.GetService() throws => todo.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ContextGetServiceReturnsNull_Todo()
        {
            // Arrange

            // Act
            // todo - IMessageHandlerContext.GetService() returns null => todo.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void ContextFactory_ContextGetServiceReturnsBadResult_Todo()
        {
            // Arrange

            // Act
            // todo - IMessageHandlerContext.GetService() returns bad result (e.g. not IMessageHandler/IAsyncMessageHandler) => todo.

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region Subscribe(Type)

        [Test]
        public void SubscribeType_SingleSyncHandler_HandlesMessagesWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (sync, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_MultipleSyncHandlers_HandleMessagesWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (sync, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_SingleAsyncHandler_HandlesMessagesWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (async, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_MultipleAsyncHandlers_HandleMessagesWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (async, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsNull_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is null => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is abstract => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is not class => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsNotGenericSyncOrAsyncHandler_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsSyncAfterAsync_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is sync, while there are already async handlers for this type of message => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsAsyncAfterSync_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is async, while there are already sync handlers for this type of message => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeImplementsIMessageHandlerTMessageMoreThanOnce_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements IMessageHandler<TMessage> more than once => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeImplementsIAsyncMessageHandlerTMessageMoreThanOnce_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements IAsyncMessageHandler<TMessage> more than once => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeImplementsBothSyncAndAsync_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements both sync and async handler, same or different TMessage => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_SyncTypeAlreadySubscribed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - type already subscribed => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_AsyncTypeAlreadySubscribed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - type already subscribed => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TMessageIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - TMessage is abstract => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TMessageIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - TMessage is not class => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TMessageCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TMessagePropertyThrows_Todo()
        {
            // Arrange

            // Act
            // todo - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_SyncHandlerCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - sync handler's ctor is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_SyncHandlerHandleThrows_Todo()
        {
            // Arrange

            // Act
            // todo - sync handler's Handle is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_AsyncHandlerCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's ctor is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_AsyncHandlerHandleAsyncThrows_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_AsyncHandlerHandleAsyncIsFaulted_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is faulted => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_AsyncHandlerHandleAsyncIsCanceled_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is canceled => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_Started_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - started, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region Subscribe(Type, string)

        [Test]
        public void SubscribeTypeString_SingleSyncHandler_HandlesMessagesWithProperTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages with proper topic (sync, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_MultipleSyncHandlers_HandleMessagesWithProperTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages with proper topic (sync, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SingleAsyncHandler_HandlesMessagesWithProperTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages with proper topic (async, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_MultipleAsyncHandlers_HandleMessagesWithProperTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages with proper topic (async, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TopicIsNullOrEmpty_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - topic is null or empty => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsNull_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is null => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is abstract => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is not class => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsNotGenericSyncOrAsyncHandler_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsSyncAfterAsyncSameTopic_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is sync, while there are already async handlers for this type of message AND this topic => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsSyncAfterAsyncButThatAsyncHasDifferentTopic_TodoOk()
        {
            // Arrange

            // Act
            // todo - arg is sync, while there are already async handlers for this type of message BUT different topic => no problem

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsSyncAfterAsyncButThatAsyncIsTopicless_TodoOk()
        {
            // Arrange

            // Act
            // todo - arg is sync, while there are already async handlers for this type of message BUT those handlers are topicless => no problem

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsAsyncAfterSyncSameTopic_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg is async, while there are already sync handlers for this type of message AND this topic => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsAsyncAfterSyncButThatSyncHasDifferentTopic_TodoOk()
        {
            // Arrange

            // Act
            // todo - type is async, while there are already sync handlers for this type of message BUT different topic => no problem

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeIsAsyncAfterSyncButThatSyncIsTopicless_TodoOk()
        {
            // Arrange

            // Act
            // todo - type is async, while there are already sync handlers for this type of message BUT those handlers are topicless => no problem

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeImplementsIMessageHandlerTMessageMoreThanOnce_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements IMessageHandler<TMessage> more than once => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeImplementsIAsyncMessageHandlerTMessageMoreThanOnce_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements IAsyncMessageHandler<TMessage> more than once => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TypeImplementsBothSyncAndAsync_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - arg implements both sync and async handler, same or different TMessage => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SyncTypeAlreadySubscribedToTheSameTopic_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SyncTypeAlreadySubscribedButToDifferentTopic_TodoOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SyncTypeAlreadySubscribedButWithoutTopic_TodoOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncTypeAlreadySubscribedToTheSameTopic_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncTypeAlreadySubscribedButToDifferentTopic_TodoOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncTypeAlreadySubscribedButWithoutTopic_TodoOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TMessageIsAbstract_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - TMessage is abstract => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TMessageIsNotClass_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - TMessage is not class => throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TMessageCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_TMessagePropertyThrows_Todo()
        {
            // Arrange

            // Act
            // todo - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SyncHandlerCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - sync handler's ctor is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_SyncHandlerHandleThrows_Todo()
        {
            // Arrange

            // Act
            // todo - sync handler's Handle is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncHandlerCtorThrows_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's ctor is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncHandlerHandleAsyncThrows_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is throwing => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncHandlerHandleAsyncIsFaulted_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is faulted => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_AsyncHandlerHandleAsyncIsCanceled_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is canceled => logs, stops loop gracefully.

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_Started_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - started, throws

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeTypeString_Disposed_ThrowsTodo()
        {
            // Arrange

            // Act
            // todo - disposed, throws

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region GetSubscriptions()

        [Test]
        public void GetSubscriptions_JustCreated_ReturnsEmptyArray()
        {
            // Arrange

            // Act
            // todo - just created, returns empty array

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetSubscriptions_Running_ReturnsSubscriptions()
        {
            // Arrange

            // Act
            // todo - running, returns subscriptions

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetSubscriptions_Stopped_ReturnsSubscriptions()
        {
            // Arrange

            // Act
            // todo - stopped, returns subscriptions

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetSubscriptions_Disposed_ReturnsEmptyArray()
        {
            // Arrange

            // Act
            // todo - disposed, returns empty array

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
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
        }

        [Test]
        public void State_Started_EqualsToStarted()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            subscriber.Start();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Running));
        }

        [Test]
        public void State_Stopped_EqualsToStopped()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");
            subscriber.Start();

            // Act
            subscriber.Stop();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
        }


        [Test]
        public void State_DisposedJustAfterCreation_EqualsToStopped()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory());

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
        }

        [Test]
        public void State_DisposedAfterStarted_EqualsToStopped()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");
            subscriber.Start();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
        }

        [Test]
        public void State_DisposedAfterStopped_EqualsToStopped()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");
            subscriber.Start();
            subscriber.Stop();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
        }

        [Test]
        public void State_DisposedAfterDisposed_EqualsToStopped()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");
            subscriber.Start();
            subscriber.Stop();
            subscriber.Dispose();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo(WorkerState.Stopped));
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
        public void Start_JustCreated_StartsAndHandlesMessages()
        {
            // Arrange

            // Act
            // todo - just created, starts, handles messages

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
        public void Start_Stopped_StartsAndHandlesMessages()
        {
            // Arrange

            // Act
            // todo - stopped, starts, handles messages

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
        public void Stop_JustCreated_ThrowsInappropriateWorkerStateException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };

            // Act
            var ex = Assert.Throws<InappropriateWorkerStateException>(() => subscriber.Stop());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Inappropriate worker state (Stopped)."));
        }

        [Test]
        public void Stop_Started_StopsAndCancelsCurrentAsyncTasks()
        {
            // Arrange

            // Act
            // todo - started, stops, cancels current async tasks, shown in logs

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Stop_Stopped_ThrowsInappropriateWorkerStateException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };

            subscriber.Start();
            subscriber.Stop();

            // Act
            var ex = Assert.Throws<InappropriateWorkerStateException>(() => subscriber.Stop());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Inappropriate worker state (Stopped)."));
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
        public void Dispose_Started_DisposesAndCancelsCurrentAsyncTasks()
        {
            // Arrange

            // Act
            // todo - started, disposes, cancels current async tasks, shown in logs

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
