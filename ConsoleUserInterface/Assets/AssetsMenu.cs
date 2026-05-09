using ConsoleUserInterface.Assets.Interfaces;
using ConsoleUserInterface.Assets.Models;
using ConsoleUserInterface.Helper;

namespace ConsoleUserInterface.Assets
{
    public class AssetsMenu : IAssetsMenu
    {
        private readonly IAddAssetMenu _addAssetMenu;
        private readonly IViewAssetsMenu _viewAssetsMenu;

        private readonly Dictionary<int, string> _menuOptions = new()
        {
            { (int)AssetMenuOptions.AddAsset, "Add New Asset" },
            { (int)AssetMenuOptions.ViewAssets, "View Assets" },
            { (int)AssetMenuOptions.PreviousPage, "Go To Previous Page" }
        };

        public AssetsMenu(IAddAssetMenu addAssetMenu, IViewAssetsMenu viewAssetsMenu)
        {
            _addAssetMenu = addAssetMenu;
            _viewAssetsMenu = viewAssetsMenu;
        }

        public async Task ShowAssetsMenu(CancellationToken cancellationToken)
        {
            while (true)
            {
                Console.WriteLine("What would you like to do?");
                UIHelper.DisplayMenuOptions(_menuOptions);
                var userInput = Console.ReadLine();

                var parsedInput = UIHelper.ParseAndValidateUserInput(userInput, _menuOptions);

                if (!parsedInput.IsValidInt)
                {
                    Console.WriteLine("That is not a valid option.");
                    continue;
                }

                switch (parsedInput.UserOption)
                {
                    case (int)AssetMenuOptions.AddAsset:
                        await _addAssetMenu.ShowAddAssetMenu(cancellationToken);
                        break;
                    case (int)AssetMenuOptions.ViewAssets:
                        await _viewAssetsMenu.ShowViewAssetsMenu(cancellationToken);
                        break;
                    case (int)AssetMenuOptions.PreviousPage:
                        return;
                }
            }
        }
    }
}
