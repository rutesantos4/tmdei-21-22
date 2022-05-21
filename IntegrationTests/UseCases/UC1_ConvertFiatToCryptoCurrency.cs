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
    public class UC1_ConvertFiatToCryptoCurrency : IClassFixture<TestFixture<Startup>>
    {
        /// <summary>
        /// Current HTTP Client being used to perform API requests
        /// </summary>
        private readonly HttpClient httpClient;
        private readonly IFixture fixture;
        private readonly TestFixture<Startup> testFixture;

        private const string baseUrl = "/Payment";
        private const string errorBaseMessage = "Invalid operation, check the collection of errors for more details.";

        public UC1_ConvertFiatToCryptoCurrency(TestFixture<Startup> testFixture)
        {
            this.testFixture = testFixture;
            httpClient = testFixture
                .CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                    BaseAddress = new Uri("http://localhost:5001"),
                });
            httpClient.DefaultRequestHeaders.Add("Authorization", testFixture.AuthorizationHeader);
            fixture = new Fixture();
        }

        [Fact]
        public async Task GivenMissingHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var amount = fixture.Create<double>() % 1;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount)
                .Create();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
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
            var amount = fixture.Create<double>() % 1;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount)
                .Create();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", fixture.Create<string>());
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
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
            var amount = fixture.Create<double>() % 1;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount)
                .Create();

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4x");
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotNull(responseMessageEx);
            Assert.Equal("Invalid Username or Password", responseMessageEx?.Message.ToString());
        }

        [Fact]
        public async Task GivenInvalidAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var amount = fixture.Create<double>() % 1;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount )
                .Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx.Message.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Amount must be bigger than zero.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenInvalidFiatCurrency_ShouldReturnBadRequest()
        {
            // Arrange
            string? currency = null;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, currency)
                .Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx?.Message?.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Missing Fiat Currency.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenInvalidCryptoCurrency_ShouldReturnBadRequest()
        {
            // Arrange
            string? currency = null;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.CryptoCurrency, currency)
                .Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx?.Message?.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Single(responseMessage?.ErrorMessages);
            Assert.Equal("Missing Cryptocurrency.", responseMessage?.ErrorMessages[0]);
        }

        [Fact]
        public async Task GivenInvalidRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var amount = fixture.Create<double>() % 1;
            string? currency = null;
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.Amount, amount)
                .With(e => e.FiatCurrency, currency)
                .With(e => e.CryptoCurrency, currency)
                .Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageEx = await response.Content.ReadFromJsonAsync<ExceptionResult>();
            var responseMessage = JsonSerializer.Deserialize<ApplicationErrorCollection>(responseMessageEx?.Message?.ToString());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal(errorBaseMessage, responseMessage?.BaseMessage);
            Assert.Equal(3, responseMessage?.ErrorMessages.Count);
            Assert.True(responseMessage?.ErrorMessages.Contains("Missing Fiat Currency."));
            Assert.True(responseMessage?.ErrorMessages.Contains("Missing Cryptocurrency."));
            Assert.True(responseMessage?.ErrorMessages.Contains("Amount must be bigger than zero."));
        }

        [Fact]
        public async Task GivenValidRequest_ShouldReturnGetRatesDto()
        {
            // Arrange
            var dto = fixture
                .Build<CreatePaymentTransactionDto>()
                .With(e => e.FiatCurrency, "EUR")
                .With(e => e.CryptoCurrency, "BTC")
                .Create();

            // Act
            var response = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessage = await response.Content.ReadFromJsonAsync<GetRatesDto>();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.NotNull(responseMessage?.TransactionId);
            Assert.NotEmpty(responseMessage?.TransactionId);
            Assert.NotEqual(0, responseMessage?.Amount);
            Assert.NotNull(responseMessage?.FiatCurrency);
            Assert.NotEmpty(responseMessage?.FiatCurrency);
            Assert.NotNull(responseMessage?.Rate);
            Assert.NotEqual(0, responseMessage?.Rate?.Rate);
            Assert.NotEqual(0, responseMessage?.Rate?.Amount);
            Assert.NotNull(responseMessage?.Rate?.Currency);
            Assert.NotEmpty(responseMessage?.Rate?.Currency);
            Assert.Equal(DateTime.UtcNow.Day, responseMessage?.Rate?.ExpiryDate.Day);
            Assert.Equal(DateTime.UtcNow.Month, responseMessage?.Rate?.ExpiryDate.Month);
            Assert.Equal(DateTime.UtcNow.Year, responseMessage?.Rate?.ExpiryDate.Year);
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
            var responseCreate = await httpClient.PostAsJsonAsync(baseUrl, dto);
            var responseMessageCreate = await responseCreate.Content.ReadFromJsonAsync<GetRatesDto>();

            // Act
            var response = await httpClient.GetAsync($"{baseUrl}/{responseMessageCreate.TransactionId}");
            var responseMessage = await response.Content.ReadFromJsonAsync<GetTransactionDto>();

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(responseMessage);
            Assert.Equal("Payment", responseMessage?.TransactionType);
            Assert.Equal("CurrencyConverted", responseMessage?.TransactionState);
            Assert.Equal(dto.TransactionReference, responseMessage?.MerchantTransactionReference);
            Assert.Equal(responseMessageCreate.TransactionId, responseMessage?.TransactionReference);
            Assert.NotNull(responseMessage?.PaymentGateway);
            Assert.NotEmpty(responseMessage?.PaymentGateway);
            Assert.NotNull(responseMessage?.Details);
            Assert.NotNull(responseMessage?.Details.Conversion);
            Assert.Null(responseMessage?.Details.Conversion.Reason);
            Assert.Null(responseMessage?.Details.Conversion.Message);
            Assert.NotNull(responseMessage?.Details.Conversion.ExpiryDate);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Conversion.DateTime.Date);
            Assert.Equal(DateTime.UtcNow.Date, responseMessage?.Details.Conversion?.ExpiryDate?.Date);
            Assert.NotNull(responseMessage?.Details.Conversion?.FiatCurrency);
            Assert.Equal(responseMessageCreate.FiatCurrency, responseMessage?.Details.Conversion?.FiatCurrency?.Currency);
            Assert.Equal(responseMessageCreate.Amount, responseMessage?.Details.Conversion?.FiatCurrency?.Amount);
            Assert.NotNull(responseMessage?.Details.Conversion?.CryptoCurrency);
            Assert.Equal(responseMessageCreate.Rate?.Currency, responseMessage?.Details.Conversion?.CryptoCurrency?.Currency);
            Assert.Equal(responseMessageCreate.Rate?.Amount, responseMessage?.Details.Conversion?.CryptoCurrency?.Amount);
            Assert.Null(responseMessage?.Details.Init);
            Assert.Null(responseMessage?.Details.Debit);
        }

        [Fact]
        public async Task GivenDifferentMerchatId_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = testFixture.TransactionFailded;

            // Act
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", testFixture.AuthorizationHeader2);
            var response = await httpClient.GetAsync($"{baseUrl}/{transactionId}");
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
        public async Task GivenInvalidTransactionId_ShouldReturnBadRequest()
        {
            // Arrange
            var transactionId = fixture.Create<string>();

            // Act
            var response = await httpClient.GetAsync($"{baseUrl}/{transactionId}");
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
    }
}
