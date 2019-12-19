using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests.Handlers
{
    public class RosterFillingHandler : IMessageHandler<PersonDto>
    {
        private readonly IPersonRoster _personRoster;

        public RosterFillingHandler(IPersonRoster personRoster)
        {
            _personRoster = personRoster;
        }

        public void Handle(PersonDto message)
        {
            _personRoster.Add(message);
        }
    }
}
