using Microsoft.Azure.Cosmos;

namespace ExpensesApi.Utility.CosmosDb;

public interface IContainer
{
    Task<IEnumerable<T>> GetItemLinqQueryable<T>(string id, CancellationToken cancellationToken = default);
    Task<T> ReadItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default);
    Task CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default);
    Task UpsertItemAsync<T>(T item, PartitionKey? partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default);
    Task DeleteItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default);
}

public class ContainerException : Exception
{
    public ContainerException(string message) : base(message)
    {
    }
}