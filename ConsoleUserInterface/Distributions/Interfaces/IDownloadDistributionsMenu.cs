namespace ConsoleUserInterface.Distributions.Interfaces
{
    public interface IDownloadDistributionsMenu
    {
        Task DownloadDistributionsCsv(CancellationToken cancellationToken);
    }
}