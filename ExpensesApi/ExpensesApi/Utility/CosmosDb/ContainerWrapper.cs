using System.Net;
using Microsoft.Azure.Cosmos;

namespace ExpensesApi.Utility.CosmosDb;

public class ContainerWrapper : IContainer
{
    private readonly Container _container;

    public ContainerWrapper(Container container)
    {
        _container = container;
    }

    public async Task<IEnumerable<T>> GetItemLinqQueryable<T>(string id, CancellationToken cancellationToken = default)
    {
        var items = new List<T>();
        using var iterator = _container.GetItemQueryIterator<T>(new QueryDefinition(
                "SELECT * FROM c WHERE c.id = @id")
            .WithParameter("@id", id));
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);

            items.AddRange(response.ToList());
        }

        return items;
    }

    public async Task<T> ReadItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, partitionKey, requestOptions, cancellationToken);
            return response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException();
        }
    }

    public async Task CreateItemAsync<T>(T item, PartitionKey? partitionKey = null, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        await _container.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);
    }

    public async Task UpsertItemAsync<T>(T item, PartitionKey? partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        await _container.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken);
    }

    public async Task DeleteItemAsync<T>(string id, PartitionKey partitionKey, ItemRequestOptions? requestOptions = null, CancellationToken cancellationToken = default)
    {
        await _container.DeleteItemStreamAsync(id, partitionKey, requestOptions, cancellationToken);
    }
}