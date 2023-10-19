using ExpensesApi.Repositories.Entities;
using Category = ExpensesApi.Models.Category;

namespace ExpensesApi.Repositories;

public class Filter : IFilter
{
    public Filter()
    {
        ResetValues();
    }

    private Category? _category;
    private string? _from;
    private string? _in;
    private Tuple<string, string>? _between;

    public IFilter From(string startDate)
    {
        _from = startDate;
        return this;
    }

    public IFilter In(string date)
    {
        _in = date;
        return this;
    }

    public IFilter Between(string startDate, string endDate)
    {
        _between = new Tuple<string, string>(startDate, endDate);
        return this;
    }

    public IFilter WithCategory(Category category)
    {
        _category = category;
        return this;
    }

    public IEnumerable<ExpenseEntity> Apply(IEnumerable<ExpenseEntity> items)
    {
        var expenseEntities = items.ToList();
        if (_from is not null)
        {
            expenseEntities = expenseEntities.Where(i => string.Compare(i.Date, _from, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        if (_in is not null)
        {
            expenseEntities = expenseEntities.Where(i => i.Date!.StartsWith(_in)).ToList();
        }

        if (_between is not null)
        {
            expenseEntities = expenseEntities.Where(i => 
                    string.Compare(i.Date, _between.Item1, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    string.Compare(i.Date, _between.Item2, StringComparison.OrdinalIgnoreCase) <= 0)
                .ToList();
        }

        if (_category is not null)
        {
            expenseEntities = expenseEntities.Where(i => i.Category == ToEntityCategory(_category.Value)).ToList();
        }

        ResetValues();

        return expenseEntities;
    }

    public IEnumerable<IncomeEntity> Apply(IEnumerable<IncomeEntity> items)
    {
        var incomeEntities = items.ToList();
        if (_from is not null)
        {
            incomeEntities = incomeEntities.Where(i => string.Compare(i.Date, _from, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        }

        if (_in is not null)
        {
            incomeEntities = incomeEntities.Where(i => i.Date!.StartsWith(_in)).ToList();
        }

        if (_between is not null)
        {
            incomeEntities = incomeEntities.Where(i =>
                    string.Compare(i.Date, _between.Item1, StringComparison.OrdinalIgnoreCase) >= 0 &&
                    string.Compare(i.Date, _between.Item2, StringComparison.OrdinalIgnoreCase) <= 0)
                .ToList();
        }

        ResetValues();

        return incomeEntities;
    }

    #region Utility Methods

    private void ResetValues()
    {
        _from = null;
        _in = null;
        _between = null;
        _category = null;
    }

    private static Entities.Category ToEntityCategory(Category category)
    {
        return category switch
        {
            Category.HealthAndPersonalCare => Entities.Category.HealthAndPersonalCare,
            Category.Sport => Entities.Category.Sport,
            Category.HousingAndSupplies => Entities.Category.HousingAndSupplies,
            Category.Transportation => Entities.Category.Transportation,
            Category.Clothing => Entities.Category.Clothing,
            Category.Entertainment => Entities.Category.Entertainment,
            Category.BillsAndUtilities => Entities.Category.BillsAndUtilities,
            Category.Pets => Entities.Category.Pets,
            Category.Insurance => Entities.Category.Insurance,
            Category.Gifts => Entities.Category.Gifts,
            Category.Others => Entities.Category.Others,
            _ => Entities.Category.Others
        };
    }

    #endregion
}