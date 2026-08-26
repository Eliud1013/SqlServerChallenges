using FluentAssertions;
using SqlServerChallenges.Core.Services;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Features.Services;

public class MsSqlQuerySyntaxCheckerTests
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void Validate_ShouldReturnExpectedValidity(string sql, bool expected)
    {
        var syntaxChecker = new MsSqlQuerySyntaxChecker();
        
        SqlSyntaxResult result = syntaxChecker.Validate(sql);

        result.IsValid.Should().Be(expected);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return ["SELECT * FROM sysobjects", true];
        yield return ["SELECT col1, col2, col3 FROM some_table", true];
        yield return ["SELECT col1, col2, col3 FROM some_table WHERE some_col = 'some_value'", true];
        yield return ["SELECT COUNT(*) AS cnt FROM orders GROUP BY status HAVING COUNT(*) > 1", true];
        yield return ["SELECT * FROM (SELECT id FROM users) AS sub", true];
        yield return ["WITH cte AS (SELECT id FROM users) SELECT * FROM cte", true];
        yield return ["SELECT DENSE_RANK() OVER (PARTITION BY dept ORDER BY salary DESC) AS rnk FROM employees", true];
        yield return ["INSERT INTO logs (message) VALUES ('hello')", true];
        yield return ["UPDATE accounts SET balance = balance + 100 WHERE id = 1", true];
        yield return ["DELETE FROM sessions WHERE expires < GETUTCDATE()", true];
        yield return ["CREATE TABLE #temp (id INT)", true];
        yield return ["SELECT * FROM t1 INNER JOIN t2 ON t1.id = t2.id", true];
        yield return ["SELECT * FROM t1 LEFT JOIN t2 ON t1.id = t2.id WHERE t2.id IS NULL", true];
        yield return ["SELECT CASE WHEN score >= 90 THEN 'A' ELSE 'B' END AS grade FROM results", true];
        yield return ["SELECT COUNT(*) OVER ()", true];
        yield return ["", true];
        
        yield return ["SELEC *", false];
        yield return ["SELECT FROM", false];
        yield return ["SELECT * FROM", false];
        yield return ["SELECT col1, col2, col3 FROM some_table WHERE some_col  some_value", false];
        yield return ["SELECT * FORM users", false];
        yield return ["INSERT INTO (message) VALUES ('hello')", false];
        yield return ["UPDAT accounts SET balance = 0", false];
        yield return ["UL48gU5t9ltVExbo", false];
    }
}