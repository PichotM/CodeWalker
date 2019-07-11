using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodReader
{
    class ImportedLink
    {
        [JsonProperty("trafficLightDirection")]
        public int TrafficLightDirection { get; set; }

        [JsonProperty("numRightLanes")]
        public int NumRightLanes { get; set; }

        [JsonProperty("trafficLightBehaviour")]
        public int TrafficLightBehaviour { get; set; }

        [JsonProperty("numLeftLanes")]
        public int NumLeftLanes { get; set; }

        [JsonProperty("isTrainCrossing")]
        public int IsTrainCrossing { get; set; }

        [JsonProperty("targetNode")]
        public int TargetNode { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("targetArea")]
        public int TargetArea { get; set; }

        public override string ToString()
        {
            return "{ TrafficLightDirection : " + TrafficLightDirection + ", NumRightLanes : " + NumRightLanes + ", TrafficLightBehaviour: " + TrafficLightBehaviour
            + ", NumLeftLanes: " + NumLeftLanes + ", IsTrainCrossing: " + IsTrainCrossing + ", TargetNode: " + TargetNode + ", Length: " + Length + ", TargetArea: " + TargetArea + " }";
        }
    }
}
