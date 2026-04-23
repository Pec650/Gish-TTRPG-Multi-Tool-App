namespace ToDoList.Pages;

using System.Collections.ObjectModel;
using System.Linq;

public static class ToDoService
{
    public static ObservableCollection<ToDoClass> Items { get; } = new();
    public static ObservableCollection<CompletedTask> CompletedItems { get; } = new();

    private static int _nextId = 1;

    public static void Add(string name, string description)
    {
        Items.Add(new ToDoClass
        {
            item_id = _nextId++,
            item_name = name,
            item_description = description,
            status = string.Empty,
            user_id = 0
        });
    }

    public static void Remove(ToDoClass item) => Items.Remove(item);

    public static void Update(ToDoClass item, string name, string description)
    {
        item.item_name = name;
        item.item_description = description;
    }

    public static void Complete(ToDoClass item)
    {
        Items.Remove(item);
        CompletedItems.Add(new CompletedTask { Item = item, CompletedAt = DateTime.Now });
    }

    public static void PurgeExpired()
    {
        var expired = CompletedItems
            .Where(x => (DateTime.Now - x.CompletedAt).TotalDays >= 7)
            .ToList();
        foreach (var x in expired)
            CompletedItems.Remove(x);
    }
}
