using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;

namespace RepoStatsExtractor
{
  internal class CommitInfo
  {
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    internal string CommitHash { get; }
    internal string AuthorName { get; }
    internal string AuthorEmail { get; }
    internal DateTime AuthorDate { get; }
    internal string CommitterName { get; }
    internal string CommitterEmail { get; }
    internal DateTime CommitterDate { get; }
    internal int FilesChanged { get; }
    internal int Insertions { get; }
    internal int Deletions { get; }
    internal int PullRequest { get; private set; }
    internal void SetPullRequest(int number)
    {
      PullRequest = number;
    }

    public const string PRETTY_FORMAT = "%H;%an;%ae;%ad;%cn;%ce;%cd;%d";
    public const string DATE_FORMAT = "ddd MMM d HH:mm:ss yyyy K";
    internal CommitInfo(string commitHash, string authorName, string authorEmail, DateTime authorDate, string committerName, string committerEmail, DateTime committerDate, int filesChanged, int insertions, int deletions)
    {
      this.CommitHash = commitHash;
      this.AuthorName = authorName;
      this.AuthorEmail = authorEmail;
      this.AuthorDate = authorDate;
      this.CommitterName = committerName;
      this.CommitterEmail = committerEmail;
      this.CommitterDate = committerDate;
      this.FilesChanged = filesChanged;
      this.Insertions = insertions;
      this.Deletions = deletions;
    }
    private static CommitInfo Parse(string description, string changes = null)
    {
      var entries = description.Split(';');
      var regex = new Regex(@"(?<filesChanged>\d+) files? changed(, (?<insertions>\d+) insertions?\(\+\))?(, (?<deletions>\d+) deletions?\(-\))?");

      var commitHash = entries[0];
      var authorName = entries[1];
      var authorEmail = entries[2];
      DateTime authorDate = GitDefaultTime(entries[3]);
      var committerName = entries[4];
      var committerEmail = entries[5];
      DateTime committerDate = GitDefaultTime(entries[6]);

      int filesChanged = 0, insertions = 0, deletions = 0;
      if (!string.IsNullOrEmpty(changes))
      {
        var match = regex.Match(changes);
        filesChanged = int.Parse(match.Groups["filesChanged"].Value);
        if (match.Groups["insertions"].Success)
          insertions = int.Parse(match.Groups["insertions"].Value);
        if (match.Groups["deletions"].Success)
          deletions = int.Parse(match.Groups["deletions"].Value);
      }

      return new CommitInfo(commitHash, authorName, authorEmail, authorDate, committerName, committerEmail, committerDate, filesChanged, insertions, deletions);
    }
    public static DateTime GitDefaultTime(string date)
    {
      DateTime.TryParseExact(date, DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var authorDate);
      return authorDate;
    }

    internal static List<CommitInfo> ParseGitShellResult(List<string> results, Dictionary<string, (Octokit.PullRequest PullRequest, IReadOnlyList<Octokit.PullRequestReview> Reviews)> mergedPullRequest = null)
    {
      mergedPullRequest = mergedPullRequest ?? new Dictionary<string, (Octokit.PullRequest PullRequest, IReadOnlyList<Octokit.PullRequestReview> Reviews)>();
      var commits = new List<CommitInfo>(results.Count / 2);
      for (int i = 0; i < results.Count; i++)
      {
        if (!results[i + 1].StartsWith(" ")) // merge commit
        {
          log.Debug($"No diff in commit sha1:{results[i].Split(';').First()}");
          var commit = Parse(results[i]);
          if (mergedPullRequest.TryGetValue(commit.CommitHash, out var pr))
            commit.SetPullRequest(pr.PullRequest.Number);
          commits.Add(commit);
        }
        else
        {
          var commit = Parse(results[i], results[i + 1]);
          if (mergedPullRequest.TryGetValue(commit.CommitHash, out var pr))
            commit.SetPullRequest(pr.PullRequest.Number);
          commits.Add(commit);
          i++;
        }
      }
      return commits;
    }

    public override bool Equals(object obj)
    {
      return obj is CommitInfo type &&
             CommitHash == type.CommitHash &&
             AuthorName == type.AuthorName &&
             AuthorEmail == type.AuthorEmail &&
             AuthorDate == type.AuthorDate &&
             CommitterName == type.CommitterName &&
             CommitterEmail == type.CommitterEmail &&
             CommitterDate == type.CommitterDate &&
             FilesChanged == type.FilesChanged &&
             Insertions == type.Insertions &&
             Deletions == type.Deletions;
    }
    public override int GetHashCode()
    {
      var hashCode = -298609710;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CommitHash);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AuthorName);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(AuthorEmail);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CommitterName);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CommitterEmail);
      hashCode = hashCode * -1521134295 + FilesChanged.GetHashCode();
      hashCode = hashCode * -1521134295 + Insertions.GetHashCode();
      hashCode = hashCode * -1521134295 + Deletions.GetHashCode();
      return hashCode;
    }

    public override string ToString()
    {
      return $"{CommitHash.Substring(0, 8)} | [{AuthorName}] <{AuthorEmail}> ({AuthorDate})";
    }
  }
}
