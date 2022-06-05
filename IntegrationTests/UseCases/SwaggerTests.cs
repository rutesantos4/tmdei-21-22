namespace IntegrationTests.UseCases
{
    using AutoFixture;
    using CryptocurrencyPaymentAPI;
    using IntegrationTests.Setup;
    using Microsoft.AspNetCore.Mvc.Testing;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Xunit;

    [Collection("Integration Collection")]
    public class SwaggerTests : IClassFixture<TestFixture<Startup>>
    {
        /// <summary>
        /// Current HTTP Client being used to perform API requests
        /// </summary>
        private readonly HttpClient httpClient;
        private readonly IFixture fixture;
        private const string baseUrl = "/swagger/index.html";

        public SwaggerTests(TestFixture<Startup> testFixture)
        {
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
            httpClient.DefaultRequestHeaders.Remove("Authorization");

            // Act
            var response = await httpClient.GetAsync(baseUrl);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.True(response.Headers.Contains("WWW-Authenticate"));
            Assert.NotEmpty(response.Headers.GetValues("WWW-Authenticate"));
            Assert.True(response.Headers.Contains("Cache-Control"));
            Assert.NotEmpty(response.Headers.GetValues("Cache-Control"));
            Assert.True(response.Headers.Contains("Pragma"));
            Assert.NotEmpty(response.Headers.GetValues("Pragma"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.NotEmpty(response.Headers.GetValues("X-Frame-Options"));
            Assert.True(response.Headers.Contains("Strict-Transport-Security"));
            Assert.NotEmpty(response.Headers.GetValues("Strict-Transport-Security"));
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
            Assert.NotEmpty(response.Headers.GetValues("Content-Security-Policy"));
        }

        [Fact]
        public async Task GivenInvalidHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", fixture.Create<string>());

            // Act
            var response = await httpClient.GetAsync(baseUrl);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.True(response.Headers.Contains("WWW-Authenticate"));
            Assert.NotEmpty(response.Headers.GetValues("WWW-Authenticate"));
            Assert.True(response.Headers.Contains("Cache-Control"));
            Assert.NotEmpty(response.Headers.GetValues("Cache-Control"));
            Assert.True(response.Headers.Contains("Pragma"));
            Assert.NotEmpty(response.Headers.GetValues("Pragma"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.NotEmpty(response.Headers.GetValues("X-Frame-Options"));
            Assert.True(response.Headers.Contains("Strict-Transport-Security"));
            Assert.NotEmpty(response.Headers.GetValues("Strict-Transport-Security"));
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
            Assert.NotEmpty(response.Headers.GetValues("Content-Security-Policy"));
        }

        [Fact]
        public async Task GivenInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4x");

            // Act
            var response = await httpClient.GetAsync(baseUrl);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.True(response.Headers.Contains("WWW-Authenticate"));
            Assert.NotEmpty(response.Headers.GetValues("WWW-Authenticate"));
            Assert.True(response.Headers.Contains("Cache-Control"));
            Assert.NotEmpty(response.Headers.GetValues("Cache-Control"));
            Assert.True(response.Headers.Contains("Pragma"));
            Assert.NotEmpty(response.Headers.GetValues("Pragma"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.NotEmpty(response.Headers.GetValues("X-Frame-Options"));
            Assert.True(response.Headers.Contains("Strict-Transport-Security"));
            Assert.NotEmpty(response.Headers.GetValues("Strict-Transport-Security"));
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
            Assert.NotEmpty(response.Headers.GetValues("Content-Security-Policy"));
        }

        [Fact]
        public async Task GivenCredentials_ShouldReturnUnauthorized()
        {
            // Arrange

            // Act
            var response = await httpClient.GetAsync(baseUrl);

            // Assert
            Assert.NotNull(response);
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.False(response.Headers.Contains("WWW-Authenticate"));
            Assert.True(response.Headers.Contains("Cache-Control"));
            Assert.NotEmpty(response.Headers.GetValues("Cache-Control"));
            Assert.True(response.Headers.Contains("Pragma"));
            Assert.NotEmpty(response.Headers.GetValues("Pragma"));
            Assert.True(response.Headers.Contains("X-Frame-Options"));
            Assert.NotEmpty(response.Headers.GetValues("X-Frame-Options"));
            Assert.True(response.Headers.Contains("Strict-Transport-Security"));
            Assert.NotEmpty(response.Headers.GetValues("Strict-Transport-Security"));
            Assert.True(response.Headers.Contains("Content-Security-Policy"));
            Assert.NotEmpty(response.Headers.GetValues("Content-Security-Policy"));
        }
    }
}
