namespace ExpensesApi.Utility.CosmosDb;

public interface ICosmosClient
{
    IContainer GetContainer(string databaseId, string containerId);
}