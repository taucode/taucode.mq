using Moq;
using NUnit.Framework;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TauCode.Mq.Tests
{
    [TestFixture]
    public class MessageQueueTest
    {
        private class SomeDto
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }

        private IMessagePublisher _messagePublisher;
        private List<object> _messages;

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

            _messages = new List<object>();
            var mock = new Mock<IMessagePublisher>();
            mock.Setup(x => x.Publish(It.IsAny<object>())).Callback<object>(x =>
            {
                if (x is int n && n == 1488)
                {
                    throw new ArgumentOutOfRangeException(nameof(n), "Fascism won't pass!");
                }

                if (x.Equals("Very big message"))
                {
                    Task.Delay(1000).Wait();
                }

                _messages.Add(x);
            });
            _messagePublisher = mock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _messagePublisher.Dispose();
            _logWriter.Dispose();
        }

        [Test]
        public void Constructor_CorrectArguments_MessageQueueIsCreatedAndIsInValidState()
        {
            // Arrange
            
            // Act
            IMessageQueue mq = new MessageQueue();

            // Assert
            Assert.That(mq.State, Is.EqualTo("NotStarted"));
            Assert.That(mq.Backlog, Is.Zero);
            Assert.That(mq.MessagePublisher, Is.Null);

            mq.Dispose();
        }

        [Test]
        public void MessagePublisher_SetWhenNotStarted_SetsMessagePublisher()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            var publisher1 = _messagePublisher;
            var publisher2 = new Mock<IMessagePublisher>().Object;

            // Act
            mq.MessagePublisher = publisher1;
            var setPublisher1 = mq.MessagePublisher;

            mq.MessagePublisher = publisher2;
            var setPublisher2 = mq.MessagePublisher;

            mq.MessagePublisher = null;
            var setPublisher3 = mq.MessagePublisher;

            // Assert
            Assert.That(setPublisher1, Is.SameAs(publisher1));
            Assert.That(setPublisher2, Is.SameAs(publisher2));
            Assert.That(setPublisher3, Is.Null);

            mq.Dispose();
        }

        [Test]
        public void MessagePublisher_SetWhenStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Start();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.MessagePublisher = _messagePublisher);

            Assert.That(ex.Message, Is.EqualTo("'MessagePublisher' can only be changed in the 'NotStarted' state"));

            mq.Dispose();
        }

        [Test]
        public void MessagePublisher_SetWhenDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Start();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.MessagePublisher = _messagePublisher);

            Assert.That(ex.Message, Is.EqualTo("'MessagePublisher' can only be changed in the 'NotStarted' state"));

            mq.Dispose();
        }

        [Test]
        public void Start_NoArguments_StateIsStarted()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();

            // Act
            mq.Start();

            // Assert
            Assert.That(mq.State, Is.EqualTo("Started"));

            mq.Dispose();
        }

        [Test]
        public void Start_AlreadyStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();

            // Act & Assert
            mq.Start();

            var ex = Assert.Throws<InvalidOperationException>(() => mq.Start());

            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));

            mq.Dispose();
        }

        [Test]
        public void Start_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.Start());

            Assert.That(ex.Message, Is.EqualTo("Not in the 'NotStarted' state."));
        }

        [Test]
        public void Enqueue_ValidMessage_MessageIsDelivered()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue
            {
                MessagePublisher = _messagePublisher,
            };
            mq.Start();

            // Act
            mq.Enqueue(new SomeDto
            {
                Number = 1599,
                Name = "ira",
            });

            Task.Delay(50).Wait();
            
            // Assert
            Assert.That(mq.Backlog, Is.Zero);
            Assert.That(_messages, Has.Count.EqualTo(1));
            var message = (SomeDto) _messages.Single();
            Assert.That(message.Number, Is.EqualTo(1599));
            Assert.That(message.Name, Is.EqualTo("ira"));

            mq.Dispose();
        }

        [Test]
        public void Enqueue_NotStarted_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.Enqueue(new SomeDto
            {
                Number = 1599,
                Name = "ira",
            }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'Started' state."));

            mq.Dispose();
        }

        [Test]
        public void Enqueue_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.Enqueue(new SomeDto
            {
                Number = 1599,
                Name = "ira",
            }));

            Assert.That(ex.Message, Is.EqualTo("Not in the 'Started' state."));
        }

        [Test]
        public void Enqueue_PublisherIsNull_NoExceptionAndWarningIsLogged()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Start();

            // Act
            mq.Enqueue(1488);
            Task.Delay(150).Wait();

            // Assert
            Assert.That(mq.Backlog, Is.Zero);
            var log = this.GetLog();
            Assert.That(log, Does.Contain("'MessagePublisher' is null. Message hase been dequeued and discarded."));

            mq.Dispose();
        }

        [Test]
        public void Enqueue_PublisherThrows_ExceptionNotThrownButLogged()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue
            {
                MessagePublisher = _messagePublisher,
            };
            mq.Start();


            // Act
            mq.Enqueue(1488);
            Task.Delay(150).Wait();

            // Assert
            Assert.That(mq.Backlog, Is.Zero);
            var log = this.GetLog();
            Assert.That(log, Does.Contain("Fascism won't pass!"));

            mq.Dispose();
        }

        [Test]
        public void Enqueue_PublisherTooSlowDisposeQueue_MessageDeliveredButBacklogNotEmpty()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue
            {
                MessagePublisher = _messagePublisher,
            };
            mq.Start();

            // Act
            mq.Enqueue(1);
            mq.Enqueue("Very big message");
            mq.Enqueue(2);
            mq.Enqueue(3);
            mq.Enqueue(4);

            Task.Delay(200).Wait();
            mq.Dispose();
            Task.Delay(1500).Wait();

            // Assert
            Assert.That(_messages, Has.Count.EqualTo(2));
            Assert.That(_messages[0], Is.EqualTo(1));
            Assert.That(_messages[1], Is.EqualTo("Very big message"));

            Assert.That(mq.Backlog, Is.EqualTo(3));
        }

        [Test]
        public void Enqueue_HugeAmountOfMessages_PublishesMessages()
        {
            // Arrange & Assert
            IMessageQueue mq = new MessageQueue
            {
                MessagePublisher = _messagePublisher,
            };
            Assert.That(mq.State, Is.EqualTo("NotStarted"));

            // Act
            mq.Start();
            
            var begin = DateTime.UtcNow;

            var count = 1 * 1000 * 1000;

            for (var i = 0; i < count; i++)
            {
                mq.Enqueue(i.ToString());
            }

            while (true)
            {
                if (mq.Backlog == 0)
                {
                    break;
                }

                Thread.Sleep(1);
            }

            mq.Dispose();

            var end = DateTime.UtcNow;

            Assert.That(mq.State, Is.EqualTo("Disposed"));

            // Assert
            Assert.That(mq.Backlog, Is.Zero);
            Assert.That(_messages, Has.Count.EqualTo(count));
            for (var i = 0; i < _messages.Count; i++)
            {
                Assert.That(_messages[i], Is.EqualTo(i.ToString()));
            }

            var elapsed = end - begin;
            var messagesPerSecond = (int)(count / elapsed.TotalSeconds);
            var millisecondsPerMessage = elapsed.TotalMilliseconds / count;

            var msg = $"Elapsed: {elapsed}; Messages per second: {messagesPerSecond}; Milliseconds per message: {millisecondsPerMessage}";
            Assert.Pass(msg);
        }

        [Test]
        public void Dispose_NotStarted_Disposes()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();

            // Act
            mq.Dispose();

            // Assert
            Assert.That(mq.State, Is.EqualTo("Disposed"));
        }

        [Test]
        public void Dispose_Started_Disposes()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Start();

            // Act
            mq.Dispose();

            // Assert
            Assert.That(mq.State, Is.EqualTo("Disposed"));
        }

        [Test]
        public void Dispose_StartedThenEnqueue_Disposes()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Start();
            mq.Enqueue(11);

            // Act
            mq.Dispose();

            // Assert
            Assert.That(mq.State, Is.EqualTo("Disposed"));
        }

        [Test]
        public void Dispose_AlreadyDisposed_ThrowsInvalidOperationException()
        {
            // Arrange
            IMessageQueue mq = new MessageQueue();
            mq.Dispose();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => mq.Dispose());
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
