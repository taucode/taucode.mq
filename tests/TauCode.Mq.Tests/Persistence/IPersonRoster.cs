using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public interface IPersonRoster
    {
        void Add(PersonDto person);

        PersonDto[] GetPersons();
    }
}
