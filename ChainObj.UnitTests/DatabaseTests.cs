﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChainObj.UnitTests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestEmptyDB()
        {
            using (var tmpDir = new TempDirectory())
            using (var db = new Database(tmpDir.Path, true))
                Assert.IsNull(db.Get(0));
        }
        [TestMethod]
        public void TestInsertAndRetrieveInDB()
        {
            using (var tmpDir = new TempDirectory())
            using (var db = new Database(tmpDir.Path, true))
            {
                db.Put(0, "data");
                Assert.IsTrue(db.Get(0).Equals("data"));
            }
        }
        [TestMethod]
        public void TestRetrieveAllSortedInDB()
        {
            using (var tmpDir = new TempDirectory())
            using (var db = new Database(tmpDir.Path, true))
            {
                for (var i = 0; i < 5; i++)
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
            using (var tmpDir = new TempDirectory())
            using (var db = new Database(tmpDir.Path, true))
            {
                var lastIndex = 5;
                for (var i = 0; i <= lastIndex; i++)
                    db.Put(i, $"New block {i}");
                var lastElmt = db.GetLast();
                Assert.IsTrue(lastElmt.Equals($"New block {lastIndex}"));
            }
        }
        [TestMethod]
        public void TestRetrieveNonExistingElement()
        {
            using (var tmpDir = new TempDirectory())
            using (var db = new Database(tmpDir.Path, true))
                Assert.IsNull(db.Get(-1));
        }
    }
}
