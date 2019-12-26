namespace TauCode.Mq.EasyNetQ.Demo.All.Commands
{
    public class PublishCommand
    {
        public string NodeName { get; set; }
        public string MessageType { get; set; }
        public string MessageJson { get; set; }
    }
}
