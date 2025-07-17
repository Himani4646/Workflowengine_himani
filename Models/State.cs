namespace WorkflowEngine.Models;

public class State
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public bool IsInitial { get; set; }
}
