namespace ExpensesApi.Utility;

public class Watch : IWatch
{
    public DateTimeOffset Now()
    {
        return DateTimeOffset.UtcNow;
    }
}