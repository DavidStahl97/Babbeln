using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedCode.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Profile
    {
        [JsonProperty("name")]
        public string Name;

        [JsonProperty("birthday")]
        public DateTime Birthday;

        [JsonProperty("city")]
        public string City;

        [JsonProperty("country")]
        public string Coutry;

        [JsonProperty("avatar")]
        public string Avatar;

        [JsonProperty("info")]
        public string Info;
    }
}
