using Newtonsoft.Json;

namespace Forms.Models.Salesforce
{
    public class SalesforceAccount
    {
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;
    }
}
