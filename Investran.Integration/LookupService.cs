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
using System.Numerics;

namespace Investran.Integration
{
    public class LookupService
    {
        public ILookupWebService Service
        {
            get
            {
                return InvestranApplication.Current.Resolve<ILookupWebService>();
            }
        }

        public virtual List<T> All<T>(int limit = 100) where T : LookupDto
        {
            try
            {
                var entities = Service.AsQueryable<LookupDto, T>();
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

        public virtual List<T> Find<T>(Expression<Func<T, bool>> predicate, BigInteger? fetch = null, int limit = 100) where T : LookupDto
        {
            try
            {
                IQueryable<T> entities;

                if (fetch != null)
                    entities = Service.AsQueryable<LookupDto, T>(fetch.Value, 1);
                else
                    entities = Service.AsQueryable<LookupDto, T>();

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

        public virtual T Read<T>(int id) where T : LookupDto
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

        public virtual T Create<T>(T t) where T : LookupDto
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    var result = Service.Publish(new[] { t });
                    t.Id = result.Ids[0];
                    transaction.Complete();
                }

                return t;
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

        public virtual T Update<T>(T t) where T : LookupDto
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    var result = Service.Publish(new[] { t });
                    transaction.Complete();
                }

                return t;
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

        public virtual T Delete<T>(T t) where T : LookupDto
        {
            try
            {
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    var result = Service.Remove(new[] { t });
                    transaction.Complete();
                }

                return t;
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
