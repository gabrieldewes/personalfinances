using System;
using System.Collections.Generic;

namespace MyApp.Models {
    public class Searchable {
        public List<long?> fields { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}