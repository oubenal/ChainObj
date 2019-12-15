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

    readonly string commitHash;
    readonly string authorName;
    readonly string authorEmail;
    readonly DateTime authorDate;
    readonly string committerName;
    readonly string committerEmail;
    readonly DateTime committerDate;
    readonly int filesChanged;
    readonly int insertions;
    readonly int deletions;

    public const string PRETTY_FORMAT = "%H;%an;%ae;%ad;%cn;%ce;%cd;%d";
    public const string DATE_FORMAT = "ddd MMM d HH:mm:ss yyyy K";
    internal CommitInfo(string commitHash, string authorName, string authorEmail, DateTime authorDate, string committerName, string committerEmail, DateTime committerDate, int filesChanged, int insertions, int deletions)
    {
      this.commitHash = commitHash;
      this.authorName = authorName;
      this.authorEmail = authorEmail;
      this.authorDate = authorDate;
      this.committerName = committerName;
      this.committerEmail = committerEmail;
      this.committerDate = committerDate;
      this.filesChanged = filesChanged;
      this.insertions = insertions;
      this.deletions = deletions;
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

    internal static List<CommitInfo> ParseGitShellResult(List<string> results)
    {
      var commits = new List<CommitInfo>(results.Count / 2);
      for (int i = 0; i < results.Count; i++)
      {
        if (!results[i + 1].StartsWith(" ")) // merge commit
        {
          log.Debug($"No diff in commit sha1:{results[i].Split(';').First()}");
          commits.Add(Parse(results[i]));
        }
        else
        {
          commits.Add(Parse(results[i], results[i + 1]));
          i++;
        }
      }
      return commits;
    }

    public override bool Equals(object obj)
    {
      return obj is CommitInfo type &&
             commitHash == type.commitHash &&
             authorName == type.authorName &&
             authorEmail == type.authorEmail &&
             authorDate == type.authorDate &&
             committerName == type.committerName &&
             committerEmail == type.committerEmail &&
             committerDate == type.committerDate &&
             filesChanged == type.filesChanged &&
             insertions == type.insertions &&
             deletions == type.deletions;
    }
    public override int GetHashCode()
    {
      var hashCode = -298609710;
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(commitHash);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(authorName);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(authorEmail);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(committerName);
      hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(committerEmail);
      hashCode = hashCode * -1521134295 + filesChanged.GetHashCode();
      hashCode = hashCode * -1521134295 + insertions.GetHashCode();
      hashCode = hashCode * -1521134295 + deletions.GetHashCode();
      return hashCode;
    }

    public override string ToString()
    {
      return $"{commitHash.Substring(0, 8)} | [{authorName}] <{authorEmail}> ({authorDate})";
    }
  }
}
