using System;

namespace LiveCurrencyExchangeRate
{
    /// <summary>
    /// Represents an exchange rate
    /// </summary>
    public partial class ExchangeRate
    {
        
        /// <summary>
        /// The three letter ISO code for the Exchange Rate, e.g. USD
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// The conversion rate of this currency from the base currency
        /// </summary>
        public decimal Value { get; set; }

        public DateTime LastModifiedDate { get; set; }

        /// <summary>
        /// Creates a new instance of the ExchangeRate class
        /// </summary>
        public ExchangeRate()
        {
            CurrencyCode = string.Empty;
            Value = 1.0m;
        }

        /// <summary>
        /// Format the rate into a string with the currency code, e.g. "USD 0.72543"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{CurrencyCode} : {Value}";
        }
    }
}
