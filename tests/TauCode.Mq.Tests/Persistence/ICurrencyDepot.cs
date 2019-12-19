using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public interface ICurrencyDepot
    {
        void AddCurrency(CurrencyDto currency);

        CurrencyDto[] GetAll();
    }
}
