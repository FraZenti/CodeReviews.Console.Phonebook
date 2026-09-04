namespace Phonebook;

internal class UserInput
{
    internal string GetName(string message)
    {
        Console.WriteLine(message);
        string? name = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Name cannot be empty. Please enter a valid name:");
            name = Console.ReadLine();
        }

        return name!;
    }

    internal string GetEmail(string message)
    {
        var validator = new Validator();

        Console.WriteLine(message);
        string? email = Console.ReadLine();

        while(string.IsNullOrWhiteSpace(email) || !validator.IsValidEmail(email))
        {
            Console.WriteLine("Invalid email format. Please enter a valid email:");
            email = Console.ReadLine();
        }

        return email!;
    }

    internal string GetPhone(string message)
    {
        var validator = new Validator();
        
        Console.WriteLine(message);
        string? phone = Console.ReadLine();

        while(string.IsNullOrWhiteSpace(phone) || !validator.IsValidPhoneNumber(phone))
        {
            Console.WriteLine("Invalid phone number format. Please enter a valid phone number (e.g., 123-456-7890):");
            phone = Console.ReadLine();
        }

        return phone!;
    }
}