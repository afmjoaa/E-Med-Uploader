using System;
using System.ComponentModel;
using System.Configuration;
using Firebase.Auth;
using Newtonsoft.Json;

namespace custom_window.Core
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class FirebaseUser
    {
        public string FirebaseToken { get; set; }

        public string RefreshToken { get; set; }

        public int ExpiresIn { get; set; }

        public DateTime Created { get; set; }

        [JsonProperty("localId", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string LocalId { get; set; }

        [JsonProperty("federatedId", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string FederatedId { get; set; }

        [JsonProperty("firstName", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string FirstName { get; set; }

        [JsonProperty("lastName", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string LastName { get; set; }

        [JsonProperty("displayName", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string DisplayName { get; set; }

        [JsonProperty("email", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string Email { get; set; }

        [JsonProperty("emailVerified", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(false)]
        public bool IsEmailVerified { get; set; }

        [JsonProperty("photoUrl", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string PhotoUrl { get; set; }

        [JsonProperty("phoneNumber", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string PhoneNumber { get; set; }

        public bool IsExpired()
        {
            return DateTime.Now > this.Created.AddSeconds((double) (this.ExpiresIn - 10));
        }
    }
}