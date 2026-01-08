using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FonYonetim.Concrete
{
    public class Context:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=Oktay; database=FonYonetim; integrated security=true;");
        }

        public DbSet<StockMarket> StockMarkets { get; set; }

        public DbSet<Nasdaq> Nasdaqs { get; set; }

        public DbSet<Bist> Bists { get; set; }

        public DbSet<BistDolar> BistDolars { get; set; }
    }

}
