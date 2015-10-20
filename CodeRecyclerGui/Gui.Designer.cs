using System.Windows.Forms;

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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.sourceFolderButton = new System.Windows.Forms.Button();
      this.SourceProjectFolderTextBox = new System.Windows.Forms.TextBox();
      this.destinationFolderButton = new System.Windows.Forms.Button();
      this.DestinationProjectFolderTextBox = new System.Windows.Forms.TextBox();
      this.projectListDataGridView = new System.Windows.Forms.DataGridView();
      this.recycleButton = new System.Windows.Forms.Button();
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
      this.projectListDataGridView.Location = new System.Drawing.Point(12, 71);
      this.projectListDataGridView.Name = "projectListDataGridView";
      this.projectListDataGridView.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.projectListDataGridView.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.projectListDataGridView.Size = new System.Drawing.Size(857, 349);
      this.projectListDataGridView.TabIndex = 5;
      this.projectListDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckProjectsList);
      this.projectListDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.CheckProjectsList);
      this.projectListDataGridView.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.CheckProjectsList);
      this.projectListDataGridView.DragDrop += new System.Windows.Forms.DragEventHandler(this.Sources_DragDrop);
      this.projectListDataGridView.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectListDataGridView_DragEnter);
      // 
      // recycleButton
      // 
      this.recycleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.recycleButton.Location = new System.Drawing.Point(797, 426);
      this.recycleButton.Name = "recycleButton";
      this.recycleButton.Size = new System.Drawing.Size(75, 23);
      this.recycleButton.TabIndex = 6;
      this.recycleButton.Text = "Recycle";
      this.recycleButton.UseVisualStyleBackColor = true;
      this.recycleButton.Click += new System.EventHandler(this.recycleButton_Click);
      // 
      // guiBindingSource
      // 
      this.guiBindingSource.DataSource = typeof(CodeRecyclerGui.Gui);
      // 
      // Gui
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(884, 461);
      this.Controls.Add(this.recycleButton);
      this.Controls.Add(this.projectListDataGridView);
      this.Controls.Add(this.DestinationProjectFolderTextBox);
      this.Controls.Add(this.destinationFolderButton);
      this.Controls.Add(this.SourceProjectFolderTextBox);
      this.Controls.Add(this.sourceFolderButton);
      this.Name = "Gui";
      this.Text = "Code Recycler by CADbloke";
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
    private System.Windows.Forms.Button recycleButton;
    private System.Windows.Forms.BindingSource guiBindingSource;
  }
}

