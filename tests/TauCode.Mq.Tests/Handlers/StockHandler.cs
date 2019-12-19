using TauCode.Mq.Tests.Dto;
using TauCode.Mq.Tests.Persistence;

namespace TauCode.Mq.Tests.Handlers
{
    public class StockHandler : IMessageHandler<CurrencyDto>
    {
        private readonly IStock _stock;

        public StockHandler(IStock stock)
        {
            _stock = stock;
        }

        public void Handle(CurrencyDto message)
        {
            _stock.PushCurrency(message);
        }
    }
}
