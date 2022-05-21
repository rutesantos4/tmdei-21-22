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

        public async Task<GetInitTransactionDto> CreatePaymentTransaction(MerchantAuthorizationDto authorizationRequestDto,  string transactionId)
        {
            log.Info($"Confirm Payment transaction '{transactionId}'");

            log.Info($"Getting transaction '{transactionId}' from DB");
            var transaction = await transactionRepository.GetByDomainIdentifier(transactionId);
            log.Info($"Got transaction '{transactionId}' from DB");

            log.Info("Validating request");
            paymentValidation.ValidateTransactionConfirm(transaction, authorizationRequestDto.MerchantId);

            var confirmPaymentTransactionDTO = new ConfirmPaymentTransactionDto()
            {
                Amount = transaction.Details.Conversion.FiatCurrency.Amount,
                FiatCurrency = transaction.Details.Conversion.FiatCurrency.Currency,
                CryptoCurrency = transaction.Details.Conversion.CryptoCurrency.Currency,
                PaymentGatewayTransactionId = transaction.PaymentGatewayTransactionId,
                PaymentGateway = transaction.PaymentGateway,
                TransactionId = transaction.DomainIdentifier
            };
            log.Info($"Creating Transaction on gateway \n{JsonConvert.SerializeObject(confirmPaymentTransactionDTO, Formatting.Indented)}");
            var paymentCreatedDto = transactionService.CreateTransaction(confirmPaymentTransactionDTO);

            log.Info($"Setting Transaction '{transaction.DomainIdentifier}'");
            transaction.PaymentGatewayTransactionId = paymentCreatedDto.PaymentGatewayTransactionId;
            transaction.Details.Init = paymentCreatedDto.ToEntity();
            transaction.TransactionState = Model.Enums.TransactionState.Initialized;

            log.Info($"Updating Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Update(transaction);
            log.Info($"Updated Transaction '{transaction.DomainIdentifier}' to DB");

            var result = transaction.ToDtoInit();

            log.Info($"Got transaction \n{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return result;
        }

        public async Task<GetRatesDto> ConvertFiatToCryptocurrency(MerchantAuthorizationDto authorizationRequestDto,
            CreatePaymentTransactionDto createPaymentTransaction)
        {
            log.Info($"Create Payment transaction \n{JsonConvert.SerializeObject(createPaymentTransaction, Formatting.Indented)}");

            log.Info("Validating request");
            paymentValidation.ValidatePaymentTransactionCreation(createPaymentTransaction);

            log.Info($"Getting Rate");
            var rates = transactionService.GetCurrencyRates(authorizationRequestDto, createPaymentTransaction);
            log.Info($"Got Rate '{rates.CurrencyRate}'");

            log.Info($"Building Transaction");
            var transaction = createPaymentTransaction.ToEntity(rates, transactionService.GetPaymentGatewayEnum(), authorizationRequestDto.MerchantId);
            log.Info($"Built Transaction");

            log.Info($"Adding Transaction '{transaction.DomainIdentifier}' to DB");
            transaction = await transactionRepository.Add(transaction);
            log.Info($"Added Transaction '{transaction.DomainIdentifier}' to DB");

            var result = transaction.ToDtoRates();

            log.Info($"Created Payment transaction \n{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return result;
        }

        public async Task<GetTransactionDto> GetTransaction(MerchantAuthorizationDto authorizationRequestDto, string transactionId)
        {
            log.Info($"Get transaction '{transactionId}'");

            log.Info($"Getting transaction '{transactionId}' from DB");
            var transaction = await transactionRepository.GetByDomainIdentifier(transactionId);
            log.Info($"Got transaction '{transactionId}' from DB");

            log.Info("Validating request");
            paymentValidation.ValidateTransactionGet(transaction, authorizationRequestDto.MerchantId);

            var result = transaction.ToDto();

            log.Info($"Got transaction \n{JsonConvert.SerializeObject(result, Formatting.Indented)}");
            return result;
        }
    }
}
