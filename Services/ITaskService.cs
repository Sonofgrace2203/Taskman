public interface ITaskService
{
    event Action? OnChange;
    Task SaveTask();
    Task LoadTasks();
    List<AppTask> GetAll();
    void Add(string title, Priority priority);
    void Edit(int id, string title, Priority priority);
    void MoveTask(string title, Priority priority);
    void Delete(int id);
    void ClearAllTask();
    Task ToggleCompleted(int id);
    int GetCompletedCount();
    int GetPendingCount();
}