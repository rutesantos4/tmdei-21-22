namespace UnitTests.CryptocurrencyPaymentAPI.Mappers
{
    using AutoFixture;
    using FluentAssertions;
    using global::CryptocurrencyPaymentAPI.DTOs.Response;
    using global::CryptocurrencyPaymentAPI.Mappers;
    using global::CryptocurrencyPaymentAPI.Model.Entities;
    using global::CryptocurrencyPaymentAPI.Model.Enums;
    using global::CryptocurrencyPaymentAPI.Model.ValueObjects;
    using global::CryptocurrencyPaymentAPI.Services.Implementation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class NotificationMapperTests
    {
        private readonly Fixture fixture;

        public NotificationMapperTests()
        {
            fixture = new Fixture();
        }

        [TestMethod]
        [DataRow("confirmed")]
        [DataRow("complete")]
        public void GivenValidBitPayNotification_ShouldMap(string status)
        {
            //Arrange
            var notification = fixture.Build<BitPayService.InvoiceResponseData>().With(x => x.Status, status).Create();
            var entity = fixture.Create<Transaction>();

            //Act
            var newEntity = entity.BitPayNotificationToEntity(notification);

            //Assert
            newEntity.Should().NotBeNull();
            newEntity.Should().BeOfType<Transaction>();
            newEntity.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers()
            .Excluding(o => o.Details.Debit)
            .Excluding(o => o.TransactionState));
            newEntity.TransactionState.Should().Be(TransactionState.Transmitted);
            newEntity.Details.Debit.Should().NotBeNull();
            newEntity.Details.Debit.ActionName.Should().Be(ActionType.Debit);
            newEntity.Details.Debit.DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            newEntity.Details.Debit.Success.Should().BeTrue();
            newEntity.Details.Debit.Message.Should().BeNull();
            newEntity.Details.Debit.Code.Should().BeNull();
            newEntity.Details.Debit.CurrencyInfo.CryptoCurrency.Should().Be(entity.Details.Conversion.CryptoCurrency.Currency);
            newEntity.Details.Debit.CurrencyInfo.FiatCurrency.Should().Be(entity.Details.Conversion.FiatCurrency.Currency);
        }

        [TestMethod]
        [DataRow("paidOver", "Transaction was overpaid by the customer.", "402")]
        [DataRow("paidPartial", "Transaction was underpaid by the customer.", "403")]
        [DataRow("", "Transaction expired.", "401")]
        public void GivenInvalidBitPayNotification_ShouldMap(string exceptionStatus, string messageExpected, string codeExpected)
        {
            //Arrange
            var notification = fixture.Build<BitPayService.InvoiceResponseData>().With(x => x.ExceptionStatus, exceptionStatus).Create();
            var entity = fixture.Create<Transaction>();

            //Act
            var newEntity = entity.BitPayNotificationToEntity(notification);

            //Assert
            newEntity.Should().NotBeNull();
            newEntity.Should().BeOfType<Transaction>();
            newEntity.Should().BeEquivalentTo(entity, o => o.ExcludingMissingMembers()
            .Excluding(o => o.Details.Debit)
            .Excluding(o => o.TransactionState));
            newEntity.TransactionState.Should().Be(TransactionState.Failed);
            newEntity.Details.Debit.Should().NotBeNull();
            newEntity.Details.Debit.ActionName.Should().Be(ActionType.Debit);
            newEntity.Details.Debit.DateTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
            newEntity.Details.Debit.Success.Should().BeFalse();
            newEntity.Details.Debit.Message.Should().Be(messageExpected);
            newEntity.Details.Debit.Code.Should().Be(codeExpected);
            newEntity.Details.Debit.CurrencyInfo.Should().BeNull();
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
