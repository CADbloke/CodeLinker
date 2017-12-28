using System.Windows.Forms;

namespace CodeLinker
{
  partial class Gui
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.sourceFolderButton = new System.Windows.Forms.Button();
            this.SourceProjectFolderTextBox = new System.Windows.Forms.TextBox();
            this.destinationFolderButton = new System.Windows.Forms.Button();
            this.DestinationProjectFolderTextBox = new System.Windows.Forms.TextBox();
            this.projectListDataGridView = new System.Windows.Forms.DataGridView();
            this.linkButton = new System.Windows.Forms.Button();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.OpenTheLogFileButton = new System.Windows.Forms.Button();
            this.CreateSubFoldersChk = new System.Windows.Forms.CheckBox();
            this.RelativePathsCheckBox = new System.Windows.Forms.CheckBox();
            this.LinkPrefixTextBox = new System.Windows.Forms.TextBox();
            this.LinkPrefixLabel = new System.Windows.Forms.Label();
            this.guiBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.guiBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // sourceFolderButton
            // 
            this.sourceFolderButton.AutoSize = true;
            this.sourceFolderButton.Location = new System.Drawing.Point(30, 9);
            this.sourceFolderButton.Name = "sourceFolderButton";
            this.sourceFolderButton.Size = new System.Drawing.Size(84, 23);
            this.sourceFolderButton.TabIndex = 0;
            this.sourceFolderButton.Text = "Source Folder";
            this.sourceFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.sourceFolderButton.Click += new System.EventHandler(this.SourceFolderButton_Click);
            // 
            // SourceProjectFolderTextBox
            // 
            this.SourceProjectFolderTextBox.AllowDrop = true;
            this.SourceProjectFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SourceProjectFolderTextBox.Location = new System.Drawing.Point(120, 9);
            this.SourceProjectFolderTextBox.Name = "SourceProjectFolderTextBox";
            this.SourceProjectFolderTextBox.Size = new System.Drawing.Size(749, 20);
            this.SourceProjectFolderTextBox.TabIndex = 2;
            this.SourceProjectFolderTextBox.TextChanged += new System.EventHandler(this.CheckProjectsList);
            this.SourceProjectFolderTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.SourceFolderTextBox_DragDrop);
            this.SourceProjectFolderTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragEnter);
            // 
            // destinationFolderButton
            // 
            this.destinationFolderButton.AutoSize = true;
            this.destinationFolderButton.Location = new System.Drawing.Point(12, 33);
            this.destinationFolderButton.Name = "destinationFolderButton";
            this.destinationFolderButton.Size = new System.Drawing.Size(102, 23);
            this.destinationFolderButton.TabIndex = 3;
            this.destinationFolderButton.Text = "Destination Folder";
            this.destinationFolderButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.destinationFolderButton.Click += new System.EventHandler(this.DestinationFolderButton_Click);
            // 
            // DestinationProjectFolderTextBox
            // 
            this.DestinationProjectFolderTextBox.AllowDrop = true;
            this.DestinationProjectFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DestinationProjectFolderTextBox.Location = new System.Drawing.Point(120, 36);
            this.DestinationProjectFolderTextBox.Name = "DestinationProjectFolderTextBox";
            this.DestinationProjectFolderTextBox.Size = new System.Drawing.Size(749, 20);
            this.DestinationProjectFolderTextBox.TabIndex = 4;
            this.DestinationProjectFolderTextBox.TextChanged += new System.EventHandler(this.CheckProjectsList);
            this.DestinationProjectFolderTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.ProjectFolderTextBox_DragDrop);
            this.DestinationProjectFolderTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragEnter);
            // 
            // projectListDataGridView
            // 
            this.projectListDataGridView.AllowDrop = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.projectListDataGridView.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.projectListDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.projectListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.projectListDataGridView.Location = new System.Drawing.Point(12, 89);
            this.projectListDataGridView.Name = "projectListDataGridView";
            this.projectListDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.projectListDataGridView.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.projectListDataGridView.Size = new System.Drawing.Size(857, 331);
            this.projectListDataGridView.TabIndex = 5;
            this.projectListDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckProjectsList);
            this.projectListDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckProjectsList);
            this.projectListDataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.CheckProjectsList);
            this.projectListDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.Sources_DragDrop);
            this.projectListDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectListDataGridView_DragEnter);
            // 
            // linkButton
            // 
            this.linkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.linkButton.Location = new System.Drawing.Point(784, 426);
            this.linkButton.Name = "linkButton";
            this.linkButton.Size = new System.Drawing.Size(88, 23);
            this.linkButton.TabIndex = 6;
            this.linkButton.Text = "Link the Codez";
            this.linkButton.UseVisualStyleBackColor = true;
            this.linkButton.Click += new System.EventHandler(this.linkButton_Click);
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StatusLabel.Location = new System.Drawing.Point(13, 426);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(671, 23);
            this.StatusLabel.TabIndex = 7;
            this.StatusLabel.Text = "hello";
            // 
            // OpenTheLogFileButton
            // 
            this.OpenTheLogFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenTheLogFileButton.Location = new System.Drawing.Point(690, 426);
            this.OpenTheLogFileButton.Name = "OpenTheLogFileButton";
            this.OpenTheLogFileButton.Size = new System.Drawing.Size(88, 23);
            this.OpenTheLogFileButton.TabIndex = 8;
            this.OpenTheLogFileButton.Text = "Open Log File";
            this.OpenTheLogFileButton.UseVisualStyleBackColor = true;
            this.OpenTheLogFileButton.Click += new System.EventHandler(this.OpenTheLogFileButton_Click);
            // 
            // CreateSubFoldersChk
            // 
            this.CreateSubFoldersChk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CreateSubFoldersChk.AutoSize = true;
            this.CreateSubFoldersChk.Checked = true;
            this.CreateSubFoldersChk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CreateSubFoldersChk.Location = new System.Drawing.Point(562, 430);
            this.CreateSubFoldersChk.Name = "CreateSubFoldersChk";
            this.CreateSubFoldersChk.Size = new System.Drawing.Size(122, 17);
            this.CreateSubFoldersChk.TabIndex = 9;
            this.CreateSubFoldersChk.Text = "Create Subfolder(s)?";
            this.CreateSubFoldersChk.UseVisualStyleBackColor = true;
            // 
            // RelativePathsCheckBox
            // 
            this.RelativePathsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RelativePathsCheckBox.AutoSize = true;
            this.RelativePathsCheckBox.Checked = true;
            this.RelativePathsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.RelativePathsCheckBox.Location = new System.Drawing.Point(746, 65);
            this.RelativePathsCheckBox.Name = "RelativePathsCheckBox";
            this.RelativePathsCheckBox.Size = new System.Drawing.Size(123, 17);
            this.RelativePathsCheckBox.TabIndex = 10;
            this.RelativePathsCheckBox.Text = "Use Relative Paths?";
            this.RelativePathsCheckBox.UseVisualStyleBackColor = true;
            this.RelativePathsCheckBox.CheckedChanged += new System.EventHandler(this.RelativePathscheckBox_CheckedChanged);
            // 
            // LinkPrefixTextBox
            // 
            this.LinkPrefixTextBox.Location = new System.Drawing.Point(132, 62);
            this.LinkPrefixTextBox.Name = "LinkPrefixTextBox";
            this.LinkPrefixTextBox.Size = new System.Drawing.Size(552, 20);
            this.LinkPrefixTextBox.TabIndex = 11;
            this.LinkPrefixTextBox.Leave += new System.EventHandler(this.LinkPrefixTextBox_TextChanged);
            // 
            // LinkPrefixLabel
            // 
            this.LinkPrefixLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LinkPrefixLabel.AutoSize = true;
            this.LinkPrefixLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.LinkPrefixLabel.Location = new System.Drawing.Point(13, 65);
            this.LinkPrefixLabel.Name = "LinkPrefixLabel";
            this.LinkPrefixLabel.Size = new System.Drawing.Size(113, 13);
            this.LinkPrefixLabel.TabIndex = 12;
            this.LinkPrefixLabel.Text = "Dest Link Folder Prefix";
            this.LinkPrefixLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // guiBindingSource
            // 
            this.guiBindingSource.DataSource = typeof(CodeLinker.Gui);
            // 
            // Gui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 461);
            this.Controls.Add(this.LinkPrefixLabel);
            this.Controls.Add(this.LinkPrefixTextBox);
            this.Controls.Add(this.RelativePathsCheckBox);
            this.Controls.Add(this.CreateSubFoldersChk);
            this.Controls.Add(this.OpenTheLogFileButton);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.linkButton);
            this.Controls.Add(this.projectListDataGridView);
            this.Controls.Add(this.DestinationProjectFolderTextBox);
            this.Controls.Add(this.destinationFolderButton);
            this.Controls.Add(this.SourceProjectFolderTextBox);
            this.Controls.Add(this.sourceFolderButton);
            this.Name = "Gui";
            this.Text = "Code Linker by CADbloke";
            ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.guiBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button sourceFolderButton;
    private System.Windows.Forms.TextBox SourceProjectFolderTextBox;
    private System.Windows.Forms.Button destinationFolderButton;
    private System.Windows.Forms.TextBox DestinationProjectFolderTextBox;
    private System.Windows.Forms.DataGridView projectListDataGridView;
    private System.Windows.Forms.Button linkButton;
    private System.Windows.Forms.BindingSource guiBindingSource;
    private Label StatusLabel;
    private Button OpenTheLogFileButton;
        private CheckBox CreateSubFoldersChk;
        private CheckBox RelativePathsCheckBox;
        private TextBox LinkPrefixTextBox;
        private Label LinkPrefixLabel;
    }
}

