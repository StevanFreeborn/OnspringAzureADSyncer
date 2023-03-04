namespace OnspringAzureADSyncer.Interfaces;

public interface IGraphService
{
  Task<PageIterator<Group, GroupCollectionResponse>?> GetGroupsIterator(List<Group> groups, int pageSize);
  Task<bool> IsConnected();
}