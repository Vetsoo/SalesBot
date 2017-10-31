using Microsoft.Azure.Documents.Spatial;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesBot.Azure.CosmosDb
{
    public class ConsultantInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("skills")]
        public ICollection<string> Skills { get; set; }

        [JsonProperty("interests")]
        public ICollection<string> Interests { get; set; }

        [JsonProperty("location")]
        public Point Location { get; set; }
    }
}

