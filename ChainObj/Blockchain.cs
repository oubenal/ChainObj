using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ChainObj
{
    class Blockchain<T> where T : IEquatable<T>, new()
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
                    db.Put(0, new Block<T>(0, null, new T()).Serialize());
                    res = db.Get(0);
                }
                LastBlock = res;
            }
            
        }
        internal Block<T> LastBlock { get; private set; }
        internal void InsertBlock(T data)
        {
            using (var db = DBFactory(true))
            {
                LastBlock = new Block<T>(LastBlock.Height + 1, LastBlock.Hash, data);
                string json = LastBlock.Serialize();
                db.Put(LastBlock.Height, json);
            }
            
        }
        internal Block<T> GetBlock(int height)
        {
            using (var db = DBFactory(false))
                return db.Get(height);
        }
        internal List<Block<T>> GetAll()
        {
            using (var db = DBFactory(false))
                return db.All().Select(json => (Block<T>)json).ToList();
        }
        internal bool IsValidChain()
        {
            var blocks = GetAll();
            var it = blocks.GetEnumerator();
            if (!it.MoveNext()) return false;
            var currBlock = it.Current;
            if (currBlock.GetSha1() == currBlock.Hash) return false;
            while (it.MoveNext())
            {
                var prevBlock = currBlock;
                currBlock = it.Current;
                if (currBlock.PreviousHash != prevBlock.Hash && currBlock.GetSha1() == currBlock.Hash)
                    return false;
            }
            return true;
        }
        public override string ToString()
        {
            return string.Concat(Environment.NewLine, GetAll());
        }
    }
}
