using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class Topic
    {
        public string topicName { get; set; }
        public int lockDuration { get; set; }
    }
}
