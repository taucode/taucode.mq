using System.Collections.Generic;
using TauCode.Mq.Tests.Dto;

namespace TauCode.Mq.Tests.Persistence
{
    public class CurrencyDepot : ICurrencyDepot
    {
        private readonly List<CurrencyDto> _currencies = new List<CurrencyDto>();

        public void AddCurrency(CurrencyDto currency)
        {
            _currencies.Add(currency);
        }

        public CurrencyDto[] GetAll()
        {
            return _currencies.ToArray();
        }
    }
}
