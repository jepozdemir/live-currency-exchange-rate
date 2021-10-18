using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace LiveCurrencyExchangeRate
{
    public class EcbExchangeRateProvider : IExchangeRateProvider
    {
        #region Fields

        private readonly ILogger<EcbExchangeRateProvider> _logger;

        #endregion

        #region Ctor

        public EcbExchangeRateProvider(ILogger<EcbExchangeRateProvider> logger)
        {
            _logger = logger;
        }

        #endregion

        #region Methods

        public IList<ExchangeRate> GetCurrencyLiveRates(string exchangeRateCurrencyCode)
        {
            if (string.IsNullOrEmpty(exchangeRateCurrencyCode))
                throw new ArgumentNullException(exchangeRateCurrencyCode, nameof(exchangeRateCurrencyCode));

            //add euro with rate 1
            var ratesToEuro = new List<ExchangeRate>
            {
                new ExchangeRate
                {
                    CurrencyCode = "EUR",
                    Value = 1,
                    LastModifiedDate = DateTime.UtcNow
                }
            };

            //get exchange rates to euro from European Central Bank
            try
            {
                var httpClient = new HttpClient();
                var stream = httpClient.GetStreamAsync("http://www.ecb.int/stats/eurofxref/eurofxref-daily.xml").Result;

                //load XML document
                var document = new XmlDocument();
                document.Load(stream);

                //add namespaces
                var namespaces = new XmlNamespaceManager(document.NameTable);
                namespaces.AddNamespace("ns", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
                namespaces.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");

                //get daily rates
                var dailyRates = document.SelectSingleNode("gesmes:Envelope/ns:Cube/ns:Cube", namespaces);
                if (!DateTime.TryParseExact(dailyRates.Attributes["time"].Value, "yyyy-MM-dd", null, DateTimeStyles.None, out var updateDate))
                    updateDate = DateTime.UtcNow;

                foreach (XmlNode currency in dailyRates.ChildNodes)
                {
                    //get rate
                    if (!decimal.TryParse(currency.Attributes["rate"].Value, NumberStyles.Currency, CultureInfo.InvariantCulture, out var currencyRate))
                        continue;

                    ratesToEuro.Add(new ExchangeRate()
                    {
                        CurrencyCode = currency.Attributes["currency"].Value,
                        Value = currencyRate,
                        LastModifiedDate = updateDate
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ECB exchange rate provider");
            }

            //return result for the euro
            if (exchangeRateCurrencyCode.Equals("eur", StringComparison.InvariantCultureIgnoreCase))
                return ratesToEuro;

            //use only currencies that are supported by ECB
            var exchangeRateCurrency = ratesToEuro.FirstOrDefault(rate => rate.CurrencyCode.Equals(exchangeRateCurrencyCode, StringComparison.InvariantCultureIgnoreCase));
            if (exchangeRateCurrency == null)
                throw new Exception("Currency not found!");

            //return result for the selected (not euro) currency
            return ratesToEuro.Select(rate => new ExchangeRate
            {
                CurrencyCode = rate.CurrencyCode,
                Value = Math.Round(rate.Value / exchangeRateCurrency.Value, 4),
                LastModifiedDate = rate.LastModifiedDate
            }).ToList();
        }

        #endregion

    }
}