using ConsoleUserInterface.UserInterface.Interfaces;
using Service.Transactions;

namespace ConsoleUserInterface
{
    public class App
    {
        private IUIController _uiController;

        public App(IUIController uiController)
        {
            _uiController = uiController;
        }
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            await _uiController.StartAsync(cancellationToken);
        }
    }
}
