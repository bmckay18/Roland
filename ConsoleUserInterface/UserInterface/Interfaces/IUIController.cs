using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUserInterface.UserInterface.Interfaces
{
    public interface IUIController
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
