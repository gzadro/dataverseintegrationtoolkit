using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Data
{
    public static class BatchCalculator
    {
        public static int CalculateBatches(int totalNumberOfItems, int maximumBatchSize)
        {
            int batchSize = totalNumberOfItems > maximumBatchSize ?
                (int)Math.Ceiling((double)totalNumberOfItems / maximumBatchSize) : 1;

            return batchSize;
        }
    }
}
