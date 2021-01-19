using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Model
{
    public class WorkApplicationDataDto
    {
        public int Id { get; set; }
        public int BoardMembersApprove { get; set; }
        public bool BoardMembeNeedsMoreData { get; set; }
        public string processDefinitionId { get; set; }
        public string processInstanceId { get; set; }
    }
}
