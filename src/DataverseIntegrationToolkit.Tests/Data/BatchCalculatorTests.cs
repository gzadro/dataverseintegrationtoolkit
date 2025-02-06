using DataverseIntegrationToolkit.Data;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Tests.Data
{
    [TestClass]
    public class BatchCalculatorTests
    {

        [TestMethod]
        public void When_TotalNumberOfItems_IsLessThanMaximumBatchSize_ItShouldReturnOneBatch()
        {
            var totalNumberOfItems = 900;
            var maximumBatchSize = 1000;

            var totalBatches = BatchCalculator.CalculateBatches(totalNumberOfItems, maximumBatchSize);

            totalBatches.ShouldBe(1);
        }

        [TestMethod]
        public void When_TotalNumberOfItems_IsLargerThanMaximumBatchSize_ItShouldReturnMoreThanOneBatch()
        {
            var totalNumberOfItems = 1200;
            var maximumBatchSize = 1000;

            var totalBatches = BatchCalculator.CalculateBatches(totalNumberOfItems, maximumBatchSize);

            totalBatches.ShouldBe(2);
        }
    }
}
