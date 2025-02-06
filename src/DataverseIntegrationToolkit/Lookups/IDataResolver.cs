using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace DataverseIntegrationToolkit.Lookups
{
    public interface IDataResolver
    {
        /// <summary>
        /// Resolves an EntityReference by retrieving an entity using a key-value pair.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="key">The key to identify the record.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An EntityReference if found; otherwise null.</returns>
        EntityReference? Resolve(string entityLogicalName, string key1, string? value1, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true);

        /// <summary>
        /// Resolves an EntityReference by retrieving an entity using two key-value pairs.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="key1">The first key to identify the record.</param>
        /// <param name="value1">The value associated with the first key.</param>
        /// <param name="key2">The second key to identify the record.</param>
        /// <param name="value2">The value associated with the second key.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An EntityReference if found; otherwise null.</returns>
        EntityReference? Resolve(string entityLogicalName, string key1, string? value1, string key2, string? value2, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true);

        /// <summary>
        /// Resolves an EntityReference by retrieving an entity using multiple key-value pairs.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="keyValues">An array of key-value pairs to identify the record.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An EntityReference if found; otherwise null.</returns>
        EntityReference? Resolve(string entityLogicalName, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true, params (string key, string? value)[] keyValues);

        /// <summary>
        /// Resolves an entity of a specific type by retrieving an entity using a key-value pair.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <typeparam name="T">The entity type to resolve.</typeparam>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="key">The key to identify the record.</param>
        /// <param name="value">The value associated with the key.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An entity of type T if found; otherwise null.</returns>
        T? Resolve<T>(string entityLogicalName, string key1, string? value1, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true) where T : Entity;

        /// <summary>
        /// Resolves an entity of a specific type by retrieving an entity using two key-value pairs.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <typeparam name="T">The entity type to resolve.</typeparam>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="key1">The first key to identify the record.</param>
        /// <param name="value1">The value associated with the first key.</param>
        /// <param name="key2">The second key to identify the record.</param>
        /// <param name="value2">The value associated with the second key.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An entity of type T if found; otherwise null.</returns>
        T? Resolve<T>(string entityLogicalName, string key1, string? value1, string key2, string? value2, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true) where T : Entity;

        /// <summary>
        /// Resolves an entity of a specific type by retrieving an entity using multiple key-value pairs.
        /// Can optionally throw an exception if the entity is not found and use cache for performance.
        /// </summary>
        /// <typeparam name="T">The entity type to resolve.</typeparam>
        /// <param name="entityLogicalName">The logical name of the entity to retrieve.</param>
        /// <param name="throwExceptionIfNotFound">Whether to throw an exception if the entity is not found.</param>
        /// <param name="useCache">Whether to use cached results if available.</param>
        /// <param name="keyValues">An array of key-value pairs to identify the record.</param>
        /// <param name="columnSet">Provide columns set for retrieval.</param>
        /// <param name="noLock">Will the Lock be used to retrieve the data from the database.</param>
        /// <returns>An entity of type T if found; otherwise null.</returns>
        T? Resolve<T>(string entityLogicalName, ColumnSet columnSet,
            bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true, params (string key, string? value)[] keyValues) where T : Entity;


        /// <summary>
        /// Resolves multiple data requests from a list of DataModel instances.
        /// Each data request contains the entity logical name, key, and value to resolve.
        /// </summary>
        /// <param name="dataRequest">A list of DataModel instances representing the data requests.</param>
        /// <returns>A ResolvedDataModel object containing the resolved entities.</returns>
        ResolvedDataModel Resolve(List<DataModel> dataRequest);
    }
}

