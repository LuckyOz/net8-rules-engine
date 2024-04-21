
using net8_rules_engine.Configs;
using net8_rules_engine.Services;

namespace net8_rules_engine.Jobs
{
    public class SetupEngineJob(ILogger<SetupEngineJob> _logger, IServiceScopeFactory _factory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await using AsyncServiceScope asyncScope = _factory.CreateAsyncScope();

                IEngineSetup engineSetup = asyncScope.ServiceProvider.GetRequiredService<IEngineSetup>();
                IEngineService engineService = asyncScope.ServiceProvider.GetRequiredService<IEngineService>();

                engineSetup.SetupWorkflow([.. await engineService.GetWorkflow()]);

                _logger.LogInformation("Success Setup Workflow Engine !!");
            } catch (Exception ex)
            {
                _logger.LogError($"Failed Setup Workflow Engine : {ex.Message} !!");
            }
        }
    }
}
