using EasyNetQ;
using NUnit.Framework;
using Serilog;
using System;
using System.Text;
using System.Threading.Tasks;
using TauCode.Extensions;
using TauCode.Infrastructure.Time;
using TauCode.Lab.Mq.EasyNetQ.Tests.BadHandlers;
using TauCode.Lab.Mq.EasyNetQ.Tests.ContextFactories;
using TauCode.Lab.Mq.EasyNetQ.Tests.Contexts;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq;
using TauCode.Mq.Exceptions;
using TauCode.Working;
using TauCode.Working.Exceptions;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessageSubscriberTests
    {
        private StringWriterWithEncoding _log;

        [SetUp]
        public void SetUp()
        {
            TimeProvider.Reset();

            _log = new StringWriterWithEncoding(Encoding.UTF8);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.TextWriter(_log)
                .MinimumLevel
                .Debug()
                .CreateLogger();
        }

        private string GetLog() => _log.ToString();

        #region ctor

        [Test]
        public void ConstructorOneArgument_ValidArgument_RunsOk()
        {
            // Arrange
            var factory = new GoodContextFactory();

            // Act
            using var subscriber = new EasyNetQMessageSubscriber(factory);

            // Assert
            Assert.That(subscriber.ConnectionString, Is.Null);
            Assert.That(subscriber.ContextFactory, Is.SameAs(factory));
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
            var factory = new GoodContextFactory();
            var connectionString = "host=localhost";

            // Act
            using var subscriber = new EasyNetQMessageSubscriber(factory, connectionString);

            // Assert
            Assert.That(subscriber.ConnectionString, Is.EqualTo(connectionString));
            Assert.That(subscriber.ContextFactory, Is.SameAs(factory));
        }

        [Test]
        public void ConstructorTwoArguments_FactoryArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new EasyNetQMessageSubscriber(
                null,
                "host=localhost"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("contextFactory"));
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
        public async Task ContextFactory_ThrowsOnCreateContext_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                true,
                false,
                false,
                false,
                false,
                false,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Method 'CreateContext' of factory '{typeof(BadContextFactory).FullName}' threw an exception."));
        }

        [Test]
        public async Task ContextFactory_ReturnsNullOnCreateContext_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                true,
                false,
                false,
                false,
                false,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Method 'CreateContext' of factory '{typeof(BadContextFactory).FullName}' returned 'null'."));
        }

        // todo: message handler failed once, next time goes well.

        [Test]
        public async Task ContextFactory_ContextBeginThrows_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                false,
                true,
                false,
                false,
                false,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Failed to begin."));
        }

        [Test]
        public async Task ContextFactory_ContextEndThrows_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                false,
                false,
                true,
                false,
                false,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Failed to end."));
        }

        [Test]
        public async Task ContextFactory_ContextGetServiceThrows_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                false,
                false,
                false,
                true,
                false,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Method 'GetService' of context '{typeof(BadContext).FullName}' threw an exception."));
            Assert.That(log, Does.Contain("Failed to get service."));
        }

        [Test]
        public async Task ContextFactory_ContextGetServiceReturnsNull_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                false,
                false,
                false,
                false,
                true,
                false,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Method 'GetService' of context '{typeof(BadContext).FullName}' returned 'null'."));
        }

        [Test]
        public async Task ContextFactory_ContextGetServiceReturnsBadResult_LogsException()
        {
            // Arrange
            IMessageHandlerContextFactory factory = new BadContextFactory(
                false,
                false,
                false,
                false,
                false,
                false,
                true,
                false);

            using IMessageSubscriber subscriber = new EasyNetQMessageSubscriber(factory, "host=localhost");
            subscriber.Subscribe(typeof(HelloHandler));

            subscriber.Start();

            using IBus bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(
                new HelloMessage
                {
                    Name = "Geki",
                });

            await Task.Delay(500);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Method 'GetService' of context '{typeof(BadContext).FullName}' returned wrong service of type '{typeof(ByeHandler).FullName}'."));
        }

        #endregion

        #region Subscribe(Type)

        [Test]
        public void SubscribeType_SingleSyncHandler_HandlesMessagesWithAndWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (sync, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_MultipleSyncHandlers_HandleMessagesWithAndWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (sync, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_SingleAsyncHandler_HandlesMessagesWithAndWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (async, single handler).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_MultipleAsyncHandlers_HandleMessagesWithAndWithoutTopic()
        {
            // Arrange

            // Act
            // todo - happy path, starts handling messages without topic (async, multiple handlers).

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SubscribeType_TypeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => subscriber.Subscribe(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
        }

        [Test]
        public void SubscribeType_TypeIsAbstract_ThrowsArgumentException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(typeof(AbstractHandler)));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
            Assert.That(ex, Has.Message.StartWith("'messageHandlerType' cannot be abstract."));
        }

        [Test]
        public void SubscribeType_TypeIsNotClass_ThrowsArgumentException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(typeof(StructHandler)));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
            Assert.That(ex, Has.Message.StartWith("'messageHandlerType' must represent a class."));
        }

        [Test]
        [TestCase(typeof(NonGenericHandler))]
        [TestCase(typeof(NotImplementingHandlerInterface))]
        public void SubscribeType_TypeIsNotGenericSyncOrAsyncHandler_ThrowsArgumentException(Type badHandlerType)
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(badHandlerType));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
            Assert.That(ex, Has.Message.StartWith("'messageHandlerType' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>'. (Parameter 'messageHandlerType')"));
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
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                Name = "sub_created",
                ConnectionString = "host=localhost"
            };

            // Act
            var nameCreated = subscriber.Name;

            subscriber.Start();
            subscriber.Name = "sub_started";

            var nameStarted = subscriber.Name;

            subscriber.Stop();
            subscriber.Name = "sub_stopped";

            var nameStopped = subscriber.Name;

            // Assert
            Assert.That(nameCreated, Is.EqualTo("sub_created"));
            Assert.That(nameStarted, Is.EqualTo("sub_started"));
            Assert.That(nameStopped, Is.EqualTo("sub_stopped"));
        }

        [Test]
        public void Name_Disposed_CanOnlyBeRead()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                Name = "name1"
            };

            // Act
            subscriber.Dispose();

            var name = subscriber.Name;
            var ex = Assert.Throws<ObjectDisposedException>(() => subscriber.Name = "name2");

            // Assert
            Assert.That(name, Is.EqualTo("name1"));
            Assert.That(subscriber.Name, Is.EqualTo("name1"));
            Assert.That(ex.ObjectName, Is.EqualTo("name1"));
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
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };

            // Assert
            Assert.That(subscriber.IsDisposed, Is.False);
        }

        [Test]
        public void IsDisposed_Started_EqualsToFalse()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };

            // Act
            subscriber.Start();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.False);
        }

        [Test]
        public void IsDisposed_Stopped_EqualsToFalse()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };
            subscriber.Start();

            // Act
            subscriber.Stop();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.False);
        }

        [Test]
        public void IsDisposed_DisposedJustAfterCreation_EqualsToTrue()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.True);
        }

        [Test]
        public void IsDisposed_DisposedAfterStarted_EqualsToTrue()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };
            subscriber.Start();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.True);
        }

        [Test]
        public void IsDisposed_DisposedAfterStopped_EqualsToTrue()
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
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.True);
        }

        [Test]
        public void IsDisposed_DisposedAfterDisposed_EqualsToTrue()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "sub"
            };
            subscriber.Dispose();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.IsDisposed, Is.True);
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
        [TestCase(null)]
        [TestCase("")]
        public void Start_ConnectionStringIsNullOrEmpty_ThrowsMqException(string badConnectionString)
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = badConnectionString,
                Name = "sub"
            };

            // Act
            var ex = Assert.Throws<MqException>(() => subscriber.Start());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Cannot start: connection string is null or empty."));
        }

        [Test]
        public void Start_Started_ThrowsInappropriateWorkerStateException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Start();

            // Act
            var ex = Assert.Throws<InappropriateWorkerStateException>(() => subscriber.Start());

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Inappropriate worker state (Running)."));
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
        public void Start_Disposed_ThrowsObjectDisposedException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };
            subscriber.Dispose();

            // Act
            var ex = Assert.Throws<ObjectDisposedException>(() => subscriber.Start());

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("my-subscriber"));
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
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory(), "host=localhost")
            {
                Name = "sub",
            };

            subscriber.Dispose();

            // Act
            var ex = Assert.Throws<ObjectDisposedException>(() => subscriber.Stop());

            // Assert
            Assert.That(ex.ObjectName, Is.EqualTo("sub"));
        }

        #endregion

        #region Dispose()

        [Test]
        public void Dispose_JustCreated_Disposes()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory());

            // Act
            subscriber.Dispose();

            // Assert
            Assert.Pass("Test passed.");
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
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory(), "host=localhost");
            subscriber.Start();
            subscriber.Stop();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.Pass("Test passed.");
        }

        [Test]
        public void Disposes_Disposed_DoesNothing()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory());
            subscriber.Dispose();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.Pass("Test passed.");
        }

        #endregion
    }
}
