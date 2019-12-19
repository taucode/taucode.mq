using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests.Handlers
{
    public class CurrencyDepotHandler : IMessageHandler<CurrencyDto>
    {
        private readonly ICurrencyDepot _currencyDepot;

        public CurrencyDepotHandler(ICurrencyDepot currencyDepot)
        {
            _currencyDepot = currencyDepot;
        }

        public void Handle(CurrencyDto message)
        {
            _currencyDepot.AddCurrency(message);
        }
    }
}
