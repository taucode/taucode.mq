using NUnit.Framework;
using Serilog;
using System;
using System.IO;
using System.Text;
using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Handlers;

namespace TauCode.Mq.Tests
{
    [TestFixture]
    public class NullMessageSubscriberTest
    {
        private StringBuilder _logContent;
        private StringWriter _logWriter;

        [SetUp]
        public void SetUp()
        {
            _logContent = new StringBuilder();
            _logWriter = new StringWriter(_logContent);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.TextWriter(_logWriter)
                .CreateLogger();

        }

        [TearDown]
        public void TearDown()
        {
            _logWriter.Dispose();
        }

        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Assert
            Assert.That(subscriber.Name, Is.EqualTo("TauCode.Mq.NullMessageSubscriber"));
            Assert.That(subscriber.State, Is.EqualTo("NotStarted"));
            Assert.That(subscriber.Subscriptions, Has.Length.Zero);

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ValidArgument_Subscribes()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act
            subscriber.Subscribe<RepoFillingHandler>();
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Subscribe<CurrencyDepotHandler>();
            subscriber.Subscribe<StockHandler>();

            // Assert
            var infos = subscriber.Subscriptions;
            Assert.That(infos, Has.Length.EqualTo(2));

            var info = infos[0];
            Assert.That(info.SubscriptionId, Is.EqualTo("TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.PersonDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(PersonDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(RepoFillingHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(RosterFillingHandler)));

            info = infos[1];
            Assert.That(info.SubscriptionId, Is.EqualTo("TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.CurrencyDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(CurrencyDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(CurrencyDepotHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(StockHandler)));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => subscriber.Subscribe(null));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ArgumentDoesNotImplementNeededInterface_ThrowsArgumentNullException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe<MessageSubscriberTest.BadHandlerNoInterfaces>());

            Assert.That(ex.Message, Does.StartWith("Type 'TauCode.Mq.Tests.MessageSubscriberTest+BadHandlerNoInterfaces' does not implement the 'IMessageHandler<TMessage>' interface."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ArgumentDoesNotImplementSingleNeededInterface_ThrowsArgumentNullException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe<MessageSubscriberTest.BadHandlerMultipleInterfaces>());

            Assert.That(ex.Message, Does.StartWith("Type 'TauCode.Mq.Tests.MessageSubscriberTest+BadHandlerMultipleInterfaces' does not implement a single 'IMessageHandler<TMessage>' interface."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_DuplicateHandlerType_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();
            subscriber.Subscribe<RepoFillingHandler>();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() =>
                subscriber.Subscribe<RepoFillingHandler>());

            Assert.That(
                ex.Message,
                Is.EqualTo("There is already a handler of type 'TauCode.Mq.Tests.Handlers.RepoFillingHandler' registered for message type 'TauCode.Mq.Tests.Dto.PersonDto'"));

            subscriber.Dispose();
        }

        [Test]
        public void Start_NotStarted_Starts()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act
            subscriber.Subscribe<RepoFillingHandler>();
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Subscribe<CurrencyDepotHandler>();
            subscriber.Subscribe<StockHandler>();

            subscriber.Start();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Started"));

            var infos = subscriber.Subscriptions;
            Assert.That(infos, Has.Length.EqualTo(2));

            var info = infos[0];
            Assert.That(info.SubscriptionId, Is.EqualTo("TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.PersonDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(PersonDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(RepoFillingHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(RosterFillingHandler)));

            info = infos[1];
            Assert.That(info.SubscriptionId, Is.EqualTo("TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.CurrencyDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(CurrencyDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(CurrencyDepotHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(StockHandler)));

            subscriber.Dispose();

            var log = this.GetLog();

            Assert.That(log, Does.Contain("Starting the 'TauCode.Mq.NullMessageSubscriber' instance."));

            Assert.That(log, Does.Contain("'TauCode.Mq.NullMessageSubscriber' instance subscribes to the 'TauCode.Mq.Tests.Dto.PersonDto' message type. Subscription ID is 'TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.PersonDto'."));
            Assert.That(log, Does.Contain("'TauCode.Mq.NullMessageSubscriber' instance subscribes to the 'TauCode.Mq.Tests.Dto.CurrencyDto' message type. Subscription ID is 'TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.CurrencyDto'."));
        }

        [Test]
        public void Start_AlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();
            subscriber.Start();

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => subscriber.Start());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));

            subscriber.Dispose();
        }

        [Test]
        public void Start_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();
            subscriber.Start();
            subscriber.Dispose();

            // Act
            var ex = Assert.Throws<InvalidOperationException>(() => subscriber.Start());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));
        }

        [Test]
        public void Start_NoSubscriptions_StartsButWritesWarningToLog()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            // Act
            subscriber.Start();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Started"));

            var infos = subscriber.Subscriptions;
            Assert.That(infos, Is.Empty);

            subscriber.Dispose();

            var log = this.GetLog();
            Assert.That(log, Does.Contain("'TauCode.Mq.NullMessageSubscriber' instance starts without subscriptions. No messages will be dispatched"));

        }

        [Test]
        public void Dispose_NotStarted_Disposes()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();

            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();
            subscriber.Subscribe<CurrencyDepotHandler>();
            subscriber.Subscribe<StockHandler>();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Disposed"));
            Assert.That(subscriber.Subscriptions, Has.Length.Zero);

            var log = this.GetLog();

            Assert.That(log, Does.Contain("Disposing the 'TauCode.Mq.NullMessageSubscriber' instance."));
            Assert.Pass(log);
        }

        [Test]
        public void Dispose_Started_Disposes()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();
            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();

            subscriber.Start();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Disposed"));

            var log = this.GetLog();

            Assert.That(log, Does.Contain("Disposing the 'TauCode.Mq.NullMessageSubscriber' instance."));
            Assert.That(log, Does.Contain("Disposing a 'TauCode.Mq.NullMessageSubscriber+NullSubscription' instance. Message type: TauCode.Mq.Tests.Dto.PersonDto. Subscription ID: TauCode.Mq.NullMessageSubscriber.TauCode.Mq.Tests.Dto.PersonDto."));

            Assert.Pass(log);
        }

        [Test]
        public void Dispose_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new NullMessageSubscriber();
            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();

            subscriber.Start();
            subscriber.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => subscriber.Dispose());
            Assert.That(ex.Message, Is.EqualTo("Already in the 'Disposed' state."));
        }

        private string GetLog()
        {
            _logWriter.Flush();
            var log = _logContent.ToString();
            return log;
        }
    }
}
