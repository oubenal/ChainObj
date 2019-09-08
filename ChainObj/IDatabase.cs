using System;
using System.Collections.Generic;

namespace ChainObj
{
    interface IDatabase : IDisposable
    {
        string Get(int key);
        void Put(int key, string value);
        string GetLast();
        IEnumerable<string> All();
    }
}
