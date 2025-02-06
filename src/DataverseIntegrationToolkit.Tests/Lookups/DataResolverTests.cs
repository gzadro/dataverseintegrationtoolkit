using DataverseIntegrationToolkit.Caching;
using DataverseIntegrationToolkit.Lookups;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using NSubstitute;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute.ReturnsExtensions;

namespace DataverseIntegrationToolkit.Tests.Lookups
{
    [TestClass]
    public class DataResolverTests
    {
        private DataverseDataResolver _DataResolver;
        private IOrganizationService _organizationService;
        private ICache _cache;
        private ILogger<DataverseDataResolver> _logger;

        public DataResolverTests()
        {
            _organizationService = Substitute.For<IOrganizationService>();
            _cache = Substitute.For<ICache>();
            _logger = Substitute.For<ILogger<DataverseDataResolver>>();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Be_Null_When_We_Have_One_Pair_KeyValue()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, new ColumnSet(true));

            data.ShouldNotBeNull();

        }

        [TestMethod]
        public void DataResolver_Should_Not_Be_Null_When_We_Have_Two_Pair_KeyValue()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, key2, value2, new ColumnSet(true));

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Be_Null_When_We_Have_Three_Pair_KeyValue()
        {

            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";
            var key3 = "key3";
            var value3 = "value3";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}-{key3}-{value3}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, new ColumnSet(true), false, false, true, (key1, value1), (key2, value2), (key3, value3));

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Null_When_We_Have_One_Pair_KeyValue_And_Is_In_Cache()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";

            var entity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).Returns(entity);

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, new ColumnSet(true), true, true);

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Null_When_We_Have_Two_Pair_KeyValue_And_Is_In_Cache()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";

            var entity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}";
            _cache.Get<Entity>(cacheKey).Returns(entity);

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, key2, value2, new ColumnSet(true), true, true);

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Null_When_We_Have_Three_Pair_KeyValue_And_Is_In_Cache()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";
            var key3 = "key3";
            var value3 = "value3";

            var entity = new Entity()
            {
                Id = Guid.NewGuid()
            };

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}-{key3}-{value3}";
            _cache.Get<Entity>(cacheKey).Returns(entity);

            var data = _DataResolver.Resolve(entityLogicalName, new ColumnSet(true), true, true, true, (key1, value1), (key2, value2), (key3, value3));

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Be_Null_When_We_Have_One_Pair_KeyValue_EmptyOrNull()
        {
            var entityLogicalName = "opportunity";
            var key1 = "";
            var value1 = "";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, new ColumnSet(true), false, false);

            data.ShouldBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Not_Be_Null_When_We_Have_One_Pair_KeyValue_And_One_Pair_KeyValue_EmptyOrNull()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "";
            var value2 = "";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, key2, value2, new ColumnSet(true));

            data.ShouldNotBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Be_Null_When_Value_IsNull()
        {
            var entityLogicalName = "opportunity";
            var key1 = "";
            string? value1 = null;

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            var entities = new List<Entity>()
            {
                new Entity()
                {
                    Id = Guid.NewGuid(),
                }
            };
            var entityList = new List<Entity>(entities);

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection(entityList));

            var data = _DataResolver.Resolve(entityLogicalName, key1, value1, new ColumnSet(true), false, false);

            data.ShouldBeNull();
        }

        [TestMethod]
        public void DataResolver_Should_Throw_Exception_If_No_Resuts_For_One_KeyValue()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection());

            Should.Throw<Exception>(() => _DataResolver.Resolve(entityLogicalName, key1, value1, new ColumnSet(true), true));
        }

        [TestMethod]
        public void DataResolver_Should_Throw_Exception_If_No_Resuts_For_Two_KeyValue()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection());

            Should.Throw<Exception>(() => _DataResolver.Resolve(entityLogicalName, key1, value1, key2, value2, new ColumnSet(true), true));
        }

        [TestMethod]
        public void DataResolver_Should_Throw_Exception_If_No_Resuts_For_Three_KeyValue()
        {
            var entityLogicalName = "opportunity";
            var key1 = "key1";
            var value1 = "value1";
            var key2 = "key2";
            var value2 = "value2";
            var key3 = "key3";
            var value3 = "value3";

            _DataResolver = new DataverseDataResolver(_organizationService, _cache, _logger);

            var cacheKey = $"{entityLogicalName}-{key1}-{value1}-{key2}-{value2}-{key3}-{value3}";
            _cache.Get<Entity>(cacheKey).ReturnsNull();

            _organizationService.RetrieveMultiple(Arg.Any<QueryExpression>()).Returns(new EntityCollection());

            Should.Throw<Exception>(() => _DataResolver.Resolve(entityLogicalName, new ColumnSet(true), true, false, true, (key1, value1), (key2, value2), (key3, value3)));
        }
    }
}
