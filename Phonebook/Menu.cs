using Spectre.Console;

namespace Phonebook;

internal class Menu
{
    internal static async Task ShowMenu(PhonebookContext context, string senderEmail, string appPassword)
    {
        bool wantToContinue = true;
        while (wantToContinue)
        {
            Console.Clear();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuActions>()
                    .Title("Select what to do next: \n")
                    .AddChoices(Enum.GetValues<MenuActions>())
            );

            try
            {
                 switch (choice)
            {
                case MenuActions.AddContact:
                    await DataCollector.AddContact(context);
                    break;
                case MenuActions.DeleteContact:
                    await DataCollector.DeleteContact(context);
                    break;
                case MenuActions.UpdateContact:
                    await DataCollector.UpdateContact(context);
                    break;
                case MenuActions.ShowContacts:
                    DataCollector.ShowContacts(context);
                    break;
                case MenuActions.SearchContact:
                    await DataCollector.SearchContact(context);
                    break;
                case MenuActions.SendEmailAsync:
                    await DataCollector.SendEmailAsync(context, senderEmail, appPassword);
                    break;
                case MenuActions.Exit:
                    wantToContinue = false;
                    break;
            }       
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine("Press any key to return to the menu...");
                Console.ReadKey();
            }
        }
    }
}