using Microsoft.EntityFrameworkCore;

namespace Phonebook;

public class PhonebookContext : DbContext
{
    public DbSet<Contact> Contacts { get; set; }

    private readonly string _connectionString;

    public PhonebookContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseSqlServer(_connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Name)
            .IsUnique();

        modelBuilder.Entity<Contact>()
            .HasIndex(c => c.Email)
            .IsUnique();
    }
}