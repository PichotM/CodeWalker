using CodeWalker.GameFiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeWalker.Project.Panels
{
    public partial class EditYndPanel : ProjectPanel
    {
        public ProjectForm ProjectForm;
        public YndFile Ynd { get; set; }

        private bool populatingui = false;
        private bool waschanged = false;

        public EditYndPanel(ProjectForm projectForm)
        {
            ProjectForm = projectForm;
            InitializeComponent();

            numericUpDownFrom.Value = ProjectForm.NodeFrom;
            numericUpDownFromTwo.Value = ProjectForm.NodeFrom2;
            numericUpDownTo.Value = ProjectForm.NodeTo;
        }

        public void SetYnd(YndFile ynd)
        {
            Ynd = ynd;
            Tag = ynd;
            UpdateFormTitle();
            UpdateYndUI();
            waschanged = ynd?.HasChanged ?? false;
        }

        public void UpdateFormTitleYndChanged()
        {
            bool changed = Ynd.HasChanged;
            if (!waschanged && changed)
            {
                UpdateFormTitle();
                waschanged = true;
            }
            else if (waschanged && !changed)
            {
                UpdateFormTitle();
                waschanged = false;
            }
        }
        private void UpdateFormTitle()
        {
            string fn = Ynd.RpfFileEntry?.Name ?? Ynd.Name;
            if (string.IsNullOrEmpty(fn)) fn = "untitled.ynd";
            Text = fn + (Ynd.HasChanged ? "*" : "");
        }


        public void UpdateYndUI()
        {
            if (Ynd == null)
            {
                //YndPanel.Enabled = false;
                YndRpfPathTextBox.Text = string.Empty;
                YndFilePathTextBox.Text = string.Empty;
                YndProjectPathTextBox.Text = string.Empty;
                YndAreaIDXUpDown.Value = 0;
                YndAreaIDYUpDown.Value = 0;
                YndAreaIDInfoLabel.Text = "ID: 0";
                YndTotalNodesLabel.Text = "Total Nodes: 0";
                YndVehicleNodesUpDown.Value = 0;
                YndVehicleNodesUpDown.Maximum = 0;
                YndPedNodesUpDown.Value = 0;
                YndPedNodesUpDown.Maximum = 0;
            }
            else
            {
                populatingui = true;
                var nd = Ynd.NodeDictionary;
                //YndPanel.Enabled = true;
                YndRpfPathTextBox.Text = Ynd.RpfFileEntry.Path;
                YndFilePathTextBox.Text = Ynd.FilePath;
                YndProjectPathTextBox.Text = (Ynd != null) ? ProjectForm.CurrentProjectFile.GetRelativePath(Ynd.FilePath) : Ynd.FilePath;
                YndAreaIDXUpDown.Value = Ynd.CellX;
                YndAreaIDYUpDown.Value = Ynd.CellY;
                YndAreaIDInfoLabel.Text = "ID: " + Ynd.AreaID.ToString();
                YndTotalNodesLabel.Text = "Total Nodes: " + (nd?.NodesCount.ToString() ?? "0");
                YndVehicleNodesUpDown.Maximum = nd?.NodesCount ?? 0;
                YndVehicleNodesUpDown.Value = Math.Min(nd?.NodesCountVehicle ?? 0, YndVehicleNodesUpDown.Maximum);
                YndPedNodesUpDown.Maximum = nd?.NodesCount ?? 0;
                YndPedNodesUpDown.Value = Math.Min(nd?.NodesCountPed ?? 0, YndPedNodesUpDown.Maximum);
                populatingui = false;
            }
        }

        private void yndMove_Click(object sender, EventArgs e)
        {
            if (populatingui) return;
            if (Ynd == null) return;

            int x = (int)YndAreaIDXUpDown.Value;
            int y = (int)YndAreaIDYUpDown.Value;
            lock (ProjectForm.ProjectSyncRoot)
            {
                int shiftX = x - Ynd.CellX;
                int shiftY = y - Ynd.CellY;

                var areaid = y * 32 + x;

                if (Ynd.AreaID != areaid && ProjectForm.WorldForm != null)
                {
                    YndNode[] yndNodes = Ynd.Nodes;

                    for (int i = 0; i < yndNodes.Length; i++)
                    {
                        YndNode yndNode = yndNodes[i];
                        // a node is 512x512
                        yndNode.SetPosition(yndNode.Position + new SharpDX.Vector3(512 * shiftX, 512 * shiftY, 0.0f));
                        yndNode.AreaID = (ushort)areaid;

                        ProjectForm.WorldForm.SetWidgetPosition(yndNode.Position);
                        ProjectForm.WorldForm.UpdatePathNodeGraphics(yndNode, false);
                    }

                    YndJunction[] yndJunctions = Ynd.Junctions;
                    if (yndJunctions != null && yndJunctions.Length != 0)
                    {
                        for (int i = 0; i < yndJunctions.Length; i++)
                        {
                            YndJunction yndJunction = yndJunctions[i];
                            yndJunction.PositionX += (short)(512 * shiftX);
                            yndJunction.PositionY += (short)(512 * shiftY);
                        }
                    }

                    // Editing links is not needed since they are already loaded as node above

                    Ynd.AreaID = areaid;
                    Ynd.Name = "nodes" + areaid.ToString() + ".ynd";
                    YndAreaIDInfoLabel.Text = "AID: " + areaid.ToString();

                    Ynd.UpdateAllNodePositions();
                    Ynd.UpdateBoundingBox();
                    Ynd.UpdateTriangleVertices();

                    ProjectForm.SetYndHasChanged(true);
                    MessageBox.Show("Don't forget to remove the former ynd!");
                }
            }
            UpdateFormTitleYndChanged();
        }

        private void YndVehicleNodesUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (populatingui) return;
            if (Ynd == null) return;
            if (Ynd.NodeDictionary == null) return;
            lock (ProjectForm.ProjectSyncRoot)
            {
                var vehnodes = (int)YndVehicleNodesUpDown.Value;
                if (Ynd.NodeDictionary.NodesCountVehicle != vehnodes)
                {
                    Ynd.NodeDictionary.NodesCountVehicle = (uint)vehnodes;
                    ProjectForm.SetYndHasChanged(true);
                }
            }
            UpdateFormTitleYndChanged();
        }

        private void YndPedNodesUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (populatingui) return;
            if (Ynd == null) return;
            if (Ynd.NodeDictionary == null) return;
            lock (ProjectForm.ProjectSyncRoot)
            {
                var pednodes = (int)YndPedNodesUpDown.Value;
                if (Ynd.NodeDictionary.NodesCountPed != pednodes)
                {
                    Ynd.NodeDictionary.NodesCountPed = (uint)pednodes;
                    ProjectForm.SetYndHasChanged(true);
                }
            }
            UpdateFormTitleYndChanged();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            lock (ProjectForm.ProjectSyncRoot)
            {
                if (ProjectForm.WorldForm != null)
                {
                    YndNode[] yndNodes = Ynd.Nodes;

                    for (int i = 0; i < yndNodes.Length; i++)
                    {
                        YndNode yndNode = yndNodes[i];
                        yndNode.SetPosition(yndNode.Position + new SharpDX.Vector3(0.0f, -0.5f, 0.0f));

                        ProjectForm.WorldForm.SetWidgetPosition(yndNode.Position);
                        ProjectForm.WorldForm.UpdatePathNodeGraphics(yndNode, false);
                    }

                    YndJunction[] yndJunctions = Ynd.Junctions;
                    if (yndJunctions != null && yndJunctions.Length != 0)
                    {
                        for (int i = 0; i < yndJunctions.Length; i++)
                        {
                            YndJunction yndJunction = yndJunctions[i];
                            yndJunction.PositionY += (short)(-0.5f);
                        }
                    }

                    Ynd.UpdateAllNodePositions();
                    Ynd.UpdateBoundingBox();
                    Ynd.UpdateTriangleVertices();

                    ProjectForm.SetYndHasChanged(true);
                }
            }
            UpdateFormTitleYndChanged();
        }

        private void ApplyNodeTemplateRecursively(YndNode node, int endNode, YndNode previousNode)
        {
            Console.WriteLine("Node: " + node.NodeID + " - previousNode: " + previousNode.NodeID);
            if (node == previousNode ) return;

            if (node.LinkCount != 0)
            {
                node.Flags2 = (byte)BitUtil.UpdateBit(node.Flags2, 7, true);
                Console.WriteLine("Done on " + node.NodeID + ".");
                if (node.NodeID == endNode) return;

                foreach(YndLink link in node.Links)
                {
                    if (link.Node1 != null && link.Node1 != previousNode && link.Node1 != node)
                    {
                        ApplyNodeTemplateRecursively(link.Node1, endNode, node);
                        break;
                    }
                    else if (link.Node2 != null && link.Node2.NodeID != previousNode.NodeID && link.Node2 != node)
                    {
                        ApplyNodeTemplateRecursively(link.Node2, endNode, node);
                        break;
                    }
                }
            }
        }

        private void buttonGo_Click(object sender, EventArgs e)
        {
            // Todo fast flag thing
            int nodeIDFrom = (int)numericUpDownFrom.Value;
            int nodeIDFromTwo = (int)numericUpDownFromTwo.Value;

            int nodeIDTo = (int)numericUpDownTo.Value;

            Console.WriteLine("From " + nodeIDFrom + " to " + nodeIDTo);

            List<YndNode> nodeList = new List<YndNode>();

            YndFile ynd = Ynd;
            foreach(YndNode node in ynd.Nodes)
            {
                if (node.NodeID == nodeIDFrom)
                {
                    node.Flags2 = (byte)BitUtil.UpdateBit(node.Flags2, 7, true);

                    YndLink[] links = node.Links;
                    YndNode secondNode = new YndNode();

                    foreach(YndLink link in links)
                    {
                        if (link.Node1.NodeID == nodeIDFromTwo)
                        {
                            secondNode = link.Node1;
                            break;
                        }
                        else if (link.Node2.NodeID == nodeIDFromTwo)
                        {
                            secondNode = link.Node2;
                            break;
                        }
                    }

                    Console.WriteLine(secondNode.AreaID + " - " + ynd.AreaID + " || " + secondNode.NodeID);
                    if (secondNode != null && secondNode.AreaID == ynd.AreaID)
                    {
                        ApplyNodeTemplateRecursively(secondNode, nodeIDTo, node);
                    }

                    Console.WriteLine("Done.");
                }
            }
        }

        private void numericUpDownFrom_ValueChanged(object sender, EventArgs e)
        {
            ProjectForm.NodeFrom = (int)this.numericUpDownFrom.Value;
        }

        private void numericUpDownFromTwo_ValueChanged(object sender, EventArgs e)
        {
            ProjectForm.NodeFrom2 = (int)this.numericUpDownFromTwo.Value;
        }

        private void numericUpDownTo_ValueChanged(object sender, EventArgs e)
        {
            ProjectForm.NodeTo = (int)this.numericUpDownTo.Value;
        }
    }
}
