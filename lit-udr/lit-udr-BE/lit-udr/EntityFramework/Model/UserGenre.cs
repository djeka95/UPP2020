
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.EntityFramework.Model
{
    public class UserGenre
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
