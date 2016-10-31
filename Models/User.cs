using System.ComponentModel.DataAnnotations;

namespace MyApp.Models {
    public class User {
        public long id { get; set; }
        
        [StringLength(45, MinimumLength = 3)]
        public string fullname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(45, MinimumLength = 3)]
        public string email { get; set; }

        [Required]
        [StringLength(45, MinimumLength = 3)]
        public string username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(45, MinimumLength = 3)]
        public string password { get; set; }
        public string role { get; set; }
    }
}