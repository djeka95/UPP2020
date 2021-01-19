using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class FetchAndLock
    {
        public string workerId { get; set; }
        public int maxTasks { get; set; }
        public List<Topic> topics { get; set; }
    }
}
