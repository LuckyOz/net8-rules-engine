
using RulesEngine.Models;
using net8_rules_engine.Models.Request;

namespace net8_rules_engine.Configs
{
    public interface IEngineSetup
    {
        bool SetupWorkflow(Workflow[] workflow);
        bool RefreshWorkflow(Workflow workflow);
        Task<List<RuleResultTree>> GetEngine(FindEngineRequest request);
    }

    public class EngineSetup : IEngineSetup
    {
        private RulesEngine.RulesEngine? _rulesEngine;

        public bool SetupWorkflow(Workflow[] workflow)
        {
            var reSettingsWithCustomTypes = new ReSettings { CustomTypes = new Type[] { typeof(EngineExpressionItemHelper) } };
            _rulesEngine = new RulesEngine.RulesEngine(workflow, reSettingsWithCustomTypes);

            return true;
        }

        public bool RefreshWorkflow(Workflow workflow)
        {
            if(_rulesEngine is null)
            {
                SetupWorkflow([workflow]);

                return true;
            }

            _rulesEngine.AddOrUpdateWorkflow(workflow);

            return true;
        }

        public async Task<List<RuleResultTree>> GetEngine(FindEngineRequest request)
        {
            RuleParameter paramsEngine = new("paramsPromo", request);

            return await _rulesEngine!.ExecuteAllRulesAsync(request.WorkflowCode, paramsEngine);
        }
    }
}
