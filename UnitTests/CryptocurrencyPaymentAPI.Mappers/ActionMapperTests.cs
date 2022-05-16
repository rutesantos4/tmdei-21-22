namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Mappers.Utils;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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
            var entity = fixture.Create<ConversionAction>();

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
            ConversionAction? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetConversionActionDto>();
            dto.Should().BeEquivalentTo(new GetConversionActionDto());
        }

        [TestMethod]
        public void GivenValidInit_ShouldMap()
        {
            //Arrange
            var entity = fixture.Create<InitAction>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetInitActionDto>();
            dto.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers().Excluding(x => x.ActionName));
            dto.ActionName.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName));
        }

        [TestMethod]
        public void GivenInvalidInit_ShouldntMap()
        {
            //Arrange
            InitAction? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().BeNull();
        }

        [TestMethod]
        public void GivenValidGetInitActionDto_ShouldMap()
        {
            //Arrange
            var dto = fixture.Create<PaymentCreatedDto>();

            //Act
            var entity = dto.ToEntity();

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<InitAction>();
            entity.Should().BeEquivalentTo(dto, o => o.ExcludingMissingMembers());
            entity.ActionName.Should().Be(ActionType.Init);
            entity.DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.MaxValue);
            entity.Success.Should().BeTrue();
            entity.ExpiryDate.Should().Be(dto.ExpiryDate);
            entity.PaymentInfo.Should().Be(dto.PaymentLink);
            entity.Message.Should().BeNull();
            entity.Code.Should().BeNull();
        }

        [TestMethod]
        [DataRow(" ")]
        [DataRow(null)]
        public void GivenValidGetInitActionDtoButNoPaymentLink_ShouldMap(string paymentLink)
        {
            //Arrange
            var dto = fixture.Build<PaymentCreatedDto>().With(x => x.PaymentLink, paymentLink).Create();

            //Act
            var entity = dto.ToEntity();

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<InitAction>();
            entity.Should().BeEquivalentTo(dto, o => o.ExcludingMissingMembers());
            entity.ActionName.Should().Be(ActionType.Init);
            entity.DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.MaxValue);
            entity.Success.Should().BeTrue();
            entity.ExpiryDate.Should().Be(dto.ExpiryDate);
            entity.PaymentInfo.Should().Be(dto.WalletId);
            entity.Message.Should().BeNull();
            entity.Code.Should().BeNull();
        }

        [TestMethod]
        public void GivenInvalidGetInitActionDto_ShouldntMap()
        {
            //Arrange
            PaymentCreatedDto? dto = null;

            //Act
            var entity = dto.ToEntity();

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<InitAction>();
            entity.Should().BeEquivalentTo(new InitAction());
        }

        [TestMethod]
        public void GivenValidDebit_ShouldMap()
        {
            //Arrange
            var entity = fixture.Create<DebitAction>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetDebitActionDto>();
            dto.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers().Excluding(x => x.ActionName));
            dto.ActionName.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.ActionName));
        }

        [TestMethod]
        public void GivenInvalidDebit_ShouldntMap()
        {
            //Arrange
            DebitAction? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().BeNull();
        }
    }
}
