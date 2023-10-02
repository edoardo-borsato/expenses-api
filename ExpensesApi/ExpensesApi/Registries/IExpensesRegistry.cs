using ExpensesApi.Models;

namespace ExpensesApi.Registries;

public interface IExpensesRegistry
{
    Task<List<Expense?>> GetAllAsync(FilterParameters filterParameters, CancellationToken cancellationToken = default);
    Task<Expense?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Expense> InsertAsync(ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
    Task<Expense?> UpdateAsync(Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}