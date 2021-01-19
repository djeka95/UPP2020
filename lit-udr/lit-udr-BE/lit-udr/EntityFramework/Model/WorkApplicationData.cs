using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.EntityFramework.Model
{
    public class WorkApplicationData
    {
        public int Id { get; set; }
        public int BoardMembersApprove { get; set; }
        public int BoardMembersInitialCount { get; set; }
        public bool BoardMembeNeedsMoreData { get; set; }
        public ICollection<UserReview> BoardMembers { get; set; }
        public string processDefinitionId { get; set; }
        public string processInstanceId { get; set; }
        public string WriterEmail { get; set; }
        public string Comments { get; set; }
    }
}
