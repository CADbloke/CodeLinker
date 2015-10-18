namespace CodeRecyclerGui
{
  partial class Form1
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
      this.SuspendLayout();
      // 
      // sourceLabel
      // 
      this.sourceLabel.AutoSize = true;
      this.sourceLabel.Location = new System.Drawing.Point(13, 13);
      this.sourceLabel.Name = "sourceLabel";
      this.sourceLabel.Size = new System.Drawing.Size(88, 13);
      this.sourceLabel.TabIndex = 0;
      this.sourceLabel.Text = "Source Project(s)";
      // 
      // SourcesListBox
      // 
      this.SourcesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.SourcesListBox.FormattingEnabled = true;
      this.SourcesListBox.Location = new System.Drawing.Point(13, 30);
      this.SourcesListBox.Name = "SourcesListBox";
      this.SourcesListBox.Size = new System.Drawing.Size(759, 95);
      this.SourcesListBox.TabIndex = 1;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(784, 361);
      this.Controls.Add(this.SourcesListBox);
      this.Controls.Add(this.sourceLabel);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label sourceLabel;
    private System.Windows.Forms.ListBox SourcesListBox;
  }
}

