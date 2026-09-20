using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.Infrastructure
{
    public sealed class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Livro> Livros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Livro>().HasData(
                new { Id = 1, Titulo = "O Senhor dos Anéis", Autor = "J.R.R. Tolkien", DataCriacao = DateTimeOffset.UtcNow },
                new { Id = 2, Titulo = "O Hobbit", Autor = "J.R.R. Tolkien", DataCriacao = DateTimeOffset.UtcNow },
                new { Id = 3, Titulo = "Homem Aranha", Autor = "Stan Lee", DataCriacao = DateTimeOffset.UtcNow }
                //new { Id = 4, Titulo = "Dom Casmurro", Autor = "Machado de Assis", DataCriacao = DateTimeOffset.UtcNow }
                //new { Id = 5, Titulo = "O Alquimista", Autor = "Paulo Coelho", DataCriacao = DateTimeOffset.UtcNow }
            );
        }
    }
}
