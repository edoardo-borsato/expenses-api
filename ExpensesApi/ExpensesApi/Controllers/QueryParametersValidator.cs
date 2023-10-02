using System.Globalization;
using ExpensesApi.Registries;

namespace ExpensesApi.Controllers;

public class QueryParametersValidator : IQueryParametersValidator
{
    public FilterParameters? Validate(GetAllQueryParameters? queryParameters)
    {
        if (queryParameters is null)
        {
            return null;
        }

        var filterParameters = new FilterParameters();

        if (queryParameters.From is not null)
        {
            ValidateDate(queryParameters.From);
            filterParameters.From = queryParameters.From;
        }

        if (queryParameters.To is not null)
        {
            ValidateDate(queryParameters.To);
            filterParameters.To = queryParameters.To;
        }

        if (queryParameters.In is not null)
        {
            ValidateDate(queryParameters.In);
            filterParameters.In = queryParameters.In;
        }

        if (queryParameters.Category is not null)
        {
            filterParameters.Category = queryParameters.Category;
        }

        return filterParameters;
    }

    #region Utility Methods

    private static void ValidateDate(string date)
    {
        try
        {
            DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            try
            {
                DateTimeOffset.ParseExact(date, "yyyy-MM", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                DateTimeOffset.ParseExact(date, "yyyy", CultureInfo.InvariantCulture);
            }
        }
    }

    #endregion
}