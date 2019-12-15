using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace RepoStatsExtractor
{
  class Program
  {
    static void Main(string[] args)
    {
      var gitShell = new GitShell(@"C:\RandD\dotnet-standard");
      gitShell.ShowVersion();
      var commits = CommitInfo.ParseGitShellResult(gitShell.GetAllCommitStats());
    }
  }
}
