using System;
using TauCode.Mq;

namespace TauCode.Lab.Mq.Testing
{
    public class TestMessageSubscriber : MessageSubscriberBase
    {
        #region Fields

        private readonly ITestMqMedia _media;

        #endregion

        #region Constructor

        public TestMessageSubscriber(ITestMqMedia media, IMessageHandlerContextFactory contextFactory)
            : base(contextFactory)
        {
            _media = media ?? throw new ArgumentNullException(nameof(media));
        }

        #endregion

        #region Overridden

        protected override void InitImpl()
        {
            throw new NotImplementedException();
        }

        protected override void ShutdownImpl()
        {
            throw new NotImplementedException();
        }

        protected override IDisposable SubscribeImpl(ISubscriptionRequest subscriptionRequest)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
