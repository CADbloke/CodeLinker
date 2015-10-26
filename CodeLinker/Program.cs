namespace CodeLinker
{
  class Program
  {
    static void Main(string[] args)
    {
      Linker.LinkCodez(args);
      Linker.Finish();
    }
  }
}
