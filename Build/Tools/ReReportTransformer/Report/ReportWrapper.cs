using System;
using System.Linq;
using System.Collections.Immutable;
using System.Xml;

namespace ReReportTransformer.Report
{
    class InspectCodeReport
    {
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
            } catch (Exception e)
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
                IssueTypes.Where( it => !it.CheckIfGlobal()).ToImmutableList(),
                ProjectsWithIssues.Select(p => p.FilterGlobal()).Where(p => p != null).ToImmutableList());
        }
    }
}
