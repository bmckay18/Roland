namespace ConsoleUserInterface.Helper
{
    public interface IAssetRetriever
    {
        Task<int?> ShowAndGetAssetId(CancellationToken cancellationToken);
    }
}