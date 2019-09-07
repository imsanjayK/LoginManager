using Newtonsoft.Json;
using System.Collections.Generic;

namespace LoginManagerApp.Service
{
    public class UserAccount
    {
        [JsonProperty(PropertyName = "id")]
        public string log_Id { get; set; }

        public string userName { get; set; }

        public string password { get; set; }

        public string website { get; set; }

        public override bool Equals(object obj)
        {
            // If the object is compared with itself then return true   
            if (obj == this)
            {
                return true;
            }

            /* Check if o is an instance of Complex or not 
              "null instanceof [type]" also returns false */
            if (!(obj is UserAccount))
            {
                return false;
            }

            var account = obj as UserAccount;
            return account != null &&
                   userName == account.userName &&
                   password == account.password &&
                   website == account.website;
        }

        public override int GetHashCode()
        {
            var hashCode = 1889006667;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(userName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(password);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(website);
            return hashCode;
        }
    }
}