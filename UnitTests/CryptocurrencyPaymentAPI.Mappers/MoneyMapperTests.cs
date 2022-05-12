namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MoneyMapperTests
    {
        private readonly Fixture fixture;

        public MoneyMapperTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void GivenValidMoney_ShouldMap()
        {
            //Arrange
            var entity = fixture.Create<Money>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetMoneyDto>();
            dto.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers());
        }

        [TestMethod]
        public void GivenInvalidMoney_ShouldntMap()
        {
            //Arrange
            Money? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetMoneyDto>();
            dto.Should().BeEquivalentTo(new GetMoneyDto());
        }
    }
}
