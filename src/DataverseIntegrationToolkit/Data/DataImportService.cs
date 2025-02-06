using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;

namespace DataverseIntegrationToolkit.Data
{
    public class DataImportService : IDataImportService
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<DataImportService> _logger;
        private readonly List<ImportServiceModel> _data = new();
        private const int MAX_BATCH_SIZE = 1000;

        public DataImportService(IOrganizationService organizationService, ILogger<DataImportService> logger)
        {
            _organizationService = organizationService;
            _logger = logger;
        }
        public void Add(Entity record)
        {
            ValidateRecord(record);

            _logger.LogInformation($"Adding {record.LogicalName} with Id {record.Id}");

            _data.Add(new ImportServiceModel(record, Operation.Create));
        }

        public void Update(Entity record)
        {
            ValidateRecord(record);

            _logger.LogInformation($"Updating {record.LogicalName} with Id {record.Id}");

            _data.Add(new ImportServiceModel(record, Operation.Update));
        }

        public void Delete(Entity record)
        {
            ValidateRecord(record);

            _logger.LogInformation($"Deleting {record.LogicalName} with Id {record.Id}");

            _data.Add(new ImportServiceModel(record, Operation.Delete));
        }
        public void Associate(Entity fromRecord, Entity toRecord, Relationship relationship)
        {
            ValidateRecord(fromRecord, toRecord, relationship);

            _logger.LogInformation($"Associating {fromRecord.LogicalName} with Id {fromRecord.Id} to {toRecord.LogicalName} with Id {toRecord.Id} with relationship {relationship}");

            _data.Add(new ImportServiceModel(fromRecord, toRecord, Operation.Associate, relationship));
        }
        public void Disassociate(Entity fromRecord, Entity toRecord, Relationship relationship)
        {
            ValidateRecord(fromRecord, toRecord, relationship);

            _logger.LogInformation($"Associating {fromRecord.LogicalName} with Id {fromRecord.Id} to {toRecord.LogicalName} with Id {toRecord.Id} with relationship {relationship}");

            _data.Add(new ImportServiceModel(fromRecord, toRecord, Operation.Disassociate, relationship));
        }

        public void ClearAll()
        {
            _data.Clear();
        }

        public int GetCurrentItemsCount()
        {
            return _data.Count;
        }

        public RequestProcessingResult Commit()
        {
            _logger.LogInformation($"Commiting data to the Organization Service...");

            var requestWithResults =
                new ExecuteTransactionRequest()
                {
                    Requests = new OrganizationRequestCollection()
                };

            foreach (var d in _data)
            {
                switch (d.Operation)
                {
                    case Operation.Create:
                        AddRequest<CreateRequest>(requestWithResults, d.Entity);
                        break;

                    case Operation.Update:
                        AddRequest<UpdateRequest>(requestWithResults, d.Entity);
                        break;

                    case Operation.Delete:
                        AddRequest<DeleteRequest>(requestWithResults, d.Entity.ToEntityReference());
                        break;

                    case Operation.Associate:
                        AddRequest<AssociateRequest>(
                            requestWithResults,
                            d.Entity.ToEntityReference(),
                            r =>
                            {
                                r.Relationship = d.Relationship;
                                r.RelatedEntities = new EntityReferenceCollection() { d.ToEntity?.ToEntityReference() };
                            }
                        );
                        break;

                    case Operation.Disassociate:
                        AddRequest<DisassociateRequest>(
                            requestWithResults,
                            d.Entity.ToEntityReference(),
                            r =>
                            {
                                r.Relationship = d.Relationship;
                                r.RelatedEntities = new EntityReferenceCollection() { d.ToEntity?.ToEntityReference() };
                            }
                        );
                        break;

                    default:
                        break;
                }
            }

            try
            {
                CommitDataToDataverse(requestWithResults);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to commit the changes: {ex}", ex.Message);

                return RequestProcessingResult.Failed;
            }

            _logger.LogInformation($"All requests are now executed against the Dataverse.");

            return RequestProcessingResult.Succeeded;
        }

        private void ValidateRecord(Entity record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Adding null record is not allowed.");
            }
        }

        private void ValidateRecord(Entity fromRecord, Entity toRecord, Relationship relationship)
        {
            if (fromRecord == null || toRecord == null || relationship == null)
            {
                throw new ArgumentNullException($"Adding null record is not allowed.");
            }
        }

        private void AddRequest<T>(
            ExecuteTransactionRequest requestWithResults,
            object target,
            Action<T> setAdditionalValues = null
        ) where T : OrganizationRequest, new()
        {
            _logger.LogInformation(
                "Adding operation {operation} for target {type}.",
                typeof(T).FullName,
                target.GetType().FullName
            );

            var request = new T();
            request.Parameters["Target"] = target;

            setAdditionalValues?.Invoke(request);
            requestWithResults.Requests.Add(request);
        }

        private void CommitDataToDataverse(ExecuteTransactionRequest requestWithResults)
        {
            int totalNumberOfRequests = requestWithResults.Requests.Count;
            int batchSize = BatchCalculator.CalculateBatches(totalNumberOfRequests, MAX_BATCH_SIZE);

            if (batchSize == 1)
            {
                _organizationService.Execute(requestWithResults);

                return;
            }

            CommitDataToDataverseInMultipleTransactions(requestWithResults, batchSize);

        }

        private void CommitDataToDataverseInMultipleTransactions(ExecuteTransactionRequest requestWithResults, int batchSize)
        {
            _logger.LogWarning($"As total number of requests exceeds maximum number of requests ({MAX_BATCH_SIZE} requests per transactional call), it is necessary to execute total of {batchSize} transactional calls to fit them." +
                $"Rollback of separate transaction call is not supported and manual cleanup of data is required in the case of errors.");

            for (int i = 0; i < batchSize; i++)
            {
                _logger.LogInformation($"Processing batch {i + 1} of {batchSize}");

                var requests = requestWithResults.Requests.Skip(i * MAX_BATCH_SIZE).Take(MAX_BATCH_SIZE).ToList();

                ExecuteTransactionRequest executeTransactionRequest = new ExecuteTransactionRequest()
                {
                    ReturnResponses = true,
                    Requests = new OrganizationRequestCollection()
                };

                foreach (var request in requests)
                {
                    executeTransactionRequest.Requests.Add(request);
                }

                _organizationService.Execute(requestWithResults);
            }
        }
    }
}
