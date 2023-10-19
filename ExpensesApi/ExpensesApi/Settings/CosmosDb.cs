namespace ExpensesApi.Settings;

public record CosmosDb
{
    public string DatabaseName { get; init; }
    public string ExpensesContainerName { get; init; }
    public string IncomesContainerName { get; init; }
    public string AccountEndpoint { get; init; }
    public string Key { get; init; }
}