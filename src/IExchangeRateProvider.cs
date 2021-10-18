using System.Collections.Generic;

namespace LiveCurrencyExchangeRate
{
    /// <summary>
    /// Exchange rate provider interface
    /// </summary>
    public partial interface IExchangeRateProvider
    {
        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode);
    }
}