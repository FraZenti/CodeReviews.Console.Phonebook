using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Phonebook;

public class PhonebookContextFactory : IDesignTimeDbContextFactory<PhonebookContext>
{
    public PhonebookContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            return new PhonebookContext(config.GetConnectionString("DefaultConnection")!);
    }
}