using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextMessage
    {
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }

        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }

        [JsonProperty(PropertyName ="date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}
