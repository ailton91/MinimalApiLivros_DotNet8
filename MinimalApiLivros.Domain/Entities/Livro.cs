namespace MinimalApiLivros.Domain.Entities
{
    public sealed class Livro : BaseEntity
    {
        public string? Titulo { get; private set; } = null;
        public string? Autor { get; private set; } = null;

        public Livro(string? titulo, string? autor)
        {
            ValidarTitulo(titulo);
            ValidarAutor(autor);

            this.Titulo = titulo;
            this.Autor = autor;
            this.DataCriacao = DateTimeOffset.UtcNow;
        }

        public void AtualizarLivro(string? titulo, string? autor)
        {
            ValidarTitulo(titulo);
            ValidarAutor(autor);

            this.Titulo = titulo;
            this.Autor = autor;
            this.DataAtualizacao = DateTimeOffset.UtcNow;
        }

        private static void ValidarTitulo(string? titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("O título do livro não pode ser nulo ou vazio.", nameof(titulo));
        }   
        private static void ValidarAutor(string? autor)
        {
            if (string.IsNullOrWhiteSpace(autor))
                throw new ArgumentException("O autor do livro não pode ser nulo ou vazio.", nameof(autor));
        }
    }

}
