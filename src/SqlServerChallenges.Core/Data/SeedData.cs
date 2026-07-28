using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;
using SqlServerChallenges.Core.Services.SqlExecutor;

namespace SqlServerChallenges.Core.Data;

public static class SeedData
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (await dbContext.Categories.AnyAsync())
            return;

        await SeedCategoriesAsync(dbContext);
        await SeedChallengesAsync(dbContext);
        await SeedChallengeSolutionsAsync(dbContext);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext dbContext)
    {
        var categories = new List<Category>
        {
            new() { Name = "Basic Queries" },
            new() { Name = "Joins" },
            new() { Name = "Aggregation" },
            new() { Name = "CTEs" },
            new() { Name = "Window Functions" },
            new() { Name = "Data Manipulation" },
        };

        dbContext.Categories.AddRange(categories);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedChallengesAsync(ApplicationDbContext dbContext)
    {
        var categories = await dbContext.Categories.ToDictionaryAsync(c => c.Name);

        var challenges = new List<Challenge>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "SELECT All",
                TaskDescription = "Write a query to return all columns from the Person.Person table.",
                Difficulty = ChallengeDifficulty.Easy,
                CategoryId = categories["Basic Queries"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Solutions = new List<ChallengeSolution>
                {
                    new()
                    {
                        DatabaseProvider = DatabaseProvider.SqlServer,
                        SolutionSql = "SELECT * FROM Person.Person;"
                    }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Filter with WHERE",
                TaskDescription = "List all products from Production.Product with a ListPrice greater than 1000.",
                Difficulty = ChallengeDifficulty.Easy,
                CategoryId = categories["Basic Queries"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Ordering Results",
                TaskDescription = "Retrieve the top 10 most expensive products, ordered by ListPrice descending.",
                Difficulty = ChallengeDifficulty.Easy,
                CategoryId = categories["Basic Queries"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "INNER JOIN",
                TaskDescription = "Join Sales.SalesOrderHeader with Sales.SalesOrderDetail to return order dates and line totals.",
                Difficulty = ChallengeDifficulty.Medium,
                CategoryId = categories["Joins"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "LEFT JOIN",
                TaskDescription = "List all customers and their orders, including customers who have never placed an order.",
                Difficulty = ChallengeDifficulty.Medium,
                CategoryId = categories["Joins"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "GROUP BY with HAVING",
                TaskDescription = "Find product categories that have more than 100 products assigned.",
                Difficulty = ChallengeDifficulty.Medium,
                CategoryId = categories["Aggregation"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Running Total with SUM OVER",
                TaskDescription = "Calculate a running total of sales amount per day using a window function.",
                Difficulty = ChallengeDifficulty.Hard,
                CategoryId = categories["Window Functions"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Recursive CTE",
                TaskDescription = "Use a recursive CTE to generate a list of dates for the current month.",
                Difficulty = ChallengeDifficulty.Hard,
                CategoryId = categories["CTEs"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "UPDATE with JOIN",
                TaskDescription = "Increase the ListPrice of all products in the 'Bikes' subcategory by 10%.",
                Difficulty = ChallengeDifficulty.Medium,
                CategoryId = categories["Data Manipulation"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "ROW_NUMBER for Deduplication",
                TaskDescription = "Use ROW_NUMBER to identify and delete duplicate email addresses from Person.EmailAddress.",
                Difficulty = ChallengeDifficulty.Hard,
                CategoryId = categories["Window Functions"].Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            },
        };

        dbContext.Challenges.AddRange(challenges);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedChallengeSolutionsAsync(ApplicationDbContext dbContext)
    {
        var challenges = await dbContext.Challenges.ToDictionaryAsync(c => c.Title);

        var solutions = new List<ChallengeSolution>
        {
            new()
            {
                ChallengeId = challenges["SELECT All"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT * FROM Person.Person;",
            },
            new()
            {
                ChallengeId = challenges["Filter with WHERE"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT * FROM Production.Product WHERE ListPrice > 1000;",
            },
            new()
            {
                ChallengeId = challenges["Ordering Results"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT TOP 10 * FROM Production.Product ORDER BY ListPrice DESC;",
            },
            new()
            {
                ChallengeId = challenges["INNER JOIN"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT soh.OrderDate, sod.LineTotal\nFROM Sales.SalesOrderHeader soh\nINNER JOIN Sales.SalesOrderDetail sod ON soh.SalesOrderID = sod.SalesOrderID;",
            },
            new()
            {
                ChallengeId = challenges["LEFT JOIN"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT c.CustomerID, soh.SalesOrderID\nFROM Sales.Customer c\nLEFT JOIN Sales.SalesOrderHeader soh ON c.CustomerID = soh.CustomerID;",
            },
            new()
            {
                ChallengeId = challenges["GROUP BY with HAVING"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT pc.Name, COUNT(*) AS ProductCount\nFROM Production.Product p\nINNER JOIN Production.ProductSubcategory psc ON p.ProductSubcategoryID = psc.ProductSubcategoryID\nINNER JOIN Production.ProductCategory pc ON psc.ProductCategoryID = pc.ProductCategoryID\nGROUP BY pc.Name\nHAVING COUNT(*) > 100;",
            },
            new()
            {
                ChallengeId = challenges["Running Total with SUM OVER"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "SELECT OrderDate, TotalDue,\n       SUM(TotalDue) OVER (ORDER BY OrderDate) AS RunningTotal\nFROM Sales.SalesOrderHeader\nORDER BY OrderDate;",
            },
            new()
            {
                ChallengeId = challenges["Recursive CTE"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "WITH DateRange AS (\n    SELECT CAST(DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1) AS DATE) AS Dt\n    UNION ALL\n    SELECT DATEADD(DAY, 1, Dt)\n    FROM DateRange\n    WHERE Dt < EOMONTH(GETDATE())\n)\nSELECT * FROM DateRange\nOPTION (MAXRECURSION 31);",
            },
            new()
            {
                ChallengeId = challenges["UPDATE with JOIN"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "UPDATE p\nSET p.ListPrice = p.ListPrice * 1.1\nFROM Production.Product p\nINNER JOIN Production.ProductSubcategory psc ON p.ProductSubcategoryID = psc.ProductSubcategoryID\nWHERE psc.Name = 'Bikes';",
            },
            new()
            {
                ChallengeId = challenges["ROW_NUMBER for Deduplication"].Id,
                DatabaseProvider = DatabaseProvider.SqlServer,
                SolutionSql = "WITH cte AS (\n    SELECT EmailAddress,\n           ROW_NUMBER() OVER (PARTITION BY EmailAddress ORDER BY EmailAddressID) AS rn\n    FROM Person.EmailAddress\n)\nDELETE FROM cte WHERE rn > 1;",
            },
        };

        dbContext.Set<ChallengeSolution>().AddRange(solutions);
        await dbContext.SaveChangesAsync();
    }
}
