
using net8_rules_engine.Configs;
using net8_rules_engine.Services;

namespace net8_rules_engine.Jobs
{
    public class SyncEngineJob(ILogger<SyncEngineJob> _logger, IServiceScopeFactory _factory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using AsyncServiceScope asyncScope = _factory.CreateAsyncScope();

                    IEngineSetup engineSetup = asyncScope.ServiceProvider.GetRequiredService<IEngineSetup>();
                    IEngineService engineService = asyncScope.ServiceProvider.GetRequiredService<IEngineService>();

                    if(!await engineService.CheckSync())
                    {
                        foreach (var loopDataEngine in await engineService.GetSyncWorkflow())
                        {
                            engineSetup.RefreshWorkflow(loopDataEngine);
                        }

                        _logger.LogInformation("Success Sync Workflow Engine, Version Engine Update !!");
                    }
                    else
                    {
                        _logger.LogInformation("Success Sync Workflow Engine, Version Engine UpToDate !!");
                    }

                } catch (Exception ex)
                {
                    _logger.LogError($"Failed Sync Workflow Engine : {ex.Message} !!");
                }
            }
        }
    }
}
