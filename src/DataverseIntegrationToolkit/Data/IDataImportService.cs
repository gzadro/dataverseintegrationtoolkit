using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;

namespace DataverseIntegrationToolkit.Data
{
    /// <summary>
    /// Service for importing data and committing it to the CRM system using IOrganizationService.
    /// Supports operations: Create, Update, Delete, Associate, and Disassociate.
    /// </summary>
    public interface IDataImportService
    {
        /// <summary>
        /// Adds a record to the import service for creation operation.
        /// </summary>
        /// <remarks>
        /// Data added via this method is not directly transfered to Dynamics, but it expects Commit method to be called in order to execute request
        /// </remarks>
        /// <param name="record">The entity record to be created.</param>
        /// <exception cref="ArgumentNullException">If provided record is null, exception will be thrown.</exception>
        void Add(Entity record);

        /// <summary>
        /// Adds a record to the import service for updating operation.
        /// </summary>
        /// <remarks>
        /// Data added via this method is not directly transfered to Dynamics, but it expects Commit method to be called in order to execute request
        /// </remarks>
        /// <param name="record">The entity record to be created.</param>
        /// <exception cref="ArgumentNullException">If provided record is null, exception will be thrown.</exception>
        void Update(Entity record);

        /// <summary>
        /// Adds a record to the import service for deletion operation.
        /// </summary>
        /// <remarks>
        /// Data added via this method is not directly transfered to Dynamics, but it expects Commit method to be called in order to execute request
        /// </remarks>
        /// <param name="record">The entity record to be created.</param>
        /// <exception cref="ArgumentNullException">If provided record is null, exception will be thrown.</exception>
        void Delete(Entity record);

        /// <summary>
        /// Gets the current count of items in the import service.
        /// </summary>
        /// <returns>The number of items currently tracked for processing.</returns>
        int GetCurrentItemsCount();

        /// <summary>
        /// Associates two records using a specified relationship.
        /// </summary>
        /// <param name="fromRecord">The primary entity in the relationship.</param>
        /// <param name="toRecord">The related entity in the relationship.</param>
        /// <param name="relationship">The relationship between the two entities.</param>
        /// <exception cref="ArgumentNullException">If provided record is null, exception will be thrown.</exception>
        void Associate(Entity fromRecord, Entity toRecord, Relationship relationship);

        /// <summary>
        /// Disassociates two records using a specified relationship.
        /// </summary>
        /// <param name="fromRecord">The primary entity in the relationship.</param>
        /// <param name="toRecord">The related entity in the relationship.</param>
        /// <param name="relationship">The relationship between the two entities.</param>
        /// <exception cref="ArgumentNullException">If provided record is null, exception will be thrown.</exception>
        void Disassociate(Entity fromRecord, Entity toRecord, Relationship relationship);

        /// <summary>
        /// Commits all pending operations to the organization service in a single transaction.
        /// </summary>
        /// <remarks>
        /// Maximum amount of records to be processed in one run is 1000 requests. If there are more than 1000 requests in one run, then multiple Transactional requests are going to be executed.
        /// </remarks>
        /// <returns>Returns RequestProcessingResult.Succeeded if successful, otherwise RequestProcessingResult.Failed.</returns>
        RequestProcessingResult Commit();

        /// <summary>
        /// Clears all pending operations.
        /// </summary>
        void ClearAll();
    }
}