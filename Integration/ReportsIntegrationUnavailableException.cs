namespace Pennywise.DAL.Integration;

public sealed class ReportsIntegrationUnavailableException : Exception
{
    public ReportsIntegrationUnavailableException()
        : base("The transaction data source has not been configured. Reports integration is not available yet.")
    {
    }
}
