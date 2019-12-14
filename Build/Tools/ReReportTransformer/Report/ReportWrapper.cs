using System;
using System.Linq;
using System.Collections.Immutable;
using System.Xml;
using log4net;

namespace ReReportTransformer.Report
{
  class InspectCodeReport
  {
    private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    private InspectCodeReport(ImmutableList<IssueType> issueTypes, ImmutableList<Project> projectsWithIssues)
    {
      IssueTypes = issueTypes;
      ProjectsWithIssues = projectsWithIssues;
    }
    internal InspectCodeReport(string filePath)
    {
      try
      {
        var reportXml = new XmlDocument();
        reportXml.Load(filePath);

        IssueTypes = IssueType.GetIssueTypes(reportXml).ToImmutableList();
        ProjectsWithIssues = Project.GetIssues(reportXml).ToImmutableList();
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw new ArgumentException($"{filePath} is not a valid xml file");
      }
    }

    internal readonly ImmutableList<IssueType> IssueTypes;
    internal readonly ImmutableList<Project> ProjectsWithIssues;

    internal InspectCodeReport FilterGlobal()
    {
      return new InspectCodeReport(
          IssueTypes.Where(issueType =>
          {
            bool isGlobal = issueType.CheckIfGlobal();
            if (isGlobal)
              log.Debug($"Filtering Issues with id '{issueType.Id}' ");
            return !isGlobal;
          }).ToImmutableList(),
          ProjectsWithIssues.Select(p => p.FilterGlobal()).Where(p => p != null).ToImmutableList());
    }
    public override string ToString()
    {
      return $@"<Report><IssueTypes>{string.Join("", IssueTypes)}</IssueTypes><Issues>{string.Join("", ProjectsWithIssues)}</Issues></Report>";
    }
  }
}
