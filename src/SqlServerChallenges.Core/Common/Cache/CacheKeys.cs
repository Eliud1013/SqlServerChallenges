namespace SqlServerChallenges.Core.Common.Cache;

public static class CacheKeys
{
    public static class Challenges
    {
        public static string SolutionSample(Guid challengeId) => $"Challenge.SolutionSample.{challengeId}";
        public static string Info(string slug) => $"Challenge.Info.{slug}";
    }
}