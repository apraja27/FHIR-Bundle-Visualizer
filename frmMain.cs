using FHIR_Bundle_Visualizer.Fhir.Parser;
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
        TreeNode selectedNode;
        string completeJSON = string.Empty;
        int fileVersion = 0;

        public void ClearAllControls()
        {
            groupBox3.Text = "Details";
            richTextBox1.Text = string.Empty;
        }

        public void ReInitializeValues()
        {
            selectedNode = null;
            completeJSON = string.Empty;
            resourceCount = 0;
            resourceList = new Dictionary<string, TreeNode>();

            btnBrowse.Enabled = false;
            checkBox1.Enabled = false;
            btnAllDetails.Enabled = false;
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

            labelFileVersion.Text = string.Empty;
            labelPatientAge.Text = string.Empty;
            labelPatientName.Text = string.Empty;
            labelBirthDate.Text = string.Empty;
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
            selectedNode = treeView1.Nodes[0];
            checkBox1.Visible = true;
            btnAllDetails.Visible = true;
            checkBox1.Checked = false;
        }

        public void SetPatientDetails(Bundle bundle)
        {
            PatientDetails patientDetails = null;
            if (fileVersion == 1)
            {
                patientDetails = Hl7FhirSTU3.GetPatientDetails(bundle);
            }
            else if (fileVersion == 2)
            {
                patientDetails = Hl7FhirR4.GetPatientDetails(bundle);
            }

            if (patientDetails != null)
            {
                labelPatientName.Text = patientDetails.Name;
                labelPatientAge.Text = patientDetails.Age;
                labelBirthDate.Text = patientDetails.BirthDate;
            }
        }

        public Bundle DeserializeFromString(string jsonString)
        {
            Bundle bundle = null;
            try
            {
                bundle = Hl7FhirSTU3.DeserializeFromString(jsonString);
                fileVersion = 1;
            }
            catch
            {
                try
                {
                    bundle = Hl7FhirR4.DeserializeFromString(jsonString);
                    fileVersion = 2;
                }
                catch
                {
                    throw;
                }
            }
            return bundle;
        }

        public void SetJsonDetails(Bundle bundle)
        {
            try
            {
                foreach (var item in bundle.Entry)
                {
                    string tag = string.Empty;
                    if (fileVersion == 1)
                    {
                        tag = Hl7FhirSTU3.SerializeToString(item.Resource);
                    }
                    else if (fileVersion == 2)
                    {
                        tag = Hl7FhirR4.SerializeToString(item.Resource);
                    }
                    if (resourceList.ContainsKey(item.Resource.TypeName))
                    {
                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = tag };
                        resourceList[item.Resource.TypeName].Nodes.Add(childNode);
                    }
                    else
                    {
                        TreeNode node = new TreeNode() { Name = item.Resource.TypeName, Text = item.Resource.TypeName, Tag = "P" };
                        TreeNode childNode = new TreeNode() { Name = item.Resource.Id, Text = item.Resource.Id, Tag = tag };
                        node.Nodes.Add(childNode);

                        resourceList.Add(item.Resource.TypeName, node);
                    }
                    resourceCount += 1;
                    labelResourceCount.Text = resourceCount.ToString();
                    labelResourceCount.Refresh();
                }
                SetPatientDetails(bundle);
                AssignValueToControls();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SetValuesToControls(string selectedKey, string resourceString)
        {
            if (resourceString != "P")
            {
                JsonDocument doc = JsonDocument.Parse(resourceString);
                JsonElement resource = doc.RootElement;
                string resourceType = resource.GetProperty("resourceType").GetString();
                if (resource.ValueKind != JsonValueKind.Null)
                {
                    groupBox3.Text = $"Resource Type: {resourceType}, Resource Id : {selectedKey}";
                    richTextBox1.Text = resourceString;
                    btnCopytoClipboard.Show();
                }
            }
            else
            {
                btnCopytoClipboard.Hide();
            }
        }

        public string GetFileVersionName()
        {
            string versionName = "Unknown";
            if (fileVersion == 1)
            {
                versionName = "STU3";
            }
            else if (fileVersion == 2)
            {
                versionName = "R4";
            }
            return versionName;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("ALL");
            comboBox1.SelectedIndex = 0;
            labelCopied.Hide();
            btnCopytoClipboard.Hide();
            checkBox1.Visible = false;
            btnAllDetails.Visible = false;
            labelFileVersion.Text = string.Empty;
            labelPatientName.Text = string.Empty;
            labelPatientAge.Text = string.Empty;
            labelBirthDate.Text = string.Empty;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            bool fileSelected = false;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Json files (*.json)|*.json";
                openFileDialog.Title = "Select a json File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtFilePath.Text = openFileDialog.FileName;
                    txtFilePath.Refresh();
                    fileSelected = true;
                }
            }
            if (fileSelected && txtFilePath.Text.Length > 0)
            {
                txtJsonText.Text = string.Empty;
                try
                {
                    ReInitializeValues();
                    var jsonString = File.ReadAllText(txtFilePath.Text);
                    completeJSON = jsonString;
                    Bundle bundle = DeserializeFromString(jsonString);
                    SetJsonDetails(bundle);
                    labelFileVersion.Text = GetFileVersionName();
                }
                catch (Exception)
                {
                    txtFilePath.Text = string.Empty;
                    MessageBox.Show("Unable to read selected file. Please select a valid FHIR json file.", "FHIR Bundle Visualizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    btnBrowse.Enabled = true;
                    checkBox1.Enabled = true;
                    btnAllDetails.Enabled = true;
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ClearAllControls();
            selectedNode = e.Node;
            SetValuesToControls(e.Node.Text, e.Node.Tag.ToString());
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ClearAllControls();
            selectedNode = e.Node;
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
                try
                {
                    var Temp = resourceList.OrderBy(r => r.Key);
                    foreach (var item in Temp)
                    {
                        if (selectedReourcetype == "ALL" || item.Key == selectedReourcetype)
                        {
                            treeView1.Nodes.Add(item.Value);
                        }
                    }
                    if (selectedReourcetype == "ALL")
                    {
                        treeView1.SelectedNode = selectedNode;
                        if (treeView1.SelectedNode != null)
                            treeView1.SelectedNode.EnsureVisible();
                    }
                }
                catch
                {
                }
                finally
                {
                    treeView1.Focus();
                    treeView1.Refresh();
                }

            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    checkBox1.Text = "Collapse All";
                    treeView1.ExpandAll();
                }
                else
                {
                    checkBox1.Text = "Expand All";
                    treeView1.CollapseAll();
                }
                treeView1.SelectedNode = selectedNode;
                if (treeView1.SelectedNode != null)
                    treeView1.SelectedNode.EnsureVisible();
            }
            catch { }
            finally
            {
                treeView1.Focus();
            }
        }

        private void btnCopytoClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(richTextBox1.Text);
                labelCopied.Show();
                timer1.Enabled = true;
            }
            catch
            {
            }
            finally
            {
                treeView1.Focus();
            }
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

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(richTextBox1.Text);
            }
            catch
            {
            }
        }


        private void btnLoadJson_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtJsonText.Text.Length > 0)
                {
                    txtFilePath.Text = string.Empty;
                    txtFilePath.Refresh();
                    ReInitializeValues();
                    var jsonString = txtJsonText.Text;
                    completeJSON = jsonString;
                    Bundle bundle = DeserializeFromString(jsonString);
                    SetJsonDetails(bundle);
                    labelFileVersion.Text = GetFileVersionName();
                }
            }
            catch (Exception)
            {
                txtJsonText.Text = string.Empty;
                MessageBox.Show("Unable to get details from text.", "FHIR Bundle Visualizer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                btnBrowse.Enabled = true;
                checkBox1.Enabled = true;
                btnAllDetails.Enabled = true;
            }
        }

        private void txtJsonText_TextChanged(object sender, EventArgs e)
        {
            if (txtJsonText.Text.Length > 0)
            {
                btnLoadJson.Enabled = true;
            }
            else
            {
                btnLoadJson.Enabled = false;
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterDistance = 300;
            }
            catch
            {
            }
        }

        private void btnAllDetails_Click(object sender, EventArgs e)
        {
            groupBox3.Text = "All Details";
            richTextBox1.Text = completeJSON;
        }
    }
}
