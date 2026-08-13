using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SqlServerChallenges.Core.Data.Entities;
using SqlServerChallenges.Core.Data.Entities.Categories;
using SqlServerChallenges.Core.Data.Entities.Challenges;
using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Tests.Common;

public class ChallengeBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _title = "Basic SELECT";
    private string _slug => $"{ToSlug(_title)}-{_id}";
    private static int _currentNumber = 1;
    private int _number = Interlocked.Increment(ref _currentNumber);
    private string _taskDescription = "Write a SELECT query";
    private ChallengeDifficulty _difficulty = ChallengeDifficulty.Easy;
    private bool _requiresOrdering = false;
    private Category _category = new() { Name = "General" };
    private List<ChallengeSolution> _solutions = new();
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;

    public ChallengeBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ChallengeBuilder WithNumber(int number)
    {
        _number = number;
        return this;
    }
    
    public ChallengeBuilder WithTaskDescription(string taskDescription)
    {
        _taskDescription = taskDescription;
        return this;
    }

    public ChallengeBuilder WithDifficulty(ChallengeDifficulty difficulty)
    {
        _difficulty = difficulty;
        return this;
    }

    public ChallengeBuilder WithRequiresOrdering(bool requiresOrdering)
    {
        _requiresOrdering = requiresOrdering;
        return this;
    }

    public ChallengeBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    public ChallengeBuilder WithUpdatedAt(DateTime updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    public ChallengeBuilder WithCategory(Category category)
    {
        _category = category;
        return this;
    }
    
    public ChallengeBuilder WithSolution(string sql, DatabaseProvider provider)
    {
        _solutions.Add(new ChallengeSolution
        {
            ChallengeId = _id,
            DatabaseProvider = provider,
            SolutionSql = sql,
        });
        
        return this;
    }

    public Challenge Build()
    {
        return new Challenge
        {
            Id = _id,
            Title = _title,
            Slug = _slug,
            Number = _number,
            TaskDescription = _taskDescription,
            Difficulty = _difficulty,
            RequiresOrdering = _requiresOrdering,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            Category = _category,
            Solutions = _solutions
        };
    }
    
    private string ToSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        title = title.ToLowerInvariant();

        var normalized = title.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (char c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        title = sb.ToString().Normalize(NormalizationForm.FormC);

        title = Regex.Replace(title, @"[^a-z0-9\s-]", "");

        title = Regex.Replace(title, @"\s+", "-");

        title = Regex.Replace(title, @"-+", "-");

        return title.Trim('-');
    }
}