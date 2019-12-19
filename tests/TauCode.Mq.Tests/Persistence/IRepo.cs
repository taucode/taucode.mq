using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public interface IRepo
    {
        void Save(PersonDto person);

        PersonDto[] GetAll();
    }
}
