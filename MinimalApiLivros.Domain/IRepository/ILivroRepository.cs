using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.Application.IRepository
{
    public interface ILivroRepository
    {
        Task<IEnumerable<Livro>> GetAllLivrosAsync(CancellationToken cancellationToken = default);
        Task<Livro?> GetLivroByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Livro> AddLivroAsync(Livro livro, CancellationToken cancellationToken = default);
        Task<Livro> DeleteLivroAsync(int id, CancellationToken cancellationToken = default);
        Task<Livro> UpdateLivroAsync(int id, Livro livro, CancellationToken cancellationToken = default);
    }
}
