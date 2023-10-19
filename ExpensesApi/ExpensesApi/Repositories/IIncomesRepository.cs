using ExpensesApi.Models;

namespace ExpensesApi.Repositories;

public interface IIncomesRepository
{
    Task<IEnumerable<Income?>> GetAllAsync(string username, IFilter filter, CancellationToken cancellationToken = default);
    Task<Income?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default);
    Task InsertAsync(string username, Income income, CancellationToken cancellationToken = default);
    Task<Income?> UpdateAsync(string username, Guid id, IncomeDetails incomeDetails, CancellationToken cancellationToken = default);
    Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default);
}