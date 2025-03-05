using Newtonsoft.Json;

namespace Forms.Models.Salesforce
{
    public class SalesforceResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; } = null!;

        [JsonProperty("errors")]
        public string[] Errors { get; set; } = null!;

    }
}
