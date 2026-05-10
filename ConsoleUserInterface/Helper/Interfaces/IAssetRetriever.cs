namespace ConsoleUserInterface.Helper.Interfaces
{
    public interface IAssetRetriever
    {
        Task<int?> ShowAndGetAssetId(CancellationToken cancellationToken);
    }
}