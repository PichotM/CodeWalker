using CodeWalker.GameFiles;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodReader
{
    class ImportedNode
    {
        [JsonProperty("baseLink")]
        public int BaseLink { get; set; }

        [JsonProperty("nodeID")]
        public int NodeID { get; set; }

        [JsonProperty("isRoadBlock")]
        public bool IsRoadBlock { get; set; }

        [JsonProperty("middleY")]
        public float MiddleY { get; set; }

        [JsonProperty("isDeadEnd")]
        public bool IsDeadEnd { get; set; }

        [JsonProperty("isEmergencyVehicleOnly")]
        public bool IsEmergencyVehicleOnly { get; set; }

        [JsonProperty("isIgnoredNode")]
        public bool IsIgnoredNode { get; set; }

        [JsonProperty("width")]
        public bool Width { get; set; }

        [JsonProperty("spawnProbability")]
        public int SpawnProbability { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("isDontWander")]
        public bool IsDontWander { get; set; }

        [JsonProperty("isRestrictedAccess")]
        public bool IsRestrictedAccess { get; set; }

        [JsonProperty("middleX")]
        public float MiddleX { get; set; }

        [JsonProperty("floodcolor")]
        public int Floodcolor { get; set; }

        [JsonProperty("unk1")]
        public bool Unk1 { get; set; }

        [JsonProperty("unk0")]
        public bool Unk0 { get; set; }

        [JsonProperty("unk2")]
        public bool Unk2 { get; set; }

        [JsonProperty("unk3")]
        public bool Unk3 { get; set; }

        [JsonProperty("unk4")]
        public bool Unk4 { get; set; }

        [JsonProperty("unk5")]
        public bool Unk5 { get; set; }

        [JsonProperty("unk6")]
        public bool Unk6 { get; set; }

        [JsonProperty("unk7")]
        public bool Unk7 { get; set; }

        [JsonProperty("unk8")]
        public bool Unk8 { get; set; }

        [JsonProperty("unk9")]
        public bool Unk9 { get; set; }

        [JsonProperty("areaID")]
        public int AreaID { get; set; }

        [JsonProperty("isWaterNode")]
        public bool IsWaterNode { get; set; }

        [JsonProperty("numberOfLinks")]
        public int NumberOfLinks { get; set; }

        [JsonProperty("speedlimit")]
        public int Speedlimit { get; set; }

        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("z")]
        public float Z { get; set; }

        [JsonProperty("behaviourType")]
        public int BehaviourType { get; set; }

        public YndNode Node { get; set; }

        public int NewAreaID { get; set; }
        public int NewNodeID { get; set; }
        public List<ImportedLink> Links { get; set; } = new List<ImportedLink>();
    }
}
