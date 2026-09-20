using Microsoft.EntityFrameworkCore;
using MinimalApiLivros.Application.IRepository;
using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.Infrastructure.Repository
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _context;
        public LivroRepository(AppDbContext context) => this._context = context;

        public async Task<Livro> AddLivroAsync(Livro livro, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Livros.Add(livro);
                await _context.SaveChangesAsync(cancellationToken);
                return livro;
            }
            catch (Exception)
            {
                throw;
            }
            
        }   

        public async Task<Livro> DeleteLivroAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                Livro? livro = await _context.Livros.FindAsync(id);

                if (livro == null)
                    throw new InvalidOperationException("Livro não encontrado.");

                _context.Livros.Remove(livro);
                await _context.SaveChangesAsync(cancellationToken);
                return livro;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Livro>> GetAllLivrosAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Livro> livros = await _context.Livros.ToListAsync(cancellationToken);

                if (livros == null || livros.Count() == 0)
                    throw new InvalidOperationException("Nenhum livro encontrado.");

                return livros;
            }
            catch (Exception)
            {
                throw;
            }
           
        }

        public async Task<Livro?> GetLivroByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                Livro? livro = await _context.Livros.FindAsync(id, cancellationToken);

                if (livro == null)
                    throw new InvalidOperationException("Livro não encontrado.");

                return livro;
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<Livro> UpdateLivroAsync(int id, Livro livro, CancellationToken cancellationToken = default)
        {
            try
            {
                Livro? livroExistente = await _context.Livros.FindAsync(id, cancellationToken);

                if (livroExistente == null)
                    throw new InvalidOperationException("Livro não encontrado.");

                _context.Entry(livroExistente).CurrentValues.SetValues(livro);
                await _context.SaveChangesAsync(cancellationToken);
                return livroExistente;
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}

