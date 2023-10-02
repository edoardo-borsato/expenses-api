using ExpensesApi.Registries;

namespace ExpensesApi.Controllers;

public interface IQueryParametersValidator
{
    FilterParameters? Validate(GetAllQueryParameters? queryParameters);
}