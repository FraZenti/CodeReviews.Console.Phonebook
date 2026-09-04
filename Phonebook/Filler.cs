namespace Phonebook;

public class Filler
{
    public static async Task FillTable(int numberOfContacts, PhonebookContext context)
    {
        var random = new Random();
        string[] names = new string[]
        {
        "Emil", "John", "Jane", "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace",
        "Henry", "Ivy", "Jack", "Karen", "Leo", "Mia", "Noah", "Olivia", "Paul", "Quinn",
        "Rachel", "Sam", "Tina", "Uma", "Victor", "Wendy", "Xander", "Yara", "Zane", "Amy",
        "Ben", "Clara", "Derek", "Ella", "Felix", "Gina", "Hugo", "Iris", "Jake", "Kate",
        "Liam", "Nora", "Oscar", "Pia", "Ryan", "Sara", "Tom", "Vera", "Will", "Zoe"
        };

        for (int i = 0; i < numberOfContacts; i++)
        {
            string name = names[i];

            string email = $"{name.ToLower()}@example.com";

            string phone = $"{random.Next(100, 1000)}-{random.Next(100, 1000)}-{random.Next(1000, 10000)}";

            Category[] categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToArray();
            Category category = categories[random.Next(0, categories.Length)];

            context.Add(new Contact(name, email, phone, category));
        }

        await context.SaveChangesAsync();
    }
}