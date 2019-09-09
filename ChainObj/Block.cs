using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ChainObj
{
    [DebuggerDisplay("{Height}, {Data}, {CommitDate.ToString()}")]
    class Block
    {
        public int Height { get; }
        public string PreviousHash { get; }
        public string Hash { get; }
        public DateTime CommitDate { get; }
        public string Data { get; }
        public static implicit operator Block(string json)
        {
            return JsonConvert.DeserializeObject<Block>(json);
        }
        public Block(int height, string previousHash, string data)
        {
            Height = height;
            PreviousHash = previousHash;
            Hash = data.GetSha1();
            CommitDate = DateTime.Now;
            Data = data;
        }
    }
}
