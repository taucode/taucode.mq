using NUnit.Framework;
using System;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessagePublisherTests
    {
        [Test]
        public void Constructor_NoArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        // todo: ctor()
        // - happy path, check name, con.str, status etc.

        // todo: ctor(string name)
        // - happy path, check name, con.str, status etc. null or empty name is ok.

        // todo: ctor(string name, string connectionString)
        // - happy path, connection string equals to what we passed, null accepted also.

        // todo: IMessagePublisher.ConnectionString
        // - when set, changes, null is also ok.
        // - when running, cannot be set, but can be read.
        // - when disposed, cannot be set, but can be read.

        // todo: IMessagePublisher.Publish(IMessage)
        // - happy path, publishes; subscription with no topic fires, subscriptions without topics don't fire.
        // - arg is null, throws
        // - arg is abstract class, throws
        // - arg is struct, throws
        // - not started, throws
        // - disposed, throws

        // todo: IMessagePublisher.Publish(IMessage, string)
        // - happy path, publishes, subscription fired with proper topic; with other topic - not fired; with no topic - not fired.
        // - arg is null, throws
        // - arg is abstract class, throws
        // - arg is struct, throws
        // - topic is null or empty, throws
        // - not started, throws
        // - disposed, throws

        // todo: IMessagePublisher.Name
        // - after disposed, name still can be read.

        // todo: IMessagePublisher.State
        // - just created, eq. to 'stopped'
        // - started, eq. to 'started'
        // - stopped, eq. to 'stopped'
        // - disposed just after creation, eq. to 'stopped'
        // - disposed after started, eq. to 'stopped'
        // - disposed after stopped, eq. to 'stopped'
        // - disposed after disposed, eq. to 'disposed'

        // todo: IMessagePublisher.IsDisposed
        // - just created, eq. to 'false'
        // - started, eq. to 'false'
        // - stopped, eq. to 'false'
        // - disposed just after creation, eq. to 'true'
        // - disposed after started, eq. to 'true'
        // - disposed after stopped, eq. to 'true'
        // - disposed after disposed, eq. to 'true'

        // todo: IMessagePublisher.Start
        // - just created, starts
        // - started, throws
        // - stopped, starts
        // - disposed, throws

        // todo: IMessagePublisher.Stop
        // - just created, throws
        // - started, stops
        // - stopped, throws
        // - disposed, throws

        // todo: IMessagePublisher.Dispose
        // - just created, disposes
        // - started, disposes
        // - stopped, disposes
        // - disposed, does nothing.

    }
}
