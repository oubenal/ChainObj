﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ReReportTransformer.Report;

namespace ReReportTransformer
{
  class Program
  {
    private const string HELP_FLAG = "-? |-h |--help";
    static int Main(string[] args)
    {
      #region debug args
#if DEBUG
      var myReportPath = @"C:\TeamCity\buildAgent\work\decc743a1148793a\result.xml";
      args = new[] {
                $"{myReportPath}",
            };
#endif
      #endregion
      Console.Out.WriteLine($"Current Folder: {Environment.CurrentDirectory}");
      var app = new CommandLineApplication(true);

      app.HelpOption(HELP_FLAG);

      var argument = app.Argument("report path", "InspectCode report to analyse");

      app.OnExecute(() =>
      {
        var reportPath = argument.Value;
        if (!string.IsNullOrWhiteSpace(reportPath))
        {
          InspectCodeReport report = new InspectCodeReport(reportPath);
          var filteredReport = report.FilterGlobal();
          return 0;
        }

        app.ShowHelp();
        return 0;
      });

      app.Description = "InspectCode report wrapper";

      return app.Execute(args);
    }
  }
}
