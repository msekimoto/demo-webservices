using Sungard.Investran.Suite.Api;
using Sungard.Investran.Suite.WebServices.Contracts;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Lookups;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Udf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

namespace Investran.Integration
{
    public class SdsService
    {
        public ISystemDataSetWebService Service
        {
            get
            {
                return InvestranApplication.Current.Resolve<ISystemDataSetWebService>();
            }
        }

        public List<LookupDto> Find(int systemDataSetId)
        {
            try
            {
                return Service.Values(systemDataSetId).ToList();
            }
            catch (FaultException<ResultFaultDto> faultException)
            {
                var errors = new List<string>();
                foreach (var errorMessage in faultException.Detail.Errors.SelectMany(error => error.Messages))
                {
                    errors.Add($"{errorMessage.PropertyName} {errorMessage.Message}");

                }
                throw new Exception(string.Join(",", errors));
            }
        }
    }
}
