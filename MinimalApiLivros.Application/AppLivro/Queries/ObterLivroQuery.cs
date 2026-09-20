namespace MinimalApiLivros.Application.AppLivro.Queries
{
    public record class ObterLivroQuery(
        int? Id,
        string? Titulo,
        string? Autor
    );

}
