using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Lookups
{
    public class ResolvedDataModel
    {
        public bool AllDataResolved
        {
            get
            {
                if (Data == null || !Data.Any())
                {
                    return true;
                }

                var nonResolved = Data.FirstOrDefault(a => a.ResolvedValue == null && !string.IsNullOrEmpty(a.Value));

                if (nonResolved == null)
                {
                    return true;
                }

                return false;
            }
        }

        public List<DataModel>? Data { get; set; }
    }
}
