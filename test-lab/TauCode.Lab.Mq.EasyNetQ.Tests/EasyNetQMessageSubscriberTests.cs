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
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Bye.Async;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Bye.Sync;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Async;
using TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Sync;
using TauCode.Lab.Mq.EasyNetQ.Tests.Messages;
using TauCode.Mq;
using TauCode.Mq.Exceptions;
using TauCode.Working;
using TauCode.Working.Exceptions;
using HelloHandler = TauCode.Lab.Mq.EasyNetQ.Tests.Handlers.Hello.Sync.HelloHandler;

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
            MessageRepository.Instance.Clear();

            _log = new StringWriterWithEncoding(Encoding.UTF8);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.TextWriter(_log)
                .MinimumLevel
                .Debug()
                .CreateLogger();

            DecayingMessage.IsPropertyDecayed = false;
            DecayingMessage.IsCtorDecayed = false;
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
            Assert.That(ex, Has.Message.StartWith("'messageHandlerType' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>' in a one-time manner."));
        }


        [Test]
        public void SubscribeType_TypeIsSyncAfterAsync_ThrowsMqException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            subscriber.Subscribe(typeof(HelloAsyncHandler));
            var ex = Assert.Throws<MqException>(() => subscriber.Subscribe(typeof(HelloHandler))); // todo: if previous 'HelloAsyncHandler' was subscribed to some topic, exception won't be thrown.

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Cannot subscribe synchronous handler '{typeof(HelloHandler).FullName}' to message '{typeof(HelloMessage)}' (no topic) because there are asynchronous handlers existing for that subscription."));
        }

        [Test]
        public void SubscribeType_TypeIsAsyncAfterSync_ThrowsMqException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            subscriber.Subscribe(typeof(HelloHandler));
            var ex = Assert.Throws<MqException>(() => subscriber.Subscribe(typeof(HelloAsyncHandler))); // todo: if previous 'HelloHandler' was subscribed to some topic, exception won't be thrown.

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Cannot subscribe asynchronous handler '{typeof(HelloAsyncHandler).FullName}' to message '{typeof(HelloMessage)}' (no topic) because there are synchronous handlers existing for that subscription."));
        }

        [Test]
        public void SubscribeType_TypeImplementsIMessageHandlerTMessageMoreThanOnce_ThrowsArgumentException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(typeof(HelloAndByeHandler)));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"'messageHandlerType' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>' in a one-time manner."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
        }

        [Test]
        public void SubscribeType_TypeImplementsIAsyncMessageHandlerTMessageMoreThanOnce_ThrowsTodo()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(typeof(HelloAndByeAsyncHandler)));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"'messageHandlerType' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>' in a one-time manner."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
        }

        [Test]
        public void SubscribeType_TypeImplementsBothSyncAndAsync_ThrowsArgumentException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(typeof(BothSyncAndAsyncHandler)));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"'messageHandlerType' must implement either 'IMessageHandler<TMessage>' or 'IAsyncMessageHandler<TMessage>' in a one-time manner."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));
        }

        [Test]
        public void SubscribeType_SyncTypeAlreadySubscribed_ThrowsMqException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            subscriber.Subscribe(typeof(HelloHandler));

            // Act
            var ex = Assert.Throws<MqException>(() => subscriber.Subscribe(typeof(HelloHandler))); // todo: there would be no error if previous subscription was with some topic.

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Handler type '{typeof(HelloHandler).FullName}' already registered for message type '{typeof(HelloMessage).FullName}' (no topic)."));
        }

        [Test]
        public void SubscribeType_AsyncTypeAlreadySubscribed_ThrowsMqException()
        {
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            subscriber.Subscribe(typeof(HelloAsyncHandler));

            // Act
            var ex = Assert.Throws<MqException>(() => subscriber.Subscribe(typeof(HelloAsyncHandler))); // todo: there would be no error if previous subscription was with some topic.

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Handler type '{typeof(HelloAsyncHandler).FullName}' already registered for message type '{typeof(HelloMessage).FullName}' (no topic)."));
        }

        [Test]
        [TestCase(typeof(AbstractMessageHandler))]
        [TestCase(typeof(AbstractMessageAsyncHandler))]
        public void SubscribeType_TMessageIsAbstract_ThrowsArgumentException(Type badHandlerType)
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(badHandlerType));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"Cannot handle abstract message type '{typeof(AbstractMessage).FullName}'."));
            Assert.That(ex.ParamName, Is.EqualTo("messageType"));
        }

        [Test]
        [TestCase(typeof(StructMessageHandler))]
        [TestCase(typeof(StructMessageAsyncHandler))]
        public void SubscribeType_TMessageIsNotClass_ThrowsArgumentException(Type badHandlerType)
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe(badHandlerType));

            // Assert
            Assert.That(ex, Has.Message.StartWith($"Cannot handle non-class message type '{typeof(StructMessage).FullName}'."));
            Assert.That(ex.ParamName, Is.EqualTo("messageType"));
        }

        [Test]
        public async Task SubscribeType_TMessageCtorThrows_LogsException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            using var bus = RabbitHutch.CreateBus("host=localhost");
            var message = new DecayingMessage
            {
                DecayedProperty = "fresh",
            };

            DecayingMessage.IsCtorDecayed = true;

            subscriber.Subscribe(typeof(DecayingMessageHandler));
            subscriber.Start();

            // Act
            bus.Publish(message);
            await Task.Delay(300);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Alas Ctor Decayed!"));
        }

        [Test]
        public async Task SubscribeType_TMessagePropertyThrows_LogsException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            using var bus = RabbitHutch.CreateBus("host=localhost");
            var message = new DecayingMessage
            {
                DecayedProperty = "fresh",
            };

            DecayingMessage.IsPropertyDecayed = true;

            subscriber.Subscribe(typeof(DecayingMessageHandler));
            subscriber.Start();
            
            // Act
            bus.Publish(message);
            await Task.Delay(300);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Alas Property Decayed!"));
        }

        [Test]
        public async Task SubscribeType_SyncHandlerHandleThrows_LogsException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            using var bus = RabbitHutch.CreateBus("host=localhost");

            var message = new HelloMessage
            {
                Name = "Big Fish",
            };

            subscriber.Subscribe(typeof(HelloHandler)); // #0, will handle
            subscriber.Subscribe(typeof(FishHaterHandler)); // #1, will throw
            subscriber.Subscribe(typeof(WelcomeHandler)); // #2, will handle

            subscriber.Start();

            // Act
            bus.Publish(message);
            await Task.Delay(300);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Hello sync, Big Fish!"));
            Assert.That(log, Does.Contain("I hate you sync, 'Big Fish'! Exception thrown!"));
            Assert.That(log, Does.Contain("Welcome sync, Big Fish!"));
        }

        [Test]
        public async Task SubscribeType_AsyncHandlerHandleAsyncThrows_LogsException()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(
                new GoodContextFactory(),
                "host=localhost");

            using var bus = RabbitHutch.CreateBus("host=localhost");

            var message = new HelloMessage
            {
                Name = "Big Fish",
            };

            subscriber.Subscribe(typeof(HelloAsyncHandler)); // #0, will handle
            subscriber.Subscribe(typeof(FishHaterAsyncHandler)); // #1, will throw
            subscriber.Subscribe(typeof(WelcomeAsyncHandler)); // #2, will handle

            subscriber.Start();

            // Act
            bus.Publish(message);
            await Task.Delay(300);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Hello async, Big Fish!"));
            Assert.That(log, Does.Contain("I hate you async, 'Big Fish'! Exception thrown!"));
            Assert.That(log, Does.Contain("Welcome async, Big Fish!"));
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
        public async Task SubscribeType_AsyncHandlerCanceledOrFaulted_RestDoRun()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloAsyncHandler)); // #0 will say 'hello'
            subscriber.Subscribe(typeof(CancelingHelloAsyncHandler)); // #1 will cancel with message
            subscriber.Subscribe(typeof(FaultingHelloAsyncHandler)); // #2 will fault with message
            subscriber.Subscribe(typeof(WelcomeAsyncHandler)); // #3 will say 'welcome', regardless of #1 canceled and #2 faulted.

            subscriber.Start();

            using var bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(new HelloMessage("Ira"));

            await Task.Delay(200);

            // Assert
            var log = this.GetLog();

            Assert.That(log, Does.Contain("Hello async, Ira!")); // #0
            Assert.That(log, Does.Contain("Sorry, I am cancelling async, Ira...")); // #1
            Assert.That(log, Does.Contain("Sorry, I am faulting async, Ira...")); // #2
            Assert.That(log, Does.Contain("Welcome async, Ira!")); // #3
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
        public void SubscribeTypeString_SyncHandlerHandleThrows_Todo()
        {
            // Arrange

            // Act
            // todo - sync handler's Handle is throwing => logs, stops loop gracefully.

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

        // todo: sync handler throwing => rest of them working.

        [Test]
        public async Task SubscribeTypeString_AsyncHandlerHandleAsyncIsCanceled_Todo()
        {
            // Arrange

            // Act
            // todo - async handler's HandleAsync is faulted => logs, stops loop gracefully.

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
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            // Act
            var subscriptions = subscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Is.Empty);
        }

        [Test]
        public void GetSubscriptions_Running_ReturnsSubscriptions()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloHandler));
            subscriber.Subscribe(typeof(WelcomeHandler));

            subscriber.Subscribe(typeof(ByeHandler));

            subscriber.Start();

            // Act
            var subscriptions = subscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Has.Count.EqualTo(2));

            var info0 = subscriptions[0];
            Assert.That(info0.MessageType, Is.EqualTo(typeof(HelloMessage)));
            Assert.That(info0.Topic, Is.Null);
            CollectionAssert.AreEqual(
                new[] { typeof(HelloHandler), typeof(WelcomeHandler) },
                info0.MessageHandlerTypes);

            var info1 = subscriptions[1];
            Assert.That(info1.MessageType, Is.EqualTo(typeof(ByeMessage)));
            Assert.That(info1.Topic, Is.Null);
            CollectionAssert.AreEqual(
                new[] { typeof(ByeHandler), },
                info1.MessageHandlerTypes);
        }

        [Test]
        public void GetSubscriptions_Stopped_ReturnsSubscriptions()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloHandler));
            subscriber.Subscribe(typeof(WelcomeHandler));

            subscriber.Subscribe(typeof(ByeHandler));

            subscriber.Start();
            subscriber.Stop();

            // Act
            var subscriptions = subscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Has.Count.EqualTo(2));

            var info0 = subscriptions[0];
            Assert.That(info0.MessageType, Is.EqualTo(typeof(HelloMessage)));
            Assert.That(info0.Topic, Is.Null);
            CollectionAssert.AreEqual(
                new[] { typeof(HelloHandler), typeof(WelcomeHandler) },
                info0.MessageHandlerTypes);

            var info1 = subscriptions[1];
            Assert.That(info1.MessageType, Is.EqualTo(typeof(ByeMessage)));
            Assert.That(info1.Topic, Is.Null);
            CollectionAssert.AreEqual(
                new[] { typeof(ByeHandler), },
                info1.MessageHandlerTypes);
        }

        [Test]
        public async Task GetSubscriptions_Disposed_ReturnsEmptyArray()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloHandler));
            subscriber.Subscribe(typeof(WelcomeHandler));

            subscriber.Start();

            // Act
            subscriber.Dispose();
            await Task.Delay(200);

            var subscriptions = subscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Is.Empty);
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
        public async Task Start_JustCreated_StartsAndHandlesMessages()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloHandler));
            subscriber.Subscribe(typeof(WelcomeHandler));

            subscriber.Subscribe(typeof(ByeAsyncHandler));
            subscriber.Subscribe(typeof(BeBackAsyncHandler));

            subscriber.Start();

            using var bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(new HelloMessage("Ira"));
            bus.Publish(new ByeMessage("Olia"));

            await Task.Delay(200);

            // Assert
            var log = this.GetLog();

            Assert.That(log, Does.Contain("Hello sync, Ira!"));
            Assert.That(log, Does.Contain("Welcome sync, Ira!"));

            Assert.That(log, Does.Contain("Bye async, Olia!"));
            Assert.That(log, Does.Contain("Be back async, Olia!"));
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
        public async Task Start_Stopped_StartsAndHandlesMessages()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory())
            {
                ConnectionString = "host=localhost",
                Name = "my-subscriber"
            };

            subscriber.Subscribe(typeof(HelloHandler));
            subscriber.Subscribe(typeof(WelcomeHandler));

            subscriber.Subscribe(typeof(ByeAsyncHandler));
            subscriber.Subscribe(typeof(BeBackAsyncHandler));

            subscriber.Start();

            using var bus = RabbitHutch.CreateBus("host=localhost");

            // Act
            bus.Publish(new HelloMessage("Ira"));
            bus.Publish(new ByeMessage("Olia"));

            await Task.Delay(200);

            subscriber.Stop();
            await Task.Delay(200);

            subscriber.Start();

            bus.Publish(new HelloMessage("Manuela"));
            bus.Publish(new ByeMessage("Alex"));

            await Task.Delay(200);

            // Assert
            var log = this.GetLog();

            Assert.That(log, Does.Contain("Hello sync, Manuela!"));
            Assert.That(log, Does.Contain("Welcome sync, Manuela!"));

            Assert.That(log, Does.Contain("Bye async, Alex!"));
            Assert.That(log, Does.Contain("Be back async, Alex!"));
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
        public async Task Stop_Started_StopsAndCancelsCurrentAsyncTasks()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory(), "host=localhost");
            subscriber.Subscribe(typeof(HelloAsyncHandler));
            subscriber.Start();

            using var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Publish(new HelloMessage()
            {
                Name = "Koika",
                MillisecondsTimeout = 3000,
            });

            // Act
            await Task.Delay(200); // let 'HelloAsyncHandler' start.

            subscriber.Stop(); // should cancel 'HelloAsyncHandler'

            await Task.Delay(100);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Async handler '{typeof(HelloAsyncHandler)}' got canceled."));
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
        public async Task Dispose_Started_DisposesAndCancelsCurrentAsyncTasks()
        {
            // Arrange
            using var subscriber = new EasyNetQMessageSubscriber(new GoodContextFactory(), "host=localhost");
            subscriber.Subscribe(typeof(HelloAsyncHandler));
            subscriber.Start();

            using var bus = RabbitHutch.CreateBus("host=localhost");
            bus.Publish(new HelloMessage()
            {
                Name = "Koika",
                MillisecondsTimeout = 3000,
            });

            // Act
            await Task.Delay(200); // let 'HelloAsyncHandler' start.

            subscriber.Dispose(); // should cancel 'HelloAsyncHandler'

            await Task.Delay(100);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain($"Async handler '{typeof(HelloAsyncHandler)}' got canceled."));
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
