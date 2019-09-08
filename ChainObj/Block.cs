using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ChainObj
{
    [DebuggerDisplay("{Height}, {Data}, {CommitDate.ToString()}")]
    class Block
    {
        private static string GetHash(string input)
        {
            var hash = new System.Security.Cryptography.SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Concat(hash.Select(_ => _.ToString("x2")));
        }

        public int Height { get;  set; }
        public string PreviousHash { get;  set; }
        public string Hash { get; }
        public DateTime CommitDate { get;  set; }
        public string Data { get;  set; }

        public Block (string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentException("null or empty string");
            JToken token = JObject.Parse(json);
            Height = (int)token.SelectToken("Height");
            PreviousHash = (string)token.SelectToken("PreviousHash");
            Hash = (string)token.SelectToken("Hash");
            CommitDate = (DateTime)token.SelectToken("CommitDate");
            Data = (string)token.SelectToken("Data");
        }
        public Block(int height, string previousHash, string data)
        {
            Height = height;
            PreviousHash = previousHash;
            Hash = GetHash(data);
            CommitDate = DateTime.Now;
            Data = data;
        }
    }
}
