using FluentAssertions;
using SqlServerChallenges.Core.Services;
using SqlServerChallenges.Core.Services.TableReferenceExtractor;
using Xunit;

namespace SqlServerChallenges.Core.Tests.Services.TableReferenceExtractor;

public class MsSqlTableReferencesExtractorTests
{
    [Theory]
    [MemberData(nameof(TestCases))]
    public void Extract_ShouldReturnExpectedTableDefinition(string sql, Dictionary<string, List<string>> expected)
    {
        var tableReferenceExtractor = new MsSqlTableReferencesExtractor();

        var result = tableReferenceExtractor.Extract(sql);

        result.Should().BeEquivalentTo(expected);
    }

    public static IEnumerable<object[]> TestCases()
    {
        yield return
        [
            "SELECT * FROM Person.Person",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person"]
            }
        ];

        yield return
        [
            @"SELECT p.FirstName, s.SalesOrderID
              FROM Person.Person p
              INNER JOIN Sales.SalesOrderHeader s
                  ON p.BusinessEntityID = s.CustomerID",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person"],
                ["Sales"] = ["SalesOrderHeader"]
            }
        ];

        yield return
        [
            @"SELECT d.Name, COUNT(e.BusinessEntityID)
              FROM HumanResources.Department d
              INNER JOIN HumanResources.EmployeeDepartmentHistory edh
                ON d.DepartmentID = edh.DepartmentID
              INNER JOIN HumanResources.Employee e
                ON edh.BusinessEntityID = e.BusinessEntityID
              GROUP BY d.Name",
            new Dictionary<string, List<string>>
            {
                ["HumanResources"] = ["Department", "EmployeeDepartmentHistory", "Employee"]
            }
        ];

        yield return
        [
            @"SELECT p.Name, pi.Quantity
              FROM Production.Product p
              JOIN Production.ProductInventory pi
                  ON p.ProductID = pi.ProductID
              WHERE pi.Quantity > 100",
            new Dictionary<string, List<string>>
            {
                ["Production"] = ["Product", "ProductInventory"]
            }
        ];

        yield return
        [
            @"SELECT c.CustomerID, st.Name
              FROM Sales.Customer c
              LEFT JOIN Sales.Store s
                  ON c.StoreID = s.BusinessEntityID
              LEFT JOIN Sales.SalesTerritory st
                  ON c.TerritoryID = st.TerritoryID",
            new Dictionary<string, List<string>>
            {
                ["Sales"] = ["Customer", "Store", "SalesTerritory"]
            }
        ];
        yield return
        [
            @"SELECT v.Name, p.Name, po.OrderDate
              FROM Purchasing.Vendor v
              INNER JOIN Purchasing.PurchaseOrderHeader po
                  ON v.BusinessEntityID = po.VendorID
              INNER JOIN Purchasing.PurchaseOrderDetail pod
                  ON po.PurchaseOrderID = pod.PurchaseOrderID
              INNER JOIN Production.Product p
                  ON pod.ProductID = p.ProductID",
            new Dictionary<string, List<string>>
            {
                ["Purchasing"] = ["Vendor", "PurchaseOrderHeader", "PurchaseOrderDetail"],
                ["Production"] = ["Product"]
            }
        ];
        yield return
        [
            @"SELECT p.Name, SUM(sod.LineTotal)
              FROM Sales.SalesOrderHeader soh
              INNER JOIN Sales.SalesOrderDetail sod
                  ON soh.SalesOrderID = sod.SalesOrderID
              INNER JOIN Production.Product p
                  ON sod.ProductID = p.ProductID
              INNER JOIN Sales.Customer c
                  ON soh.CustomerID = c.CustomerID
              INNER JOIN Person.Person pers
                  ON c.PersonID = pers.BusinessEntityID
              GROUP BY p.Name
              HAVING SUM(sod.LineTotal) > 5000",
            new Dictionary<string, List<string>>
            {
                ["Sales"] = ["SalesOrderHeader", "SalesOrderDetail", "Customer"],
                ["Production"] = ["Product"],
                ["Person"] = ["Person"]
            }
        ];

        yield return
        [
            @"WITH people AS (
                SELECT BusinessEntityID FROM Person.Person
            ),
            emails AS (
                SELECT BusinessEntityID FROM Person.EmailAddress
            )
            SELECT p.LastName, e.EmailAddress
            FROM people p
            JOIN emails e ON p.BusinessEntityID = e.BusinessEntityID",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person", "EmailAddress"]
            }
        ];

        yield return
        [
            @"SELECT p.BusinessEntityID, p.FirstName
              INTO #tempPeople
              FROM Person.Person p",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person"]
            }
        ];

        yield return
        [
            @"SELECT [p].[FirstName]
              FROM [Person].[Person] AS [p]",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person"]
            }
        ];

        yield return
        [
            @"SELECT Id, Name
              FROM LookupTable",
            new Dictionary<string, List<string>>
            {
                ["dbo"] = ["LookupTable"]
            }
        ];

        yield return
        [
            @"SELECT e1.BusinessEntityID
              FROM HumanResources.Employee e1
              INNER JOIN HumanResources.Employee e2
                  ON e1.ManagerID = e2.BusinessEntityID",
            new Dictionary<string, List<string>>
            {
                ["HumanResources"] = ["Employee"]
            }
        ];

        yield return
        [
            @"SELECT BusinessEntityID
              FROM Person.Person
              WHERE Title IN (
                  SELECT Title FROM Person.Person WHERE FirstName IS NOT NULL
              )",
            new Dictionary<string, List<string>>
            {
                ["Person"] = ["Person"]
            }
        ];

        yield return
        [
            @"UPDATE Sales.Customer
              SET StoreID = s.BusinessEntityID
              FROM Sales.Store s
              INNER JOIN Sales.Customer c
                  ON s.BusinessEntityID = c.StoreID",
            new Dictionary<string, List<string>>
            {
                ["Sales"] = ["Customer", "Store"]
            }
        ];

        yield return
        [
            @"INSERT INTO Production.ProductHistory (ProductID, Name)
              SELECT ProductID, Name
              FROM Production.Product",
            new Dictionary<string, List<string>>
            {
                ["Production"] = ["ProductHistory", "Product"]
            }
        ];
    }
}