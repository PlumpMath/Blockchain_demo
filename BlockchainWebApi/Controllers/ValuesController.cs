using System.Text;
using Blockchain_example.SocketHandlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BlockchainWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IHubContext<BlockchainHub> blockChainHub;

        public ValuesController(IHubContext<BlockchainHub> chathubContext)
        {
            blockChainHub = chathubContext;
        }

        [HttpGet]
        [Route("Genesis")]
        public IActionResult BlockchainGenesis()
        {
            var blockChain = new Blockchain.Core.Blockchain.Blockchain();
            blockChain.Genesis();

            var data = Encoding.UTF8.GetBytes("These are genesis blocks");
            blockChain.Add(blockChain.GenerateNewBlock(data));
            blockChain.Add(blockChain.GenerateNewBlock(data));
            blockChain.Add(blockChain.GenerateNewBlock(data));

            blockChain.SaveChainToStorage();

            

            return Json(new { success = true });
        }
        [Route("ValidateCurrent")]
        [HttpGet]
        public IActionResult ValidateCurrentBlockchain()
        {
            var blockChain = new Blockchain.Core.Blockchain.Blockchain();
            blockChain.LoadChainFromStorage();

            return Json(new {success = blockChain.ValidateCurrentBlockchain()});
        }

        [Route("Mine")]
        [HttpGet]
        public IActionResult MineNewBlock(string transactionPayload = "default payload data")
        {

            var transactionDataBuff = Encoding.UTF8.GetBytes(transactionPayload);

            blockChainHub.Clients.All.InvokeAsync("Mine", new object[] { transactionDataBuff });

            //var blockChain = new Blockchain.Core.Blockchain.Blockchain();
            //blockChain.LoadChainFromStorage();

            //var transactionDataBuff = Encoding.UTF8.GetBytes(transactionPayload);
            //blockChain.Add(blockChain.GenerateNewBlock(transactionDataBuff));
            //blockChain.SaveChainToStorage();

            //blockChainHub.Clients.All.InvokeAsync("broadcastMessage", new object[] { "Blockchain_root", "New block added" });

            //No proof of work. Have all the currency :/
            return Json(new { success = true});
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            var blockChain = new global::Blockchain.Core.Blockchain.Blockchain();
            var latestBlock = blockChain.Last();
            
            return Json(latestBlock);
        }
    }
}
