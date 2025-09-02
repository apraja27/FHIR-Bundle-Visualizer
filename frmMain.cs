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
            ResourceCount = 0;
            ResourceList = new Dictionary<string, TreeNode>();
            ResourceTypeList = new Dictionary<string, int>();

            treeView1.Nodes.Clear();
            comboBox1.Items.Clear();
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
            labelResourceCount.Text = ResourceCount.ToString();

            try
            {
                var parser = new FhirJsonParser();
                Bundle bundle = parser.Parse<Bundle>(jsonString);

                foreach (var item in bundle.Entry)
                {
                    var serializer = new FhirJsonSerializer(new SerializerSettings
                    {
                        Pretty = true
                    });
                    string Jresource = serializer.SerializeToString(item.Resource);
                    if (ResourceTypeList.ContainsKey(item.Resource.TypeName))
                    {
                        ResourceTypeList[item.Resource.TypeName] += 1;
                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = Jresource };
                        ResourceList[item.Resource.TypeName].Nodes.Add(childNode);
                    }
                    else
                    {
                        ResourceTypeList.Add(item.Resource.TypeName, 1);
                        TreeNode node = new TreeNode() { Name = item.Resource.TypeName, Text = item.Resource.TypeName, Tag = "P" };

                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = Jresource };
                        node.Nodes.Add(childNode);

                        ResourceList.Add(item.Resource.TypeName, node);
                    }
                    ResourceCount += 1;
                    labelResourceCount.Text = ResourceCount.ToString();
                    labelResourceCount.Refresh();
                }

                var Temp = ResourceTypeList.OrderBy(r => r.Key);
                foreach (var item in Temp)
                {
                    comboBox1.Items.Add(item.Key);
                    ResourceList[item.Key].Text = $"{ item.Key.ToString() } ({item.Value.ToString()})";
                    treeView1.Nodes.Add(ResourceList[item.Key]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to read the selected file." );
                txtFilePath.Text = string.Empty;
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
                    buttonCopytoClipboard.Show();
                }
            }
            else
            {
                buttonCopytoClipboard.Hide();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
            labelCopied.Hide();
            buttonCopytoClipboard.Hide();
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                treeView1.ExpandAll();
            }
            else
            {
                treeView1.CollapseAll();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
            labelCopied.Show();
            timer1.Enabled = true;
        }

        int counter = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= 10)
            {
                labelCopied.Visible = false;
                timer1.Stop();
                counter = 0;
            }
        }
    }
}
