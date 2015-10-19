using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeRecycler;

namespace CodeRecyclerGui
{
  public partial class Gui : Form
  {
    public Gui()
    {
      InitializeComponent();
      BindingSource source = new BindingSource(projectsList, null);
      projectListDataGridView.DataSource = source;
      projectListDataGridView.AutoGenerateColumns = true;
      projectListDataGridView.Columns[0].FillWeight = 5;
      projectListDataGridView.Columns[1].FillWeight = 2;
    }
     class BeforeAfter
    {
      public string SourceProject { get; set; } 
      public string DestinationProjectName { get; set; } 
    }
    
    BindingList<BeforeAfter> projectsList = new BindingList<BeforeAfter>();
    

    private void projectListDataGridView_DragEnter(object sender, DragEventArgs e)
    {
      e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
    }



     private void Sources_DragDrop(object sender, DragEventArgs e)
    {
      List<string> filepaths =  new List<string>();
      foreach (string s in (string[]) e.Data.GetData(DataFormats.FileDrop, false))
      {
        if (Directory.Exists(s))
        {
          filepaths.AddRange(Directory.GetFiles(s, "*.??proj", SearchOption.AllDirectories));
          SourceProjectFolderTextBox.Text = s; // last one wins. May or may not be useful.
        }
        else if (s.IsaCsOrVbProjFile()) { filepaths.Add(s); }
      }
      foreach (string filePath in filepaths)
      {
        if (!(projectsList.Any(p => p.SourceProject == filePath)))
          projectsList.Add(new BeforeAfter {SourceProject = filePath, DestinationProjectName = Path.GetFileName(filePath)});
      }
       projectListDataGridView.Refresh();
    }

    private void FolderTextBox_DragEnter(object sender, DragEventArgs e)
    {
      string[] drops = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
      if (drops.Any())
      {
       if (Directory.Exists(drops[0]))
       {
         e.Effect = DragDropEffects.All;
         return;
       }
      }
      e.Effect = DragDropEffects.None;

    }

    private void SourceFolderTextBox_DragDrop(object sender, DragEventArgs e)
    {
      ProjectFolderTextBox_DragDrop(sender, e);
      Sources_DragDrop(sender, e);
    }

    private void ProjectFolderTextBox_DragDrop(object sender, DragEventArgs e)
    {
      string[] drops = (string[]) e.Data.GetData(DataFormats.FileDrop, false);
      if (!drops.Any())
      {
        e.Effect = DragDropEffects.None;
        return;
      }
      if (Directory.Exists(drops[0]))
      {
        TextBox tb = (TextBox) sender;
        tb.Text = drops[0];
      }
    }
  }
}
