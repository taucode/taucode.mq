using NUnit.Framework;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessageSubscriberTests
    {
        // todo: ctor(IMessageHandlerContextFactory)
        // - happy path, check name, con.str, status etc.
        // - factory is null, throws

        // todo: ctor(string, IMessageHandlerContextFactory)
        // - happy path, check name, con.str, status etc. null or empty name is ok.
        // - factory is null, throws

        // todo: ctor(string, connectionString, IMessageHandlerContextFactory)
        // - happy path, connection string equals to what we passed, null accepted also.
        // - factory is null, throws
        // -todo: deal with bad factory (cannot resolve, throws, etc) => log & behave gracefully.

        // todo: IEasyNetQMessageSubscriber.ConnectionString
        // - when set, changes, null is also ok.
        // - when running, cannot be set, but can be read.
        // - when disposed, cannot be set, but can be read.

        // todo: IEasyNetQMessageSubscriber.Subscribe(Type)
        // - happy path, starts handling messages without topic (sync, single handler).
        // - happy path, starts handling messages without topic (sync, multiple handlers).
        // - happy path, starts handling messages without topic (async, single handler).
        // - happy path, starts handling messages without topic (async, multiple handlers).

        // - arg is null => throws
        // - arg is abstract => throws
        // - arg is not class => throws
        // - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws
        // - arg is sync, while there are already async handlers for this type of message => throws
        // - arg is async, while there are already sync handlers for this type of message => throws
        // - arg implements IMessageHandler<TMessage> more than once => throws
        // - arg implements IAsyncMessageHandler<TMessage> more than once => throws
        // - already have this handler type => throws

        // - TMessage is abstract => throws
        // - TMessage is not class => throws
        // - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?
        // - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

        // - sync handler's ctor is throwing => logs, stops loop gracefully.
        // - sync handler's Handle is throwing => logs, stops loop gracefully.

        // - async handler's ctor is throwing => logs, stops loop gracefully.
        // - async handler's HandleAsync is throwing => logs, stops loop gracefully.
        // - async handler's HandleAsync is canceled => logs, stops loop gracefully.

        // - started, throws
        // - disposed, throws

        // todo: IEasyNetQMessageSubscriber.Subscribe(Type, string)
        // - happy path, starts handling messages with proper topic (sync, single handler).
        // - happy path, starts handling messages with proper topic (sync, multiple handlers).
        // - happy path, starts handling messages with proper topic (async, single handler).
        // - happy path, starts handling messages with proper topic (async, multiple handlers).

        // - topic is null or empty => throws

        // - arg is null => throws
        // - arg is abstract => throws
        // - arg is not class => throws
        // - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws
        // - arg is sync, while there are already async handlers for this type of message => throws
        // - arg is async, while there are already sync handlers for this type of message => throws
        // - arg implements IMessageHandler<TMessage> more than once => throws
        // - arg implements IAsyncMessageHandler<TMessage> more than once => throws
        // - already have this handler type => throws

        // - TMessage is abstract => throws
        // - TMessage is not class => throws
        // - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?
        // - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

        // - sync handler's ctor is throwing => logs, stops loop gracefully.
        // - sync handler's Handle is throwing => logs, stops loop gracefully.

        // - async handler's ctor is throwing => logs, stops loop gracefully.
        // - async handler's HandleAsync is throwing => logs, stops loop gracefully.
        // - async handler's HandleAsync is canceled => logs, stops loop gracefully.

        // - started, throws
        // - disposed, throws

        // todo: IEasyNetQMessageSubscriber.GetSubscriptions()
        // - just created, returns empty array
        // - running, returns subscriptions
        // - stopped, returns subscriptions
        // - disposed, returns empty array

        // todo: IEasyNetQMessageSubscriber.Name
        // - after disposed, name still can be read.

        // todo: IEasyNetQMessageSubscriber.State
        // - just created, eq. to 'stopped'
        // - started, eq. to 'started'
        // - stopped, eq. to 'stopped'
        // - disposed just after creation, eq. to 'stopped'
        // - disposed after started, eq. to 'stopped'
        // - disposed after stopped, eq. to 'stopped'
        // - disposed after disposed, eq. to 'disposed'

        // todo: IEasyNetQMessageSubscriber.IsDisposed
        // - just created, eq. to 'false'
        // - started, eq. to 'false'
        // - stopped, eq. to 'false'
        // - disposed just after creation, eq. to 'true'
        // - disposed after started, eq. to 'true'
        // - disposed after stopped, eq. to 'true'
        // - disposed after disposed, eq. to 'true'

        // todo: IEasyNetQMessageSubscriber.Start
        // - just created, starts, handles messages
        // - con str is null or empty, throws.
        // - started, throws
        // - stopped, starts, handles messages
        // - disposed, throws

        // todo: IEasyNetQMessageSubscriber.Stop
        // - just created, throws
        // - started, stops, cancels current async tasks, shown in logs
        // - stopped, throws
        // - disposed, throws

        // todo: IEasyNetQMessageSubscriber.Dispose
        // - just created, disposes
        // - started, disposes, cancels current async tasks, shown in logs
        // - stopped, disposes
        // - disposed, does nothing.
    }
}
