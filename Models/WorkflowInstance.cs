using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; }
        public string WorkflowId { get; set; }
        public string CurrentStateId { get; set; }
        public List<StateTransition> History { get; set; } = new();
    }
}
