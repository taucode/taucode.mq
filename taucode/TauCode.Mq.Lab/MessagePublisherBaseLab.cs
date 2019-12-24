using System;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public abstract class MessagePublisherBaseLab : OnDemandWorkerBase, IMessagePublisherLab
    {
        protected abstract void PublishImpl(IMessageLab message);

        public void Publish(IMessageLab message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.CheckStateForOperation(WorkerState.Running);
            this.PublishImpl(message);
        }
    }
}
