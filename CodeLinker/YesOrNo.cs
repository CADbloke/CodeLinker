using System;


namespace CodeLinker
{
  internal static class YesOrNo
  {
    /// <summary> Ask a question in a <c>Windows.Forms.MessageBox</c>. Returns <c>True</c> for Yes, <c>False</c> for no. </summary>
    /// <param name="message">  The question you are asking. </param>
    /// <param name="caption">  The caption for the Messagebox. Default is &quot;Hey!&quot; </param>
    internal static bool Ask(string message, string caption = "Hey!")
    {
      if (Linker.NoConfirm) { return true; }
      Console.WriteLine(message);
      Console.WriteLine("Y/N ?");
      ConsoleKeyInfo yn = Console.ReadKey();

      if (yn.KeyChar.ToString().ToLower() == "y") return true;

      return false;
    }


    /// <summary> Ask a question in a <c>Windows.Forms.MessageBox</c>. Returns <c>True</c> for Yes, <c>False</c> for no, or <c>Null</c> if user cancels. </summary>
    /// <param name="message">  The question you are asking. </param>
    /// <param name="caption">  The caption for the Messagebox. Default is &quot;Hey!&quot; </param>
    internal static bool? OrCancel(string message, string caption = "Hey!")
    {
      if (Linker.NoConfirm) { return true; }
      Console.WriteLine(message);
      Console.WriteLine("Y/N or C for Cancel ?");
      ConsoleKeyInfo yn = Console.ReadKey();

      if (yn.KeyChar.ToString().ToLower() == "y") return true;
      if (yn.KeyChar.ToString().ToLower() == "c") return null;

      return false;
    }

    internal static void Crashing(string message, string caption = "Hey!")
    {
      Linker.Crash(message);
    }

  }
}
