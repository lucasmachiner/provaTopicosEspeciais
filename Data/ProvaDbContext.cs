using Microsoft.EntityFrameworkCore;
using provasemestral.Models;

namespace provasemestral.Data
{
    
    public class ProvaDbContext : DbContext
    {
        public ProvaDbContext(DbContextOptions<ProvaDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        //public DbSet<Cliente> Clientes { get; set; }
        //public DbSet<Fornecedor> Fornecedores { get; set; }

    }
}
