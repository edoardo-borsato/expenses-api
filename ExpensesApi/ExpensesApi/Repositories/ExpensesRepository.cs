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

    public async Task<IEnumerable<Expense?>> GetAllAsync(IFilter filter, CancellationToken cancellationToken = default)
    {
        var expensesEntities = await _container.GetItemLinqQueryable<ExpenseEntity>(cancellationToken);

        expensesEntities = filter.Apply(expensesEntities);

        return expensesEntities.Select(ToExpense);
    }

    public async Task<Expense?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKeyValue = id.ToString();
        ExpenseEntity? expenseEntity = null;
        try
        {
            expenseEntity = await _container.ReadItemAsync<ExpenseEntity>(partitionKeyValue, new PartitionKey(partitionKeyValue), null, cancellationToken);
        }
        catch (NotFoundException)
        {
            // ignored
        }

        return ToExpense(expenseEntity);
    }

    public async Task InsertAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var expenseEntity = ToExpenseEntity(expense);
        await _container.CreateItemAsync(expenseEntity, new PartitionKey(expenseEntity.Id), null, cancellationToken);
    }

    public async Task<Expense?> UpdateAsync(Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default)
    {
        var partitionKeyValue = id.ToString();
        var partitionKey = new PartitionKey(partitionKeyValue);
        var expenseEntity = await _container.ReadItemAsync<ExpenseEntity>(partitionKeyValue, partitionKey, null, cancellationToken);

        UpdateExpenseEntity(expenseDetails, expenseEntity);

        await _container.UpsertItemAsync(expenseEntity, partitionKey, null, cancellationToken);

        return ToExpense(expenseEntity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var partitionKeyValue = id.ToString();
        await _container.DeleteItemAsync<ExpenseEntity>(partitionKeyValue, new PartitionKey(partitionKeyValue), null, cancellationToken);
    }

    #region Utility Methods

    private static Expense? ToExpense(ExpenseEntity? expenseEntity)
    {
        if (expenseEntity is null)
        {
            return null;
        }

        return new Expense
        {
            Id = Guid.Parse(expenseEntity.Id!),
            ExpenseDetails = new ExpenseDetails
            {
                Value = expenseEntity.Value,
                Reason = expenseEntity.Reason,
                Date = DateTimeOffset.Parse(expenseEntity.Date!),
                Category = ToModelCategory(expenseEntity.Category)
            }
        };
    }

    private static ExpenseEntity ToExpenseEntity(Expense expense)
    {
        return new ExpenseEntity
        {
            Id = expense.Id.ToString(),
            Value = expense.ExpenseDetails.Value,
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