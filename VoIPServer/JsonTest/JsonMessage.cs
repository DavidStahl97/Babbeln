using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonTest
{ 
    [JsonObject(MemberSerialization.OptIn)]
    public class JsonMessage
    {
        [JsonProperty(PropertyName = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        [JsonProperty(PropertyName = "data")]
        public string Data { get; set; }
    }

    public enum MessageType
    {
        message,
        request,
        accept
    }
}
