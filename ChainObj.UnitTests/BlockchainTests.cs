using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChainObj.UnitTests
{
    class TempDirectory : IDisposable
    {
        internal string Path { get; }
        internal TempDirectory()
        {
            Path = Utilities.TempDirPath;
            if (System.IO.Directory.Exists(Path))
                System.IO.Directory.Delete(Path, true);
            System.IO.Directory.CreateDirectory(Path);
        }
        public void Dispose()
        {
            System.IO.Directory.Delete(Path, true);
        }
    }
    [TestClass]
    public class BlockchainTests
    {
        [TestMethod]
        public void TestEmptyBlockchain()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain(tmpDir.Path);
                Assert.AreEqual(0, blockchain.LastBlock.Height);
                Assert.IsNull(blockchain.LastBlock.PreviousHash);
                Assert.AreEqual("GenesisBlock", blockchain.LastBlock.Data);
            }
        }
        
        [TestMethod]
        public void TestInsertBlock()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain(tmpDir.Path);
                blockchain.InsertBlock("New data");
                Assert.AreEqual(1, blockchain.LastBlock.Height);
                Assert.IsNotNull(blockchain.LastBlock.PreviousHash);
                Assert.AreEqual("New data", blockchain.LastBlock.Data);
            }
        }
        [TestMethod]
        public void TestGetBlock()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain(tmpDir.Path);
                blockchain.InsertBlock("New data 1");
                blockchain.InsertBlock("New data 2");
                var block = blockchain.GetBlock(1);
                Assert.AreEqual(1, block.Height);
                Assert.AreEqual(blockchain.LastBlock.PreviousHash, block.Hash);
                Assert.AreEqual("New data 1", block.Data);
            }
        }
        [TestMethod]
        public void TestValidBlockchain()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain(tmpDir.Path);
                blockchain.InsertBlock("New data 1");
                blockchain.InsertBlock("New data 2");
                blockchain.InsertBlock("New data 3");

                Assert.IsTrue(blockchain.IsValidChain());
            }
        }
        [TestMethod]
        public void TestPersistantBlockchain()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain(tmpDir.Path);
                blockchain.InsertBlock("New data 1");
                blockchain.InsertBlock("New data 2");
                blockchain.InsertBlock("New data 3");

                var cpyBlockchain = new Blockchain(blockchain.DbPath);
                Assert.AreEqual(3, cpyBlockchain.LastBlock.Height);
            }
        }
    }
}
