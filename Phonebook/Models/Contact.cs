namespace Phonebook;

public class Contact
{
    public int Id { get; set;}
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Category Relation { get; set; }

    public Contact(string name, string email, string phone, Category relation)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Relation = relation;
    }
}