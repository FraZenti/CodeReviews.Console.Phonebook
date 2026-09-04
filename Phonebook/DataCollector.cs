namespace Phonebook;

using System.Net;
using System.Net.Mail;
using Spectre.Console;

internal static class DataCollector
{
    internal static async Task AddContact(PhonebookContext context)
    {
        AnsiConsole.MarkupLine("[green]Add Contact: [/]");

        var userInput = new UserInput();

        string name = userInput.GetName("Enter the contact's name:");

        string email = userInput.GetEmail("Enter the contact's email:");

        string phone = userInput.GetPhone("Enter the contact's phone number:");

        var relation = AnsiConsole.Prompt(
            new SelectionPrompt<Category>()
            .Title("Select the contact's relation:")
            .AddChoices(Enum.GetValues<Category>())
        );

        context.Add(new Contact(name, email, phone, relation));

        await context.SaveChangesAsync();

        Console.WriteLine("Contact added successfully! Press any key to return to the menu...");
        Console.ReadKey();
    }

    internal static async Task DeleteContact(PhonebookContext context)
    {
        AnsiConsole.MarkupLine("[red]Delete Contact[/]");
        ShowContacts(context);

        var userInput = new UserInput();
        string name = userInput.GetName("Enter the contact's name:");

        var contactToDelete = context.Contacts.FirstOrDefault(c => c.Name!.ToLower() == name.ToLower());

        if (contactToDelete != null)
        {
            context.Contacts.Remove(contactToDelete);
            await context.SaveChangesAsync();
            Console.WriteLine("Contact deleted successfully! Press any key to return to the menu...");

        }
        else
        {
            Console.WriteLine("Contact not found! Press any key to return to the menu...");
        }
        Console.ReadKey();
    }

    internal static async Task UpdateContact(PhonebookContext context)
    {
        AnsiConsole.MarkupLine("[yellow]Update Contact[/]");
        ShowContacts(context);

        var userInput = new UserInput();
        string name = userInput.GetName("Enter the contact's name:");

        var contactToChange = context.Contacts.FirstOrDefault(c => c.Name!.ToLower() == name.ToLower());
        if (contactToChange != null)
        {
            string email = userInput.GetEmail("Enter the new contact's email:");

            string phone = userInput.GetPhone("Enter the new contact's phone number:");

            var relation = AnsiConsole.Prompt(
                new SelectionPrompt<Category>()
                .Title("Select the contact's relation:")
                .AddChoices(Enum.GetValues<Category>())
            );

            contactToChange.Email = email;
            contactToChange.Phone = phone;
            contactToChange.Relation = relation;

            await context.SaveChangesAsync();

            Console.WriteLine("Contact updated successfully! Press any key to return to the menu...");
        }
        else
        {
            Console.WriteLine("Contact not found! Press any key to return to the menu...");
        }
        Console.ReadKey();
    }

    internal static void ShowContacts(PhonebookContext context)
    {
        AnsiConsole.MarkupLine("[blue]List of Contacts: [/]");
        ShowTable(context.Contacts.ToList());
    }

    internal static async Task<Contact?> SearchContact(PhonebookContext context)
    {
        AnsiConsole.MarkupLine("[purple]Search Contact[/]");

        ShowContacts(context);

        var userInput = new UserInput();

        string name = userInput.GetName("Enter the contact's name:");

        var contactToSearch = context.Contacts.FirstOrDefault(c => c.Name!.ToLower() == name.ToLower());
        List<Contact> contacts = new List<Contact>();

        if (contactToSearch != null)
        {
            contacts.Add(contactToSearch!);
            ShowTable(contacts);
            Console.WriteLine("Contact found! Press any key to return to the menu...");
            Console.ReadKey();
            return contactToSearch;
        }
        else
        {
            Console.WriteLine("Contact not found! Press any key to return to the menu...");
            Console.ReadKey();
            return null!;
        }
    }

    internal static async Task SendEmailAsync(PhonebookContext context, string senderEmail, string appPassword)
    {
        Contact? contact = await SearchContact(context);
        if (contact == null)
        {
            return;
        }

        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential(senderEmail, appPassword),
            EnableSsl = true
        };

        var subject = AnsiConsole.Ask<string>("Enter the subject of the email: ");
        var body = AnsiConsole.Ask<string>("Enter the body of the email: ");

        var message = new MailMessage(senderEmail, contact.Email!, subject, body);

        try
        {
            await client.SendMailAsync(message);
            Console.WriteLine("Email sent successfully! Press any key to return to the menu...");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey();
    }

    internal static void ShowTable(List<Contact> contacts)
    {
        var table = new Table();
        table.Border(TableBorder.Rounded);

        var properties = typeof(Contact).GetProperties();

        foreach (var property in properties)
        {
            table.AddColumn(property.Name);
        }

        foreach (var contact in contacts)
        {
            var rowValues = properties.Select(p => p.GetValue(contact)?.ToString() ?? "").ToArray();
            table.AddRow(rowValues);
        }
        AnsiConsole.Write(table);
    }
}