using System.Threading.Tasks;

namespace ChainObj
{
    class Program
    {
        static void Main(string[] args)
        {
            var blockchain = new Blockchain();
            for(var i=1; i<21; i++)
                blockchain = blockchain.InsertBlock($"New block {i}");
            var valid = blockchain.IsValidChain();
            var blocks = blockchain.GetAll();
        }
    }
}
