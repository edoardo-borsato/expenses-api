using ExpensesApi.Models;

namespace ExpensesApi.Registries;

public interface IExpensesRegistry
{
    Task<List<Expense?>> GetAllAsync(string username, FilterParameters? filterParameters, CancellationToken cancellationToken = default);
    Task<Expense?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default);
    Task<Expense> InsertAsync(string username, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
    Task<Expense?> UpdateAsync(string username, Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
    Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default);
}