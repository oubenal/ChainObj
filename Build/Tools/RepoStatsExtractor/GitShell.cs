using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;

namespace RepoStatsExtractor
{
  internal class GitShell
  {
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private List<string> RunCommand(string arguments)
    {
      log.Debug($@"Git repository ""{RepositoryDir.Name}""");
      using (var process = new Process())
      {
        string errorOutput = null;
        process.StartInfo = new ProcessStartInfo
        {
          FileName = @"C:\Program Files\Git\cmd\git.exe",
          Arguments = $"{arguments}",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
          CreateNoWindow = true,
          WorkingDirectory = RepositoryDir.FullName
        };
        process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => errorOutput += e.Data);

        var outputStr = new List<string> { "" };
        log.Debug($@"will run  ""git {arguments}""");
        process.Start();
        process.BeginErrorReadLine();
        while (!process.StandardOutput.EndOfStream)
        {
          var line = process.StandardOutput.ReadLine();
          outputStr.Add(line);
          log.Debug("[git] -- " + line);
        }
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
          log.Error(errorOutput);
          log.Fatal($"Git exited with code {process.ExitCode}");
          throw new InvalidOperationException();
        }
        return outputStr;
      }
    }
    public GitShell(string repoPath)
    {
      RepositoryDir = new DirectoryInfo(repoPath);
    }
    public DirectoryInfo RepositoryDir { get; }

    internal void ShowVersion()
    {
      var arguments = new[] {
        "--version"
      };
      RunCommand(string.Join(" ", arguments));
    }
    internal List<CommitInfo> GetAllCommitStats()
    {
      var arguments = new[] {
        "log",
        $@"--pretty=""format:{CommitInfo.PRETTY_FORMAT}""",
        @"--shortstat",
        //@"--since=48.hour",
        @"--no-merges"
      };
      var results = RunCommand(string.Join(" ", arguments)).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
      var commits = new List<CommitInfo>(results.Count / 2);
      for (int i = 0; i < results.Count / 2; i++)
      {
        commits.Add(new CommitInfo(results[2 * i], results[2 * i + 1]));
      }
      return commits;
    }
  }
}
