using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChainObj.UnitTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestEmptyDB()
        {
            using (var db = new Database(Utilities.TempDirPath, true))
                Assert.IsNull(db.Get(0));
        }
        [TestMethod]
        public void TestInsertAndRetrieveInDB()
        {
            using (var db = new Database(Utilities.TempDirPath, true))
            {
                db.Put(0, "data");
                Assert.IsTrue(db.Get(0).Equals("data"));
            }
        }
        [TestMethod]
        public void TestRetrieveAllSortedInDB()
        {
            using (var db = new Database(Utilities.TempDirPath, true))
            {
                for (var i = 0; i < 21; i++)
                    db.Put(i, $"New block {i}");
                var listElmt = db.All();
                var ind = 0;
                foreach (var elmt in listElmt)
                    Assert.IsTrue(elmt.Equals($"New block {ind++}"));
            }
        }
        [TestMethod]
        public void TestRetrieveLastInDB()
        {
            using (var db = new Database(Utilities.TempDirPath, true))
            {
                var lastIndex = 20;
                for (var i = 0; i <= lastIndex; i++)
                    db.Put(i, $"New block {i}");
                var lastElmt = db.GetLast();
                Assert.IsTrue(lastElmt.Equals($"New block {lastIndex}"));
            }
        }
    }
}
