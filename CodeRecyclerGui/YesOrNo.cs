using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace CodeRecyclerGui
{
  internal static class YesOrNo
  {
    /// <summary> Ask a question in a <c>Windows.Forms.MessageBox</c>. Returns <c>True</c> for Yes, <c>False</c> for no. </summary>
    /// <param name="message">  The question you are asking. </param>
    /// <param name="caption">  The caption for the Messagebox. Default is &quot;Hey!&quot; </param>
    internal static bool Ask(string message, string caption = "Hey!")
    {
      DialogResult dialogResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes) { return true; }
      return false;
    }


    /// <summary> Ask a question in a <c>Windows.Forms.MessageBox</c>. Returns <c>True</c> for Yes, <c>False</c> for no, or <c>Null</c> if user cancels. </summary>
    /// <param name="message">  The question you are asking. </param>
    /// <param name="caption">  The caption for the Messagebox. Default is &quot;Hey!&quot; </param>
    internal static bool? OrCancel(string message, string caption = "Hey!")
    {
      DialogResult dialogResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
      if (dialogResult == DialogResult.Yes)    return true;
      if (dialogResult == DialogResult.Cancel) return null;
      return false;
    }

    internal static void Crashing(string message, string caption = "Hey!")
    {
      MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

  }
}
