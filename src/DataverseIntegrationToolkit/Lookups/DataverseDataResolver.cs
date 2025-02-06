using DataverseIntegrationToolkit.Caching;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DataverseIntegrationToolkit.Lookups
{
    public class DataverseDataResolver : IDataResolver
    {
        private readonly IOrganizationService _organizationService;
        private readonly ICache _cache;
        private readonly ILogger<DataverseDataResolver> _logger;

        public DataverseDataResolver(IOrganizationService organizationService, ICache cache,
            ILogger<DataverseDataResolver> logger)
        {
            _organizationService = organizationService;
            _cache = cache;
            _logger = logger;
        }

        /// <inheritdoc />
        public EntityReference? Resolve(string entityLogicalName, string key1, string? value1, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true)
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, (key1, value1))?.ToEntityReference();
        }

        /// <inheritdoc />
        public EntityReference? Resolve(string entityLogicalName, string key1, string? value1, string key2, string? value2, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true)
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, (key1, value1), (key2, value2))?.ToEntityReference();
        }

        /// <inheritdoc />
        public EntityReference? Resolve(string entityLogicalName, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true, params (string key, string? value)[] keyValues)
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, keyValues)?.ToEntityReference();
        }

        /// <inheritdoc />
        public T? Resolve<T>(string entityLogicalName, string key1, string? value1, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true) where T : Entity
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, (key1, value1))?.ToEntity<T>();
        }

        /// <inheritdoc />
        public T? Resolve<T>(string entityLogicalName, string key1, string? value1, string key2, string? value2, ColumnSet columnSet, bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true) where T : Entity
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, (key1, value1), (key2, value2))?.ToEntity<T>();
        }

        /// <inheritdoc />
        public T? Resolve<T>(string entityLogicalName, ColumnSet columnSet,
            bool throwExceptionIfNotFound = false, bool useCache = false, bool noLock = true, params (string key, string? value)[] keyValues) where T : Entity
        {
            return RunResolveData(entityLogicalName, throwExceptionIfNotFound, useCache, columnSet, noLock, keyValues)?.ToEntity<T>();
        }

        /// <inheritdoc />
        public ResolvedDataModel Resolve(List<DataModel> dataModel)
        {
            foreach (var lr in dataModel)
            {
                if (string.IsNullOrEmpty(lr.Value))
                {
                    lr.ResolvedValue = null;
                    continue;
                }

                lr.ResolvedValue = Retrieve(lr.EntityLogicalName, new ColumnSet(true), true, false, (lr.Key, lr.Value));
            }

            return new()
            {
                Data = dataModel
            };
        }

        private Entity? RunResolveData(string entityLogicalName, bool throwExceptionIfNotFound, bool useCache, ColumnSet columnSet, bool noLock, params (string key, string? value)[] keyValues)
        {
            var data = Retrieve(entityLogicalName, columnSet, noLock, useCache, keyValues);

            ValidateRetrievedData(entityLogicalName, throwExceptionIfNotFound, data, keyValues);

            return data;
        }


        private void ValidateRetrievedData(string entityLogicalName, bool throwExceptionIfNotFound, Entity? data, params (string key, string? value)[] keyValues)
        {
            if (data == null)
            {
                string message = $"Could not find record for entity {entityLogicalName}, " +
                                 string.Join(", ", keyValues.Select(kv => $"{kv.key} {kv.value}"));

                if (throwExceptionIfNotFound)
                {
                    throw new Exception(message);
                }
                else
                {
                    _logger.LogWarning(message);
                }
            }
        }

        private Entity? Retrieve(string entityLogicalName, ColumnSet columnSet, bool noLock, bool useCache = false, params (string key, string? value)[] keyValues)
        {
            var cacheKey = string.Empty;

            if (useCache)
            {
                cacheKey = $"{entityLogicalName}-{keyValues[0].key}-{ConvertValueToText(keyValues[0].value)}";

                if (!IsValid(entityLogicalName, keyValues[0].key))
                {
                    return null;
                }

                foreach (var (key, value) in keyValues.Skip(1))
                {
                    cacheKey = AdjustCacheKey(entityLogicalName, cacheKey, (key, value));
                }

                var cacheData = _cache.Get<Entity>(cacheKey);

                if (cacheData != null)
                {
                    cacheData = ManageCachedData(entityLogicalName, cacheData, keyValues);

                    return cacheData;
                }
            }

            var query = new QueryExpression(entityLogicalName) { ColumnSet = columnSet, NoLock = noLock };

            foreach (var (key, value) in keyValues)
            {
                if (IsValid(entityLogicalName, key))
                {
                    AddParameterToQuery(key, value, query);
                }
            }

            query.Criteria.FilterOperator = LogicalOperator.And;
            EntityCollection? result = null;

            if (query.Criteria.Conditions.Count() > 0)
            {
                result = _organizationService.RetrieveMultiple(query);
            }

            if (result == null || !result.Entities.Any())
            {
                HandleMissingDataMessage(entityLogicalName, keyValues);
                return null;
            }

            var lookupResult = result.Entities.FirstOrDefault();
            if (lookupResult == null)
            {
                HandleMissingDataMessage(entityLogicalName, keyValues);
                return null;
            }

            if (useCache)
            {
                _cache.Set(lookupResult, cacheKey);
            }

            HandleExistingDataMessage(entityLogicalName, lookupResult, keyValues);

            return lookupResult;
        }

        private static void AddParameterToQuery(string key, string? value, QueryExpression query)
        {
            if (value != null)
            {
                query.Criteria.AddCondition(new(key, ConditionOperator.Equal, value));
            }
            else
            {
                query.Criteria.AddCondition(new(key, ConditionOperator.Null));
            }
        }

        private Entity? ManageCachedData(string entityLogicalName, Entity cacheData, params (string key, string? value)[] keyValues)
        {
            var builder = new StringBuilder();
            builder.Append(
                $"Lookup for {entityLogicalName}, ");

            foreach (var (key, value) in keyValues)
            {
                if (IsValid(entityLogicalName, key))
                {
                    builder.Append(
                        $"field {key}, value {ConvertValueToText(value)}");
                }
            }

            builder.Append(" exists in cache. Returning the data from cache.");

            _logger.LogInformation(builder.ToString());

            return cacheData;
        }

        private static string AdjustCacheKey(string entityLogicalName, string cacheKey, params (string key, string? value)[] keyValues)
        {
            foreach (var (key, value) in keyValues)
            {
                if (IsValid(entityLogicalName, key))
                {
                    cacheKey += $"-{key}-{ConvertValueToText(value)}";
                }
            }

            return cacheKey;
        }

        private void HandleExistingDataMessage(string entityLogicalName, Entity lookupResult, params (string key, string? value)[] keyValues)
        {
            var message = $"Query for {entityLogicalName}, field {keyValues[0].key}, value {keyValues[0].value}";

            foreach (var (key, value) in keyValues.Skip(1))
            {
                if (IsValid(entityLogicalName, key))
                {
                    message += $" field {key}, value {ConvertValueToText(value)}";
                }
            }

            message += $" resolved as {lookupResult.Id} in Dynamics.";

            _logger.LogInformation(message);
        }

        private void HandleMissingDataMessage(string entityLogicalName, params (string key, string? value)[] keyValues)
        {
            var message = $"Query for {entityLogicalName}, field {keyValues[0].key}, value {keyValues[0].value}";

            foreach (var (key, value) in keyValues.Skip(1))
            {
                if (IsValid(entityLogicalName, key))
                {
                    message += $" field {key}, value {ConvertValueToText(value)}";
                }
            }

            message += " doesn't exist in Dynamics.";

            _logger.LogWarning(message);
        }

        private static bool IsValid(string entityLogicalName, string key)
        {
            return !string.IsNullOrEmpty(entityLogicalName) &&
                   !string.IsNullOrEmpty(key);
        }

        private static string ConvertValueToText(string? value)
        {
            if (value == null)
            {
                return "NULL";
            }

            return value;
        }
    }
}
