namespace ConsoleUserInterface.Common
{
    public interface IDownloadCsvService
    {
        Task DownloadCsvAsync(CancellationToken cancellationToken);
    }
}