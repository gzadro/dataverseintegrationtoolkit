using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Data
{
    public interface IMessageDeserializer
    {
        T? Deserialize<T>(string json, JsonSerializerOptions? options = null) where T : class;
        string Serialize(object obj, JsonSerializerOptions? options = null);
    }
}
