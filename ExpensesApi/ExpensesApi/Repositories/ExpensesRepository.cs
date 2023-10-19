using ExpensesApi.Models;
using ExpensesApi.Repositories.Entities;
using ExpensesApi.Utility;
using ExpensesApi.Utility.CosmosDb;
using Microsoft.Azure.Cosmos;

namespace ExpensesApi.Repositories;

public class ExpensesRepository : IExpensesRepository
{
    private readonly IContainer _container;

    public ExpensesRepository(IContainer container)
    {
        _container = container;
    }

    public async Task<IEnumerable<Expense?>> GetAllAsync(string username, IFilter filter, CancellationToken cancellationToken = default)
    {
        var entities = await _container.GetItemLinqQueryable<ExpenseEntity>(username, cancellationToken);

        entities = filter.Apply(entities);

        return entities.Select(ToExpense);
    }

    public async Task<Expense?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        ExpenseEntity? entity = null;
        try
        {
            entity = await _container.ReadItemAsync<ExpenseEntity>(username, partitionKey, null, cancellationToken);
        }
        catch (NotFoundException)
        {
            // ignored
        }

        return ToExpense(entity);
    }

    public async Task InsertAsync(string username, Expense expense, CancellationToken cancellationToken = default)
    {
        var entity = ToExpenseEntity(username, expense);
        var partitionKey = new PartitionKeyBuilder()
            .Add(entity.Username)
            .Add(entity.Guid)
            .Build();
        await _container.CreateItemAsync(entity, partitionKey, null, cancellationToken);
    }

    public async Task<Expense?> UpdateAsync(string username, Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        var entity = await _container.ReadItemAsync<ExpenseEntity>(username, partitionKey, null, cancellationToken);

        UpdateExpenseEntity(expenseDetails, entity);

        await _container.UpsertItemAsync(entity, partitionKey, null, cancellationToken);

        return ToExpense(entity);
    }

    public async Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKey = new PartitionKeyBuilder()
            .Add(username)
            .Add(id.ToString())
            .Build();
        await _container.DeleteItemAsync<ExpenseEntity>(username, partitionKey, null, cancellationToken);
    }

    #region Utility Methods

    private static Expense? ToExpense(ExpenseEntity? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new Expense
        {
            Id = Guid.Parse(entity.Guid!),
            ExpenseDetails = new ExpenseDetails
            {
                Value = entity.Value,
                Reason = entity.Reason,
                Date = DateTimeOffset.Parse(entity.Date!),
                Category = ToModelCategory(entity.Category)
            }
        };
    }

    private static ExpenseEntity ToExpenseEntity(string username, Expense expense)
    {
        return new ExpenseEntity
        {
            Id = username,
            Username = username,
            Guid = expense.Id.ToString(),
            Value = expense.ExpenseDetails!.Value,
            Reason = expense.ExpenseDetails.Reason,
            // ReSharper disable once PossibleInvalidOperationException
            Date = expense.ExpenseDetails.Date!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            Category = ToEntityCategory(expense.ExpenseDetails.Category)
        };
    }

    private static Entities.Category ToEntityCategory(Models.Category? category)
    {
        return category switch
        {
            Models.Category.HealthAndPersonalCare => Entities.Category.HealthAndPersonalCare,
            Models.Category.Sport => Entities.Category.Sport,
            Models.Category.HousingAndSupplies => Entities.Category.HousingAndSupplies,
            Models.Category.Transportation => Entities.Category.Transportation,
            Models.Category.Clothing => Entities.Category.Clothing,
            Models.Category.Entertainment => Entities.Category.Entertainment,
            Models.Category.BillsAndUtilities => Entities.Category.BillsAndUtilities,
            Models.Category.Pets => Entities.Category.Pets,
            Models.Category.Insurance => Entities.Category.Insurance,
            Models.Category.Gifts => Entities.Category.Gifts,
            Models.Category.Others => Entities.Category.Others,
            _ => Entities.Category.Others
        };
    }

    private static Models.Category? ToModelCategory(Entities.Category category)
    {
        return category switch
        {
            Entities.Category.HealthAndPersonalCare => Models.Category.HealthAndPersonalCare,
            Entities.Category.Sport => Models.Category.Sport,
            Entities.Category.HousingAndSupplies => Models.Category.HousingAndSupplies,
            Entities.Category.Transportation => Models.Category.Transportation,
            Entities.Category.Clothing => Models.Category.Clothing,
            Entities.Category.Entertainment => Models.Category.Entertainment,
            Entities.Category.BillsAndUtilities => Models.Category.BillsAndUtilities,
            Entities.Category.Pets => Models.Category.Pets,
            Entities.Category.Insurance => Models.Category.Insurance,
            Entities.Category.Gifts => Models.Category.Gifts,
            Entities.Category.Others => Models.Category.Others,
            _ => Models.Category.Others
        };
    }

    private static void UpdateExpenseEntity(ExpenseDetails expense, ExpenseEntity expenseEntity)
    {
        expenseEntity.Value = expense.Value;
        expenseEntity.Reason = expense.Reason;
        expenseEntity.Date = expense.Date!.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
        expenseEntity.Category = ToEntityCategory(expense.Category);
    }

    #endregion
}