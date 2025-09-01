using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FHIR_Bundle_Visualizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        Dictionary<string, TreeNode> ResourceList;
        Dictionary<string, int> ResourceTypeList;
        int ResourceCount = 0;
        public void ClearAllControls()
        {
            groupBox3.Text = "Details";
            richTextBox1.Text = string.Empty;
        }

        public void SetJSONDetails(string jsonString)
        {
            JsonDocument doc = JsonDocument.Parse(jsonString);
            try
            {
                ResourceList = new Dictionary<string, TreeNode>();
                ResourceTypeList = new Dictionary<string, int>();
                JsonElement entryNode = doc.RootElement.GetProperty("entry");
                foreach (JsonElement item in entryNode.EnumerateArray())
                {
                    JsonElement resource = item.GetProperty("resource");
                    string id = resource.GetProperty("id").GetString();
                    string resourceType = resource.GetProperty("resourceType").GetString();

                    if (ResourceTypeList.ContainsKey(resourceType))
                    {
                        ResourceTypeList[resourceType] += 1;
                        TreeNode childNode = new TreeNode() { Name = id, Text = id, Tag = resource };
                        treeView1.Nodes[resourceType].Nodes.Add(childNode);
                        treeView1.Refresh();
                    }
                    else
                    {
                        ResourceTypeList.Add(resourceType, 1);
                        TreeNode node = new TreeNode() { Name = resourceType, Text = resourceType, Tag = "P" };
                        treeView1.Nodes.Add(node);
                        TreeNode childNode = new TreeNode() { Name = id, Text = id, Tag = resource };
                        treeView1.Nodes[resourceType].Nodes.Add(childNode);
                        treeView1.Refresh();
                        comboBox1.Items.Add(resourceType);
                    }

                    ResourceCount += 1;
                    labelResourceCount.Text = ResourceCount.ToString();
                    labelResourceCount.Refresh();
                }

                foreach (var item in ResourceTypeList)
                {
                    treeView1.Nodes[item.Key.ToString()].Text = $"{ item.Key.ToString() } ({item.Value.ToString()})";
                    ResourceList.Add(item.Key.ToString(), treeView1.Nodes[item.Key.ToString()]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to read the Json file.");
            }
        }

        public void SetValuesToControls(string SelectedKey, string ResourceString)
        {
            if (ResourceString != "P")
            {
                JsonDocument doc = JsonDocument.Parse(ResourceString);
                JsonElement resource = doc.RootElement;
                if (resource.ValueKind != JsonValueKind.Null)
                {
                    groupBox3.Text = SelectedKey;
                    richTextBox1.Text = ResourceString;
                }
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.Title = "Select a File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    txtFilePath.Refresh();
                }
            }
            if (txtFilePath.Text.Length > 0)
            {
                var jsonString = File.ReadAllText(txtFilePath.Text);
                SetJSONDetails(jsonString);
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ClearAllControls();
            SetValuesToControls(e.Node.Text, e.Node.Tag.ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearAllControls();
            SetValuesToControls(e.Node.Text, e.Node.Tag.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResourceList != null && ResourceList.Count > 0)
            {
                string selectedReourcetype = comboBox1.SelectedItem.ToString();
                treeView1.Nodes.Clear();
                foreach (TreeNode item in ResourceList.Values)
                {
                    if (selectedReourcetype == "ALL" || item.Name == selectedReourcetype)
                    {
                        treeView1.Nodes.Add(item);
                    }
                }
            }
        }

    }
}
