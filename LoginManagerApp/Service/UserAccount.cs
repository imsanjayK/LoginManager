using Newtonsoft.Json;

namespace LoginManagerApp.Service
{
    public class UserAccount
    {
        [JsonProperty(PropertyName = "id")]
        public string log_Id { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string website { get; set; }
    }
}