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

    [Collection("Integration Collection")]
    public class UC2_CreatePaymentTransaction : IClassFixture<TestFixture<Startup>>
    {
        /// <summary>
        /// Current HTTP Client being used to perform API requests
        /// </summary>
        private readonly HttpClient httpClient;
        private readonly IFixture fixture;
        private readonly TestFixture<Startup> testFixture;

        private const string baseUrl = "/Payment/";
        private const string errorBaseMessage = "Invalid operation, check the collection of errors for more details.";

        public UC2_CreatePaymentTransaction(TestFixture<Startup> testFixture)
        {
            this.testFixture = testFixture;
            httpClient = testFixture
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    BaseAddress = new Uri("http://localhost:5001")
                });
            httpClient.DefaultRequestHeaders.Add("Authorization", testFixture.AuthorizationHeader);
            fixture = new Fixture();
        }

        [Fact]
        public async Task GivenMissingHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var transactionId = fixture.Create<string>();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(responseMessageEx);
            Assert.Equal("Missing Authorization Header", responseMessageEx?.Message.ToString());
        }

        [Fact]
        public async Task GivenInvalidHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var transactionId = fixture.Create<string>();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", fixture.Create<string>());
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(responseMessageEx);
            Assert.Equal("Invalid Authorization Header", responseMessageEx?.Message.ToString());
        }

        [Fact]
        public async Task GivenInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var transactionId = fixture.Create<string>();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4x");
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(responseMessageEx);
            Assert.Equal("Invalid Username or Password", responseMessageEx?.Message.ToString());
        }

        [Fact]
        public async Task GivenInvalidTransactionId_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = fixture.Create<string>();

            // Act
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
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
        public async Task GivenDifferentMerchatId_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionFailded;

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", testFixture.AuthorizationHeader2);
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx?.Message?.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Transaction does not exists.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenExpiredRate_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionRateExpired;

            // Act
            var response = await httpClient.PostAsync(baseUrl + transactionId, null);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx.Message.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Convertion Rate expired. Please perform convertion again.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenValidRequest_ShouldReturnGetInitTransactionDto()
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, "EUR")
                .With(e => e.CryptoCurrency, "BTC")
                .Create();
            var responseConvertion = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageConvertion = await responseConvertion.Content.ReadFromJsonAsync<GetRatesDto>();

            // Act
            var response = await httpClient.PostAsync(baseUrl + responseMessageConvertion.TransactionId, null);
            var responseMessage = await response.Content.ReadFromJsonAsync<GetInitTransactionDto>();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.NotNull(responseMessage?.TransactionId);
            Assert.NotEmpty(responseMessage?.TransactionId);
            Assert.NotNull(responseMessage?.PaymentInfo);
            Assert.NotEmpty(responseMessage?.PaymentInfo);
            Assert.NotNull(responseMessage?.ExpiryDate);
            Assert.Equal(DateTime.UtcNow.Day, responseMessage?.ExpiryDate?.Day);
            Assert.Equal(DateTime.UtcNow.Month, responseMessage?.ExpiryDate?.Month);
            Assert.Equal(DateTime.UtcNow.Year, responseMessage?.ExpiryDate?.Year);
        }

        [Fact]
        public async Task GivenDifferentMerchatId_GetShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionFailded;

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", testFixture.AuthorizationHeader2);
            var response = await httpClient.GetAsync(baseUrl + transactionId);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx?.Message?.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Transaction does not exists.", responseMessage?.ErrorMessages[0]);
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
            var responseConvertion = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageConvertion = await responseConvertion.Content.ReadFromJsonAsync<GetRatesDto>();
            var responseInit = await httpClient.PostAsync(baseUrl + responseMessageConvertion?.TransactionId, null);
            var responseMessageInit = await responseInit.Content.ReadFromJsonAsync<GetInitTransactionDto>();

            // Act
            var response = await httpClient.GetAsync(baseUrl + responseMessageConvertion.TransactionId);
            var responseMessage = await response.Content.ReadFromJsonAsync<GetTransactionDto>();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal("Payment", responseMessage?.TransactionType);
            Assert.Equal("Initialized", responseMessage?.TransactionState);
            Assert.Equal(dto.TransactionReference, responseMessage?.MerchantTransactionReference);
            Assert.Equal(responseMessageConvertion.TransactionId, responseMessage?.TransactionReference);
            Assert.NotNull(responseMessage?.PaymentGateway);
            Assert.NotEmpty(responseMessage?.PaymentGateway);
            Assert.NotNull(responseMessage?.Details);

            Assert.NotNull(responseMessage?.Details.Conversion);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Conversion.DateTime.Date);
            Assert.NotNull(responseMessage?.Details.Conversion.ActionName);
            Assert.True(responseMessage?.Details.Conversion.Success);
            Assert.Null(responseMessage?.Details.Conversion.Reason);
            Assert.Null(responseMessage?.Details.Conversion.Message);
            Assert.NotNull(responseMessage?.Details.Conversion.ExpiryDate);
            Assert.NotNull(responseMessage?.Details.Conversion.FiatCurrency);
            Assert.Equal(responseMessageConvertion.FiatCurrency, responseMessage?.Details.Conversion.FiatCurrency?.Currency);
            Assert.Equal(responseMessageConvertion.Amount, responseMessage?.Details.Conversion.FiatCurrency?.Amount);
            Assert.NotNull(responseMessage?.Details.Conversion.CryptoCurrency);
            Assert.Equal(responseMessageConvertion?.Rate?.Currency, responseMessage?.Details.Conversion.CryptoCurrency?.Currency);
            Assert.Equal(responseMessageConvertion?.Rate?.Amount, responseMessage?.Details.Conversion.CryptoCurrency?.Amount);

            Assert.NotNull(responseMessage.Details.Init);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage.Details.Init?.DateTime.Date);
            Assert.NotNull(responseMessage?.Details.Init?.ActionName);
            Assert.True(responseMessage?.Details.Init?.Success);
            Assert.Null(responseMessage?.Details.Init?.Reason);
            Assert.Null(responseMessage?.Details.Init?.Message);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Init?.ExpiryDate.Date);
            Assert.Equal(responseMessageInit?.PaymentInfo, responseMessage?.Details.Init?.PaymentInfo);
            Assert.Equal(responseMessageInit?.ExpiryDate, responseMessage?.Details.Init?.ExpiryDate);
            Assert.Equal(responseMessageInit?.TransactionId, responseMessage?.TransactionReference);

            Assert.Null(responseMessage?.Details.Debit);
        }
    }
}
