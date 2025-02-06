namespace DataverseIntegrationToolkit.Lookups
{
	public class DataModel
	{
		public object? ResolvedValue { get; set; }

		public string EntityLogicalName { get; set; }

		public string Key { get; set; }

		public string? Value { get; set; }

		public DataModel(string entityLogicalName, string key, string value)
		{
			EntityLogicalName = entityLogicalName;
			Key = key;
			Value = value;
		}
	}
}
