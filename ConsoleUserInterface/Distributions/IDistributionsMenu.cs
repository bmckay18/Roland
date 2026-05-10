namespace ConsoleUserInterface.Distributions
{
    public interface IDistributionsMenu
    {
        Task ShowDistributionsMenu(CancellationToken cancellationToken);
    }
}