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
        private readonly ICryptoGatewayFactory cryptoGatewayFactory;

        public PaymentService(ITransactionRepository transactionRepository, ICryptoGatewayFactory cryptoGatewayFactory)
        {
            this.transactionRepository = transactionRepository;
            this.cryptoGatewayFactory = cryptoGatewayFactory;
        }

        public async Task<GetRatesDto> CreatePaymentTransaction(CreatePaymentTransactionDto createPaymentTransaction)
        {
            // TODO - Validation of request
            log.Info($"Create Payment transaction \n{JsonConvert.SerializeObject(createPaymentTransaction, Formatting.Indented)}");
            var cryptoGatewayService = cryptoGatewayFactory.GetCryptoGatewayService();

            log.Info($"Getting Rate");
            var rates = cryptoGatewayService.GetCurrencyRates(createPaymentTransaction);
            log.Info($"Got Rate '{rates?.CurrencyRate}'");

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
                        CryptoCurrency = new Money()
                        {
                            Currency = rates?.CurrencyRate?.Currency ?? string.Empty,
                            Amount = rates?.CurrencyRate?.Amount ?? 0,
                        },
                        ExpiryDate = DateTime.UtcNow.AddMinutes(1)
                    }
                },
                TransactionState = Model.Enums.TransactionState.CurrencyConverted,
                PaymentGatewayTransactionId = rates?.PaymentGatewayTransactionId ?? string.Empty,
                TransactionType = Model.Enums.TransactionType.Payment,
                TransactionReference = createPaymentTransaction.TransactionReference,
                MerchantId = "TODO"
            };
            log.Info($"Built Transaction");

            log.Info($"Adding Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Add(transaction);
            log.Info($"Added Transaction '{transaction.DomainIdentifier}' to DB");

            var result = new GetRatesDto()
            {
                Amount = createPaymentTransaction.Amount,
                FiatCurrency = createPaymentTransaction.FiatCurrency,
                Rate = new CurrencyRateDto()
                {
                    Amount = rates?.CurrencyRate?.Amount ?? 0,
                    Currency = rates?.CurrencyRate?.Currency ?? string.Empty,
                    Rate = rates?.CurrencyRate?.Rate ?? 0,
                },
                TransactionId = transaction.DomainIdentifier
            };

            log.Info($"Created Payment transaction \n{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return result;
        }
    }
}
