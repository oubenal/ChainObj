using System;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ChainObj
{
    [DebuggerDisplay("{Height}, {Data}, {CommitDate.ToString()}")]
    class Block<T>
    {
        public int Height { get; }
        public string PreviousHash { get; }
        public string Hash { get; }
        public DateTime CommitDate { get; }
        public T Data { get; }
        public static implicit operator Block<T>(string json)
        {
            return JsonConvert.DeserializeObject<Block<T>>(json);
        }
        public Block(int height, string previousHash, T data)
        {
            Height = height;
            PreviousHash = previousHash;
            Hash = data.GetSha1();
            CommitDate = DateTime.Now;
            Data = data;
        }
        public override string ToString()
        {
            return $@"
                Height={Height}
                PreviousHash={PreviousHash}
                Hash={Hash}
                CommitDate={CommitDate}
                Data={Data}";
        }
    }
}
