using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Model
{
    public class NewUserDataDto
    {
        public string Id { get; set; }
        public string Hash { get; set; }
        public string NewUserEmmail { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessInstanceId { get; set; }
        public string TaskId { get; set; }
        public bool SimulateFail { get; set; }
    }
}
