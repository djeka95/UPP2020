using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.EntityFramework.Model
{
    public class NewUserData
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public bool Writer { get; set; }
        public string NewUserEmmail { get; set; }
        public string processDefinitionId { get; set; }
        public string processInstanceId { get; set; }

    }
}
