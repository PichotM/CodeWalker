using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodReader
{
    class NodeOpenable
    {
        [JsonProperty("nodeCount")]
        public int NodeCount { get; set; }

        [JsonProperty("vehCount")]
        public int VehCount { get; set; }

        [JsonProperty("linkCount")]
        public int LinkCount { get; set; }

        [JsonProperty("carCount")]
        public int CarCount { get; set; }

        [JsonProperty("nodes")]
        public ImportedNode[] Nodes { get; set; }

        [JsonProperty("links")]
        public ImportedLink[] Links { get; set; }
    }
}
