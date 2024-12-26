using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EventGridHTTPSend
{
    public class GridEvent
    {
        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get; set;
        }

        [JsonProperty(PropertyName = "topic")]
        public string Topic
        {
            get; set;
        }

        [JsonProperty(PropertyName = "subject")]
        public string Subject
        {
            get; set;
        }

        [JsonProperty(PropertyName = "eventType")]
        public string EventType
        {
            get; set;
        }

        [JsonProperty(PropertyName = "eventTime")]
        public DateTime EventTime
        {
            get; set;
        }

        [JsonProperty(PropertyName = "data")]
        public object Data
        {
            get; set;
        }

        [JsonProperty(PropertyName = "dataVersion")]
        public string DataVersion
        {
            get; set;
        }
    }

    public class ItemReceivedEventData
    {
        [JsonProperty(PropertyName = "itemSku")]
        public string ItemSku
        {
            get; set;
        }
    }

}
