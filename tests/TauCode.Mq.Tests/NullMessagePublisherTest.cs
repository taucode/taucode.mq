using NUnit.Framework;
using Serilog;
using System;
using System.IO;
using System.Text;
using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests
{
    [TestFixture]
    public class NullMessagePublisherTest
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
        public void Constructor_NoArguments_RunsOk()
        {
            // Arrange

            // Act
            IMessagePublisher publisher = new NullMessagePublisher();

            // Assert
            Assert.That(publisher.State, Is.EqualTo("NotStarted"));
            Assert.That(publisher.MessageTypes, Is.Empty);

            publisher.Dispose();
        }

        [Test]
        public void MessageTypes_NoArguments_ReturnsExcpectedResult()
        {
            // Arrange

            // Act
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Assert
            CollectionAssert.AreEquivalent(publisher.MessageTypes, new[] { typeof(PersonDto), typeof(CurrencyDto) });

            publisher.Dispose();
        }

        [Test]
        public void Start_NotStarted_Starts()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Assert
            Assert.That(publisher.State, Is.EqualTo("Started"));
            CollectionAssert.AreEquivalent(new[] { typeof(PersonDto), typeof(CurrencyDto) }, publisher.MessageTypes);

            publisher.Dispose();

            var log = this.GetLog();
            Assert.That(log, Does.Contain("Starting the 'TauCode.Mq.NullMessagePublisher' instance."));
        }

        [Test]
        public void Start_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => publisher.Start(null));

            Assert.That(ex.ParamName, Is.EqualTo("messageTypes"));

            publisher.Dispose();
        }

        [Test]
        public void Start_ArgumentIsEmpty_StartsButWritesWarningToLog()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act
            publisher.Start(Type.EmptyTypes);

            // Assert
            Assert.That(publisher.State, Is.EqualTo("Started"));
            Assert.That(publisher.MessageTypes, Is.Empty);

            var log = this.GetLog();
            Assert.That(log, Does.Contain("Starting message publisher with no message types defined. No messages will be published."));

            publisher.Dispose();
        }

        [Test]
        public void Start_ArgumentContainsNulls_ThrowsArgumentException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => publisher.Start(new[] { typeof(PersonDto), null }));

            Assert.That(ex.Message, Does.StartWith("Message types cannot contain nulls."));
            Assert.That(ex.ParamName, Is.EqualTo("messageTypes"));

            publisher.Dispose();
        }

        [Test]
        public void Start_ArgumentContainsDuplicates_ThrowsArgumentException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => publisher.Start(new[] { typeof(PersonDto), typeof(PersonDto) }));

            Assert.That(ex.Message, Does.StartWith("Duplicate message types."));
            Assert.That(ex.ParamName, Is.EqualTo("messageTypes"));

            publisher.Dispose();
        }

        [Test]
        public void Start_AlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));

            publisher.Dispose();
        }

        [Test]
        public void Start_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));
        }

        [Test]
        public void Publish_ValidMessage_PublishesMessage()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto), typeof(BadDto) });

            // Act
            publisher.Publish(new PersonDto
            {
                Number = 1488,
                Name = "nutriad!",
            });

            publisher.Publish(new BadDto
            {
                Name = "wat",
            });

            // Assert
            publisher.Dispose();

            var log = this.GetLog();
            Assert.That(log, Does.Contain("The 'TauCode.Mq.NullMessagePublisher' instance publishes a message. It only writes the message to the log. Message payload: \r\n"));
            Assert.That(log, Does.Contain("nutriad!"));

            Assert.That(log, Does.Contain("TauCode.Mq.Tests.Dto.BadDto (Failed to serialize as JSON)"));

            Assert.Pass(log);
        }

        [Test]
        public void Publish_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => { publisher.Publish(null); });

            Assert.That(ex.ParamName, Is.EqualTo("message"));

            publisher.Dispose();
        }

        [Test]
        public void Publish_MessageTypeNotSupported_ThrowsArgumentException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => publisher.Publish(new CountryDto
            {
                Code = "ua",
                Name = "Ukraine",
            }));

            Assert.That(ex.Message, Does.StartWith("Message type not supported: 'TauCode.Mq.Tests.Dto.CountryDto'."));
            Assert.That(ex.ParamName, Is.EqualTo("message"));

            publisher.Dispose();
        }

        [Test]
        public void Publish_NotStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => publisher.Publish(new CurrencyDto
            {
                Code = "eur",
                Name = "Euro",
            }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'Started' state."));

            publisher.Dispose();
        }

        [Test]
        public void Publish_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });
            publisher.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => publisher.Publish(new CurrencyDto
            {
                Code = "eur",
                Name = "Euro",
            }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'Started' state."));
        }

        [Test]
        public void Dispose_NotStarted_Disposes()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();

            // Act
            publisher.Dispose();

            // Assert
            Assert.That(publisher.State, Is.EqualTo("Disposed"));
            Assert.That(publisher.MessageTypes, Is.Empty);

            var log = this.GetLog();
            Assert.That(log, Does.Contain("Disposing the 'TauCode.Mq.NullMessagePublisher' instance."));
        }

        [Test]
        public void Dispose_Started_Disposes()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });

            // Act
            publisher.Dispose();

            // Assert
            Assert.That(publisher.State, Is.EqualTo("Disposed"));
            Assert.That(publisher.MessageTypes, Is.Empty);

            var log = this.GetLog();
            Assert.That(log, Does.Contain("Disposing the 'TauCode.Mq.NullMessagePublisher' instance."));
        }

        [Test]
        public void Dispose_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessagePublisher publisher = new NullMessagePublisher();
            publisher.Start(new[] { typeof(PersonDto), typeof(CurrencyDto) });
            publisher.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => publisher.Dispose());

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
