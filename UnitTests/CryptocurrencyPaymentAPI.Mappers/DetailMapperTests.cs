namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DetailMapperTests
    {
        private readonly Fixture fixture;

        public DetailMapperTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void GivenValidDetail_ShouldMap()
        {
            //Arrange
            var entity = fixture.Create<Detail>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetDetailDto>();
            dto.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers()
            .Excluding(o => o.Conversion.ActionName)
            .Excluding(o => o.Init.ActionName));
        }

        [TestMethod]
        public void GivenInvalidDetail_ShouldntMap()
        {
            //Arrange
            Detail? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetDetailDto>();
            dto.Should().BeEquivalentTo(new GetDetailDto());
        }
    }
}
