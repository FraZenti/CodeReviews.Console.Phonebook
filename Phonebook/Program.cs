using Microsoft.Extensions.Configuration;
using Phonebook;

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string connectionString = config.GetConnectionString("DefaultConnection")!;
string senderEmail = config.GetSection("EmailSettings")["SenderEmail"]!;
string appPassword = config.GetSection("EmailSettings")["AppPassword"]!;

var context = new PhonebookContext(connectionString);

//await Filler.FillTable(50, context);

await Menu.ShowMenu(context, senderEmail, appPassword);