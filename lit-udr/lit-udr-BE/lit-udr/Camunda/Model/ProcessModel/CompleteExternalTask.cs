using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model.ProcessModel
{
    public class DataCorrect
    {
        public bool value { get; set; }
    }

    public class Writer
    {
        public bool value { get; set; }
    }

    public class NumberOfTrials
    {
        public int value { get; set; }
    }

    public class ClanoviOdbora
    {
        public dynamic value { get; set; }
    }

    public class ClanOdbora
    {
        public string value { get; set; }
    }

    public class Variables
    {
        public DataCorrect DataCorrect { get; set; }
    }

    public class WriterFailed
    {
        public bool value { get; set; }
    }
    public class WriterFailedAbstract
    {
        public dynamic WriterFailed { get; set; }
    }

    public class CompleteUserTaskWithVariable
    {
        public WriterFailedAbstract variables { get; set; }
    }

    public class VariablesSecondTask
    {
        public NumberOfTrials NumberOfTrials { get; set; }

        public ClanoviOdbora ClanoviOdbora { get; set; }

        public ClanOdbora ClanOdbora { get; set; }
    }

    public class VariablesFirstTask
    {
        public Writer Writer { get; set; }
    }

    public class CompleteExternalTask
    {
        public string workerId { get; set; }
        public dynamic variables { get; set; }
    }

}
