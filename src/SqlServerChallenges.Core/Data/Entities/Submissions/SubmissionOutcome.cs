using System.ComponentModel;

namespace SqlServerChallenges.Core.Data.Entities.Submissions;

public enum SubmissionOutcome
{
    Accepted,
    WrongAnswer,
    SyntaxError,
    QueryTimeout,
    QueryExecutionError
}