using Newtonsoft.Json;

namespace Forms.Models.Salesforce
{
    public class SalesforceContact
    {
        [JsonProperty("Email")]
        public string Email { get; set; } = null!;

        [JsonProperty("Phone")]
        public string Phone { get; set; } = null!;

        [JsonProperty("LastName")]
        public string LastName { get; set; } = null!;

        [JsonProperty("FirstName")]
        public string FirstName { get; set; } = null!;

        [JsonProperty("AccountId")]
        public string AccountId { get; set; } = null!;
    }
}
