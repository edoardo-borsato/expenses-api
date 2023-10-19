using ExpensesApi.Models;
using ExpensesApi.Repositories.Entities;
using ExpensesApi.Utility;
using ExpensesApi.Utility.CosmosDb;
using Microsoft.Azure.Cosmos;

namespace ExpensesApi.Repositories;

public class IncomesRepository : IIncomesRepository
{
    private readonly IContainer _container;

    public IncomesRepository(IContainer container)
    {
        _container = container;
    }

    public async Task<IEnumerable<Income?>> GetAllAsync(string username, IFilter filter, CancellationToken cancellationToken = default)
    {
        var entities = await _container.GetItemLinqQueryable<IncomeEntity>(username, cancellationToken);

        entities = filter.Apply(entities);

        return entities.Select(ToIncome);
    }

    public async Task<Income?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        IncomeEntity? entity = null;
        try
        {
            entity = await _container.ReadItemAsync<IncomeEntity>(username, partitionKey, null, cancellationToken);
        }
        catch (NotFoundException)
        {
            // ignored
        }

        return ToIncome(entity);
    }

    public async Task InsertAsync(string username, Income income, CancellationToken cancellationToken = default)
    {
        var entity = ToIncomeEntity(username, income);
        var partitionKey = new PartitionKeyBuilder()
            .Add(entity.Username)
            .Add(entity.Guid)
            .Build();
        await _container.CreateItemAsync(entity, partitionKey, null, cancellationToken);
    }

    public async Task<Income?> UpdateAsync(string username, Guid id, IncomeDetails incomeDetails, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        var entity = await _container.ReadItemAsync<IncomeEntity>(username, partitionKey, null, cancellationToken);

        UpdateIncomeEntity(incomeDetails, entity);

        await _container.UpsertItemAsync(entity, partitionKey, null, cancellationToken);

        return ToIncome(entity);
    }

    public async Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        await _container.DeleteItemAsync<IncomeEntity>(username, partitionKey, null, cancellationToken);
    }

    #region Utility Methods

    private static Income? ToIncome(IncomeEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new Income
        {
            Id = Guid.Parse(entity.Guid!),
            IncomeDetails = new IncomeDetails
            {
                Value = entity.Value,
                Reason = entity.Reason,
                Date = DateTimeOffset.Parse(entity.Date!)
            }
        };
    }

    private static IncomeEntity ToIncomeEntity(string username, Income income)
    {
        return new IncomeEntity
        {
            Id = username,
            Username = username,
            Guid = income.Id.ToString(),
            Value = income.IncomeDetails!.Value,
            Reason = income.IncomeDetails.Reason,
            // ReSharper disable once PossibleInvalidOperationException
            Date = income.IncomeDetails.Date!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ")
        };
    }

    private static void UpdateIncomeEntity(IncomeDetails income, IncomeEntity entity)
    {
        entity.Value = income.Value;
        entity.Reason = income.Reason;
        entity.Date = income.Date!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    #endregion
}