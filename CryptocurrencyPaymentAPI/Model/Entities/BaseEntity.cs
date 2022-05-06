namespace CryptocurrencyPaymentAPI.Model.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
        public long Version { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string DomainIdentifier { get; set; }
    }
}
