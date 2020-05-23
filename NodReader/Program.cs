using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeWalker.Core;
using CodeWalker.GameFiles;
using SharpDX.Mathematics;
using SharpDX;
using CodeWalker;

/*
 * 
 * 
 * 
 *  RECUPERER BACKUP CAR ECHERCHE FLAG PERDUE/RESET
 * 
 * 
 * 
 * 
 * 
 * 
 */

namespace NodReader
{
    // TODO PED NODE flag?
    class Program
    {
        //private static double OffsetX = 67.9358;
       // private static double OffsetY = 839.95;
        //private static double OffsetZ = 2.90625;
        private static double OffsetX = 68.05;
        private static double OffsetY = -177.64;
        private static double OffsetZ = 2.90625;

        private static NodeOpenable nodeInfo;

        private static List<ImportedNode> NodeList = new List<ImportedNode>();
        private static Dictionary<string, ImportedLink[]> LinkList = new Dictionary<string, ImportedLink[]>();
        private static Dictionary<int, YndFile> YndFileList = new Dictionary<int, YndFile>();

        private static int GetAreaIDFromPos(Vector3 pos)
        {
            int NodeSize = 512;

            int shiftX = 1;
            while (Math.Abs(pos.X) > NodeSize * shiftX)
            {
                shiftX++;
            }

            int shiftY = 1;
            while (Math.Abs(pos.Y) > NodeSize * shiftY)
            {
                shiftY++;
            }

            int finalX = pos.X >= 0 ? shiftX : -shiftX;
            int finalY = pos.Y >= 0 ? shiftY : -shiftY;

            return 528 + (finalX + (pos.X >= 0 ? -1 : 0)) + (finalY + (pos.Y >= 0 ? -1 : 0)) * 32;
        }

        static void Main(string[] args)
        {
            string pathNode = "E://Users//Pichot//Downloads//Transfert//nodes_hints//nodes//iv_nodes";
            string[] allNodes = Directory.GetFiles(pathNode);

            NodeList.Clear();

            foreach (string file in allNodes)
            {
                if (Path.GetFileName(file).EndsWith(".nod.txt"))
                {
                    nodeInfo = JsonConvert.DeserializeObject<NodeOpenable>(File.ReadAllText(file));
                    foreach (ImportedNode node in nodeInfo.Nodes)
                    {
                        Vector3 newPos = new Vector3(node.X + (float)OffsetX, node.Y + (float)OffsetY, node.Z * 2);
                        node.NewAreaID = GetAreaIDFromPos(newPos);

                        NodeList.Add(node);
                    }

                    string areaidstr = Path.GetFileName(file).ToLowerInvariant().Replace("nodes", "").Replace(".nod.txt", "");
                    LinkList[areaidstr] = nodeInfo.Links;
                }
            }

            foreach(ImportedNode newNode in NodeList)
            {
                YndFile ynd;
                if (!YndFileList.ContainsKey(newNode.NewAreaID))
                {
                    ynd = new YndFile();
                    ynd.AreaID = (ushort)newNode.NewAreaID;
                    ynd.NodeDictionary = new NodeDictionary();

                    YndFileList[newNode.NewAreaID] = ynd;
                } else
                {
                    ynd = YndFileList[newNode.NewAreaID];
                }

                YndNode node = ynd.AddNode();
                node.AreaID = (ushort)ynd.AreaID;
                node.SetPosition(new Vector3(newNode.X + (float)OffsetX, newNode.Y + (float)OffsetY, newNode.Z * 2));

                // Flag let's go
                uint flags0 = 0;
                uint flags1 = 0;
                uint flags2 = 0;
                uint flags3 = 0;
                uint flags4 = 0;
                uint flags5 = 0;
                flags0 = BitUtil.UpdateBit(flags0, 0, false); // Scripted
                flags0 = BitUtil.UpdateBit(flags0, 1, true); // GPS Enable
                flags0 = BitUtil.UpdateBit(flags0, 2, false); // Unused 4
                flags0 = BitUtil.UpdateBit(flags0, 3, false); // Gravel road?
                flags0 = BitUtil.UpdateBit(flags0, 4, false); // Unused 16
                flags0 = BitUtil.UpdateBit(flags0, 5, false); // Slow unk
                flags0 = BitUtil.UpdateBit(flags0, 6, false); // Junction unk 1
                flags0 = BitUtil.UpdateBit(flags0, 7, false); // Junction unk 2

                flags1 = BitUtil.UpdateBit(flags1, 0, false); // L Turn lane
                flags1 = BitUtil.UpdateBit(flags1, 1, false); // L turn no return
                flags1 = BitUtil.UpdateBit(flags1, 2, false); // R turn no return
                flags1 = BitUtil.UpdateBit(flags1, 3, false); // Traffic light unk 1 // A
                flags1 = BitUtil.UpdateBit(flags1, 4, false); // Traffic light unk 2
                flags1 = BitUtil.UpdateBit(flags1, 5, false); // Junction unk 3 // A
                flags1 = BitUtil.UpdateBit(flags1, 6, false); // Traffic light unk 3
                flags1 = BitUtil.UpdateBit(flags1, 7, false); // Junction unk 4

                flags2 = BitUtil.UpdateBit(flags2, 0, false); // Slow unk 2
                flags2 = BitUtil.UpdateBit(flags2, 1, false); // Unused 2
                flags2 = BitUtil.UpdateBit(flags2, 2, false); // Junction unk 5
                flags2 = BitUtil.UpdateBit(flags2, 3, false); // Unused 8

                bool IsPedNode = newNode.BehaviourType == 8;
                bool backRoad = (newNode.IsRestrictedAccess || newNode.IsWaterNode || newNode.Unk4);
                flags2 = BitUtil.UpdateBit(flags2, 4, false); // Slow unk 3
                flags2 = BitUtil.UpdateBit(flags2, 5, false); // Water/boats
                flags2 = BitUtil.UpdateBit(flags2, 6, true); // Freeway
                flags2 = BitUtil.UpdateBit(flags2, 7, false); // Back Road

                flags3 = BitUtil.UpdateBit(flags3, 0, false); // Interior Node
                flags3 += (((uint)75 & 127u) << 1); // Unk

                
                flags4 = BitUtil.UpdateBit(flags4, 0, IsPedNode); // Slow unk 4
                flags4 += (((IsPedNode == true ? (uint)7 : (uint)4) & 7u) << 1); // Unk
                flags4 = BitUtil.UpdateBit(flags4, 4, IsPedNode); // Special 1
                flags4 = BitUtil.UpdateBit(flags4, 5, IsPedNode); // Special 2
                flags4 = BitUtil.UpdateBit(flags4, 6, IsPedNode); // Special 3
                flags4 = BitUtil.UpdateBit(flags4, 7, false); // Junction unk 6

                flags5 = BitUtil.UpdateBit(flags5, 0, false); // Has junction heightmap
                flags5 = BitUtil.UpdateBit(flags5, 1, false); // Speed unk 1
                flags5 = BitUtil.UpdateBit(flags5, 2, true); // Speed unk 2

                if (IsPedNode)
                {
                    ynd.NodeDictionary.NodesCountPed++;
                } else
                {
                    ynd.NodeDictionary.NodesCountVehicle++;
                }

                node.Flags0 = (byte)flags0;
                node.Flags1 = (byte)flags1;
                node.Flags2 = (byte)flags2;
                node.Flags3 = (byte)flags3;
                node.Flags4 = (byte)flags4;
                node.LinkCountUnk = (byte)flags5;

                //node.StreetName;

                newNode.Node = node;
                newNode.NewNodeID = node.NodeID;

                Console.WriteLine("Node " + node.NodeID + " done.");
            }

            foreach (KeyValuePair<int, YndFile> entry in YndFileList)
            {
                YndFile ynd = entry.Value;
                foreach (YndNode yndNode in ynd.Nodes)
                {
                    ImportedNode node = new ImportedNode();
                    foreach(ImportedNode iindoe in NodeList)
                    {
                        if (iindoe.NewAreaID == yndNode.AreaID && iindoe.NewNodeID == yndNode.NodeID)
                        {
                            node = iindoe;
                            break;
                        }
                    }

                    int links = node.NumberOfLinks;
                    if (links != 0 && node != null && node.Node != null)
                    {
                        for (var i = 0; i < links; i++)
                        {
                            int linkArrayIndex = node.BaseLink + i;
                            ImportedLink targetLink = LinkList[node.AreaID.ToString()][linkArrayIndex];
                            foreach (ImportedNode node2 in NodeList)
                            {
                                if (node2.NodeID == targetLink.TargetNode && node2.AreaID == targetLink.TargetArea)
                                {
                                    YndLink newLink = node.Node.AddLink(node2.Node);

                                    uint flags0 = 0;
                                    uint flags1 = 0;
                                    uint flags2 = 0;

                                    // Always 128?
                                    flags0 = BitUtil.UpdateBit(flags0, 0, false); // Special 1
                                    flags0 = BitUtil.UpdateBit(flags0, 1, false); // Scripted unk
                                    flags0 += (((uint)0 & 7u) << 2); // Unk 1
                                    flags0 += (((uint)4 & 7u) << 5); // Unk 2

                                    flags1 = BitUtil.UpdateBit(flags1, 0, false); // Unused 1
                                    flags1 = BitUtil.UpdateBit(flags1, 1, false); // Unknown 1
                                    flags1 = BitUtil.UpdateBit(flags1, 2, false); // Dead end
                                    flags1 = BitUtil.UpdateBit(flags1, 3, false); // Dead end exit
                                    flags1 += (((uint)0 & 7u) << 4); // Ofset Size
                                    flags1 = BitUtil.UpdateBit(flags1, 7, false); // Negative offset

                                    flags2 = BitUtil.UpdateBit(flags2, 0, false); // Angled/merget link
                                    flags2 = BitUtil.UpdateBit(flags2, 1, false); // Lane change / U-turn
                                    flags2 += (((uint)targetLink.NumRightLanes & 7u) << 2); // Back lanes
                                    flags2 += (((uint)targetLink.NumLeftLanes & 7u) << 5); // Fwd lanes

                                    newLink.Flags0 = (byte)flags0;
                                    newLink.Flags1 = (byte)flags1;
                                    newLink.Flags2 = (byte)flags2;
                                }
                            }
                        }
                    }
                }
            }

            foreach(KeyValuePair<int, YndFile> entry in YndFileList)
            {
                YndFile ynd = entry.Value;
                byte[] newData = ynd.Save();
                Console.WriteLine("Successfully written " + ynd.AreaID + " node.");
                File.WriteAllBytes(pathNode + "//build//nodes" + ynd.AreaID + ".ynd", newData);
            }

            Console.WriteLine("Hello are you ready?");
            Console.ReadKey();
        }
    }
}
