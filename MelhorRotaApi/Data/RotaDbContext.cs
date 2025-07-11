using Microsoft.EntityFrameworkCore;
using MelhorRotaApi.Models;

namespace MelhorRotaApi.Data
{
    public class RotaDbContext : DbContext
    {
        public RotaDbContext(DbContextOptions<RotaDbContext> options) : base(options) { }
        public DbSet<Rota> Rotas { get; set; }
    }
}