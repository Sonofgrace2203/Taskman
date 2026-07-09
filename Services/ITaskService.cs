public interface ITaskService
{
    event Action? OnChange;
    Task SaveTask();
    Task LoadTasks();
    List<AppTask> GetAll();
    // IEnumerable<AppTask> GetCompletedTasks();
    void Add(string title, Priority priority);
    void Edit(int id, string title, Priority priority);
    void MoveTask(string title, Priority priority);
    void Delete(int id);
    void ClearAllTask();
    void ToggleCompleted(int id);
    int GetCompletedCount();
    int GetPendingCount();
}