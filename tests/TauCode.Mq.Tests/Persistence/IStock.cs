using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public interface IStock
    {
        void PushCurrency(CurrencyDto currency);

        CurrencyDto[] Enumerate();
    }
}
