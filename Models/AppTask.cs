public class AppTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public Priority Priority { get; set; } = Priority.Medium;
    public bool IsComplete { get; set; } = false;
    public string Date { get; set; } = "";
}