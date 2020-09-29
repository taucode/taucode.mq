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

        // todo: IEasyNetQMessagePublisher.ConnectionString
        // - when set, changes, null is also ok.
        // - when running, cannot be set, but can be read.
        // - when disposed, cannot be set, but can be read.

        // todo: IEasyNetQMessagePublisher.Publish(IMessage)
        // - happy path, publishes; subscription with no topic fires, subscriptions without topics don't fire.
        // - arg is null, throws
        // - arg is abstract class, throws
        // - arg is struct, throws
        // - not started, throws
        // - disposed, throws

        // todo: IEasyNetQMessagePublisher.Publish(IMessage, string)
        // - happy path, publishes, subscription fired with proper topic; with other topic - not fired; with no topic - not fired.
        // - arg is null, throws
        // - arg is abstract class, throws
        // - arg is struct, throws
        // - topic is null or empty, throws
        // - not started, throws
        // - disposed, throws

        // todo: IEasyNetQMessagePublisher.Name
        // - after disposed, name still can be read.

        // todo: IEasyNetQMessagePublisher.State
        // - just created, eq. to 'stopped'
        // - started, eq. to 'started'
        // - stopped, eq. to 'stopped'
        // - disposed just after creation, eq. to 'stopped'
        // - disposed after started, eq. to 'stopped'
        // - disposed after stopped, eq. to 'stopped'
        // - disposed after disposed, eq. to 'disposed'

        // todo: IEasyNetQMessagePublisher.IsDisposed
        // - just created, eq. to 'false'
        // - started, eq. to 'false'
        // - stopped, eq. to 'false'
        // - disposed just after creation, eq. to 'true'
        // - disposed after started, eq. to 'true'
        // - disposed after stopped, eq. to 'true'
        // - disposed after disposed, eq. to 'true'

        // todo: IEasyNetQMessagePublisher.Start
        // - just created, starts
        // - con str is null or empty, throws.
        // - started, throws
        // - stopped, starts
        // - disposed, throws

        // todo: IEasyNetQMessagePublisher.Stop
        // - just created, throws
        // - started, stops
        // - stopped, throws
        // - disposed, throws

        // todo: IEasyNetQMessagePublisher.Dispose
        // - just created, disposes
        // - started, disposes
        // - stopped, disposes
        // - disposed, does nothing.
    }
}
