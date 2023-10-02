using ExpensesApi.Repositories;

namespace ExpensesApi.Registries;

public class FilterFactory : IFilterFactory
{
    public IFilter Create(FilterParameters? parameters)
    {
        var filter = new Filter();

        if (parameters is not null)
        {
            AddCategoryFilter(parameters, filter);
            AddDateFilters(parameters, filter);
        }

        return filter;
    }

    #region Utility Methods

    private static void AddDateFilters(FilterParameters parameters, IFilter filter)
    {
        if (!string.IsNullOrWhiteSpace(parameters.From))
        {
            if (!string.IsNullOrWhiteSpace(parameters.To))
            {
                filter.Between(parameters.From, parameters.To);
            }
            else
            {
                filter.From(parameters.From);
            }
        }

        if (!string.IsNullOrWhiteSpace(parameters.In))
        {
            filter.In(parameters.In);
        }
    }

    private static void AddCategoryFilter(FilterParameters parameters, IFilter filter)
    {
        if (parameters.Category is not null)
        {
            filter.WithCategory(parameters.Category.Value);
        }
    }

    #endregion
}