using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using WorkflowEngine.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// In-memory stores
var workflowDefinitions = new Dictionary<string, WorkflowDefinition>();
var workflowInstances = new Dictionary<string, WorkflowInstance>();

// API: Create a workflow definition
app.MapPost("/workflows", async (WorkflowDefinition definition) =>
{
    if (definition.States.Count(s => s.IsInitial) != 1)
        return Results.BadRequest("Workflow must have exactly one initial state.");

    if (workflowDefinitions.ContainsKey(definition.Id))
        return Results.BadRequest("Workflow with this ID already exists.");

    workflowDefinitions[definition.Id] = definition;
    return Results.Ok("Workflow created successfully.");
});

// API: Get workflow definition by ID
app.MapGet("/workflows/{id}", (string id) =>
{
    if (workflowDefinitions.TryGetValue(id, out var def))
        return Results.Ok(def);

    return Results.NotFound("Workflow not found.");
});

// API: Start a new workflow instance
app.MapPost("/instances/{workflowId}", (string workflowId) =>
{
    if (!workflowDefinitions.TryGetValue(workflowId, out var definition))
        return Results.NotFound("Workflow not found.");

    var initialState = definition.States.First(s => s.IsInitial);
    var instance = new WorkflowInstance
    {
        Id = Guid.NewGuid().ToString(),
        WorkflowId = workflowId,
        CurrentStateId = initialState.Id,
        History = new List<StateTransition>()
    };

    workflowInstances[instance.Id] = instance;
    return Results.Ok(instance);
});

// API: Execute an action on a workflow instance
// API: Execute an action on a workflow instance
app.MapPost("/instances/{instanceId}/actions/{actionId}", (string instanceId, string actionId) =>
{
    if (!workflowInstances.TryGetValue(instanceId, out var instance))
        return Results.NotFound("Instance not found.");

    if (!workflowDefinitions.TryGetValue(instance.WorkflowId, out var definition))
        return Results.NotFound("Workflow definition not found.");

    var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
    if (action == null)
        return Results.BadRequest("Action not found.");

    if (action.FromState != instance.CurrentStateId)
        return Results.BadRequest("Action not allowed from current state.");

    instance.History.Add(new StateTransition { ActionId = actionId, Timestamp = DateTime.UtcNow });
    instance.CurrentStateId = action.ToState;

    return Results.Ok(instance);
});


// API: Get current state and history of an instance
app.MapGet("/instances/{instanceId}", (string instanceId) =>
{
    if (!workflowInstances.TryGetValue(instanceId, out var instance))
        return Results.NotFound("Instance not found.");

    return Results.Ok(instance);
});

app.Run();
