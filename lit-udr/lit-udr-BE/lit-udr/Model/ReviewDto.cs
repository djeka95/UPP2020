using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Model
{
    public class ReviewDto
    {
        public string Token { get; set; }
        public int Id { get; set; }
        public string Result { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessInstanceId { get; set; }
        public string Comment { get; set; }
    }
}
