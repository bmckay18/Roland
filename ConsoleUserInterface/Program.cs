using ConsoleUserInterface;
using ConsoleUserInterface.UserInterface;
using ConsoleUserInterface.UserInterface.Interfaces;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Assets;
using Service.Distributions;
using Service.Transactions;

var builder = Host.CreateApplicationBuilder(args);

// Setup database context
var dbPath = builder.Configuration.GetConnectionString("Default");

var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

dbPath = dbPath!.Replace("|DataDirectory|", appDataFolder);

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(dbPath));

// Setup services
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IDistributionsService, DistributionsService>();

// Setup UI screens
builder.Services.AddTransient<IUIController, UIController>();
builder.Services.AddTransient<IStartMenu>(sp =>
{
    return new StartMenu(new[]
    {
        "Assets",
        "Transactions"
    });
});

builder.Services.AddTransient<App>();

var host = builder.Build();

var app = host.Services.GetRequiredService<App>();

var cancellationToken = new CancellationTokenSource();

await app.RunAsync(cancellationToken.Token);