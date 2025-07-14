using Newtonsoft.Json;

namespace msfs_simple_sail
{
    public class PlaneEvent
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("parameter")]
        public object Parameter { get; set; }

        public override string ToString()
        {
            return $"{this.Event} {this.Parameter}";
        }
    }
}
