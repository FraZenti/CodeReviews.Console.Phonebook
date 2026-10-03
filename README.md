# Phone Book

**C# Console Application | Entity Framework Core (Code-First) | SQL Server**

A console application for recording contacts and their phone numbers, built as part of the C# Academy learning path. This was the first project using Entity Framework Core instead of raw ADO.NET or Dapper, letting EF Core generate and manage the database schema directly from the C# model classes.

---

## How to Run

1. Clone the repository and open the `Phonebook` folder.
2. Open `appsettings.json` and update `ConnectionStrings:DefaultConnection` with your own SQL Server instance (and, optionally, `EmailSettings` with a Gmail address + App Password if you want to test the email feature — see "Sending Email" below).
3. From the project folder, create and apply the database schema:
   ```
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
4. Run the app:
   ```
   dotnet run
   ```
5. On first run, the app automatically seeds 50 sample contacts if the `Contacts` table is empty (see "Seeding" below).

---

## What the App Does / How It Works

From the main menu, the user can:

- **Add** a new contact (name, email, phone, and a category: Family, Friends, or Work)
- **Delete** a contact by name
- **Update** an existing contact's email, phone, or category
- **Show** the full list of contacts
- **Search** for one contact by name
- **Send an email** to a contact directly from the app

All input is validated before it ever reaches the database: names must contain only letters, emails and phone numbers are checked against regex patterns and the user is told the expected format and re-prompted until it's met. `Name` and `Email` are also enforced as unique at the database level (via EF Core's Fluent API), so no two contacts can share either.

### Seeding

Updated previous error: Now on startup, the app checks whether the `Contacts` table already has any rows (no need to comment out the code anymore):

```csharp
if (!context.Contacts.Any())
{
    await Filler.FillTable(50, context);
}
```

This means the seeding logic can stay active permanently. The first time the app runs against a fresh database it seeds 50 contacts with randomized names, emails, phone numbers, and categories; every run after that, since the table is no longer empty, it skips seeding entirely and goes straight to the menu.

### Sending Email

The email feature uses .NET's built-in `System.Net.Mail` (`SmtpClient` + `MailMessage`) through Gmail's SMTP server. Since Gmail no longer allows sign-in with a regular account password for apps like this, it requires a 16-character **App Password** generated from the Google Account's security settings (with 2-Step Verification enabled), stored in `appsettings.json` rather than hardcoded anywhere in the code.

---

## Architectural Choices

- **Entity Framework Core, Code-First** the database schema is generated entirely from the `Contact` model class and a `PhonebookContext : DbContext`, via `dotnet ef migrations`. No raw SQL is written anywhere in the project.
- **A separate design-time factory** (`PhonebookContextFactory : IDesignTimeDbContextFactory<PhonebookContext>`) exists purely so the `dotnet ef` CLI tooling can construct a context for itself at design time, independent of how the app constructs one when it actually runs.
- **Separation of concerns across dedicated static classes**, each with a single responsibility:
  - `Contact` / `Enums` the data model and the `Category`/`MenuActions` enums
  - `PhonebookContext` the EF Core database layer
  - `UserInput` reads and validates everything typed by the user, re-prompting until it's valid, and never lets invalid data escape into the rest of the app
  - `Validator` the actual regex rules for emails and phone numbers, kept separate from `UserInput` so it can be unit tested on its own
  - `DataCollector` every user-facing action (Add/Delete/Update/Show/Search/Email), each one talking to `PhonebookContext` directly
  - `Filler` seed-data generation, isolated from the rest of the app's logic
  - `Menu` the only place that loops, prompts for a top-level action, and routes to `DataCollector`
- **A single `try/catch` wrapped around the whole menu loop**, rather than scattered throughout, so an unexpected failure anywhere (a lost database connection, a failed email send) shows a message and returns to the menu instead of crashing the whole application.
- **Enums instead of raw strings** for both the category (`Family`/`Friends`/`Work`) and the menu actions, so invalid values are caught by the compiler rather than at runtime.

---

## Fixes From Review

Two issues were flagged during review and are both addressed in this submission:

1. **The contact list appeared to "disappear"/truncate right after printing.** The actual cause wasn't a data problem but he fact that `ShowContacts()` had no pause after printing the table, so the menu loop's next `Console.Clear()` wiped the screen almost immediately after the table was drawn, making it look cut off. Added a `Console.WriteLine("Press any key to return to the menu...")` + `Console.ReadKey()` right after the table is printed, matching the pattern already used in `AddContact`/`DeleteContact`/`UpdateContact`.
2. **Seeding required commenting the call out by hand to avoid re-seeding on every run.** Fixed by checking `context.Contacts.Any()` before calling `Filler.FillTable(...)`, so seeding only ever runs once, automatically, against a genuinely empty database.

---

## What I Learned

This was my first real project with Entity Framework Core, and a lot of it was about building the right mental model rather than just learning new syntax:

- A `DbContext` isn't a pre-filled list sitting in memory, `context.Contacts` is a live, queryable window directly onto the real table. Nothing is fetched until you actually write a query against it.
- Why the `dotnet ef` CLI tooling needs its own separate way to build a context (the design-time factory) it runs *before* the app itself ever starts, so it has no way to read `appsettings.json` through the app's own normal startup code.
- The difference between fetching an existing tracked entity and mutating it (for updates) versus accidentally calling `Add(...)` again on an object that already exists in the database. I hit this directly as a `UNIQUE constraint` violation while building `UpdateContact`, before realizing the fix was to fetch-then-mutate rather than insert a second time.
- Writing and reasoning through regex patterns from scratch for email and phone validation, including deliberately testing them against both valid and invalid inputs with `xUnit`'s `[Theory]`/`[InlineData]`, rather than just trusting that a pattern that *looked* right actually worked.
- A genuinely easy mistake to make with a startup seeding step: if it's not guarded by a check against the current state of the database, it either needs to be remembered and commented out manually every time (error-prone) or it silently keeps duplicating data. A one-line `Any()` check removes the need to remember anything at all.
