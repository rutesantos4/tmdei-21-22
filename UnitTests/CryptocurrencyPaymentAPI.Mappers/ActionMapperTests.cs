namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Mappers.Utils;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ActionMapperTests
    {
        private readonly Fixture fixture;

        public ActionMapperTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void GivenValidConversion_ShouldMap()
        {
            //Arrange
            var entity = fixture.Create<Conversion>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetConversionActionDto>();
            dto.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers().Excluding(x => x.ActionName));
            dto.ActionName.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName));
        }

        [TestMethod]
        public void GivenInvalidConversion_ShouldntMap()
        {
            //Arrange
            Conversion? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetConversionActionDto>();
            dto.Should().BeEquivalentTo(new GetConversionActionDto());
        }
    }
}
