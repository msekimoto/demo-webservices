using Sungard.Investran.Suite.Api;
using Sungard.Investran.Suite.WebServices.Contracts;
using Sungard.Investran.Suite.WebServices.Contracts.Common.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Transactions;
using Sungard.Investran.Suite.WebServices.Linq;
using Sungard.Investran.Suite.WebServices.Contracts.Accounting.GeneralLedger.AllocationRules;
using Sungard.Investran.Suite.WebServices.Contracts.Accounting.GeneralLedger;

namespace Investran.Integration
{
    public class AllocationService
    {
        public IAllocationRuleWebService Service
        {
            get
            {
                return InvestranApplication.Current.Resolve<IAllocationRuleWebService>();
            }
        }

        public virtual List<T> All<T>(int limit = 100) where T : AllocationRuleDto
        {
            try
            {
                var entities = Service.AsQueryable<AllocationRuleDto, T>();
                return entities.ToList();
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

        public virtual List<T> Find<T>(Expression<Func<T, bool>> predicate, int limit = 100) where T : AllocationRuleDto
        {
            try
            {
                var entities = Service.AsQueryable<AllocationRuleDto, T>();
                return entities.Where(predicate).Take(limit).ToList();
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

        public virtual T Read<T>(int id) where T : AllocationRuleDto
        {
            try
            {
                var entity = (T)Service.Load(typeof(T).Name, id, FetchOptions.All);
                return entity;
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

        public virtual T Create<T>(T t) where T : AllocationRuleDto
        {
            throw new InvalidOperationException();
        }

        public virtual T Update<T>(T t) where T : AllocationRuleDto
        {
            throw new InvalidOperationException();
        }

        public virtual T Delete<T>(T t) where T : AllocationRuleDto
        {
            throw new InvalidOperationException();
        }
    }
}
