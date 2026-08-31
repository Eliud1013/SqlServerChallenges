using System.Xml.Linq;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public record OutputTable
{
    public IReadOnlyList<string> Columns { get;  }
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows { get; private set; }
    public QueryPlanInfo? Plan { get; private set; }

    public OutputTable(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyDictionary<string, object?>> rows)
    {
        Columns = columns;
        Rows = rows;
    }

    public void WithPlan(string plan)
    {
        var doc = XDocument.Parse(plan);

        var rootRelOp = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "RelOp");

        if (rootRelOp is null)
            return;

        var actualRows = rootRelOp
            .Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "RunTimeCountersPerThread")
            ?.Attribute("ActualRows")?.Value;

        Plan = new QueryPlanInfo(
            rootRelOp.Attribute("PhysicalOp")?.Value,
            rootRelOp.Attribute("LogicalOp")?.Value,
            ParseLong(actualRows),
            ParseDouble(rootRelOp.Attribute("EstimateIO")?.Value),
            ParseDouble(rootRelOp.Attribute("EstimateCPU")?.Value));
    }

    public void OrderRows()
    {
        var orderBy = Columns.First();
        Rows = Rows.OrderBy(r => r[orderBy]?.ToString() ?? "")
            .ToList();
    }

    private static double? ParseDouble(string? value)
        => value is not null && double.TryParse(value, out var d) ? d : null;

    private static long? ParseLong(string? value)
        => value is not null && long.TryParse(value, out var l) ? l : null;
}