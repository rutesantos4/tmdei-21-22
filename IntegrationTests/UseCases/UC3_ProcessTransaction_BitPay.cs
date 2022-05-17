namespace IntegrationTests.UseCases
{
    using AutoFixture;
    using CryptocurrencyPaymentAPI;
    using CryptocurrencyPaymentAPI.DTOs.Request;
    using CryptocurrencyPaymentAPI.DTOs.Response;
    using CryptocurrencyPaymentAPI.Middlewares;
    using CryptocurrencyPaymentAPI.Validations.Exceptions;
    using IntegrationTests.Setup;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Xunit;
    using static CryptocurrencyPaymentAPI.Services.Implementation.BitPayService;

    [Collection("Integration Collection")]
    public class UC3_ProcessTransaction_BitPay : IClassFixture<TestFixture<Startup>>
    {
        /// <summary>
        /// Current HTTP Client being used to perform API requests
        /// </summary>
        private readonly HttpClient httpClient;
        private readonly IFixture fixture;
        private readonly TestFixture<Startup> testFixture;

        private const string baseUrl = "/notification/bitpay/";
        private const string errorBaseMessage = "Invalid operation, check the collection of errors for more details.";

        public UC3_ProcessTransaction_BitPay(TestFixture<Startup> testFixture)
        {
            this.testFixture = testFixture;
            httpClient = testFixture
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    BaseAddress = new Uri("http://localhost:5001")
                });
            fixture = new Fixture();
        }

        [Fact]
        public async Task GivenInvalidTransactionId_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = fixture.Create<string>();
            var body = fixture.Create<InvoiceResponseData>();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl + transactionId, body);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx.Message.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Transaction does not exists.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenFailedTransaction_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionFailded;
            var body = fixture.Create<InvoiceResponseData>();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl + transactionId, body);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx.Message.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Transaction State is wrong, it should be Initialized.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenTransmittedTransaction_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionTransmitted;
            var body = fixture.Create<InvoiceResponseData>();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl + transactionId, body);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx.Message.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Transaction State is wrong, it should be Initialized.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenValidRequest_ShouldReturnEmpty()
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, "EUR")
                .With(e => e.CryptoCurrency, "BTC")
                .Create();
            var responseConvertion = await httpClient.PostAsJsonAsync("/payment/", dto);
            var responseMessageConvertion = await responseConvertion.Content.ReadFromJsonAsync<GetRatesDto>();
            var responseAdd = await httpClient.PostAsync("/payment/" + responseMessageConvertion?.TransactionId, null);
            var responseMessageAdd = await responseAdd.Content.ReadFromJsonAsync<GetInitTransactionDto>();
            var notification = fixture.Build<InvoiceResponseData>().With(x => x.Status, "confirmed").Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl + responseMessageAdd?.TransactionId, notification);
            var responseMessageEx = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(responseMessageEx);
        }

        [Fact]
        public async Task GivenValidTransactionId_ShouldReturnGetTransactionDto()
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, "EUR")
                .With(e => e.CryptoCurrency, "BTC")
                .Create();
            var responseConvertion = await httpClient.PostAsJsonAsync("/payment/", dto);
            var responseMessageConvertion = await responseConvertion.Content.ReadFromJsonAsync<GetRatesDto>();
            var responseInit = await httpClient.PostAsync("/payment/" + responseMessageConvertion?.TransactionId, null);
            var responseMessageInit = await responseInit.Content.ReadFromJsonAsync<GetInitTransactionDto>();
            var notification = fixture.Build<InvoiceResponseData>().With(x => x.Status, "confirmed").Create();
            await httpClient.PostAsJsonAsync(baseUrl + responseMessageInit?.TransactionId, notification);

            // Act
            var response = await httpClient.GetAsync("/payment/" + responseMessageConvertion?.TransactionId);
            var responseMessage = await response.Content.ReadFromJsonAsync<GetTransactionDto>();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal("Payment", responseMessage?.TransactionType);
            Assert.Equal("Transmitted", responseMessage?.TransactionState);
            Assert.Equal(dto.TransactionReference, responseMessage?.MerchantTransactionReference);
            Assert.Equal(responseMessageConvertion?.TransactionId, responseMessage?.TransactionReference);
            Assert.NotNull(responseMessage?.PaymentGateway);
            Assert.NotEmpty(responseMessage?.PaymentGateway);
            Assert.NotNull(responseMessage?.Details);

            Assert.NotNull(responseMessage?.Details.Conversion);
            Assert.NotNull(responseMessage?.Details.Conversion.DateTime);
            Assert.NotNull(responseMessage?.Details.Conversion.ActionName);
            Assert.True(responseMessage?.Details.Conversion.Success);
            Assert.Null(responseMessage?.Details.Conversion.Reason);
            Assert.Null(responseMessage?.Details.Conversion.Message);
            Assert.NotNull(responseMessage?.Details.Conversion.ExpiryDate);
            Assert.NotNull(responseMessage?.Details.Conversion.FiatCurrency);
            Assert.Equal(responseMessageConvertion?.FiatCurrency, responseMessage?.Details.Conversion.FiatCurrency?.Currency);
            Assert.Equal(responseMessageConvertion?.Amount, responseMessage?.Details.Conversion.FiatCurrency?.Amount);
            Assert.NotNull(responseMessage?.Details.Conversion.CryptoCurrency);
            Assert.Equal(responseMessageConvertion?.Rate?.Currency, responseMessage?.Details.Conversion.CryptoCurrency?.Currency);
            Assert.Equal(responseMessageConvertion?.Rate?.Amount, responseMessage?.Details.Conversion.CryptoCurrency?.Amount);

            Assert.NotNull(responseMessage?.Details.Init);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Init?.ExpiryDate.Date);
            Assert.NotNull(responseMessage?.Details.Init?.ActionName);
            Assert.True(responseMessage?.Details.Init?.Success);
            Assert.Null(responseMessage?.Details.Init?.Reason);
            Assert.Null(responseMessage?.Details.Init?.Message);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Init?.ExpiryDate.Date);
            Assert.Equal(responseMessageInit?.PaymentInfo, responseMessage?.Details.Init?.PaymentInfo);
            Assert.Equal(responseMessageInit?.ExpiryDate, responseMessage?.Details.Init?.ExpiryDate);
            Assert.Equal(responseMessageInit?.TransactionId, responseMessage?.TransactionReference);

            Assert.NotNull(responseMessage?.Details.Debit);
            Assert.Equal(responseMessageConvertion?.Rate?.Currency, responseMessage?.Details.Debit?.CryptoCurrency);
            Assert.Equal(responseMessageConvertion?.FiatCurrency, responseMessage?.Details.Debit?.FiatCurrency);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Debit?.DateTime.Date);
            Assert.NotNull(responseMessage?.Details.Debit?.ActionName);
            Assert.True(responseMessage?.Details.Debit?.Success);
            Assert.Null(responseMessage?.Details.Debit?.Reason);
            Assert.Null(responseMessage?.Details.Debit?.Message);
        }
    }
}
