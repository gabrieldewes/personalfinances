using System.Collections.Generic;

namespace MyApp.Models {

    public class FinanceMetadata {
        public long userId { get; set; }
        public List<FinanceType> financeTypes { get; set; }
        public List<Place> places { get; set; }
    }
}