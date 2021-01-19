using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Model
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Genre { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public bool Writer { get; set; }
        public bool BetaReader { get; set; }
        public string TaskId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessInstanceId { get; set; }
    }
}
