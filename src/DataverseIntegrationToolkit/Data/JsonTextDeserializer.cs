using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace DataverseIntegrationToolkit.Data
{
    public class JsonTextDeserializer : IMessageDeserializer
    {
        private readonly ILogger<JsonTextDeserializer> _logger;

        public JsonTextDeserializer(ILogger<JsonTextDeserializer> logger)
        {
            _logger = logger;
        }

        private static readonly JsonSerializerOptions DefaultOptions = new()
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public T? Deserialize<T>(string json, JsonSerializerOptions? options = null) where T : class
        {
            if (string.IsNullOrEmpty(json)) throw new InvalidOperationException("Received JSON is empty");
            options ??= DefaultOptions;

            try
            {
                return JsonSerializer.Deserialize<T>(json, options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public string Serialize(object obj, JsonSerializerOptions? options = null)
        {
            if (obj == null) throw new InvalidOperationException("Cannot serialize NULL value.");
            options ??= DefaultOptions;

            try
            {
                return JsonSerializer.Serialize(obj, options);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
