using System.ComponentModel.DataAnnotations;

namespace MyApp.Models {

    public class UserDTO {
        public long id { get; set; }
        [Required]
        [StringLength(45, MinimumLength = 3)]
        public string username { get; set; }
        [Required]
        [StringLength(45, MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}