using ConsoleUserInterface;
using ConsoleUserInterface.Assets;
using ConsoleUserInterface.Assets.Interfaces;
using ConsoleUserInterface.Helper;
using ConsoleUserInterface.Transactions;
using ConsoleUserInterface.Transactions.Interfaces;
using ConsoleUserInterface.UserInterface;
using ConsoleUserInterface.UserInterface.Interfaces;
using Data;
using Microsoft.Data.Sqlite;
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

var sqliteBuilder = new SqliteConnectionStringBuilder(dbPath);
var filepath = sqliteBuilder.DataSource;

var directory = Path.GetDirectoryName(filepath);
if (!Directory.Exists(directory)) Directory.CreateDirectory(directory!);

builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(dbPath));

// Setup services
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddScoped<IAssetsService, AssetsService>();
builder.Services.AddScoped<IDistributionsService, DistributionsService>();

// Setup UI screens
builder.Services.AddTransient<IUIController, UIController>();
builder.Services.AddTransient<IStartMenu, StartMenu>();
builder.Services.AddTransient<ITransactionsMenu, TransactionsMenu>();
builder.Services.AddTransient<ICreateTransactionMenu, CreateTransactionMenu>();
builder.Services.AddTransient<IDownloadTransactionsMenu, DownloadTransactionsMenu>();
builder.Services.AddTransient<IAssetsMenu, AssetsMenu>();
builder.Services.AddTransient<IAddAssetMenu, AddAssetMenu>();
builder.Services.AddTransient<IViewAssetsMenu, ViewAssetsMenu>();
builder.Services.AddTransient<IAssetRetriever, AssetRetriever>();

builder.Services.AddTransient<App>();

var host = builder.Build();

// Apply Migrations
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
}

// Run App
using (var scope = host.Services.CreateScope())
{
    var app = host.Services.GetRequiredService<App>();

    var cancellationToken = new CancellationTokenSource();

    await app.RunAsync(cancellationToken.Token);
}