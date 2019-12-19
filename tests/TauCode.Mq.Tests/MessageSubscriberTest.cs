using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Fakes;
using TauCode.Mq.Tests.Handlers;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests
{
    [TestFixture]
    public class MessageSubscriberTest
    {
        public class BadHandlerNoInterfaces
        {
            public void Handle(PersonDto message)
            {
                // idle
            }
        }

        public class BadHandlerMultipleInterfaces : IMessageHandler<PersonDto>, IMessageHandler<CurrencyDto>
        {
            public void Handle(PersonDto message)
            {
                // idle
            }

            public void Handle(CurrencyDto message)
            {
                // idle
            }
        }

        private IMessagePublisher _messagePublisher;
        private StringBuilder _logContent;
        private StringWriter _logWriter;

        private IPersonRoster _personRoster;
        private IRepo _repo;
        private ICurrencyDepot _currencyDepot;
        private IStock _stock;

        private IMessageHandlerWrapperFactory _factory;
        private Mock<FakeMessageHandlerWrapperFactory> _factoryMock;

        [SetUp]
        public void SetUp()
        {
            _messagePublisher = new FakeMessagePublisher();

            _logContent = new StringBuilder();
            _logWriter = new StringWriter(_logContent);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.TextWriter(_logWriter)
                .CreateLogger();

            _personRoster = new PersonRoster();
            _repo = new Repo();
            _currencyDepot = new CurrencyDepot();
            _stock = new Stock();

            _factory = new FakeMessageHandlerWrapperFactory(_personRoster, _repo, _currencyDepot, _stock);
            _factoryMock = new Mock<FakeMessageHandlerWrapperFactory>(_personRoster, _repo, _currencyDepot, _stock);

            FakeTransport.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _messagePublisher.Dispose();
            _logWriter.Dispose();
        }

        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Assert
            Assert.That(subscriber.Name, Is.EqualTo("My subscriber"));
            Assert.That(subscriber.State, Is.EqualTo("NotStarted"));
            Assert.That(subscriber.Subscriptions, Has.Length.Zero);

            subscriber.Dispose();
        }

        [Test]
        public void Constructor_NameIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new FakeMessageSubscriber(null, _factory));

            Assert.That(ex.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void Subscribe_ValidArgument_Subscribes()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Act
            subscriber.Subscribe<RepoFillingHandler>();
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Subscribe<CurrencyDepotHandler>();
            subscriber.Subscribe<StockHandler>();

            // Assert
            var infos = subscriber.Subscriptions;
            Assert.That(infos, Has.Length.EqualTo(2));

            var info = infos[0];
            Assert.That(info.SubscriptionId, Is.EqualTo("My subscriber.TauCode.Mq.Tests.Dto.PersonDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(PersonDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(RepoFillingHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(RosterFillingHandler)));

            info = infos[1];
            Assert.That(info.SubscriptionId, Is.EqualTo("My subscriber.TauCode.Mq.Tests.Dto.CurrencyDto"));
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
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => subscriber.Subscribe(null));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ArgumentDoesNotImplementNeededInterface_ThrowsArgumentNullException()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe<BadHandlerNoInterfaces>());

            Assert.That(ex.Message, Does.StartWith("Type 'TauCode.Mq.Tests.MessageSubscriberTest+BadHandlerNoInterfaces' does not implement the 'IMessageHandler<TMessage>' interface."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_ArgumentDoesNotImplementSingleNeededInterface_ThrowsArgumentNullException()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => subscriber.Subscribe<BadHandlerMultipleInterfaces>());

            Assert.That(ex.Message, Does.StartWith("Type 'TauCode.Mq.Tests.MessageSubscriberTest+BadHandlerMultipleInterfaces' does not implement a single 'IMessageHandler<TMessage>' interface."));
            Assert.That(ex.ParamName, Is.EqualTo("messageHandlerType"));

            subscriber.Dispose();
        }

        [Test]
        public void Subscribe_DuplicateHandlerType_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
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
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

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
            Assert.That(info.SubscriptionId, Is.EqualTo("My subscriber.TauCode.Mq.Tests.Dto.PersonDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(PersonDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(RepoFillingHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(RosterFillingHandler)));

            info = infos[1];
            Assert.That(info.SubscriptionId, Is.EqualTo("My subscriber.TauCode.Mq.Tests.Dto.CurrencyDto"));
            Assert.That(info.MessageType, Is.SameAs(typeof(CurrencyDto)));
            Assert.That(info.MessageHandlerTypes, Has.Length.EqualTo(2));
            Assert.That(info.MessageHandlerTypes[0], Is.SameAs(typeof(CurrencyDepotHandler)));
            Assert.That(info.MessageHandlerTypes[1], Is.SameAs(typeof(StockHandler)));
            
            subscriber.Dispose();

            Assert.That(this.GetLog(), Is.Empty);
        }

        [Test]
        public void Start_AlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
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
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
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
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            // Act
            subscriber.Start();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Started"));

            var infos = subscriber.Subscriptions;
            Assert.That(infos, Is.Empty);

            subscriber.Dispose();

            var log = this.GetLog();
            Assert.That(log, Does.Contain("'TauCode.Mq.Tests.Fakes.FakeMessageSubscriber' instance starts without subscriptions. No messages will be dispatched"));
        }

        [Test]
        public void Dispatch_ValidFlow_DispatchesMessage()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
            subscriber.Subscribe<RosterFillingHandler>();
            
            subscriber.Start();
            _messagePublisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act
            _messagePublisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "ira",
            });

            Task.Delay(100).Wait();

            // Assert
            var persons = _personRoster.GetPersons();
            Assert.That(persons, Has.Length.EqualTo(1));
            var person = persons.Single();
            Assert.That(person.Number, Is.EqualTo(1488));
            Assert.That(person.Name, Is.EqualTo("ira"));

            subscriber.Dispose();
        }

        [Test]
        public void Dispatch_MessageIsNull_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange
            IPersonRoster personRoster = new PersonRoster();
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Start();

            // Act 
            FakeTransport.Instance.ManualRaise(typeof(PersonDto), null);

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Subscription 'My subscriber' caused an error. Exception message: Value cannot be null."));
            Assert.That(personRoster.GetPersons(), Has.Length.Zero);
        }

        [Test]
        public void Dispatch_MessageTypeNotSupported_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange
            IPersonRoster personRoster = new PersonRoster();
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Start();

            // Act 
            FakeTransport.Instance.ManualRaise(typeof(PersonDto), "hello!"); // message of type 'string' which is unexpected

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Subscription 'My subscriber' caused an error. Exception message: Non-supported message type: System.String."));
            Assert.That(personRoster.GetPersons(), Has.Length.Zero);
        }

        [Test]
        public void Dispatch_FactoryIsNull_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange
            IPersonRoster personRoster = new PersonRoster();

            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", null);
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Start();
            _messagePublisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act 
            _messagePublisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "ira",
            });

            Task.Delay(100).Wait();

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("'MessageHandlerWrapperFactory' is null. Message is accepted but won't be handled."));
            Assert.That(personRoster.GetPersons(), Has.Length.Zero);
        }

        [Test]
        public void Dispatch_FactoryRaisesException_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange
            IPersonRoster personRoster = new PersonRoster();

            // factory throws an exception while creating the message handler wrapper
            _factoryMock.Setup(x => x.Create(It.IsAny<Type>())).Throws(new NotSupportedException("I won't create this wrapper!"));

            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factoryMock.Object);
            subscriber.Subscribe<RosterFillingHandler>();

            subscriber.Start();
            _messagePublisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act 
            _messagePublisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "ira",
            });

            Task.Delay(100).Wait();

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("I won't create this wrapper!"));
            Assert.That(personRoster.GetPersons(), Has.Length.Zero);
        }

        [Test]
        public void Dispatch_FactoryReturnsNull_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange

            // factory returns null while creating the message handler wrapper for 'RosterFillingHandler'
            _factoryMock.Setup(x => x.Create(It.Is<Type>(typeArg => typeArg == typeof(RosterFillingHandler)))).Returns((IMessageHandlerWrapper)null);
            _factoryMock
                .Setup(x => x.Create(It.Is<Type>(typeArg => typeArg == typeof(RepoFillingHandler))))
                .Returns(new FakeMessageHandlerWrapper(new RepoFillingHandler(_repo)));

            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factoryMock.Object);
            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();

            subscriber.Start();
            _messagePublisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act 
            _messagePublisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "ira",
            });

            Task.Delay(100).Wait();

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Subscription 'My subscriber' caused an error. Handler type: TauCode.Mq.Tests.Handlers.RosterFillingHandler. Exception message: Created message handler wrapper was null for type 'TauCode.Mq.Tests.Handlers.RosterFillingHandler'."));

            // roster was not filled...
            Assert.That(_personRoster.GetPersons(), Has.Length.Zero);

            // ...but repo was!
            var personsFromRepo = _repo.GetAll();
            Assert.That(personsFromRepo, Has.Length.EqualTo(1));
            var personFromRepo = personsFromRepo.Single();
            Assert.That(personFromRepo.Number, Is.EqualTo(1488));
            Assert.That(personFromRepo.Name, Is.EqualTo("ira"));
        }

        [Test]
        public void Dispatch_HandlerWrapperRaisesException_DoesNotThrowButDoesNotHandleAndWritesToLog()
        {
            // Arrange
            var badWrapper = new Mock<IMessageHandlerWrapper>();
            badWrapper
                .Setup(x => x.Handle(It.IsAny<object>()))
                .Throws(new NotSupportedException("I am a BAD wrapper!"));

            _factoryMock
                .Setup(x => x.Create(It.Is<Type>(typeArg => typeArg == typeof(RosterFillingHandler))))
                .Returns(badWrapper.Object);

            _factoryMock
                .Setup(x => x.Create(It.Is<Type>(typeArg => typeArg == typeof(RepoFillingHandler))))
                .Returns(new FakeMessageHandlerWrapper(new RepoFillingHandler(_repo)));

            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factoryMock.Object);
            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();

            subscriber.Start();
            _messagePublisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act
            _messagePublisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "ira",
            });

            Task.Delay(100).Wait();

            // Assert
            var log = this.GetLog();
            Assert.That(log, Does.Contain("I am a BAD wrapper!"));

            // roster was not filled...
            Assert.That(_personRoster.GetPersons(), Has.Length.Zero);

            // ...but repo was!
            var personsFromRepo = _repo.GetAll();
            Assert.That(personsFromRepo, Has.Length.EqualTo(1));
            var personFromRepo = personsFromRepo.Single();
            Assert.That(personFromRepo.Number, Is.EqualTo(1488));
            Assert.That(personFromRepo.Name, Is.EqualTo("ira"));
        }

        [Test]
        public void Dispose_NotStarted_Disposes()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);

            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();
            subscriber.Subscribe<CurrencyDepotHandler>();
            subscriber.Subscribe<StockHandler>();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Disposed"));
            Assert.That(subscriber.Subscriptions, Has.Length.Zero);
        }

        [Test]
        public void Dispose_Started_Disposes()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
            subscriber.Subscribe<RosterFillingHandler>();
            subscriber.Subscribe<RepoFillingHandler>();

            subscriber.Start();

            // Act
            subscriber.Dispose();

            // Assert
            Assert.That(subscriber.State, Is.EqualTo("Disposed"));
            Assert.That(FakeTransport.Instance.GetSubscriptions(), Is.Empty);
        }

        [Test]
        public void Dispose_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageSubscriber subscriber = new FakeMessageSubscriber("My subscriber", _factory);
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
