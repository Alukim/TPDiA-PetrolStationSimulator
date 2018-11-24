namespace PetrolStation.Infrastructure
{
    public class IndexNameProvider<TType>
    {
        public IndexNameProvider(string indexName)
        {
            IndexName = indexName;
        }

        public string IndexName { get; }
    }
}
