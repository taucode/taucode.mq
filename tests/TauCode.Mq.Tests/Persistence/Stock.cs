using System.Collections.Generic;
using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public class Stock : IStock
    {
        private readonly List<CurrencyDto> _currencies = new List<CurrencyDto>();

        public void PushCurrency(CurrencyDto currency)
        {
            _currencies.Add(currency);
        }

        public CurrencyDto[] Enumerate()
        {
            return _currencies.ToArray();
        }
    }
}
