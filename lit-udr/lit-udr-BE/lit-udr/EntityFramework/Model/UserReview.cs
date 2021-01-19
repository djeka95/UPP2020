using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.EntityFramework.Model
{
    public class UserReview
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int WorkApplicationDataId { get; set; }
        public WorkApplicationData WorkApplicationData { get; set; }
    }
}
