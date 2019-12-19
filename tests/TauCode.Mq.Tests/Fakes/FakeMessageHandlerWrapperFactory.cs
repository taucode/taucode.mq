using System;
using TauCode.Mq.Tests.Handlers;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests.Fakes
{
    public class FakeMessageHandlerWrapperFactory : IMessageHandlerWrapperFactory
    {
        private readonly IPersonRoster _personRoster;
        private readonly IRepo _repo;
        private readonly ICurrencyDepot _currencyDepot;
        private readonly IStock _stock;

        public FakeMessageHandlerWrapperFactory(
            IPersonRoster personRoster,
            IRepo repo,
            ICurrencyDepot currencyDepot,
            IStock stock)
        {
            _personRoster = personRoster;
            _repo = repo;
            _currencyDepot = currencyDepot;
            _stock = stock;
        }

        public virtual IMessageHandlerWrapper Create(Type messageHandlerType)
        {
            object handler;

            if (messageHandlerType == typeof(RosterFillingHandler))
            {
                handler = new RosterFillingHandler(_personRoster);
            }
            else if (messageHandlerType == typeof(RepoFillingHandler))
            {
                handler = new RepoFillingHandler(_repo);
            }
            else if (messageHandlerType == typeof(CurrencyDepotHandler))
            {
                handler = new CurrencyDepotHandler(_currencyDepot);
            }
            else if (messageHandlerType == typeof(StockHandler))
            {
                handler = new StockHandler(_stock);
            }
            else
            {
                throw new NotSupportedException();
            }

            var wrapper = new FakeMessageHandlerWrapper(handler);
            return wrapper;
        }
    }
}
