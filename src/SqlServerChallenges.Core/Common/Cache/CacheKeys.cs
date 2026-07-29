namespace SqlServerChallenges.Core.Common.Cache;

public static class CacheKeys
{
    public static class Challenges
    {
        public static string SolutionSample(Guid challengeId) => $"Challenge.SolutionSample.{challengeId}";
    }
}