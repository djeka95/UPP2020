using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.EntityFramework.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public ICollection<UserGenre> UserGenres { get; set; }

        public ICollection<UserReview> WorkApplicationData { get; set; }

        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]

        public bool Writer { get; set; }

        public bool BetaReader { get; set; }

        public bool UserVerified { get; set; }

        public int UserRetryCount { get; set; }
    }
}
