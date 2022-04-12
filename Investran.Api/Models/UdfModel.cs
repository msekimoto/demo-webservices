using Sungard.Investran.Suite.WebServices.Contracts.Common.Lookups;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Udf;
using System.Collections.Generic;

namespace Investran.Api.Models
{

    public class UdfModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EntityType { get; set; }
        public bool Required { get; set; }
        public List<string> Values { get; set; }
    }
}
