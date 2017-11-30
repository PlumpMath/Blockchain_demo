using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blockchain.Core.Blockchain;
using Microsoft.AspNetCore.SignalR;

namespace Blockchain_example.SocketHandlers
{
    public class BlockchainHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.InvokeAsync("broadcastMessage", new object[]{name,message});
        }

        public void BroadcastLatestBlockchain()
        {
            var blockChain = new Blockchain.Core.Blockchain.Blockchain();

            Clients.All.InvokeAsync("getlatestBlockchain", blockChain.Blocks);
        }

        public void Mine(byte[] data)
        {
            var blockChain = new Blockchain.Core.Blockchain.Blockchain();
            blockChain.LoadChainFromStorage();

            blockChain.Add(blockChain.GenerateNewBlock(data));
            blockChain.SaveChainToStorage();

            Send("some client","has minted a new block");
        }
    }
}
