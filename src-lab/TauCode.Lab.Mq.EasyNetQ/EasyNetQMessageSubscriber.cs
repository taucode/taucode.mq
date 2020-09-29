using EasyNetQ;
using EasyNetQ.NonGeneric;
using System;
using TauCode.Mq;

namespace TauCode.Lab.Mq.EasyNetQ
{
    public class EasyNetQMessageSubscriber : MessageSubscriberBase, IEasyNetQMessageSubscriber
    {
        #region Fields

        private IBus _bus;

        #endregion

        #region Constructor

        public EasyNetQMessageSubscriber(IMessageHandlerContextFactory contextFactory)
            : base(contextFactory)
        {
        }

        #endregion

        #region Overridden

        protected override void InitImpl()
        {
            _bus = RabbitHutch.CreateBus(this.ConnectionString);
        }

        protected override void ShutdownImpl()
        {
            _bus.Dispose();
        }

        protected override IDisposable SubscribeImpl(ISubscriptionRequest subscriptionRequest)
        {
            if (subscriptionRequest.Handler != null)
            {
                // got sync handler
                throw new NotImplementedException();
            }
            else
            {
                // got async handler

                var subscriptionId = Guid.NewGuid().ToString();
                var handle = _bus.SubscribeAsync(
                    subscriptionRequest.MessageType,
                    subscriptionId,
                    subscriptionRequest.AsyncHandler);

                return handle;
            }
        }

        #endregion

        #region IEasyNetQMessageSubscriber Members

        public string ConnectionString { get; set; }

        #endregion
    }
}
