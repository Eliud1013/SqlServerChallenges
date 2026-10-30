using System.Xml.Linq;

namespace SqlServerChallenges.Core.Services.QueryExecutor;

public record OutputTable
{
    public IReadOnlyList<string> Columns { get;  }
    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows { get; private set; }

    public OutputTable(IReadOnlyList<string> columns, IReadOnlyList<IReadOnlyDictionary<string, object?>> rows)
    {
        Columns = columns;
        Rows = rows;
    }
    
    public void OrderRows()
    {
        var orderBy = Columns.First();
        Rows = Rows.OrderBy(r => r[orderBy]?.ToString() ?? "")
            .ToList();
    }
}
