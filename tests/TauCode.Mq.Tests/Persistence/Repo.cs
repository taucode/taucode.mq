using System.Collections.Generic;
using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public class Repo : IRepo
    {
        private readonly List<PersonDto> _persons = new List<PersonDto>();

        public void Save(PersonDto person)
        {
            _persons.Add(person);
        }

        public PersonDto[] GetAll()
        {
            return _persons.ToArray();
        }
    }
}
