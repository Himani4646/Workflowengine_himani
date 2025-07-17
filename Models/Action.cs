namespace WorkflowEngine.Models;

public class Action
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string FromState { get; set; } = default!;
    public string ToState { get; set; } = default!;
}
