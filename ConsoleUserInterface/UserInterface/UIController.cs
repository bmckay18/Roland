using ConsoleUserInterface.Assets.Interfaces;
using ConsoleUserInterface.Distributions.Interfaces;
using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.UserInterface.Interfaces;
using ConsoleUserInterface.UserInterface.Models;

namespace ConsoleUserInterface.UserInterface
{
    public class UIController : IUIController
    {
        private readonly IStartMenu _startMenu;
        private readonly ITransactionsMenu _transactionsMenu;
        private readonly IAssetsMenu _assetsMenu;
        private readonly IDistributionsMenu _distributionsMenu;

        public UIController(IStartMenu startMenu, ITransactionsMenu transactionsMenu, IAssetsMenu assetsMenu, IDistributionsMenu distributionsMenu)
        {
            _startMenu = startMenu;
            _transactionsMenu = transactionsMenu;
            _assetsMenu = assetsMenu;
            _distributionsMenu = distributionsMenu;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var isRunning = true;
            while (isRunning)
            {
                var menuOption = _startMenu.ShowStartMenu();

                switch (menuOption)
                {
                    case (int)StartMenuOptionTypes.Transactions:
                        await _transactionsMenu.ShowTransactionsMenu(cancellationToken);
                        break;
                    case (int)StartMenuOptionTypes.Assets:
                        await _assetsMenu.ShowAssetsMenu(cancellationToken);
                        break;
                    case (int)StartMenuOptionTypes.Distributions:
                        await _distributionsMenu.ShowDistributionsMenu(cancellationToken);
                        break;
                    case (int)StartMenuOptionTypes.Exit:
                        isRunning = false;
                        break;
                }
            }
        }
    }
}
