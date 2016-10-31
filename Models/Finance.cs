using System;
using System.ComponentModel.DataAnnotations;

namespace MyApp.Models {

    public class Finance {
        public long id { get; set; }
        [Required]
        public long userId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime date { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public Decimal value { get; set; }
        [Required]
        public int status { get; set; }
        [Required]
        public int level { get; set; }
        [Required]
        public FinanceType financeType { get; set; }
        [Required]
        public Place place { get; set; }
    }
}