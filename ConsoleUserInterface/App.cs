using ConsoleUserInterface.UserInterface.Interfaces;

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
            try
            {
                await _uiController.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred. Error: {ex.Message}");
                Console.WriteLine("Press any key to close the application.");
                Console.ReadKey();
            }
        }
    }
}
