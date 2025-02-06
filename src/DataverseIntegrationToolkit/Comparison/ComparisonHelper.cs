namespace DataverseIntegrationToolkit.Comparison
{
    public static class ComparisonHelper
    {
        public static ComparisonResult<TEntity, TNew>
            CompareLists<TEntity, TNew>(List<TEntity> source, List<TNew> newList, Func<TEntity, TNew, bool> comparer)
        {
            var toUpdate = new List<(TEntity, TNew)>();
            foreach (var s in source)
            {
                var matchingNewItems = newList.Where(n => comparer(s, n)).ToList();
                foreach (var newItem in matchingNewItems)
                {
                    toUpdate.Add((s, newItem));
                }
            }

            var result = new ComparisonResult<TEntity, TNew>
            {
                ToCreate = newList.Where(n => !source.Any(s => comparer(s, n))).ToList(),
                ToUpdate = toUpdate,
                ToDelete = source.Where(s => !newList.Any(n => comparer(s, n))).ToList(),
            };
            return result;
        }
    }
}
