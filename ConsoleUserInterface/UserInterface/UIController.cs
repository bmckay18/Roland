using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.UserInterface.Interfaces;
using ConsoleUserInterface.UserInterface.Models;

namespace ConsoleUserInterface.UserInterface
{
    public class UIController : IUIController
    {
        private readonly IStartMenu _startMenu;
        private readonly ITransactionsMenu _transactionsMenu;

        public UIController(IStartMenu startMenu, ITransactionsMenu transactionsMenu)
        {
            _startMenu = startMenu;
            _transactionsMenu = transactionsMenu;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var menuOption = _startMenu.ShowStartMenu();
            
            switch (menuOption)
            {
                case (int)StartMenuOptionTypes.Transactions:
                    await _transactionsMenu.ShowTransactionsMenu(cancellationToken);
                    break;
            }

        }
    }
}
