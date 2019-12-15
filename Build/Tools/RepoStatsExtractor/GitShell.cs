using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;

namespace RepoStatsExtractor
{
  internal class GitShell : IGitShell
  {
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private List<string> RunCommand(string arguments)
    {
      log.Info($@"Git repository ""{RepositoryDir.FullName}""");
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

        var outputStr = new List<string> { };
        log.Info($@"will run  ""git {arguments}""");
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

    public void ShowVersion()
    {
      var arguments = new[] {
        "--version"
      };
      RunCommand(string.Join(" ", arguments));
    }
    public List<string> GetAllCommitStats()
    {
      var arguments = new[] {
        "log",
        $@"--pretty=""format:{CommitInfo.PRETTY_FORMAT}""",
        @"--shortstat",
        //@"--since=48.hour",
        //@"--no-merges",
        //@"--first-parent"
      };
      return RunCommand(string.Join(" ", arguments)).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
    }
  }
  internal interface IGitShell
  {
    void ShowVersion();
    List<string> GetAllCommitStats();
  }
}
