namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PaymentService : IPaymentService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ITransactionRepository transactionRepository;

        public PaymentService(ITransactionRepository transactionRepository)
        {
            this.transactionRepository = transactionRepository;
        }

        public async Task<GetRatesDto> CreatePaymentTransaction(CreatePaymentTransactionDto createPaymentTransaction)
        {
            log.Info($"Create Payment transaction \n{JsonConvert.SerializeObject(createPaymentTransaction, Formatting.Indented)}");
            ICryptoGatewayService cryptoGatewayService = new CryptoGatewayFactory().GetCryptoGatewayService();

            log.Info($"Getting Rates");
            var rates = cryptoGatewayService.GetCurrencyRates();
            log.Info($"Got {rates?.CurrencyRates?.Count} Rates");

            log.Info($"Building Transaction");
            var transaction = new Transaction()
            {
                DomainIdentifier = Guid.NewGuid().ToString(),
                PaymentGateway = cryptoGatewayService.GetPaymentGatewayEnum(),
                History = new List<Action>()
                {
                    new Conversion()
                    {
                        ActionName = Model.Enums.ActionType.Convert,
                        DateTime = DateTime.UtcNow,
                        Success = true,
                        FiatCurrency = new Money()
                        {
                            Amount = createPaymentTransaction.Amount,
                            Currency = createPaymentTransaction.FiatCurrency
                        },
                        CryptoCurrencies = rates?.CurrencyRates?.Select(y => new Money()
                        {
                            Currency = y.Currency,
                            Amount = y.Amount,
                        })?.ToList() ?? new List<Money>(),
                        ExpiryDate = DateTime.UtcNow.AddMinutes(1)
                    }
                },
                TransactionState = Model.Enums.TransactionState.CurrencyConverted,
                PaymentGatewayTransactionId = rates?.PaymentGatewayTransactionId ?? string.Empty,
                TransactionType = Model.Enums.TransactionType.Payment,
                TransactionReference = createPaymentTransaction.TransactionReference
            };
            log.Info($"Built Transaction");

            log.Info($"Adding Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Add(transaction);
            log.Info($"Added Transaction '{transaction.DomainIdentifier}' to DB");

            var result = new GetRatesDto()
            {
                Amount = createPaymentTransaction.Amount,
                FiatCurrency = createPaymentTransaction.FiatCurrency,
                Rates = rates?.CurrencyRates?.Select(x => new CurrencyRateDto())?.ToList() ?? new List<CurrencyRateDto>(),
                TransactionId = transaction.DomainIdentifier
            };

            return result;
        }
    }
}
