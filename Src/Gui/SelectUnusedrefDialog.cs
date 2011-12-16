using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace RemoveUnusedRef.Gui
{
    public class SelectUnusedrefDialog : Form
    {
        private List<ProjectReference> m_selectedProjectReferences;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel buttonsPanel;
        private System.Windows.Forms.Panel listViewPanel;
        private System.Windows.Forms.ListView referencesListView;
        private System.Windows.Forms.ColumnHeader referenceNameHeader;
        private System.Windows.Forms.ColumnHeader referenceVersionHeader;
        private System.Windows.Forms.ColumnHeader referenceLocationHeader;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing) 
            {
                if (components != null) 
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.buttonsPanel = new System.Windows.Forms.Panel();
            this.listViewPanel = new System.Windows.Forms.Panel();
            this.referencesListView = new System.Windows.Forms.ListView();
            this.referenceNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.referenceVersionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.referenceLocationHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonsPanel.SuspendLayout();
            this.listViewPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(355, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = ResourceService.GetString("Global.CancelButtonText");
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(274, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = ResourceService.GetString("Global.OKButtonText");
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Controls.Add(this.okButton);
            this.buttonsPanel.Controls.Add(this.cancelButton);
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonsPanel.Location = new System.Drawing.Point(0, 358);
            this.buttonsPanel.Name = "buttonsPanel";
            this.buttonsPanel.Size = new System.Drawing.Size(436, 35);
            this.buttonsPanel.TabIndex = 8;
            // 
            // listViewPanel
            // 
            this.listViewPanel.Controls.Add(this.referencesListView);
            this.listViewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPanel.Location = new System.Drawing.Point(0, 0);
            this.listViewPanel.Name = "listViewPanel";
            this.listViewPanel.Size = new System.Drawing.Size(436, 358);
            this.listViewPanel.TabIndex = 9;
            // 
            // referencesListView
            // 
            this.referencesListView.CheckBoxes = true;
            this.referencesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.referenceNameHeader,
            this.referenceLocationHeader,
            this.referenceVersionHeader});
            this.referencesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.referencesListView.FullRowSelect = true;
            this.referencesListView.Location = new System.Drawing.Point(0, 0);
            this.referencesListView.MultiSelect = false;
            this.referencesListView.Name = "referencesListView";
            this.referencesListView.Size = new System.Drawing.Size(436, 358);
            this.referencesListView.TabIndex = 8;
            this.referencesListView.UseCompatibleStateImageBehavior = false;
            this.referencesListView.View = System.Windows.Forms.View.Details;
            // 
            // referenceNameHeader
            // 
            this.referenceNameHeader.Text = ResourceService.GetString("RemoveUnusedRef.SelectUnusedRefDialog.HeaderReferenceName");
            this.referenceNameHeader.Width = 150;
            
            this.referenceLocationHeader.Text = ResourceService.GetString("RemoveUnusedRef.SelectUnusedRefDialog.HeaderReferenceLocation");
            this.referenceLocationHeader.Width = 200;
            // 
            // referenceVersionHeader
            // 
            this.referenceVersionHeader.Text = ResourceService.GetString("RemoveUnusedRef.SelectUnusedRefDialog.HeaderReferenceVersion");
            this.referenceVersionHeader.Width = 80;
            // 
            // SelectUnusedrefDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(436, 393);
            this.Controls.Add(this.listViewPanel);
            this.Controls.Add(this.buttonsPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 350);
            this.Name = "SelectUnusedrefDialog";
            this.ShowInTaskbar = false;
            this.Text = ResourceService.GetString("RemoveUnusedRef.SelectUnusedRefDialog.Text");
            this.buttonsPanel.ResumeLayout(false);
            this.listViewPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        
        public SelectUnusedrefDialog(IEnumerable<ProjectReference> projectReferences)
        {
            if (projectReferences == null)
                throw new ArgumentNullException("projectReferences");
            InitializeComponent();
            m_selectedProjectReferences = new List<ProjectReference>();
            FillReferencesListView(projectReferences);
        }
        
        public IEnumerable<ProjectReference> SelectedProjectReferences 
        { 
            get
            {
                return m_selectedProjectReferences;
            }
        }
        
        private void OkButtonClick(object sender, EventArgs e)
        {
            m_selectedProjectReferences.Clear();
            foreach(ListViewItem item in referencesListView.Items)
            {
                if (item.Checked)
                {
                    m_selectedProjectReferences.Add((ProjectReference)item.Tag);
                }
            }
        }
        
        private void FillReferencesListView(IEnumerable<ProjectReference> projectReferences)
        {
            referencesListView.BeginUpdate();
            try
            {
                referencesListView.Items.Clear();
                foreach(var projectReference in projectReferences)
                {
                    var item = new ListViewItem
                    {
                        Text = projectReference.Name,
                        Checked = true,
                        Tag = projectReference
                    };
                    item.SubItems.Add(projectReference.Location);
                    item.SubItems.Add(projectReference.Version == null ? 
                                      string.Empty
                                      : projectReference.Version.ToString());
                    referencesListView.Items.Add(item);
                }
            }
            finally
            {
                referencesListView.EndUpdate();
            }
        }
    }
}