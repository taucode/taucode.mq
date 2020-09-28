//using System;
//using TauCode.Mq.Abstractions;
//using TauCode.Working;

//namespace TauCode.Mq
//{
//    public class MessageQueue : QueueWorkerBase<IMessage>, IMessageQueue
//    {
//        public MessageQueue(IMessagePublisher messagePublisher)
//        {
//            this.MessagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
//        }

//        protected override void DoAssignment(IMessage message)
//        {
//            this.MessagePublisher.Publish(message);
//        }

//        public IMessagePublisher MessagePublisher { get; }
//    }
//}

// todo clean