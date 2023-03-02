internal class Program
{
  internal async static Task<int> Main(string[] args)
  {
    return await Syncer
    .BuildRootCommand()
    .InvokeAsync(args);
  }
}