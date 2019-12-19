using System.Collections.Generic;
using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public class PersonRoster : IPersonRoster
    {
        private readonly List<PersonDto> _persons = new List<PersonDto>();

        public void Add(PersonDto person)
        {
            _persons.Add(person);
        }

        public PersonDto[] GetPersons()
        {
            return _persons.ToArray();
        }
    }
}
