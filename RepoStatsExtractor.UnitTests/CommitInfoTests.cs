using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoStatsExtractor;

namespace RepoStatsExtractor.UnitTests
{
  [TestClass]
  public class CommitInfoTests
  {
    [TestMethod]
    public void TestParseGitShellResult()
    {
      var input = new[] {
        @"216c583cf726064b28e18a1c3b183ab9dc6f8e1e;Bryan P. Arant;bryanar@microsoft.com;Tue Jan 31 15:40:43 2017 -0800;GitHub;noreply@github.com;Tue Jan 31 15:40:43 2017 -0800;",
        @"5dc153c2f2a7cfeb4fe310fa30d4096a34b6354c;Bryan Arant;bryanar@microsoft.com;Mon Jan 30 17:44:57 2017 -0800;Bryan Arant;bryanar@microsoft.com;Tue Jan 31 15:38:05 2017 -0800;",
        @" 2 files changed, 20 insertions(+)",
        @"08044f93c71a050c32b00b97877ee1e647001987;Wes Haggard;Wes.Haggard@microsoft.com;Mon Jan 30 13:52:45 2017 -0800;Wes Haggard;Wes.Haggard@microsoft.com;Mon Jan 30 13:52:45 2017 -0800;",
        @" 1 file changed, 3 insertions(+), 3 deletions(-)",
        @"4d5ee1df5de2fbd408209dfcedbf08e95d6987b9;Dan Moseley;danmose@microsoft.com;Mon Jan 30 12:39:37 2017 -0700;GitHub;noreply@github.com;Mon Jan 30 12:39:37 2017 -0700;",
        @"45a8355090db5ae65c1b07516005ddb1a662366f;Chandan Rai;chandanr@exzeoindia.com;Mon Jan 30 14:48:47 2017 +0530;GitHub;noreply@github.com;Mon Jan 30 14:48:47 2017 +0530;",
        @" 1 file changed, 4 insertions(+), 4 deletions(-)",
        @"68323fff89222bcc4bbd930ae0165b2bfd4838ab;Vicey Wang;vicey@live.com;Sun Jan 29 21:51:33 2017 +0800;Vicey Wang;vicey@live.com;Sun Jan 29 21:51:33 2017 +0800;",
        @" 1 file changed, 2 deletions(-)",
      };
      var expected = new List<CommitInfo> {
        new CommitInfo("216c583cf726064b28e18a1c3b183ab9dc6f8e1e","Bryan P. Arant","bryanar@microsoft.com",CommitInfo.GitDefaultTime("Tue Jan 31 15:40:43 2017 -0800"),"GitHub","noreply@github.com",CommitInfo.GitDefaultTime("Tue Jan 31 15:40:43 2017 -0800"),0,0,0),
        new CommitInfo("5dc153c2f2a7cfeb4fe310fa30d4096a34b6354c","Bryan Arant","bryanar@microsoft.com",CommitInfo.GitDefaultTime("Mon Jan 30 17:44:57 2017 -0800"),"Bryan Arant","bryanar@microsoft.com",CommitInfo.GitDefaultTime("Tue Jan 31 15:38:05 2017 -0800"),2,20,0),
        new CommitInfo("08044f93c71a050c32b00b97877ee1e647001987","Wes Haggard","Wes.Haggard@microsoft.com",CommitInfo.GitDefaultTime("Mon Jan 30 13:52:45 2017 -0800"),"Wes Haggard","Wes.Haggard@microsoft.com",CommitInfo.GitDefaultTime("Mon Jan 30 13:52:45 2017 -0800"),1,3,3),
        new CommitInfo("4d5ee1df5de2fbd408209dfcedbf08e95d6987b9","Dan Moseley","danmose@microsoft.com",CommitInfo.GitDefaultTime("Mon Jan 30 12:39:37 2017 -0700"),"GitHub","noreply@github.com",CommitInfo.GitDefaultTime("Mon Jan 30 12:39:37 2017 -0700"),0,0,0),
        new CommitInfo("45a8355090db5ae65c1b07516005ddb1a662366f","Chandan Rai","chandanr@exzeoindia.com",CommitInfo.GitDefaultTime("Mon Jan 30 14:48:47 2017 +0530"),"GitHub","noreply@github.com",CommitInfo.GitDefaultTime("Mon Jan 30 14:48:47 2017 +0530"),1,4,4),
        new CommitInfo("68323fff89222bcc4bbd930ae0165b2bfd4838ab","Vicey Wang","vicey@live.com",CommitInfo.GitDefaultTime("Sun Jan 29 21:51:33 2017 +0800"),"Vicey Wang","vicey@live.com",CommitInfo.GitDefaultTime("Sun Jan 29 21:51:33 2017 +0800"),1,0,2),
      };
      var result = CommitInfo.ParseGitShellResult(new List<string>(input));
      Assert.AreEqual(6, result.Count);
      CollectionAssert.AreEqual(expected, result);
    }
  }
}
