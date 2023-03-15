namespace OnspringAzureADSyncer.Interfaces;

public interface ISyncer
{
  Task<int> Run();
}