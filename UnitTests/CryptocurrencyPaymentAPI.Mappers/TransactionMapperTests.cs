﻿namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs;
    using global::CryptocurrencyPaymentAPI.DTOs.Request;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Mappers.Utils;
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
        public void GivenValidCreatePaymentTransactionDto_ShouldMapTransaction()
        {
            //Arrange
            var dto = fixture.Create<CreatePaymentTransactionDto>();
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayName = fixture.Create<PaymentGatewayName>();
            var merchantId = fixture.Create<string>();

            //Act
            var entity = dto.ToEntity(dtoConvertion, paymentGatewayName, merchantId);

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<Transaction>();
            entity.DomainIdentifier.Should().NotBeNullOrEmpty();
            entity.PaymentGateway.Should().Be(paymentGatewayName);
            entity.TransactionState.Should().Be(TransactionState.CurrencyConverted);
            entity.PaymentGatewayTransactionId.Should().Be(dtoConvertion.PaymentGatewayTransactionId);
            entity.TransactionReference.Should().Be(dto.TransactionReference);
            entity.MerchantId.Should().Be(merchantId);
            entity.Details.Should().NotBeNull();
            entity.Details.Conversion.Should().BeOfType<ConversionAction>();
            entity.Details.Conversion.ActionName.Should().Be(ActionType.Convert);
            entity.Details.Conversion.DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(15));
            entity.Details.Conversion.Success.Should().BeTrue();
            entity.Details.Conversion.FiatCurrency.Amount.Should().Be(dto.Amount);
            entity.Details.Conversion.FiatCurrency.Currency.Should().Be(dto.FiatCurrency);
            entity.Details.Conversion.CryptoCurrency.Amount.Should().Be(dtoConvertion.CurrencyRate.Amount);
            entity.Details.Conversion.CryptoCurrency.Currency.Should().Be(dtoConvertion.CurrencyRate.Currency);
            entity.Details.Init.Should().BeNull();
            entity.Details.Debit.Should().BeNull();
        }

        [TestMethod]
        public void GivenInvalidCreatePaymentTransactionDto_ShouldntMapTransaction()
        {
            //Arrange
            CreatePaymentTransactionDto? dto = null;
            var dtoConvertion = fixture.Create<CurrencyConvertedDto>();
            var paymentGatewayName = fixture.Create<PaymentGatewayName>();
            var merchantId = fixture.Create<string>();

            //Act
            var entity = dto.ToEntity(dtoConvertion, paymentGatewayName, merchantId);

            //Assert
            entity.Should().NotBeNull();
            entity.Should().BeOfType<Transaction>();
            entity.Should().BeEquivalentTo(new Transaction());
        }

        [TestMethod]
        public void GivenValidTransaction_ShouldMapGetRatesDto()
        {
            //Arrange
            var entity = fixture.Create<Transaction>();

            //Act
            var dto = entity.ToDtoRates();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetRatesDto>();
            dto.TransactionId.Should().Be(entity.DomainIdentifier);
            dto.Amount.Should().Be(entity.Details.Conversion.FiatCurrency.Amount);
            dto.FiatCurrency.Should().Be(entity.Details.Conversion.FiatCurrency.Currency);
            dto.Rate.Should().NotBeNull();
            dto.Rate?.Amount.Should().Be(entity.Details.Conversion.CryptoCurrency.Amount);
            dto.Rate?.Currency.Should().Be(entity.Details.Conversion.CryptoCurrency.Currency);
            dto.Rate?.Rate.Should().Be(entity.Details.Conversion.Rate);
            dto.Rate?.ExpiryDate.Should().Be(entity.Details.Conversion.ExpiryDate);
        }

        [TestMethod]
        public void GivenInvalidTransaction_ShouldntMapGetRatesDto()
        {
            //Arrange
            Transaction? entity = null;

            //Act
            var dto = entity.ToDtoRates();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetRatesDto>();
            dto.Should().BeEquivalentTo(new GetRatesDto());
        }

        [TestMethod]
        public void GivenValidTransaction_ShouldMapGetTransactionDto()
        {
            //Arrange
            var entity = fixture.Create<Transaction>();

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetTransactionDto>();
            dto.Should().BeEquivalentTo(entity, o => o
                .ExcludingMissingMembers()
                .Excluding(e => e.TransactionReference)
                .Excluding(e => e.TransactionState)
                .Excluding(e => e.TransactionType)
                .Excluding(e => e.PaymentGateway)
                .Excluding(e => e.Details.Conversion.ActionName)
                .Excluding(e => e.Details.Init)
                .Excluding(e => e.Details.Debit));
            dto.TransactionReference.Should().Be(entity.DomainIdentifier);
            dto.MerchantTransactionReference.Should().Be(entity.TransactionReference);
            dto.TransactionState.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.TransactionState));
            dto.TransactionType.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.TransactionType));
            dto.PaymentGateway.Should().Be(EnumDescriptionHelper.GetEnumValueAsString(entity.PaymentGateway));
        }

        [TestMethod]
        public void GivenInvalidTransaction_ShouldntMapGetTransactionDto()
        {
            //Arrange
            Transaction? entity = null;

            //Act
            var dto = entity.ToDto();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetTransactionDto>();
            dto.Should().BeEquivalentTo(new GetTransactionDto());
        }

        [TestMethod]
        public void GivenValidTransaction_ShouldMapGetInitTransactionDto()
        {
            //Arrange
            var entity = fixture.Create<Transaction>();

            //Act
            var dto = entity.ToDtoInit();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetInitTransactionDto>();
            dto.Should().BeEquivalentTo(entity, o => o
                .ExcludingMissingMembers());
            dto.TransactionId.Should().Be(entity.DomainIdentifier);
            dto.ExpiryDate.Should().Be(entity.Details.Init?.ExpiryDate);
            dto.PaymentInfo.Should().Be(entity.Details.Init?.PaymentInfo);
        }

        [TestMethod]
        public void GivenInvalidTransaction_ShouldntMapGetInitTransactionDto()
        {
            //Arrange
            Transaction? entity = null;

            //Act
            var dto = entity.ToDtoInit();

            //Assert
            dto.Should().NotBeNull();
            dto.Should().BeOfType<GetInitTransactionDto>();
            dto.Should().BeEquivalentTo(new GetInitTransactionDto());
        }
    }
}
