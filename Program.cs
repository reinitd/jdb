using Newtonsoft.Json;
using JDB;
using System.Data;
using System.Reflection;


namespace Program;

public class Program
{
    public static async Task Main()
    {
        Jatabase db = new("db/test.json");

        List<string> res = await db.GetTablesAsync();
        Console.WriteLine($"Tables: {string.Join("", res)}");

        Table? table = await db.GetTableAsync("default");
        if (table == null) { return; }
        Console.WriteLine($"Table Created At: {table.CreatedAt}");

        Dictionary<string, object>? data = table.Data ?? ([]);

        try
        {
            // Default dictionary actions make manipulating data easy and simple.
            // No need to learn new methods to do the things you want.
            data.Add("user", "admin");
            data.Add("dob", "1/1/1970");
            // Alternatively:
            data["email"] = "administrator@example.com";
        }
        catch (ArgumentException)
        {
            data.Remove("user");
            data.Remove("dob");
        }

        bool updateRes = await db.UpdateTableAsync("default", data);
        Console.WriteLine($"Updated: {updateRes}");


        bool createTableRes = true;
        try
        {
            createTableRes = await db.CreateTableAsync("posts");
            Console.WriteLine($"Created Table: {createTableRes}");
        }
        catch (ArgumentException)
        {
            // Do nothing.
        }

        if (createTableRes)
        {
            Post post = new()
            {
                Uuid = Guid.NewGuid(),
                Title = "Hello, World!",
                Content = "This is my first post.",
                AuthorUuid = Guid.NewGuid(),
                CreatedAt = DateTime.Now.ToFileTimeUtc()
            };

            bool updateAtKeyRes = await db.UpdateTableAtKeyAsync("posts", "posts", new List<Post>() { post });
            Console.WriteLine($"Updated at Key: {updateAtKeyRes}");
        }

    }
}

public class Post
{
    public Guid Uuid { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid AuthorUuid { get; set; }
    public long CreatedAt { get; set; }

}
