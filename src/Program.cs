using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace LiveCurrencyExchangeRate
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IExchangeRateProvider, TcbExchangeRateProvider>()
            .AddSingleton<IExchangeRateProvider, EcbExchangeRateProvider>()
            .BuildServiceProvider();

            var exchangeRateProviders = serviceProvider.GetServices<IExchangeRateProvider>();
            foreach (var provider in exchangeRateProviders)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Provider : {provider.GetType().Name.ToUpper()}");
                var rates = provider.GetCurrencyLiveRates("USD");
                PrintResults(rates);
            }
            Console.ResetColor();
        }
        static void PrintResults(IList<ExchangeRate> rates)
        {
            foreach (var rate in rates)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(rate.ToString());
            }
        }
    }
}
