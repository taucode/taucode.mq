using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Mq.Exceptions;
using TauCode.Mq.Tests.MessageHandlers;
using TauCode.Mq.Tests.Messages;

namespace TauCode.Mq.Tests
{
    [TestFixture]
    public class SubscriberTests
    {
        [Test]
        public void GetSubscriptions_NoArguments_ReturnsExpectedSubscriptions()
        {
            // Arrange
            IMessageSubscriber messageSubscriber = new TestMessageSubscriber();

            // Act
            messageSubscriber.Subscribe(typeof(SmartHelloMessageHandler));
            messageSubscriber.Subscribe(typeof(SmartHelloMessageHandler), "some-topic");

            messageSubscriber.Subscribe(typeof(DumbPersonMessageHandler));
            messageSubscriber.Subscribe(typeof(DumbPersonMessageHandler));

            var subscriptions = messageSubscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Has.Length.EqualTo(4));

            var subs1 = subscriptions.Where(x => x.HandlerType == typeof(SmartHelloMessageHandler)).ToList();
            Assert.That(subs1, Has.Count.EqualTo(2));

            var sub = subs1.SingleOrDefault(x => x.Topic == null);
            Assert.That(sub, Is.Not.Null);
            Assert.That(sub.MessageType, Is.SameAs(typeof(HelloMessage)));

            sub = subs1.SingleOrDefault(x => x.Topic == "some-topic");
            Assert.That(sub, Is.Not.Null);
            Assert.That(sub.MessageType, Is.SameAs(typeof(HelloMessage)));

            var subs2 = subscriptions.Where(x => x.HandlerType == typeof(DumbPersonMessageHandler)).ToList();
            Assert.That(subs2, Has.Count.EqualTo(2));
            Assert.That(subs2.All(x => x.Topic == null && x.MessageType == typeof(PersonMessage)), Is.True);
        }

        [Test]
        public void Subscribe_MultiHandler_ThrowsMqException()
        {
            // Arrange
            IMessageSubscriber messageSubscriber = new TestMessageSubscriber();

            // Act
            var ex1 = Assert.Throws<MqException>(() =>
                messageSubscriber.Subscribe(typeof(MultiMessageHandler)));

            var ex2 = Assert.Throws<MqException>(() =>
                messageSubscriber.Subscribe(typeof(NonGenericMessageHandler)));

            var ex3 = Assert.Throws<MqException>(() =>
                messageSubscriber.Subscribe(typeof(NotAMessageHandler)));

            // Assert
            Assert.That(
                (new Exception[] { ex1, ex2, ex3 })
                .All(x => x.Message ==
                          "Message handler must implement 'IMessageHandler<TMessage>'; multiple implementation is not allowed."),
                Is.True);
        }

        [Test]
        public void UnsubscribeAll_NoArguments_RemovesAllSubscriptions()
        {
            // Arrange
            IMessageSubscriber messageSubscriber = new TestMessageSubscriber();
            messageSubscriber.Subscribe(typeof(SmartHelloMessageHandler));
            messageSubscriber.Subscribe(typeof(SmartHelloMessageHandler), "some-topic");

            messageSubscriber.Subscribe(typeof(DumbPersonMessageHandler));
            messageSubscriber.Subscribe(typeof(DumbPersonMessageHandler));

            // Act
            messageSubscriber.UnsubscribeAll();

            var subscriptions = messageSubscriber.GetSubscriptions();

            // Assert
            Assert.That(subscriptions, Is.Empty);
        }
    }
}
