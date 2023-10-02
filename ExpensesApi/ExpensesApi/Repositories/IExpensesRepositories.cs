using ExpensesApi.Models;

namespace ExpensesApi.Repositories
{
    public interface IExpensesRepository
    {
        Task<IEnumerable<Expense>> GetAllAsync(IFilter filter, CancellationToken cancellationToken = default);
        Task<Expense> GetAsync(Guid id, CancellationToken cancellationToken = default);
        Task InsertAsync(Expense expense, CancellationToken cancellationToken = default);
        Task<Expense> UpdateAsync(Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
