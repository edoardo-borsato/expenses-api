using ExpensesApi.Repositories.Entities;
using Category = ExpensesApi.Models.Category;

namespace ExpensesApi.Repositories;

public interface IFilter
{
    IFilter From(string startDate);
    IFilter In(string date);
    IFilter Between(string startDate, string endDate);
    IFilter WithCategory(Category category);
    IEnumerable<ExpenseEntity> Apply(IEnumerable<ExpenseEntity> items);
}