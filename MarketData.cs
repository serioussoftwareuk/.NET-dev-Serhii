using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finapi
{
    [Table("Markets")]
    public class Market
    {
        [Key]
        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string MarketId { get; set; }
        public string ProviderId { get; set; }
        public string Response { get; set; }
    }
}
