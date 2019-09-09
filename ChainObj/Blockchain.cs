using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ChainObj
{
    class Blockchain
    {
        internal string DbPath { get; }
        private IDatabase DBFactory(bool createIfMissing) => new Database(DbPath, createIfMissing);
        internal Blockchain(string dbPath = @"C:\temp\tempdb")
        {
            DbPath = dbPath;
            using (var db = DBFactory(true))
            {
                var res = db.GetLast();
                if (res == null)
                {
                    db.Put(0, JsonConvert.SerializeObject(new Block(0, null, "GenesisBlock")));
                    res = db.Get(0);
                }
                LastBlock = new Block(res);
            }
            
        }
        internal Block LastBlock { get; private set; }
        internal void InsertBlock(string data)
        {
            using (var db = DBFactory(true))
            {
                var newBlock = new Block(LastBlock.Height + 1, LastBlock.Hash, data);
                db.Put(newBlock.Height, JsonConvert.SerializeObject(newBlock));
                LastBlock = newBlock;
            }
            
        }
        internal Block GetBlock(int height)
        {
            using (var db = DBFactory(false))
                return new Block(db.Get(height));
        }
        internal List<Block> GetAll()
        {
            using (var db = DBFactory(false))
                return db.All().Select(_ => new Block(_)).ToList();
        }
        internal bool IsValidChain()
        {
            var blocks = GetAll();
            var it = blocks.GetEnumerator();
            if (!it.MoveNext()) return false;
            var currBlock = it.Current;
            while (it.MoveNext())
            {
                var prevBlock = currBlock;
                currBlock = it.Current;
                if (currBlock.PreviousHash != prevBlock.Hash) return false;
            }
            return true;
        }
    }
}
