using Serilog;
using System;

namespace TauCode.Mq
{
    public sealed class NullMessageSubscriber : MessageSubscriberBase
    {
        #region Nested Types

        private class NullMessageHandlerWrapperFactory : IMessageHandlerWrapperFactory
        {
            internal static readonly IMessageHandlerWrapperFactory Instance = new NullMessageHandlerWrapperFactory();

            private NullMessageHandlerWrapperFactory()
            {
            }

            public IMessageHandlerWrapper Create(Type messageHandlerType)
            {
                throw new InvalidOperationException("This method should not be called.");
            }
        }

        private class NullSubscription : IDisposable
        {
            private readonly Type _messageType;
            private readonly string _subscriptionId;

            public NullSubscription(Type messageType, string subscriptionId)
            {
                _messageType = messageType;
                _subscriptionId = subscriptionId;
            }

            public void Dispose()
            {
                Log.Warning($"Disposing a '{this.GetType().FullName}' instance. Message type: {this._messageType.FullName}. Subscription ID: {this._subscriptionId}.");
            }
        }

        #endregion

        #region Constructor

        public NullMessageSubscriber()
            : base(typeof(NullMessageSubscriber).FullName)
        {
            this.MessageHandlerWrapperFactory = NullMessageHandlerWrapperFactory.Instance;
        }

        #endregion

        #region Overridden

        protected override IDisposable SubscribeImpl(Type messageType, string subscriptionId, Action<object> callback)
        {
            Log.Warning($"'{this.GetType().FullName}' instance subscribes to the '{messageType.FullName}' message type. Subscription ID is '{subscriptionId}'.");
            return new NullSubscription(messageType, subscriptionId);
        }

        protected override void StartImpl()
        {
            Log.Warning($"Starting the '{this.GetType().FullName}' instance.");
        }

        protected override void DisposeImpl()
        {
            Log.Warning($"Disposing the '{this.GetType().FullName}' instance.");
        }

        #endregion
    }
}
