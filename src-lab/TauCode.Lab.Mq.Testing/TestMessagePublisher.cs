using System;
using TauCode.Mq;
using TauCode.Mq.Abstractions;

namespace TauCode.Lab.Mq.Testing
{
    public class TestMessagePublisher : MessagePublisherBase
    {
        #region Fields

        private readonly ITestMqMedia _media;

        #endregion

        #region Constructor

        public TestMessagePublisher(ITestMqMedia media)
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

        protected override void PublishImpl(IMessage message)
        {
            throw new NotImplementedException();
        }

        protected override void PublishImpl(IMessage message, string topic)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
