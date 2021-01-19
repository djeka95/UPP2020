using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class CamundaTask
    {
        public string id { get; set; }
        public string name { get; set; }
        public string assignee { get; set; }
        public DateTime created { get; set; }
        public DateTime? due { get; set; }
        public object followUp { get; set; }
        public object delegationState { get; set; }
        public string description { get; set; }
        public string executionId { get; set; }
        public object owner { get; set; }
        public object parentTaskId { get; set; }
        public int priority { get; set; }
        public string processDefinitionId { get; set; }
        public string processInstanceId { get; set; }
        public string taskDefinitionKey { get; set; }
        public object caseExecutionId { get; set; }
        public object caseInstanceId { get; set; }
        public object caseDefinitionId { get; set; }
        public bool suspended { get; set; }
        public string formKey { get; set; }
        public object tenantId { get; set; }
    }
}
