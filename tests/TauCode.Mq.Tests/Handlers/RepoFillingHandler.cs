using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests.Handlers
{
    public class RepoFillingHandler : IMessageHandler<PersonDto>
    {
        private readonly IRepo _repo;

        public RepoFillingHandler(IRepo repo)
        {
            _repo = repo;
        }

        public void Handle(PersonDto message)
        {
            _repo.Save(message);
        }
    }
}
