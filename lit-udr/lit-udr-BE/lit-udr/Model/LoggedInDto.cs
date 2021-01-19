using lit_udr.EntityFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Model
{
    public class LoggedInDto
    {
        public string Token { get; set; }
        
        public WorkApplicationData workApplicationData { get; set; }

    }
}
