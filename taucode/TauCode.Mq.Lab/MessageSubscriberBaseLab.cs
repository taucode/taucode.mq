using System;
using TauCode.Working.Lab;

namespace TauCode.Mq.Lab
{
    public class MessageSubscriberBaseLab : OnDemandWorkerBase, IMessageSubscriberLab
    {
        public void Subscribe(Type messageHandlerType)
        {
            if (messageHandlerType == null)
            {
                throw new ArgumentNullException(nameof(messageHandlerType));
            }

            this.CheckStateForOperation(WorkerState.Stopped);

            throw new NotImplementedException();
        }

        public void Subscribe(Type messageHandlerType, string topic)
        {
            throw new NotImplementedException();
        }
    }
}
