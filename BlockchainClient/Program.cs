using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Blockchain.Core.Blockchain;

namespace console_client
{
    public class Program
    {
        private static HubConnection connection;

        static void Main(string[] args)
        {
            //Blockchain.Core.Blockchain.Blockchain blockchain = new Blockchain.Core.Blockchain.Blockchain();
            //blockchain.LoadChainFromStorage();
            StartConnectionAsync();
            connection.On<string, string>("broadcastMessage", (name, message) =>
            {
                Console.WriteLine($"{name} said: {message}");
            });
            connection.On<Dictionary<int,Block>>("getlatestBlockchain", (blocks) =>
            {
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(blocks));
            });

            while (true)
            {
                var input = Console.ReadLine();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                switch (input)
                {
                    case "Mine":
                    {
                        var data = Encoding.UTF8.GetBytes("Newly minted block data");
                        
                        connection.InvokeAsync("Mine", new object[] { data });
                        connection.InvokeAsync("BroadcastLatestBlockchain");
                            break;
                    }
                }
            }

            DisposeAsync();
        }
        
        public static async Task StartConnectionAsync()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:6560/blockchain")
                .WithConsoleLogger()
                .Build();

            await connection.StartAsync();
        }

        public static async Task DisposeAsync()
        {
            await connection.DisposeAsync();
        }
    }
}