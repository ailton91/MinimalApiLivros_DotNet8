using MinimalApiLivros.Application.AppLivro.Commands;
using MinimalApiLivros.Application.AppLivro.Queries;
using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.Application.AppLivro.Services
{
    public interface ILivroService
    {
        Task<IEnumerable<Livro>> ObterTodosLivrosAsync(CancellationToken cancellationToken = default);
        Task<Livro?> ObterLivroPorIdAsync(ObterLivroQuery query, CancellationToken cancellationToken = default);
        Task<Livro> AdicionarLivroAsync(CriarLivroCommand command, CancellationToken cancellationToken = default);
        Task<Livro> DeletarLivroAsync(int Id, CancellationToken cancellationToken = default);
        Task<Livro> AtualizarLivroAsync(int id, AtualizarLivroCommand command, CancellationToken cancellationToken = default);
    }
}
