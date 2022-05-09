namespace UnitTests.CryptocurrencyPaymentAPI.Validations.Exceptions
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.Validations.Exceptions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.Json;

    [TestClass]
    public class ApplicationErrorCollectionTests
    {
        private readonly IFixture fixture;

        public ApplicationErrorCollectionTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void OnApplicationErrorCollection_GivenNoParams_ShouldHaveDefaultValues()
        {
            // Arrange

            // Act
            var applicationErrorCollection = new ApplicationErrorCollection();

            // Assert
            applicationErrorCollection.ErrorMessages.Should().BeEmpty();
            applicationErrorCollection.BaseMessage.Should().BeEmpty();
        }

        [TestMethod]
        public void OnToString_GivenNoParams_ShouldReturnJsonString()
        {
            // Arrange
            var applicationErrorCollection = fixture.Create<ApplicationErrorCollection>();

            // Act
            var result = applicationErrorCollection.ToString();

            // Assert
            result.Should().Be(JsonSerializer.Serialize(applicationErrorCollection));
        }
    }
}
