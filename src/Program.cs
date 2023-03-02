internal class Program
{
  internal static void Main(string[] args)
  {
    Syncer
    .CreateOptions()
    .CreateRootCommand()
    .BuildCommandLine()
    .UseHost(_ => Host.CreateDefaultBuilder())
  }
}