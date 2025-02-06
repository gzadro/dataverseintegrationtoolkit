using DataverseIntegrationToolkit.Lookups;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Tests.Lookups
{
    [TestClass]
    public class ResolvedLookupsTests
    {
        [TestMethod]
        public void AllLookupsResolved_Should_Return_True_When_Lookups_Is_Null()
        {
            var resolvedLookups = new ResolvedDataModel { Data = null };

            var result = resolvedLookups.AllDataResolved;

            result.ShouldBe(true);
        }

        [TestMethod]
        public void AllLookupsResolved_Should_Return_True_When_Lookups_Is_Empty()
        {
            var resolvedLookups = new ResolvedDataModel { Data = new List<DataModel>() };

            var result = resolvedLookups.AllDataResolved;

            result.ShouldBe(true);
        }

        [TestMethod]
        public void AllLookupsResolved_Should_Return_True_When_All_Lookups_Are_Resolved()
        {
            var lookup1 = new DataModel("123", "123", "123") { ResolvedValue = "value1" };
            var lookup2 = new DataModel("123", "123", "123") { ResolvedValue = "value2" };
            var resolvedLookups = new ResolvedDataModel { Data = new List<DataModel> { lookup1, lookup2 } };

            var result = resolvedLookups.AllDataResolved;

            result.ShouldBe(true);
        }

        [TestMethod]
        public void AllLookupsResolved_Should_Return_False_When_At_Least_One_Lookup_Is_Not_Resolved()
        {
            var lookup1 = new DataModel("123", "123", "123") { ResolvedValue = "value1" };
            var lookup2 = new DataModel("123", "123", "123") { ResolvedValue = null };
            var resolvedLookups = new ResolvedDataModel { Data = new List<DataModel> { lookup1, lookup2 } };

            var result = resolvedLookups.AllDataResolved;

            result.ShouldBe(false);
        }
    }
}
