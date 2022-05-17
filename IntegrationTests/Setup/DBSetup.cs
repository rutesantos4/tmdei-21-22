namespace IntegrationTests.Setup
{
    using AutoFixture;
    using CryptocurrencyPaymentAPI.Model.Entities;
    using CryptocurrencyPaymentAPI.Model.ValueObjects;
    using CryptocurrencyPaymentAPI.Repositories;
    using System;

    internal static class DBSetup
    {
        internal static readonly string TransactionRateExpired = "TransactionRateExpired";
        internal static readonly string TransactionFailded = "TransactionFailded";
        internal static readonly string TransactionTransmitted = "TransactionTransamitted";

        internal static void InitializeDbForTests(DatabaseContext db)
        {
            var fixture = new Fixture();
            var conversion = fixture
                .Build<ConversionAction>()
                .With(x => x.ExpiryDate, new DateTime(2022, 05, 10, 22, 35, 5))
                .Create();
            var details = fixture
                .Build<Detail>()
                .With(x => x.Conversion, conversion)
                .Create();
            var entity = fixture
                .Build<Transaction>()
                .With(e => e.IsDeleted, false)
                .With(e => e.DomainIdentifier, TransactionRateExpired)
                .With(e => e.Details, details)
                .With(e => e.TransactionState, CryptocurrencyPaymentAPI.Model.Enums.TransactionState.CurrencyConverted)
                .Create();
            db.Transactions.Add(entity);

            var transactionFailed = fixture
                .Build<Transaction>()
                .With(e => e.IsDeleted, false)
                .With(e => e.DomainIdentifier, TransactionFailded)
                .With(e => e.TransactionState, CryptocurrencyPaymentAPI.Model.Enums.TransactionState.Failed)
                .Create();
            db.Transactions.Add(transactionFailed);

            var transactionTransmitted = fixture
                .Build<Transaction>()
                .With(e => e.IsDeleted, false)
                .With(e => e.DomainIdentifier, TransactionTransmitted)
                .With(e => e.TransactionState, CryptocurrencyPaymentAPI.Model.Enums.TransactionState.Transmitted)
                .Create();
            db.Transactions.Add(transactionTransmitted);

            db.SaveChanges();
        }

    }
}
