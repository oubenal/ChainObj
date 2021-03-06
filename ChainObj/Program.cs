﻿namespace ChainObj
{
    class Program
    {
        static void Main(string[] args)
        {
            var blockchain = new Blockchain<MyData>(@"C:\temp\blockchain");
            for(var i=1; i<21; i++)
                blockchain.AddBlock($"New block {i}");
            var valid = blockchain.IsValidChain();
            var block = blockchain.GetBlockByHash(blockchain.GetBlock(10).Hash);
        }
    }
    class MyData : System.IEquatable<MyData>
    {
        public string Data;
        public MyData() { }
        public MyData(string data)
        {
            Data = data;
        }
        public static implicit operator MyData(string data)
        {
            return new MyData(data);
        }
        public bool Equals(MyData other)
        {
            return Data == other.Data;
        }
    }

}
