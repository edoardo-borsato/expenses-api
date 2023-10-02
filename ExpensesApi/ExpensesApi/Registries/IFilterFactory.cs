using ExpensesApi.Repositories;

namespace ExpensesApi.Registries;

public interface IFilterFactory
{
    IFilter Create(FilterParameters? parameters);
}