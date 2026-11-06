using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using MinimalApiLivros.Application.AppLivro.Commands;
using MinimalApiLivros.Application.AppLivro.Queries;
using MinimalApiLivros.Application.AppLivro.Services;
using MinimalApiLivros.Application.IRepository;
using MinimalApiLivros.Domain.Entities;
using Moq;
using System.Reflection.Metadata;

namespace MinimalApiLivros.TesteXUnit.TestServices
{
    public class LivroServiceTest
    {
        private readonly Mock<ILivroRepository> _mockLivroRepository;
        private readonly Mock<IValidator<CriarLivroCommand>> _mockValidatorCriarLivroCommand;
        private readonly Mock<IValidator<AtualizarLivroCommand>> _mockValidatorAtualizarLivroCommand;
        private readonly Mock<IValidator<ObterLivroQuery>> _mockValidatorQueryLivro;
        private readonly Mock<ILogger<LivroService>> _mockLogger;
        private readonly LivroService _livroService;

        public LivroServiceTest()
        {
            _mockLivroRepository = new Mock<ILivroRepository>();
            _mockValidatorCriarLivroCommand = new Mock<IValidator<CriarLivroCommand>>();
            _mockValidatorAtualizarLivroCommand = new Mock<IValidator<AtualizarLivroCommand>>();
            _mockValidatorQueryLivro = new Mock<IValidator<ObterLivroQuery>>();
            _mockLogger = new Mock<ILogger<LivroService>>();

            _livroService = new LivroService(
                _mockLivroRepository.Object,
                _mockValidatorCriarLivroCommand.Object,
                _mockValidatorAtualizarLivroCommand.Object,
                _mockValidatorQueryLivro.Object,
                _mockLogger.Object
            );
        }

        #region ObterTodosLivrosAsync Tests
        [Fact]
        public async Task ObterTodosLivrosAsync_DeveRetornarListaDeLivros_QuandoExistemLivros()
        {
            // Arrange
            var livros = new List<Livro>
            {
                new Livro("Livro 1", "Autor 1"),
                new Livro("Livro 2", "Autor 2")
            };

            _mockLivroRepository
                .Setup(r => r.GetAllLivrosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(livros);

            // Act
            var resultado = await _livroService.ObterTodosLivrosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count());
            _mockLivroRepository.Verify(r => r.GetAllLivrosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterTodosLivrosAsync_DeveRetornarListaVazia_QuandoNaoExistemLivros()
        {
            // Arrange
            var livros = new List<Livro>();

            _mockLivroRepository
                .Setup(r => r.GetAllLivrosAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(livros);

            // Act
            var resultado = await _livroService.ObterTodosLivrosAsync();

            // Assert
            Assert.NotNull(resultado);
            Assert.Empty(resultado);
            _mockLivroRepository.Verify(r => r.GetAllLivrosAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ObterTodosLivrosAsync_DeveLancarException_QuandoRepositorioLancaExcecao()
        {
            // Arrange
            _mockLivroRepository
                .Setup(r => r.GetAllLivrosAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro no repositório"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _livroService.ObterTodosLivrosAsync());
        }
        #endregion

        #region ObterLivroPorIdAsync Tests
        [Fact]
        public async Task ObterLivroPorIdAsync_DeveRetornarLivro_QuandoIdValido()
        {
            // Arrange
            var livro = new Livro("Livro Teste", "Autor Teste");
            var query = new ObterLivroQuery(1, "Livro Teste", "Autor Teste");

            _mockValidatorQueryLivro
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.GetLivroByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livro);

            // Act
            var resultado = await _livroService.ObterLivroPorIdAsync(query);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Livro Teste", resultado.Titulo);
            Assert.Equal("Autor Teste", resultado.Autor);
        }

        [Fact]
        public async Task ObterLivroPorIdAsync_DeveLancarArgumentException_QuandoQueryNula()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.ObterLivroPorIdAsync(null!));
        }

        [Fact]
        public async Task ObterLivroPorIdAsync_DeveLancarArgumentException_QuandoIdNulo()
        {
            // Arrange
            var query = new ObterLivroQuery(null, "Livro Teste", "Autor Teste");

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.ObterLivroPorIdAsync(query));
        }

        [Fact]
        public async Task ObterLivroPorIdAsync_DeveLancarArgumentException_QuandoValidacaoFalha()
        {
            // Arrange
            var query = new ObterLivroQuery(1, "Livro Teste", "Autor Teste");
            var validationResult = new ValidationResult(
                new[] { new ValidationFailure("Id", "Id inválido") }
            );

            _mockValidatorQueryLivro
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.ObterLivroPorIdAsync(query));
        }

        [Fact]
        public async Task ObterLivroPorIdAsync_DeveRetornarNull_QuandoLivroNaoExiste()
        {
            // Arrange
            var query = new ObterLivroQuery(99, "Livro Inexistente", "Autor Inexistente");

            _mockValidatorQueryLivro
                .Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.GetLivroByIdAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Livro?)null);

            // Act
            var resultado = await _livroService.ObterLivroPorIdAsync(query);

            // Assert
            Assert.Null(resultado);
        }
        #endregion

        #region AdicionarLivroAsync Tests
        [Fact]
        public async Task AdicionarLivroAsync_DeveAdicionarLivro_QuandoCommandValido()
        {
            // Arrange
            var command = new CriarLivroCommand("Novo Livro", "Novo Autor");
            var livroAdicionado = new Livro("Novo Livro", "Novo Autor");

            _mockValidatorCriarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroAdicionado);

            // Act
            var resultado = await _livroService.AdicionarLivroAsync(command);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Novo Livro", resultado.Titulo);
            Assert.Equal("Novo Autor", resultado.Autor);
            _mockLivroRepository.Verify(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task AdicionarLivroAsync_QuandoValidacaoFalha_DeveLancarArgumentException()
        {
            // Arrange - Simula o validator retornando IsValid = false
            var commandInvalido = new CriarLivroCommand("Novo Livro", "Novo Autor");

            var validationFailure = new ValidationFailure("Titulo", "O título do livro é obrigatório.");
            var validationResultInvalido = new ValidationResult(new[] { validationFailure });

            _mockValidatorCriarLivroCommand
                .Setup(v => v.ValidateAsync(commandInvalido, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResultInvalido);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _livroService.AdicionarLivroAsync(commandInvalido, CancellationToken.None));

            Assert.Equal("Dados do livro inválidos.", exception.Message);

            // Garante que o repositório NUNCA foi chamado se a validação falhou
            _mockLivroRepository.Verify(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Fact]
        public async Task AdicionarLivroAsync_DeveLancarArgumentException_QuandoCommandNulo()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.AdicionarLivroAsync(null!));
        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveLancarArgumentException_QuandoValidacaoFalha()
        {
            // Arrange
            var command = new CriarLivroCommand("", "Autor");
            var validationResult = new ValidationResult(
                new[] { new ValidationFailure("Titulo", "Título é obrigatório") }
            );

            _mockValidatorCriarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.AdicionarLivroAsync(command));
        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveAdicionarLivroComSucesso_QuandoCommandValidoERepositorioRetornaLivro()
        {
            // Arrange
            var command = new CriarLivroCommand("Novo Livro", "Novo Autor");
            var livroEsperado = new Livro("Novo Livro", "Novo Autor");

            _mockValidatorCriarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroEsperado);

            // Act
            var resultado = await _livroService.AdicionarLivroAsync(command);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Novo Livro", resultado.Titulo);
            Assert.Equal("Novo Autor", resultado.Autor);
            _mockLivroRepository.Verify(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveLancarArgumentException_QuandoRepositorioLancaExcecao()
        {
            // Arrange
            var command = new CriarLivroCommand("Novo Livro", "Novo Autor");

            _mockValidatorCriarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.AddLivroAsync(It.IsAny<Livro>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao adicionar livro"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _livroService.AdicionarLivroAsync(command));
        }
        #endregion

        #region AtualizarLivroAsync Tests
        [Fact]
        public async Task AtualizarLivroAsync_DeveAtualizarLivro_QuandoIdValido()
        {
            // Arrange
            var id = 1;
            var command = new AtualizarLivroCommand("Livro Atualizado", "Autor Atualizado");
            var livroExistente = new Livro("Livro Original", "Autor Original");
            var livroAtualizado = new Livro("Livro Atualizado", "Autor Atualizado");

            _mockValidatorAtualizarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.GetLivroByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroExistente);

            _mockLivroRepository
                .Setup(r => r.UpdateLivroAsync(id, It.IsAny<Livro>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(livroAtualizado);

            // Act
            var resultado = await _livroService.AtualizarLivroAsync(id, command);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Livro Atualizado", resultado.Titulo);
            Assert.Equal("Autor Atualizado", resultado.Autor);
        }

        [Fact]
        public async Task AtualizarLivroAsync_DeveLancarArgumentException_QuandoLivroNaoExiste()
        {
            // Arrange
            var id = 99;
            var command = new AtualizarLivroCommand("Livro", "Autor");

            _mockValidatorAtualizarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mockLivroRepository
                .Setup(r => r.GetLivroByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Livro?)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.AtualizarLivroAsync(id, command));
        }

        [Fact]
        public async Task AtualizarLivroAsync_DeveLancarArgumentException_QuandoValidacaoFalha()
        {
            // Arrange
            var id = 1;
            var command = new AtualizarLivroCommand("", "Autor");
            var validationResult = new ValidationResult(
                new[] { new ValidationFailure("Titulo", "Título é obrigatório") }
            );

            _mockValidatorAtualizarLivroCommand
                .Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.AtualizarLivroAsync(id, command));
        }

        [Fact]
        public async Task AtualizarLivroAsync_QuandoValidacaoFalha_DeveLancarArgumentException()
        {
            // Arrange
            int id = 1;
            var commandInvalido = new AtualizarLivroCommand("Título Inválido", "Autor Inválido");

            var validationFailure = new ValidationFailure("Titulo", "O título do livro é obrigatório.");
            var validationResultInvalido = new ValidationResult(new[] { validationFailure });

            _mockValidatorAtualizarLivroCommand
                .Setup(v => v.ValidateAsync(commandInvalido, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResultInvalido);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _livroService.AtualizarLivroAsync(id, commandInvalido, CancellationToken.None));

            Assert.Equal("Dados do livro inválidos.", exception.Message);

            // Garante que a busca no banco e a atualização NUNCA foram chamadas
            _mockLivroRepository.Verify(r => r.GetLivroByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockLivroRepository.Verify(r => r.UpdateLivroAsync(It.IsAny<int>(), It.IsAny<Livro>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        #endregion

        #region DeletarLivroAsync Tests
        [Fact]
        public async Task DeletarLivroAsync_DeveDeletarLivro_QuandoIdValido()
        {
            // Arrange
            var id = 1;
            var livro = new Livro("Livro a Deletar", "Autor");

            _mockLivroRepository
                .Setup(r => r.DeleteLivroAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(livro);

            // Act
            var resultado = await _livroService.DeletarLivroAsync(id);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("Livro a Deletar", resultado.Titulo);
            _mockLivroRepository.Verify(r => r.DeleteLivroAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeletarLivroAsync_DeveLancarException_QuandoRepositorioFalha()
        {
            // Arrange
            var id = 1;

            _mockLivroRepository
                .Setup(r => r.DeleteLivroAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro ao deletar"));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _livroService.DeletarLivroAsync(id));
        }
        #endregion
    }
}
