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
        internal void AddBlock(T data)
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
                return db.Get(height) ?? throw new KeyNotFoundException("height not found");
        }
        internal Block<T> GetBlockByHash(string hash)
        {
            using (var db = DBFactory(false))
                return db.All().FirstOrDefault(_ => ((Block<T>)_).Hash == hash) ?? throw new KeyNotFoundException("hash not found");
        }
        internal bool IsValidChain()
        {
            for(int i = 0; i < LastBlock.Height; i++)
            {
                var lBlock = GetBlock(i);
                var rBlock = GetBlock(i + 1);
                if (lBlock.Hash != rBlock.PreviousHash && lBlock.GetSha1() != lBlock.Hash)
                    return false;
            }
            return true;
        }
    }
}
