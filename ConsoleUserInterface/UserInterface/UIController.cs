using ConsoleUserInterface.Assets.Interfaces;
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

        public UIController(IStartMenu startMenu, ITransactionsMenu transactionsMenu, IAssetsMenu assetsMenu)
        {
            _startMenu = startMenu;
            _transactionsMenu = transactionsMenu;
            _assetsMenu = assetsMenu;
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
                    case (int)StartMenuOptionTypes.Exit:
                        isRunning = false;
                        break;
                }
            }
        }
    }
}
