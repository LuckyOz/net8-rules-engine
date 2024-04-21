
using Microsoft.AspNetCore.Mvc;
using net8_rules_engine.Services;
using net8_rules_engine.Models.Request;

namespace net8_rules_engine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EngineController(IEngineService _engineService) : ControllerBase
    {
        [HttpPost("setup-workflow-engine")]
        public IActionResult SetupWorkflowEngine(List<EngineRequest> request)
        {
            return Ok(_engineService.SetupWorkflow(request));
        }

        [HttpPost("setup-workflow-engine-sync")]
        public async Task<IActionResult> SetupWorkflowEngineSync(List<EngineRequest> request)
        {
            return Ok(await _engineService.SetupWorkflowSync(request));
        }

        [HttpPost("refresh-workflow-engine")]
        public IActionResult RefreshWorkflowEngine(EngineRequest request)
        {
            return Ok(_engineService.RefreshWorkflow(request));
        }

        [HttpPost("refresh-workflow-engine-sync")]
        public async Task<IActionResult> RefreshWorkflowEngineSync(EngineRequest request)
        {
            return Ok(await _engineService.RefreshWorkflowSync(request));
        }

        [HttpPost("find-engine")]
        public async Task<IActionResult> FindEngine(FindEngineRequest request)
        {
            return Ok (await _engineService.GetEngine(request));
        }
    }
}
