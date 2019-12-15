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
      var gitShell = new GitShell(@"C:\RandD\dotnet-standard");
      gitShell.ShowVersion();
      //var commits = CommitInfo.ParseGitShellResult(gitShell.GetAllCommitStats());

    }
  }
}
