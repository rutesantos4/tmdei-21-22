namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class TransactionMapperTests
    {
        private readonly Fixture fixture;

        public TransactionMapperTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        public void GivenValidCreatePaymentTransactionDto_ShouldMap()
        {
            //Arrange
            var dto = fixture.Create<CreatePaymentTransactionDto>();
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayName = fixture.Create<PaymentGatewayName>();

            //Act
            var entity = dto.ToEntity(dtoConvertion, paymentGatewayName);

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<Transaction>();
            entity.DomainIdentifier.Should().NotBeNullOrEmpty();
            entity.PaymentGateway.Should().Be(paymentGatewayName);
            entity.TransactionState.Should().Be(TransactionState.CurrencyConverted);
            entity.PaymentGatewayTransactionId.Should().Be(dtoConvertion.PaymentGatewayTransactionId);
            entity.TransactionReference.Should().Be(dto.TransactionReference);
            entity.MerchantId.Should().Be("TODO");
            entity.History.Should().NotBeNullOrEmpty();
            entity.History.Should().HaveCount(1);
            entity.History[0].Should().BeOfType<Conversion>();
            entity.History[0].ActionName.Should().Be(ActionType.Convert);
            entity.History[0].DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.MaxValue);
            entity.History[0].Success.Should().BeTrue();
            ((Conversion)entity.History[0]).FiatCurrency.Amount.Should().Be(dto.Amount);
            ((Conversion)entity.History[0]).FiatCurrency.Currency.Should().Be(dto.FiatCurrency);
            ((Conversion)entity.History[0]).CryptoCurrency.Amount.Should().Be(dtoConvertion.CurrencyRate.Amount);
            ((Conversion)entity.History[0]).CryptoCurrency.Currency.Should().Be(dtoConvertion.CurrencyRate.Currency);
        }

        [TestMethod]
        public void GivenInvalidCreatePaymentTransactionDto_ShouldntMap()
        {
            //Arrange
            CreatePaymentTransactionDto? dto = null;
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayName = fixture.Create<PaymentGatewayName>();

            //Act
            var entity = dto.ToEntity(dtoConvertion, paymentGatewayName);

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<Transaction>();
            entity.Should().BeEquivalentTo(new Transaction());
        }


        [TestMethod]
        public void GivenValidTransaction_ShouldMap()
        {
            //Arrange
            var entity = fixture.Build<Transaction>().Without(e => e.History).Create();
            var dtoCreation = fixture.Create<CreatePaymentTransactionDto>();
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();

            //Act
            var dto = entity.ToDto(dtoCreation, dtoConvertion);

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetRatesDto>();
            dto.TransactionId.Should().Be(entity.DomainIdentifier);
            dto.Amount.Should().Be(dtoCreation.Amount);
            dto.FiatCurrency.Should().Be(dtoCreation.FiatCurrency);
            dto.Rate.Should().NotBeNull();
            dto.Rate.Amount.Should().Be(dtoConvertion.CurrencyRate.Amount);
            dto.Rate.Currency.Should().Be(dtoConvertion.CurrencyRate.Currency);
            dto.Rate.Rate.Should().Be(dtoConvertion.CurrencyRate.Rate);
        }

        [TestMethod]
        public void GivenInvalidTransaction_ShouldntMap()
        {
            //Arrange
            Transaction? entity = null;
            var dtoCreation = fixture.Create<CreatePaymentTransactionDto>();
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();

            //Act
            var dto = entity.ToDto(dtoCreation, dtoConvertion);

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetRatesDto>();
            dto.Should().BeEquivalentTo(new GetRatesDto());
        }
    }
}
