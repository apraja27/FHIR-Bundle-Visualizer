//https://www.nuget.org/packages/Hl7.Fhir.STU3/6.0.0-rc1
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
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

        Dictionary<string, TreeNode> resourceList;
        int resourceCount = 0;
        TreeNode SelectedNode;
        public void ClearAllControls()
        {
            groupBox3.Text = "Details";
            richTextBox1.Text = string.Empty;
        }

        public void ReInitializeValues()
        {
            resourceCount = 0;
            resourceList = new Dictionary<string, TreeNode>();

            btnBrowse.Enabled = false;
            checkBox1.Enabled = false;
            btnCopytoClipboard.Visible = false;

            labelCopied.Hide();
            labelCopied.Refresh();

            groupBox3.Text = "Details";
            groupBox3.Refresh();
            richTextBox1.Text = string.Empty;
            richTextBox1.Refresh();
            labelResourceCount.Text = resourceCount.ToString();

            treeView1.Nodes.Clear();
            treeView1.Refresh();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
        }

        public void AssignValueToControls()
        {
            var Temp = resourceList.OrderBy(r => r.Key);
            foreach (var item in Temp)
            {
                resourceList[item.Key].Text = $"{ item.Key.ToString() } ({item.Value.Nodes.Count.ToString()})";
                comboBox1.Items.Add(item.Key);
                treeView1.Nodes.Add(resourceList[item.Key]);
                treeView1.Refresh();
            }
            treeView1.SelectedNode = treeView1.Nodes[0];
            SelectedNode = treeView1.Nodes[0];
            checkBox1.Visible = true;
            checkBox1.Checked = false;
        }

        public void SetJSONDetails(Bundle bundle)
        {
            try
            {
                foreach (var item in bundle.Entry)
                {
                    var serializer = new FhirJsonSerializer(new SerializerSettings { Pretty = true });
                    string jRresource = serializer.SerializeToString(item.Resource);
                    if (resourceList.ContainsKey(item.Resource.TypeName))
                    {
                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = jRresource };
                        resourceList[item.Resource.TypeName].Nodes.Add(childNode);
                    }
                    else
                    {
                        TreeNode node = new TreeNode() { Name = item.Resource.TypeName, Text = item.Resource.TypeName, Tag = "P" };
                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = jRresource };
                        node.Nodes.Add(childNode);

                        resourceList.Add(item.Resource.TypeName, node);
                    }
                    resourceCount += 1;
                    labelResourceCount.Text = resourceCount.ToString();
                    labelResourceCount.Refresh();
                }
                AssignValueToControls();
            }
            catch (Exception ex)
            {
                throw ex;
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
                    btnCopytoClipboard.Show();
                }
            }
            else
            {
                btnCopytoClipboard.Hide();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
            labelCopied.Hide();
            btnCopytoClipboard.Hide();
            checkBox1.Visible = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            txtFilePath.Text = string.Empty;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Json files (*.json)|*.json";
                openFileDialog.Title = "Select a json File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    txtFilePath.Refresh();
                }
            }
            if (txtFilePath.Text.Length > 0)
            {
                try
                {
                    ReInitializeValues();
                    var jsonString = File.ReadAllText(txtFilePath.Text);
                    var parser = new FhirJsonParser();
                    Bundle bundle = parser.Parse<Bundle>(jsonString);
                    SetJSONDetails(bundle);
                }
                catch (Exception)
                {
                    txtFilePath.Text = string.Empty;
                    MessageBox.Show("Unable to read selected file.", "FHIR Bundle Visualizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    btnBrowse.Enabled = true;
                    checkBox1.Enabled = true;
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ClearAllControls();
            SelectedNode = e.Node;
            SetValuesToControls(e.Node.Text, e.Node.Tag.ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearAllControls();
            SelectedNode = e.Node;
            SetValuesToControls(e.Node.Text, e.Node.Tag.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (resourceList != null && resourceList.Count > 0)
            {
                string selectedReourcetype = comboBox1.SelectedItem.ToString();
                treeView1.Nodes.Clear();
                groupBox3.Text = "Details";
                richTextBox1.Text = string.Empty;

                var Temp = resourceList.OrderBy(r => r.Key);
                foreach (var item in Temp)
                {
                    if (selectedReourcetype == "ALL" || item.Key == selectedReourcetype)
                    {
                        treeView1.Nodes.Add(item.Value);
                    }
                }
                if(selectedReourcetype == "ALL")
                {
                    treeView1.SelectedNode = SelectedNode;
                    treeView1.SelectedNode.EnsureVisible();
                    treeView1.Focus();
                    treeView1.Refresh();
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox1.Text = "Collapse All";
                treeView1.ExpandAll();
                treeView1.SelectedNode = SelectedNode;
                treeView1.SelectedNode.EnsureVisible();
                treeView1.Focus();
                treeView1.Refresh();
            }
            else
            {
                checkBox1.Text = "Expand All";
                treeView1.CollapseAll();
                treeView1.SelectedNode = SelectedNode;
                treeView1.SelectedNode.EnsureVisible();
                treeView1.Focus();
                treeView1.Refresh();
            }
        }

        private void btnCopytoClipboard_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
            labelCopied.Show();
            timer1.Enabled = true;
        }

        int counter = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= 3)
            {
                labelCopied.Visible = false;
                timer1.Stop();
                counter = 0;
            }
        }
    }
}
