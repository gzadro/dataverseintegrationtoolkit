using Microsoft.Extensions.Caching.Memory;

namespace DataverseIntegrationToolkit.Caching
{
	public class AppServiceCache : ICache
	{
		private readonly IMemoryCache _memoryCache;

		public AppServiceCache(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		public T? Get<T>(string key)
		{
			return _memoryCache.TryGetValue(key, out T value)
				? value
				: default;
		}

		public void Set(object data, string key, TimeSpan? expiryTime = null)
		{
			if (data == null) return;
			expiryTime ??= new(0, 1, 0, 0);

			_memoryCache.Set(key, data, expiryTime.Value);
		}
	}
}
