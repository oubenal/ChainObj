using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ChainObj
{
    class Blockchain
    {
        private IDatabase DBFactory(bool createIfMissing) => new Database(createIfMissing);
        Block LastBlock { get; }
        private Blockchain(Block block)
        {
            LastBlock = block;
        }
        internal Blockchain()
        {
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
        int Height => LastBlock.Height;
        internal Blockchain InsertBlock(string data)
        {
            using (var db = DBFactory(true))
            {
                var newBlock = new Block(Height + 1, LastBlock.Hash, data);
                db.Put(newBlock.Height, JsonConvert.SerializeObject(newBlock));
                return new Blockchain(newBlock);
            }
            
        }
        internal List<Block> GetAll()
        {
            using (var db = DBFactory(false))
                return db.All().Select(_ => new Block(_)).ToList();
        }
        internal bool IsValidChain()
        {
            using (var db = DBFactory(false))
            {
                var listBlock = db.All().Select(_ => new Block(_));
                var it = listBlock.GetEnumerator();
                if (!it.MoveNext()) return false;
                var currBlock = it.Current;
                while(it.MoveNext())
                {
                    var prevBlock = currBlock;
                    currBlock = it.Current;
                    if (currBlock.PreviousHash != prevBlock.Hash) return false;
                }
            }
            return true;
        }
    }
}
