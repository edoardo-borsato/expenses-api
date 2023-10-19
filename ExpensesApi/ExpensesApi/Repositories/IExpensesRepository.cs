using ExpensesApi.Models;

namespace ExpensesApi.Repositories;

public interface IExpensesRepository
{
    Task<IEnumerable<Expense?>> GetAllAsync(string username, IFilter filter, CancellationToken cancellationToken = default);
    Task<Expense?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default);
    Task InsertAsync(string username, Expense expense, CancellationToken cancellationToken = default);
    Task<Expense?> UpdateAsync(string username, Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
    Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default);
}