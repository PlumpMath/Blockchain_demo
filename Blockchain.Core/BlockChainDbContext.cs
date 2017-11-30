using Microsoft.EntityFrameworkCore;

namespace Blockchain_example.Data{
    public class BlockChainDbContext : DbContext{
        public DbSet<Blockchain.Core.Blockchain.Block> Blocks{get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=.//db/blocks.db");
        }
    }
}

