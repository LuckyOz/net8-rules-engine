namespace net8_rules_engine.Models.Request
{
    public class EngineRequest
    {
        public string? WorkflowCode { get; set; }
        public int Vloop { get; set; } = 1;
        public int Hloop { get; set; } = 1;
    }
}
