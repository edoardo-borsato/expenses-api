using ExpensesApi.Registries;

namespace ExpensesApi.Controllers;

public interface IQueryParametersValidator
{
    FilterParameters? Validate(ExpensesGetAllQueryParameters? queryParameters);
    FilterParameters? Validate(IncomesGetAllQueryParameters? queryParameters);
}