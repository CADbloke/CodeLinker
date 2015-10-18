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
      this.sourceLabel = new System.Windows.Forms.Label();
      this.SourcesListBox = new System.Windows.Forms.ListBox();
      this.SourceProjectFolderTextBox = new System.Windows.Forms.TextBox();
      this.destinationLabel = new System.Windows.Forms.Label();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // sourceLabel
      // 
      this.sourceLabel.AutoSize = true;
      this.sourceLabel.Location = new System.Drawing.Point(12, 16);
      this.sourceLabel.Name = "sourceLabel";
      this.sourceLabel.Size = new System.Drawing.Size(88, 13);
      this.sourceLabel.TabIndex = 0;
      this.sourceLabel.Text = "Source Project(s)";
      // 
      // SourcesListBox
      // 
      this.SourcesListBox.AllowDrop = true;
      this.SourcesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SourcesListBox.FormattingEnabled = true;
      this.SourcesListBox.Location = new System.Drawing.Point(13, 39);
      this.SourcesListBox.Name = "SourcesListBox";
      this.SourcesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
      this.SourcesListBox.Size = new System.Drawing.Size(959, 121);
      this.SourcesListBox.TabIndex = 1;
      this.SourcesListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.SourcesListBox_DragDrop);
      this.SourcesListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.SourcesListBox_DragEnter);
      // 
      // SourceProjectFolderTextBox
      // 
      this.SourceProjectFolderTextBox.AllowDrop = true;
      this.SourceProjectFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SourceProjectFolderTextBox.Location = new System.Drawing.Point(106, 13);
      this.SourceProjectFolderTextBox.Name = "SourceProjectFolderTextBox";
      this.SourceProjectFolderTextBox.Size = new System.Drawing.Size(385, 20);
      this.SourceProjectFolderTextBox.TabIndex = 2;
      this.SourceProjectFolderTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragDrop);
      this.SourceProjectFolderTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.FolderTextBox_DragEnter);
      // 
      // destinationLabel
      // 
      this.destinationLabel.AutoSize = true;
      this.destinationLabel.Location = new System.Drawing.Point(865, 16);
      this.destinationLabel.Name = "destinationLabel";
      this.destinationLabel.Size = new System.Drawing.Size(107, 13);
      this.destinationLabel.TabIndex = 3;
      this.destinationLabel.Text = "Destination Project(s)";
      this.destinationLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
      // 
      // textBox1
      // 
      this.textBox1.AllowDrop = true;
      this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.textBox1.Location = new System.Drawing.Point(494, 13);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(365, 20);
      this.textBox1.TabIndex = 4;
      // 
      // Gui
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(984, 361);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.destinationLabel);
      this.Controls.Add(this.SourceProjectFolderTextBox);
      this.Controls.Add(this.SourcesListBox);
      this.Controls.Add(this.sourceLabel);
      this.Name = "Gui";
      this.Text = "Gui";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label sourceLabel;
    private System.Windows.Forms.ListBox SourcesListBox;
    private System.Windows.Forms.TextBox SourceProjectFolderTextBox;
    private System.Windows.Forms.Label destinationLabel;
    private System.Windows.Forms.TextBox textBox1;
  }
}

