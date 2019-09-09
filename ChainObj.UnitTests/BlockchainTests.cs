using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChainObj.UnitTests
{
    [TestClass]
    public class BlockchainTests
    {
        private static Blockchain CreateBlockchain()
        {
            var dbPath = Utilities.TempDirPath;
            if (System.IO.Directory.Exists(dbPath))
                System.IO.Directory.Delete(dbPath, true);

            var blockchain = new Blockchain(dbPath);
            return blockchain;
        }
        [TestMethod]
        public void TestEmptyBlockchain()
        {
            var blockchain = CreateBlockchain();
            Assert.AreEqual(0, blockchain.LastBlock.Height);
            Assert.IsNull(blockchain.LastBlock.PreviousHash);
            Assert.AreEqual("GenesisBlock", blockchain.LastBlock.Data);
        }
        
        [TestMethod]
        public void TestInsertBlock()
        {
            var blockchain = CreateBlockchain();
            blockchain.InsertBlock("New data");
            Assert.AreEqual(1, blockchain.LastBlock.Height);
            Assert.IsNotNull(blockchain.LastBlock.PreviousHash);
            Assert.AreEqual("New data", blockchain.LastBlock.Data);
        }
        [TestMethod]
        public void TestGetBlock()
        {
            var blockchain = CreateBlockchain();
            blockchain.InsertBlock("New data 1");
            blockchain.InsertBlock("New data 2");
            var block = blockchain.GetBlock(1);
            Assert.AreEqual(1, block.Height);
            Assert.AreEqual(blockchain.LastBlock.PreviousHash, block.Hash);
            Assert.AreEqual("New data 1", block.Data);
        }
        [TestMethod]
        public void TestValidBlockchain()
        {
            var blockchain = CreateBlockchain();
            blockchain.InsertBlock("New data 1");
            blockchain.InsertBlock("New data 2");
            blockchain.InsertBlock("New data 3");
            
            Assert.IsTrue(blockchain.IsValidChain());
        }
    }
}
