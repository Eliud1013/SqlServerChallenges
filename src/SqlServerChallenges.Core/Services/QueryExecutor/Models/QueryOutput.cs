namespace SqlServerChallenges.Core.Services.QueryExecutor.Models;

public class QueryOutput
{
    public OutputTable OutputTable { get; init; }
    public QueryPlanInfo? QueryPlan { get; private set; }

    public QueryOutput(OutputTable outputTable)
    {
        OutputTable = outputTable;
    }

    public void SetPlan(QueryPlanInfo plan)
    {
        QueryPlan = plan;
    }
}