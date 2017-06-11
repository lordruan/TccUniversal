using SQLite.Net.Attributes;

namespace TccUniversal
{
    [Table("Posts")]
    public class PostsResponse
    {
        public decimal id { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public bool active { get; set; }
        public decimal users_id { get; set; }
        public decimal geo_x { get; set; }
        public decimal geo_y { get; set; }
        public decimal category_id { get; set; }
    }
}
