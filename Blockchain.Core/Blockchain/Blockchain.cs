using Blockchain_example.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.EntityFrameworkCore;

namespace Blockchain.Core.Blockchain
{
    //TODO: Move utility functions out to helpers.
    //TODO: Transport architecture for p2p.
    //TODO: Move to in memory management.
    public class Blockchain
    {
        private static Dictionary<int, Block> blocks;
        public static bool Validate(Block block, Block previousBlock)
        {
            if (block.Index != previousBlock.Index + 1)
                return false;
            if (previousBlock.Hash != block.PrevHash)
                return false;
            if (block.Hash != Block.CalculateHash(block.Index, block.PrevHash, block.Timestamp, block.Data))
                return false;

            return true;
        }

        public void Add(Block block)
        {
            blocks[block.Index] = block;
        }

        public Dictionary<int, Block> Blocks
        {
            get { LoadChainFromStorage();return blocks; }
        }

        public void SaveChainToStorage()
        {
            var tableName = "Blocks";
            using (var context = new BlockChainDbContext())
            {
                context.Database.ExecuteSqlCommand("DELETE FROM Blocks;");
                foreach (var block in blocks)
                {
                    context.Blocks.Add(block.Value);
                }
                context.SaveChanges();
            }

            
        }

        public void ReplaceChain(Dictionary<int,Block> blocks)
        {
            if (IsValidChain(blocks))
            {
                Blockchain.blocks = blocks;
            }
        }

        public void AddBlock(Block block)
        {
            if (Validate(block, Last()))
            {
                blocks[block.Index] = block;
            }
        }

        //TODO: May want to track invalid blocks. Will likely have to add something here then.
        private bool IsValidChain(Dictionary<int,Block> blocksToCheck)
        {
            
            if (blocks[blocks.Keys.Min()] != First())
                return false;
            
            foreach (var i in blocksToCheck.Keys)
            {
                var blockToCheck = blocksToCheck[i];
                if (String.IsNullOrEmpty(blockToCheck.PrevHash))
                    continue;
                if (!Validate(blockToCheck, blocksToCheck[i - 1]))
                    return false;
            }

            return true;
        }

        public bool ValidateCurrentBlockchain()
        {
            return IsValidChain(blocks);
        }

        public Block GenerateNewBlock(byte[] data, Block previousBlock = null, bool isFirst = false)
        {
            if (previousBlock is null && !isFirst)
                previousBlock = blocks[blocks.Keys.Max()];

            var nextIndex = (previousBlock?.Index + 1) ?? 0;
            var nextTimestamp = DateTime.Now;
            var nextHash = Block.CalculateHash(nextIndex, previousBlock?.Hash ?? string.Empty, nextTimestamp, data);

            return new Block
            {
                Hash = nextHash,
                Index = nextIndex,
                Timestamp = nextTimestamp,
                Data = data,
                PrevHash = previousBlock?.Hash ?? string.Empty
            };
        }

        public static List<Block> ValidateChain(List<Block> newBlocks, List<Block> currentBlocks)
        {
            var blocks = currentBlocks;
            if (newBlocks.Count > currentBlocks.Count)
                blocks = newBlocks;

            return blocks;
        }

        public void LoadChainFromStorage()
        {
            blocks = new Dictionary<int, Block>();
            using (var context = new BlockChainDbContext())
            {
                var tempBlocks = context.Blocks.ToList();
                foreach (var tempBlock in tempBlocks)
                {
                    blocks[tempBlock.Index] = tempBlock;
                }
            }
        }

        public Block Last()
        {
            return blocks[blocks.Keys.Max()];
        }

        public Block First()
        {
            return blocks[blocks.Keys.Min()];
        }

        public void Genesis()
        {
            var genesis = GenerateNewBlock(System.Text.Encoding.UTF8.GetBytes("Test data"), null, true);

            blocks = new Dictionary<int,Block>();
            blocks[genesis.Index] = genesis; 
            SaveChainToStorage();
        }
    }
}
