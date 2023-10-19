using ExpensesApi.Models;
using ExpensesApi.Repositories;
using ExpensesApi.Utility;
using System.Diagnostics;

namespace ExpensesApi.Registries;

public class IncomesRegistry : IIncomesRegistry
{
    #region Private fields

    private readonly ILogger<IncomesRegistry> _logger;
    private readonly IIncomesRepository _repository;
    private readonly IFilterFactory _filterFactory;
    private readonly IWatch _watch;

    #endregion

    #region Initialization

    public IncomesRegistry(ILoggerFactory loggerFactory, IIncomesRepository repository, IFilterFactory filterFactory, IWatch watch)
    {
        _logger = loggerFactory is not null ? loggerFactory.CreateLogger<IncomesRegistry>() : throw new ArgumentNullException(nameof(loggerFactory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _filterFactory = filterFactory ?? throw new ArgumentNullException(nameof(filterFactory));
        _watch = watch ?? throw new ArgumentNullException(nameof(watch));
    }

    #endregion

    public async Task<List<Income?>> GetAllAsync(string username, FilterParameters? filterParameters, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(GetAllAsync)} invoked");
        var sw = Stopwatch.StartNew();

        var filter = _filterFactory.Create(filterParameters);

        var incomes = (await _repository.GetAllAsync(username, filter, cancellationToken)).ToList();

        _logger.LogDebug($"{nameof(GetAllAsync)} completed. Expenses Count: {incomes.Count}. Elapsed: {sw.Elapsed}");

        return incomes;
    }

    public async Task<Income?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(GetAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        var income = await _repository.GetAsync(username, id, cancellationToken);

        _logger.LogDebug($"{nameof(GetAsync)} completed. Username: {username}; ID: {id}. Elapsed: {sw.Elapsed}");

        return income;
    }

    public async Task<Income> InsertAsync(string username, IncomeDetails incomeDetails, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(InsertAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        ValidateDetails(incomeDetails);

        var newGuid = Guid.NewGuid();
        var newIncome = new Income
        {
            Id = newGuid,
            IncomeDetails = incomeDetails with { Date = incomeDetails.Date ?? _watch.Now() }
        };

        await _repository.InsertAsync(username, newIncome, cancellationToken);

        _logger.LogDebug($"{nameof(InsertAsync)} completed. Username: {username}; ID: {newGuid}. Elapsed: {sw.Elapsed}");

        return newIncome;
    }

    public async Task<Income?> UpdateAsync(string username, Guid id, IncomeDetails incomeDetails, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(UpdateAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        ValidateDetails(incomeDetails);

        var updatedIncome = await _repository.UpdateAsync(username, id, incomeDetails, cancellationToken);

        _logger.LogDebug($"{nameof(UpdateAsync)} completed. Username:  {username} ; ID: {id}. Elapsed: {sw.Elapsed}");

        return updatedIncome;
    }

    public async Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(DeleteAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        await _repository.DeleteAsync(username, id, cancellationToken);

        _logger.LogDebug($"{nameof(DeleteAsync)} completed. Username: {username}; ID: {id}. Elapsed: {sw.Elapsed}");
    }

    #region Utility Methods

    private static void ValidateDetails(IncomeDetails incomeDetails)
    {
        if (incomeDetails.Value < 0)
        {
            throw new ArgumentException(nameof(incomeDetails.Value));
        }
    }

    #endregion
}