using ExpensesApi.Models;

namespace ExpensesApi.Registries;

public interface IIncomesRegistry
{
    Task<List<Income?>> GetAllAsync(string username, FilterParameters? filterParameters, CancellationToken cancellationToken = default);
    Task<Income?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default);
    Task<Income> InsertAsync(string username, IncomeDetails incomeDetails, CancellationToken cancellationToken = default);
    Task<Income?> UpdateAsync(string username, Guid id, IncomeDetails incomeDetails, CancellationToken cancellationToken = default);
    Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default);
}