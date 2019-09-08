using System.Collections.Generic;
using LevelDB;

namespace ChainObj
{
    internal class Database : IDatabase
    {
        private readonly DB db;
        internal Database()
            : this(@"C:\temp\tempdb", false)
        { }
        internal Database(bool createIfMissing)
            : this(@"C:\temp\tempdb", createIfMissing)
        { }
        internal Database(string dbpath)
            : this(dbpath, false)
        { }
        internal Database(string dbpath, bool createIfMissing)
        {
            var options = new Options { CreateIfMissing = createIfMissing };
            db = new DB(options, dbpath);
        }
        public string Get(int key)
        {
            return db.Get(key.ToFixedHex());
        }
        public void Put(int key, string value)
        {
            db.Put(key.ToFixedHex(), value);
        }
        public IEnumerable<string> All()
        {
            using (var it = db.CreateIterator())
            {
                for (it.SeekToFirst(); it.IsValid(); it.Next())
                    yield return it.ValueAsString();
            }
        }
        public string GetLast()
        {
            using (var it = db.CreateIterator())
            {
                it.SeekToLast();
                if (it.IsValid()) return it.ValueAsString();
                return null;
            }
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
