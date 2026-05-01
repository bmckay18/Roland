using Service.Transactions;
using Service.Transactions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleUserInterface.UserInterface
{
    public class App
    {
        private ITransactionsService _service;

        public App(ITransactionsService service)
        {
            _service = service;
        }
        public void Run()
        {
            Console.ReadKey();
        }
    }
}
