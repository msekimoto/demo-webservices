using Sungard.Investran.Suite.Api;
using Sungard.Investran.Suite.WebServices.Contracts;
using Sungard.Investran.Suite.WebServices.Contracts.Accounting.GeneralLedger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace Investran.Integration
{
    public class BatchService
    {
        public BatchDto Update(BatchDto batchDto)
        {
            try
            {
                var generalLedgerWebService = InvestranApplication.Current.Resolve<IGeneralLedgerWebService>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    var result = generalLedgerWebService.Publish(new[] { batchDto });
                    transaction.Complete();
                }

                return batchDto;
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

        public BatchDto Create(BatchDto batchDto)
        {
            try
            {
                var generalLedgerWebService = InvestranApplication.Current.Resolve<IGeneralLedgerWebService>();

                using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(60)))
                {
                    var result = generalLedgerWebService.Publish(new[] { batchDto });
                    batchDto.Id = result.Ids[0];
                    transaction.Complete();
                }

                return batchDto;
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
            catch (FaultException<FaultReasonText> faultException)
            {
                throw new Exception(faultException.Detail.Text);
            }
        }

        public BatchDto Read(int batchId)
        {
            try
            {
                var generalLedgerWebService = InvestranApplication.Current.Resolve<IGeneralLedgerWebService>();
                var batch = (BatchDto)generalLedgerWebService.Load(typeof(BatchDto).Name,
                    batchId,
                    FetchOptions.AsString
                    (
                        FetchOptions.AsBigInteger(FetchOptions.BatchJournalEntries) |
                        FetchOptions.AsBigInteger(FetchOptions.BatchLegalEntity) |
                        FetchOptions.AsBigInteger(FetchOptions.TransactionDeal) |
                        FetchOptions.AsBigInteger(FetchOptions.TransactionPosition) |
                        FetchOptions.AsBigInteger(FetchOptions.TransactionInvestorAllocations) |
                        FetchOptions.AsBigInteger(FetchOptions.InvestorAllocationInvestor) |
                        FetchOptions.AsBigInteger(FetchOptions.JournalEntryTransactions)
                    ));

                return batch;
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
