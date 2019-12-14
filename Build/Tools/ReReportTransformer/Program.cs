using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using ReReportTransformer.Report;
using log4net;

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
      var log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      log.Debug($"Current Folder: {Environment.CurrentDirectory}");
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

          var reportDir = System.IO.Path.GetDirectoryName(reportPath);
          log.Info($@"Writting filtered report to : {reportDir}\filter-report.xml");
          System.IO.File.WriteAllText($@"{reportDir}\filter-report.xml", filteredReport.ToString());
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
