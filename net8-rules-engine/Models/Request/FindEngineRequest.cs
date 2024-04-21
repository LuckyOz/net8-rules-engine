namespace net8_rules_engine.Models.Request
{
    public class FindEngineRequest
    {
        public string? WorkflowCode { get; set; }
        public List<Item>? Items { get; set; }
    }

    public class Item
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public int Qty { get; set; }
    }
}
