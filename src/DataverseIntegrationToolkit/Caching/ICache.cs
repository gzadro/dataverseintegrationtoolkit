namespace DataverseIntegrationToolkit.Caching;

public interface ICache
{
	T? Get<T>(string key);
	void Set(object data, string key, TimeSpan? expiryTime = null);
}