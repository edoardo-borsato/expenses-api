using System.Diagnostics;
using ExpensesApi.Models;
using ExpensesApi.Repositories;
using ExpensesApi.Utility;

namespace ExpensesApi.Registries;

public class ExpensesRegistry : IExpensesRegistry
{
    #region Private fields

    private readonly ILogger<ExpensesRegistry> _logger;
    private readonly IExpensesRepository _repository;
    private readonly IFilterFactory _filterFactory;
    private readonly IWatch _watch;

    #endregion

    #region Initialization

    public ExpensesRegistry(ILoggerFactory loggerFactory, IExpensesRepository repository, IFilterFactory filterFactory, IWatch watch)
    {
        _logger = loggerFactory is not null ? loggerFactory.CreateLogger<ExpensesRegistry>() : throw new ArgumentNullException(nameof(loggerFactory));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _filterFactory = filterFactory ?? throw new ArgumentNullException(nameof(filterFactory));
        _watch = watch ?? throw new ArgumentNullException(nameof(watch));
    }

    #endregion

    public async Task<List<Expense?>> GetAllAsync(string username, FilterParameters? filterParameters, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(GetAllAsync)} invoked");
        var sw = Stopwatch.StartNew();

        var filter = _filterFactory.Create(filterParameters);

        var expenses = (await _repository.GetAllAsync(username, filter, cancellationToken)).ToList();

        _logger.LogDebug($"{nameof(GetAllAsync)} completed. Expenses Count: {expenses.Count}. Elapsed: {sw.Elapsed}");

        return expenses;
    }

    public async Task<Expense?> GetAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(GetAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        var expense = await _repository.GetAsync(username, id, cancellationToken);

        _logger.LogDebug($"{nameof(GetAsync)} completed. Username: {username}; ID: {id}. Elapsed: {sw.Elapsed}");

        return expense;
    }

    public async Task<Expense> InsertAsync(string username, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(InsertAsync)} invoked. Username: {username}");
        var sw = Stopwatch.StartNew();

        ValidateDetails(expenseDetails);

        var newGuid = Guid.NewGuid();
        var newExpense = new Expense
        {
            Id = newGuid,
            ExpenseDetails = expenseDetails with { Date = expenseDetails.Date ?? _watch.Now(), Category = expenseDetails.Category ?? Category.Others }
        };

        await _repository.InsertAsync(username, newExpense, cancellationToken);

        _logger.LogDebug($"{nameof(InsertAsync)} completed. Username: {username}; ID: {newGuid}. Elapsed: {sw.Elapsed}");

        return newExpense;
    }

    public async Task<Expense?> UpdateAsync(string username, Guid id, ExpenseDetails expenseDetails, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(UpdateAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        ValidateDetails(expenseDetails);

        var updatedExpense = await _repository.UpdateAsync(username, id, expenseDetails, cancellationToken);

        _logger.LogDebug($"{nameof(UpdateAsync)} completed. Username:  {username} ; ID: {id}. Elapsed: {sw.Elapsed}");

        return updatedExpense;
    }

    public async Task DeleteAsync(string username, Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(DeleteAsync)} invoked. Username: {username}; ID: {id}");
        var sw = Stopwatch.StartNew();

        await _repository.DeleteAsync(username, id, cancellationToken);

        _logger.LogDebug($"{nameof(DeleteAsync)} completed. Username: {username}; ID: {id}. Elapsed: {sw.Elapsed}");
    }

    #region Utility Methods

    private static void ValidateDetails(ExpenseDetails expenseDetails)
    {
        if (expenseDetails.Value < 0)
        {
            throw new ArgumentException(nameof(expenseDetails.Value));
        }
    }

    #endregion
}