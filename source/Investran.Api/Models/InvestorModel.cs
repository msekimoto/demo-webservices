using System;
using System.Collections.Generic;
using Sungard.Investran.Suite.WebServices.Contracts.Common;

namespace Investran.Api.Models
{
    public class InvestorModel
    {
        public InvestorModel()
        {
            Udfs = new List<UdfValue>();
        }

        public string Name { get; set; }
        public string NameAlias { get; set; }        
        public int DomainId { get; set; }
        public int EntityTypeId { get; set; }
        public int ReviewStatusId { get; set; }
        public int? ContactOrganizationId { get; set; }
        public int? ContactIndividualId { get; set; }
        public string TaxId { get; set; }
        public int InvestorClassId { get; internal set; }
        public List<UdfValue> Udfs { get; set; }
    }
}
