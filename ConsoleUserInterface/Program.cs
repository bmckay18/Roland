using ConsoleUserInterface.UserInterface;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Service.Transactions;

var builder = Host.CreateApplicationBuilder(args);

// Setup database context
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Setup services
builder.Services.AddScoped<ITransactionsService, TransactionsService>();
builder.Services.AddTransient<App>();

var host = builder.Build();

using var scope = host.Services.CreateScope();
var app = scope.ServiceProvider.GetRequiredService<App>();
app.Run();