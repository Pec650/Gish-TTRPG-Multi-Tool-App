namespace ToDoList.Pages;

public class CompletedTask
{
    public ToDoClass Item { get; set; } = new();
    public DateTime CompletedAt { get; set; } = DateTime.Now;
}
