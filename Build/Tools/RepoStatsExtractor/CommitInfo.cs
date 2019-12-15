using System;
using System.Text.RegularExpressions;

namespace RepoStatsExtractor
{
  internal class CommitInfo
  {
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
    internal CommitInfo(string description, string changes)
    {
      var entries = description.Split(';');
      var regex = new Regex(@"(?<filesChanged>\d+) files? changed(, (?<insertions>\d+) insertions?\(\+\))?(, (?<deletions>\d+) deletions?\(-\))?");
      var match = regex.Match(changes);

      commitHash = entries[0];
      authorName = entries[1];
      authorEmail = entries[2];
      DateTime.TryParseExact(entries[3], DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out authorDate);
      committerName = entries[4];
      committerEmail = entries[5];
      DateTime.TryParseExact(entries[6], DATE_FORMAT, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out authorDate);
      filesChanged = int.Parse(match.Groups["filesChanged"].Value);
      if(match.Groups["insertions"].Success)
        insertions = int.Parse(match.Groups["insertions"].Value);
      if (match.Groups["deletions"].Success)
        deletions = int.Parse(match.Groups["deletions"].Value);
    }
  }
}
