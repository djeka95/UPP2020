using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lit_udr.Camunda.Model
{
    public class FormVariables
    {
        public FirstName firstName { get; set; }
        public LastName lastName { get; set; }
        public Writer writer { get; set; }
        public BetaReader betaReader { get; set; }
        public Email email { get; set; }
        public Password password { get; set; }
        public Genre genre { get; set; }
        public Country country { get; set; }
        public City city { get; set; }
        public User Korisnik { get; set; }
        public CamundaFile camundaFile { get; set; }
        public string TaskId { get; set; }
        public string ProcessDefinitionId { get; set; }
        public string ProcessInstanceId { get; set; }

    }
    public class ValueInfo
    {
    }

    public class AllValueInfo
    {
        public string valueName { get; set; }
        public object value { get; set; }
    }

    public class FirstName
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo valueInfo { get; set; }
    }

    public class ValueInfo2
    {
    }

    public class LastName
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo2 valueInfo { get; set; }
    }

    public class Writer
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo valueInfo { get; set; }
    }

    public class BetaReader
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo valueInfo { get; set; }
    }

    public class ValueInfo3
    {
    }

    public class Password
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo3 valueInfo { get; set; }
    }

    public class CamundaFile
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo3 valueInfo { get; set; }
    }

    public class ValueInfo4
    {
    }

    public class Genre
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo4 valueInfo { get; set; }
    }

    public class ValueInfo5
    {
    }

    public class Country
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo5 valueInfo { get; set; }
    }

    public class ValueInfo6
    {
    }

    public class Korisnik
    {
        public string type { get; set; }
        public string value { get; set; }
        public ValueInfo6 valueInfo { get; set; }
    }

    public class ValueInfo7
    {
    }

    public class Email
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo7 valueInfo { get; set; }
    }

    public class ValueInfo8
    {
    }

    public class City
    {
        public string type { get; set; }
        public object value { get; set; }
        public ValueInfo8 valueInfo { get; set; }
    }

}
