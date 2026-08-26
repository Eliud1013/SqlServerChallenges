using SqlServerChallenges.Core.Data.Entities.ChallengeSolutions;

namespace SqlServerChallenges.Core.Common.Cache;

public static class CacheKeys
{
    public static class Challenges
    {
        public static string SolutionSample(Guid challengeId, int rowLimit) => $"Challenge.SolutionSample.{challengeId}.{rowLimit}";
        public static string Info(string slug) => $"Challenge.Info.{slug}";
        public static string ExpectedOutput(Guid challengeId, DatabaseProvider provider) => $"Challenge.ExpectedOutput:{challengeId}.{provider}";
    }
}