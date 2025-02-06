namespace DataverseIntegrationToolkit.Comparison
{
    public class ComparisonResult<TEntity, TNew>
    {
        public List<TNew>? ToCreate { get; set; }
        public List<(TEntity Target, TNew Source)>? ToUpdate { get; set; }
        public List<TEntity>? ToDelete { get; set; }
    }
}
