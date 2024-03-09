using Postgrest.Attributes;
using Postgrest.Models;

namespace Metron.Models
{
    [Table("ingestion")]
    public class Ingestion : BaseModel
    {
        public int id { get; set; }

        public DateTime created_at { get; set; }

        public string? user_id { get; set;}

        public string? data_schema_name { get; set; }

        public string? data_schema_version { get; set; }

        public Dictionary<string, object> data { get; set; }
    }
}
