using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class CamundaExternalTask
    {
        public string activityId { get; set; }
        public string activityInstanceId { get; set; }
        public object errorMessage { get; set; }
        public string executionId { get; set; }
        public string id { get; set; }
        public DateTime lockExpirationTime { get; set; }
        public string processDefinitionId { get; set; }
        public string processDefinitionKey { get; set; }
        public object processDefinitionVersionTag { get; set; }
        public string processInstanceId { get; set; }
        public object retries { get; set; }
        public bool suspended { get; set; }
        public string workerId { get; set; }
        public string topicName { get; set; }
        public object tenantId { get; set; }
        public int priority { get; set; }
        public object businessKey { get; set; }
    }
}
