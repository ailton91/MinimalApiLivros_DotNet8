using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.TesteXUnit.TestEntities
{
    public class LivroTests
    {
        [Fact]
        public void CriarLivro_Valido_DeveCriarLivroComSucesso()
        {
            // Arrange
            string titulo = "Título_01";
            string autor = "Autor_01";

            // Act
            var livro = new Livro(titulo, autor);

            // Assert
            Assert.Equal(titulo, livro.Titulo);
            Assert.Equal(autor, livro.Autor);
        }

        //

        [Theory]
        [InlineData(null, "Autor_01")]
        [InlineData("", "Autor_01")]
        [InlineData(" ", "Autor_01")]
        public void CriarLivro_TituloInvalido_DeveLancarArgumentException(string? titulo, string? autor)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Livro(titulo, autor));
            Assert.Equal("O título do livro não pode ser nulo ou vazio. (Parameter 'titulo')", exception.Message);
        }

        [Theory]
        [InlineData("Título_01", null)]
        [InlineData("Título_01", "")]
        [InlineData("Título_01", " ")]
        public void CriarLivro_AutorInvalido_DeveLancarArgumentException(string? titulo, string? autor)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Livro(titulo, autor));
            Assert.Equal("O autor do livro não pode ser nulo ou vazio. (Parameter 'autor')", exception.Message);
        }

        [Theory]
        [InlineData("Título_01", null)]
        [InlineData("Título_01", "")]
        [InlineData("Título_01", " ")]
        public void AtualizarLivro_AutorInvalido_DeveLancarArgumentException(string? titulo, string? autor)
        {
            // Arrange
            var livro = new Livro("Título Original", "Autor Original");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => livro.AtualizarLivro(titulo, autor));
            Assert.Equal("O autor do livro não pode ser nulo ou vazio. (Parameter 'autor')", exception.Message);
        }

        [Theory]
        [InlineData(null, "Ailton")]
        [InlineData("", "Ailton")]
        [InlineData(" ", "Ailton")]
        public void AtualizarLivro_TituloInvalido_DeveLancarArgumentException(string? titulo, string? autor)
        {
            // Arrange
            var livro = new Livro("Título Original", "Autor Original");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => livro.AtualizarLivro(titulo, autor));
            Assert.Equal("O título do livro não pode ser nulo ou vazio. (Parameter 'titulo')", exception.Message);
        }

    }
}
