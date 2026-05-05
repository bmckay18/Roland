using ConsoleUserInterface.UserInterface.Interfaces;

namespace ConsoleUserInterface.UserInterface
{
    public class UIController : IUIController
    {
        private readonly IStartMenu _startMenu;

        public UIController(IStartMenu startMenu)
        {
            _startMenu = startMenu;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var menuOption = _startMenu.ShowStartMenu();
        }
    }
}
