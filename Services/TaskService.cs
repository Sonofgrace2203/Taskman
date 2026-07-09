using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using miniapp.Components.Pages;

public class TaskService : ITaskService
{
    IJSRuntime JS;

    public TaskService(IJSRuntime jS)
    {
        JS = jS;
    }

    private readonly object _lock = new();

    public AppTask newTask = new();

    public List<AppTask> tasks = new();
    // public int nextId = 1;

    public event Action? OnChange;
    public void Notify() => OnChange?.Invoke();

    public List<AppTask> GetAll()
    {
        lock (_lock)
        {
            return tasks.ToList();
        }
    }

    // public IEnumerable<AppTask> GetCompletedTasks(
    // {

    // }

    public void Add(string title, Priority priority)
    {
        lock (_lock)
        {

            newTask = new AppTask
            {
                Id = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1,
                Title = title,
                Priority = priority,
                Date = DateTime.Now.ToString("dd MMM yyyy"),
                IsComplete = false
            };
            tasks.Add(newTask);
        }
        Notify();
    }

    public void Edit(int id, string title, Priority priority)
    {
        lock (_lock)
        {
            var existingTask = tasks.FirstOrDefault(t => t.Id == id);

            if (existingTask != null)
            {
                existingTask.Title = title;
                existingTask.Priority = priority;
            }
        }
        Notify();
    }

    public void MoveTask(string title, Priority priority)
    {
        lock (_lock)
        {

            newTask = new AppTask
            {
                Id = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1,
                Title = title,
                Priority = priority,
                Date = DateTime.Now.ToString("dd MMM yyyy"),
                IsComplete = true
            };
            tasks.Add(newTask);
        }
        Notify();
    }

    public void Delete(int id)
    {
        lock (_lock)
        {
            tasks.RemoveAll(t => t.Id == id);
        }
        Notify();
    }

    public void ClearAllTask()
    {
        lock (_lock)
        {
            tasks.Clear();
        }
        Notify();
    }

    public async Task SaveTask()
    {
        var jsonString = JsonSerializer.Serialize(tasks);
        await JS.InvokeVoidAsync("localStorage.setItem", "All_Tasks", jsonString);
    }

    bool loaded;
    public async Task LoadTasks()
    {
        if (loaded)
            return;
        var jsonString = await JS.InvokeAsync<string>("localStorage.getItem", "All_Tasks");
        if (!string.IsNullOrEmpty(jsonString))
        {
            var savedTasks = JsonSerializer.Deserialize<List<AppTask>>(jsonString);
            if (savedTasks != null)
            {
                tasks = savedTasks;
            }
        }
        loaded = true;
    }

    public void ToggleCompleted(int id)
    {
        lock (_lock)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);

            task?.IsComplete = !task.IsComplete;

            SaveTask();
            LoadTasks();
            Notify();
        }


        // var task = tasks.FirstOrDefault(t => t.Id == id);

        // if (task != null)
        // {
        //     task.IsComplete = !task.IsComplete;
        //     Notify();
        // }
    }

    public int GetCompletedCount()
    {
        return tasks.Count(t => t.IsComplete);
    }

    public int GetPendingCount()
    {
        return tasks.Count(t => !t.IsComplete);
    }
}