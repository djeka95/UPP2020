using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class CamundaConstraint
    {
        public bool Required { get; set; }
        public string MaxLength { get; set; }
        public string MinLength { get; set; }
        public string Min { get; set; }
        public object value { get; set; }

    }
}
