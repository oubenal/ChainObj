using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChainObj.UnitTests
{
    class Mock : IEquatable<Mock>
    {
        public string Data;
        public Mock() { }
        public Mock(string data)
        {
            Data = data;
        }
        public bool Equals(Mock other)
        {
            return Data == other.Data;
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
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                Assert.AreEqual(0, blockchain.LastBlock.Height);
                Assert.IsNull(blockchain.LastBlock.PreviousHash);
                Mock expected = new Mock();
                Assert.IsTrue(expected.Equals(blockchain.LastBlock.Data));
                Assert.IsTrue(blockchain.LastBlock.Data.Equals(new Mock()));
            }
        }
        
        [TestMethod]
        public void TestInsertBlock()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data = new Mock("New data 1");
                blockchain.AddBlock(data);
                Assert.AreEqual(1, blockchain.LastBlock.Height);
                Assert.IsNotNull(blockchain.LastBlock.PreviousHash);
                Assert.IsTrue(data.Equals(blockchain.LastBlock.Data));
            }
        }
        [TestMethod]
        public void TestGetBlock()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data1 = new Mock("New data 1");
                var data2 = new Mock("New data 2");
                blockchain.AddBlock(data1);
                blockchain.AddBlock(data2);
                var block = blockchain.GetBlock(1);
                Assert.AreEqual(1, block.Height);
                Assert.AreEqual(blockchain.LastBlock.PreviousHash, block.Hash);
                Assert.IsTrue(data1.Equals(block.Data));
            }
        }
        [TestMethod]
        public void TestGetBlockInvalid()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                Assert.ThrowsException<KeyNotFoundException>(() => blockchain.GetBlock(-1));
                Assert.AreEqual(0, blockchain.LastBlock.Height);
            }
        }
        [TestMethod]
        public void TestGetBlockByHash()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data1 = new Mock("New data 1");
                var data2 = new Mock("New data 2");
                blockchain.AddBlock(data1);
                blockchain.AddBlock(data2);
                var block = blockchain.GetBlockByHash(data1.GetSha1());
                Assert.AreEqual(1, block.Height);
                Assert.AreEqual(blockchain.LastBlock.PreviousHash, block.Hash);
                Assert.IsTrue(data1.Equals(block.Data));
            }
        }
        [TestMethod]
        public void TestGetBlockByHashInvalid()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data = new Mock("New data");
                Assert.ThrowsException<KeyNotFoundException>(() => blockchain.GetBlockByHash(data.GetSha1()));
                Assert.AreEqual(0, blockchain.LastBlock.Height);
            }
        }
        [TestMethod]
        public void TestValidBlockchain()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data1 = new Mock("New data 1");
                var data2 = new Mock("New data 2");
                var data3 = new Mock("New data 3");
                blockchain.AddBlock(data1);
                blockchain.AddBlock(data2);
                blockchain.AddBlock(data3);

                Assert.IsTrue(blockchain.IsValidChain());
            }
        }
        [TestMethod]
        public void TestPersistantBlockchain()
        {
            using (var tmpDir = new TempDirectory())
            {
                var blockchain = new Blockchain<Mock>(tmpDir.Path);
                var data1 = new Mock("New data 1");
                var data2 = new Mock("New data 2");
                var data3 = new Mock("New data 3");
                blockchain.AddBlock(data1);
                blockchain.AddBlock(data2);
                blockchain.AddBlock(data3);

                var cpyBlockchain = new Blockchain<Mock>(blockchain.DbPath);
                Assert.AreEqual(3, cpyBlockchain.LastBlock.Height);
            }
        }
    }
}
