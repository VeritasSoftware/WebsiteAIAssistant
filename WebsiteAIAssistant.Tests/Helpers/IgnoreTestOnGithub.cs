namespace WebsiteAIAssistant.Tests.Helpers
{
    using System;
    using Xunit;

    public sealed class Fact_IgnoreTestOnGithub : FactAttribute
    {
        public Fact_IgnoreTestOnGithub()
        {
            if (IsGitHubAction())
            {
                Skip = "Ignore the test when run in Github agent.";
            }
        }

        private static bool IsGitHubAction()
            => Environment.GetEnvironmentVariable("GITHUB_ACTION") != null;
    }

    public sealed class Theory_IgnoreTestOnGithub : FactAttribute
    {
        public Theory_IgnoreTestOnGithub()
        {
            if (IsGitHubAction())
            {
                Skip = "Ignore the test when run in Github agent.";
            }
        }

        private static bool IsGitHubAction()
            => Environment.GetEnvironmentVariable("GITHUB_ACTION") != null;
    }
}
