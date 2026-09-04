namespace Phonebook;

public class Validator
{
    public bool IsValidPhoneNumber(string phoneNumber)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^[0-9]{3}-[0-9]{3}-[0-9]{4}$");
    }

    public bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@ ]+@[^@ ]+\.[^@ ]+$");
    }
}