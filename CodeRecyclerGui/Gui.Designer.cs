namespace CodeRecyclerGui
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
      this.sourceLabel = new System.Windows.Forms.Label();
      this.SourceProjectFolderTextBox = new System.Windows.Forms.TextBox();
      this.destinationLabel = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.projectListDataGridView = new System.Windows.Forms.DataGridView();
      this.recycleButton = new System.Windows.Forms.Button();
      this.settingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.guiBindingSource = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.guiBindingSource)).BeginInit();
      this.SuspendLayout();
      // 
      // sourceLabel
      // 
      this.sourceLabel.AutoSize = true;
      this.sourceLabel.Location = new System.Drawing.Point(32, 20);
      this.sourceLabel.Name = "sourceLabel";
      this.sourceLabel.Size = new System.Drawing.Size(88, 13);
      this.sourceLabel.TabIndex = 0;
      this.sourceLabel.Text = "Source Project(s)";
      this.sourceLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // SourceProjectFolderTextBox
      // 
      this.SourceProjectFolderTextBox.AllowDrop = true;
      this.SourceProjectFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SourceProjectFolderTextBox.Location = new System.Drawing.Point(126, 13);
      this.SourceProjectFolderTextBox.Name = "SourceProjectFolderTextBox";
      this.SourceProjectFolderTextBox.Size = new System.Drawing.Size(282, 20);
      this.SourceProjectFolderTextBox.TabIndex = 2;
      this.SourceProjectFolderTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragDrop);
      this.SourceProjectFolderTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragEnter);
      // 
      // destinationLabel
      // 
      this.destinationLabel.AutoSize = true;
      this.destinationLabel.Location = new System.Drawing.Point(13, 42);
      this.destinationLabel.Name = "destinationLabel";
      this.destinationLabel.Size = new System.Drawing.Size(107, 13);
      this.destinationLabel.TabIndex = 3;
      this.destinationLabel.Text = "Destination Project(s)";
      this.destinationLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // textBox1
      // 
      this.textBox1.AllowDrop = true;
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(126, 39);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(282, 20);
      this.textBox1.TabIndex = 4;
      // 
      // projectListDataGridView
      // 
      this.projectListDataGridView.AllowDrop = true;
      this.projectListDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.projectListDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.projectListDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.projectListDataGridView.Location = new System.Drawing.Point(12, 71);
      this.projectListDataGridView.Name = "projectListDataGridView";
      this.projectListDataGridView.Size = new System.Drawing.Size(854, 249);
      this.projectListDataGridView.TabIndex = 5;
      this.projectListDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.Sources_DragDrop);
      this.projectListDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectListDataGridView_DragEnter);
      // 
      // recycleButton
      // 
      this.recycleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.recycleButton.Location = new System.Drawing.Point(794, 326);
      this.recycleButton.Name = "recycleButton";
      this.recycleButton.Size = new System.Drawing.Size(75, 23);
      this.recycleButton.TabIndex = 6;
      this.recycleButton.Text = "Recycle";
      this.recycleButton.UseVisualStyleBackColor = true;
      // 
      // settingsBindingSource
      // 
      this.settingsBindingSource.DataSource = typeof(CodeRecycler.Settings);
      // 
      // guiBindingSource
      // 
      this.guiBindingSource.DataSource = typeof(CodeRecyclerGui.Gui);
      // 
      // Gui
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(881, 361);
      this.Controls.Add(this.recycleButton);
      this.Controls.Add(this.projectListDataGridView);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.destinationLabel);
      this.Controls.Add(this.SourceProjectFolderTextBox);
      this.Controls.Add(this.sourceLabel);
      this.Name = "Gui";
      this.Text = "Code Recycler by CADbloke";
      ((System.ComponentModel.ISupportInitialize)(this.projectListDataGridView)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.settingsBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.guiBindingSource)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label sourceLabel;
    private System.Windows.Forms.TextBox SourceProjectFolderTextBox;
    private System.Windows.Forms.Label destinationLabel;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.DataGridView projectListDataGridView;
    private System.Windows.Forms.Button recycleButton;
    private System.Windows.Forms.BindingSource settingsBindingSource;
    private System.Windows.Forms.BindingSource guiBindingSource;
  }
}

