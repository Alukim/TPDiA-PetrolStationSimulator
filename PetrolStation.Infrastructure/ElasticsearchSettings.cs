namespace PetrolStation.Infrastructure
{
    public class ElasticsearchSettings
    {
        public string NodeUrl { get; set; }

        public string IndexName { get; set; }

        public int ShardsNumber { get; set; } = 1;
    }
}
