namespace CryptocurrencyPaymentAPI.Services.Implementation
{
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Mappers;
    using CryptocurrencyPaymentAPI.Repositories.Interfaces;
    using CryptocurrencyPaymentAPI.Services.Interfaces;
    using CryptocurrencyPaymentAPI.Validations.Validators.Interfaces;
    using log4net;
    using Newtonsoft.Json;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PaymentService : IPaymentService
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod()?.DeclaringType);
        private readonly ITransactionRepository transactionRepository;
        private readonly IPaymentValidation paymentValidation;
        private readonly ITransactionService transactionService;

        public PaymentService(ITransactionRepository transactionRepository, 
            ITransactionService transactionService,
            IPaymentValidation paymentValidation)
        {
            this.transactionRepository = transactionRepository;
            this.paymentValidation = paymentValidation;
            this.transactionService = transactionService;
        }

        public async Task<GetRatesDto> CreatePaymentTransaction(CreatePaymentTransactionDto createPaymentTransaction)
        {
            log.Info($"Create Payment transaction \n{JsonConvert.SerializeObject(createPaymentTransaction, Formatting.Indented)}");

            log.Info("Validating request");
            paymentValidation.ValidatePaymentTransactionCreation(createPaymentTransaction);

            log.Info($"Getting Rate");
            var rates = transactionService.GetCurrencyRates(createPaymentTransaction);
            log.Info($"Got Rate '{rates.CurrencyRate}'");

            log.Info($"Building Transaction");
            var transaction = createPaymentTransaction.ToEntity(rates, transactionService.GetPaymentGatewayEnum());
            log.Info($"Built Transaction");

            log.Info($"Adding Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Add(transaction);
            log.Info($"Added Transaction '{transaction.DomainIdentifier}' to DB");

            var result = transaction.ToDto(createPaymentTransaction, rates);

            log.Info($"Created Payment transaction \n{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return result;
        }
    }
}
