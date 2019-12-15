using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO;

namespace RepoStatsExtractor
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var gitShell = new GitShell(@"C:\RandD\dotnet-BuildPerformanceTestAssets");
      gitShell.ShowVersion();
      var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("my-github-stats"));
#if DEBUG
      github.Credentials = new Octokit.Credentials(File.ReadAllText(@"C:\RandD\octokit.tkt"));
#endif
      var miscellaneousRateLimit = await github.Miscellaneous.GetRateLimits();

      Octokit.PullRequestRequest request = new Octokit.PullRequestRequest()
      {
        State = Octokit.ItemStateFilter.All
      };

      const string Owner = "dotnet";
      const string RepoName = "BuildPerformanceTestAssets";
      Octokit.RateLimit rateLimit;
      var repositoryPullRequests = await github.PullRequest.GetAllForRepository(Owner, RepoName, request);
      rateLimit = github.GetLastApiInfo().RateLimit;

      var prReviews = new Dictionary<string, (Octokit.PullRequest PullRequest,IReadOnlyList<Octokit.PullRequestReview> Reviews)>();
      foreach (var pullRequest in repositoryPullRequests)
      {
        if (!string.IsNullOrEmpty(pullRequest?.MergeCommitSha))
          prReviews.Add(pullRequest.MergeCommitSha, (pullRequest, await github.PullRequest.Review.GetAll(Owner, RepoName, pullRequest.Number)));
        rateLimit = github.GetLastApiInfo().RateLimit;
      }

      var commits = CommitInfo.ParseGitShellResult(gitShell.GetAllCommitStats(), prReviews);

    }
  }
}
