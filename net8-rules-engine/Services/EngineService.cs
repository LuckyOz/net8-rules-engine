
using RulesEngine.Models;
using Microsoft.EntityFrameworkCore;
using net8_rules_engine.Configs;
using net8_rules_engine.Contexts;
using net8_rules_engine.Models.Request;
using net8_rules_engine.Models.Entities;

namespace net8_rules_engine.Services
{
    public interface IEngineService
    {
        Task<bool> CheckSync();
        Task<List<Workflow>> GetWorkflow();
        Task<List<Workflow>> GetSyncWorkflow();
        bool RefreshWorkflow(EngineRequest request);
        Task<bool> RefreshWorkflowSync(EngineRequest request);
        bool SetupWorkflow(List<EngineRequest> request);
        Task<bool> SetupWorkflowSync(List<EngineRequest> request);
        Task<List<RuleResultTree>> GetEngine(FindEngineRequest request);
    }

    public class EngineService(IEngineSetup _engineSetup, DataDbContext _dataDbContext, GlobalConfig globalConfig) : IEngineService
    {
        public async Task<bool> CheckSync()
        {
            return globalConfig.EngineVersion == await _dataDbContext.VersionEngines.OrderByDescending(q => q.CreatedDate).Select(q => q.Id).FirstOrDefaultAsync();
        }

        public async Task<List<Workflow>> GetWorkflow()
        {
            List<Workflow> workflows = [];

            foreach (var loopDataEngine in await _dataDbContext.DataEngines.ToListAsync())
            {
                workflows.Add(new Workflow()
                {
                    WorkflowName = loopDataEngine.Code,
                    Rules = GetRules(loopDataEngine.Vloop)
                });
            }

            globalConfig.EngineVersion = await _dataDbContext.VersionEngines.OrderByDescending(q => q.CreatedDate).Select(q => q.Id).FirstOrDefaultAsync();

            return workflows;
        }

        public async Task<List<Workflow>> GetSyncWorkflow()
        {
            List<Workflow> workflows = [];

            VersionEngine? versionEngine = 
                await _dataDbContext.VersionEngines.OrderByDescending(q => q.CreatedDate).Include(q => q.Details).FirstOrDefaultAsync();

            if (versionEngine is not null)
            {
                foreach (var loopVersionEngineDetail in versionEngine.Details!)
                {
                    DataEngine? dataEngine = await _dataDbContext.DataEngines.FirstOrDefaultAsync(q => q.Code == loopVersionEngineDetail.EngineCode);

                    if (dataEngine is not null)
                    {
                        workflows.Add(new Workflow()
                        {
                            WorkflowName = dataEngine.Code,
                            Rules = GetRules(dataEngine.Vloop)
                        });
                    }
                }

                globalConfig.EngineVersion = versionEngine.Id;
            }

            return workflows;
        }

        public bool SetupWorkflow(List<EngineRequest> request)
        {
            List<Workflow> workflows = [];

            foreach (var req in request)
            {
                Workflow workflow = new()
                {
                    WorkflowName = req.WorkflowCode,
                    Rules = GetRules(req.Vloop)
                };

                workflows.Add(workflow);
            }

            _engineSetup.SetupWorkflow([.. workflows]);

            return true;
        }

        public async Task<bool> SetupWorkflowSync(List<EngineRequest> request)
        {
            List<Workflow> workflows = [];
            List<DataEngine> dataEngines = [];
            List<VersionEngineDetail> versionEngineDetails = [];

            using var dbContextTransaction = await _dataDbContext.Database.BeginTransactionAsync();

            try
            {
                foreach (var req in request)
                {
                    Workflow workflow = new()
                    {
                        WorkflowName = req.WorkflowCode,
                        Rules = GetRules(req.Vloop)
                    };

                    versionEngineDetails.Add(new VersionEngineDetail
                    {
                        Id = Guid.NewGuid(),
                        EngineCode = req.WorkflowCode,
                    });

                    if (!await _dataDbContext.DataEngines.AnyAsync(q => q.Code == req.WorkflowCode))
                    {
                        dataEngines.Add(new DataEngine
                        {
                            Id = Guid.NewGuid(),
                            Code = req.WorkflowCode,
                            Vloop = req.Vloop,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        await _dataDbContext.DataEngines
                            .Where(q => q.Code == req.WorkflowCode)
                            .ExecuteUpdateAsync(q => q
                                .SetProperty(q => q.Vloop, req.Vloop)
                                .SetProperty(q => q.UpdatedDate, DateTime.UtcNow));
                    }

                    workflows.Add(workflow);
                }

                VersionEngine versionEngine = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    Details = versionEngineDetails
                };

                _dataDbContext.DataEngines.AddRange(dataEngines);
                _dataDbContext.VersionEngines.Add(versionEngine);

                await _dataDbContext.SaveChangesAsync();

                _engineSetup.SetupWorkflow([.. workflows]);

                globalConfig.EngineVersion = versionEngine.Id;
            }
            catch
            {
                await dbContextTransaction.RollbackAsync();

                return false;
            }

            await dbContextTransaction.CommitAsync();

            return true;
        }

        public bool RefreshWorkflow(EngineRequest request)
        {
            Workflow workflow = new()
            {
                WorkflowName = request.WorkflowCode,
                Rules = GetRules(request.Vloop)
            };

            _engineSetup.RefreshWorkflow(workflow);

            return true;
        }

        public async Task<bool> RefreshWorkflowSync(EngineRequest request)
        {
            using var dbContextTransaction = await _dataDbContext.Database.BeginTransactionAsync();

            try
            {
                Workflow workflow = new()
                {
                    WorkflowName = request.WorkflowCode,
                    Rules = GetRules(request.Vloop)
                };

                if (!await _dataDbContext.DataEngines.AnyAsync(q => q.Code == request.WorkflowCode))
                {
                    _dataDbContext.DataEngines.Add(new DataEngine
                    {
                        Id = Guid.NewGuid(),
                        Code = request.WorkflowCode,
                        Vloop = request.Vloop,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    });
                }
                else
                {
                    await _dataDbContext.DataEngines
                    .Where(q => q.Code == request.WorkflowCode)
                        .ExecuteUpdateAsync(q => q
                            .SetProperty(q => q.Vloop, request.Vloop)
                            .SetProperty(q => q.UpdatedDate, DateTime.UtcNow));
                }

                VersionEngine versionEngine = new()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    Details =
                    [
                        new VersionEngineDetail
                    {
                        Id = Guid.NewGuid(),
                        EngineCode = request.WorkflowCode
                    }
                    ]
                };

                _dataDbContext.VersionEngines.Add(versionEngine);

                await _dataDbContext.SaveChangesAsync();

                _engineSetup.RefreshWorkflow(workflow);

                globalConfig.EngineVersion = versionEngine.Id;

            } catch
            {
                await dbContextTransaction.RollbackAsync();

                return false;
            }

            await dbContextTransaction.CommitAsync();

            return true;
        }

        public async Task<List<RuleResultTree>> GetEngine(FindEngineRequest request)
        {
            return await _engineSetup.GetEngine(request);
        }

        private static List<Rule> GetRules(int vloop)
        {
            List<Rule> rules = [];

            for (int i = 1; i <= vloop; i++)
            {
                rules.Add(new Rule
                {
                    RuleName = $"Test Rule {i}",
                    Expression = $"paramsPromo.Items.Any(q => q.Code == \"item{i}1\" && q.Qty >= 1)"
                    //Expression = "paramsPromo.Items.Any(q => new string[] { \"item1\" }.Contains(q.Code))"
                    //Expression = "paramsPromo.Items.Exists(q => q.Code == \"item1\")"
                    //Expression = "paramsPromo.Items.FirstOrDefault(q => q.Code == \"item1\") != null"
                    //Expression = "CheckContains.CheckContains(paramsPromo.Items, \"item1\") == true"
                    //Expression = "paramsPromo.Items.CheckContains(\"item1\")"
                    //Expression = "paramsPromo.WorkflowCode == \"F100\""
                });
            }

            return rules;
        }
    }
}
