using ExpensesApi.Repositories.Entities;
using PaymentMethod = ExpensesApi.Models.PaymentMethod;

namespace ExpensesApi.Repositories;

public interface IFilter
{
    IFilter From(string startDate);
    IFilter In(string date);
    IFilter Between(string startDate, string endDate);
    IFilter WithPaymentMethod(PaymentMethod paymentMethod);
    IEnumerable<ExpenseEntity> Apply(IEnumerable<ExpenseEntity> items);
}