using NUnit.Framework;

namespace TauCode.Lab.Mq.EasyNetQ.Tests
{
    [TestFixture]
    public class EasyNetQMessageSubscriberTests
    {
        #region ctor

        // ctor(IMessageHandlerContextFactory)
        // todo - happy path, check name, con.str, status etc.
        // todo - factory is null, throws

        // ctor(string, IMessageHandlerContextFactory)
        // todo - happy path, connection string equals to what we passed, null accepted also.
        // todo - factory is null, throws
        // todo - multi todo: deal with bad factory (cannot resolve, throws, etc) => log & behave gracefully.

        #endregion

        #region ConnectionString

        // todo - when set, changes, null is also ok.
        // todo - when running, cannot be set, but can be read.
        // todo - when disposed, cannot be set, but can be read.

        #endregion

        #region Subscribe(Type)

        // todo - happy path, starts handling messages without topic (sync, single handler).
        // todo - happy path, starts handling messages without topic (sync, multiple handlers).
        // todo - happy path, starts handling messages without topic (async, single handler).
        // todo - happy path, starts handling messages without topic (async, multiple handlers).

        // todo - arg is null => throws
        // todo - arg is abstract => throws
        // todo - arg is not class => throws
        // todo - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws
        // todo - arg is sync, while there are already async handlers for this type of message => throws
        // todo - arg is async, while there are already sync handlers for this type of message => throws
        // todo - arg implements IMessageHandler<TMessage> more than once => throws
        // todo - arg implements IAsyncMessageHandler<TMessage> more than once => throws
        // todo - already have this handler type => throws

        // todo - TMessage is abstract => throws
        // todo - TMessage is not class => throws
        // todo - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?
        // todo - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

        // todo - sync handler's ctor is throwing => logs, stops loop gracefully.
        // todo - sync handler's Handle is throwing => logs, stops loop gracefully.

        // todo - async handler's ctor is throwing => logs, stops loop gracefully.
        // todo - async handler's HandleAsync is throwing => logs, stops loop gracefully.
        // todo - async handler's HandleAsync is canceled => logs, stops loop gracefully.

        // todo - started, throws
        // todo - disposed, throws

        #endregion

        #region Subscribe(Type, string)

        // todo - happy path, starts handling messages with proper topic (sync, single handler).
        // todo - happy path, starts handling messages with proper topic (sync, multiple handlers).
        // todo - happy path, starts handling messages with proper topic (async, single handler).
        // todo - happy path, starts handling messages with proper topic (async, multiple handlers).

        // todo - topic is null or empty => throws

        // todo - arg is null => throws
        // todo - arg is abstract => throws
        // todo - arg is not class => throws
        // todo - arg is not (IMessageHandler<TMessage> xor IAsyncMessageHandler<TMessage>) => throws
        // todo - arg is sync, while there are already async handlers for this type of message => throws
        // todo - arg is async, while there are already sync handlers for this type of message => throws
        // todo - arg implements IMessageHandler<TMessage> more than once => throws
        // todo - arg implements IAsyncMessageHandler<TMessage> more than once => throws
        // todo - already have this handler type => throws

        // todo - TMessage is abstract => throws
        // todo - TMessage is not class => throws
        // todo - TMessage is throwing in ctor => todo: wat? will EasyNetQ handle this?
        // todo - TMessage is throwing when querying properties => todo: wat? will EasyNetQ handle this?

        // todo - sync handler's ctor is throwing => logs, stops loop gracefully.
        // todo - sync handler's Handle is throwing => logs, stops loop gracefully.

        // todo - async handler's ctor is throwing => logs, stops loop gracefully.
        // todo - async handler's HandleAsync is throwing => logs, stops loop gracefully.
        // todo - async handler's HandleAsync is canceled => logs, stops loop gracefully.

        // todo - started, throws
        // todo - disposed, throws

        #endregion

        #region GetSubscriptions()

        // todo - just created, returns empty array
        // todo - running, returns subscriptions
        // todo - stopped, returns subscriptions
        // todo - disposed, returns empty array


        #endregion

        #region Name

        // todo - when set, reflects, can be any value
        // todo - after disposed, name cannot be set.
        // todo - after disposed, name still can be read

        #endregion

        #region State

        // todo - just created, eq. to 'stopped'
        // todo - started, eq. to 'started'
        // todo - stopped, eq. to 'stopped'
        // todo - disposed just after creation, eq. to 'stopped'
        // todo - disposed after started, eq. to 'stopped'
        // todo - disposed after stopped, eq. to 'stopped'
        // todo - disposed after disposed, eq. to 'disposed'

        #endregion

        #region IsDisposed

        // todo - just created, eq. to 'false'
        // todo - started, eq. to 'false'
        // todo - stopped, eq. to 'false'
        // todo - disposed just after creation, eq. to 'true'
        // todo - disposed after started, eq. to 'true'
        // todo - disposed after stopped, eq. to 'true'
        // todo - disposed after disposed, eq. to 'true'

        #endregion

        #region Start()

        // todo - just created, starts, handles messages
        // todo - con str is null or empty, throws.
        // todo - started, throws
        // todo - stopped, starts, handles messages
        // todo - disposed, throws

        #endregion

        #region Stop()

        // todo - just created, throws
        // todo - started, stops, cancels current async tasks, shown in logs
        // todo - stopped, throws
        // todo - disposed, throws

        #endregion

        #region Dispose()

        // todo - just created, disposes
        // todo - started, disposes, cancels current async tasks, shown in logs
        // todo - stopped, disposes
        // todo - disposed, does nothing.

        #endregion
    }
}
