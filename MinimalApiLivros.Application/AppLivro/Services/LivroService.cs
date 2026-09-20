using FluentValidation;
using Microsoft.Extensions.Logging;
using MinimalApiLivros.Application.AppLivro.Commands;
using MinimalApiLivros.Application.AppLivro.Queries;
using MinimalApiLivros.Application.IRepository;
using MinimalApiLivros.Domain.Entities;

namespace MinimalApiLivros.Application.AppLivro.Services
{
    public class LivroService : ILivroService
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IValidator<CriarLivroCommand> _validatorCriarLivroCommand;
        private readonly IValidator<AtualizarLivroCommand> _validatorAtualizarLivroCommand;
        private readonly IValidator<ObterLivroQuery> _validatorQueryLivro;
        private readonly ILogger<LivroService> _logger;

        public LivroService(
            ILivroRepository livroRepository
            , IValidator<CriarLivroCommand> validatorCriarLivroCommand
            , IValidator<AtualizarLivroCommand> validatorAtualizarLivroCommand
            , IValidator<ObterLivroQuery> validatorQueryLivro
            , ILogger<LivroService> logger
            )
        {
            this._livroRepository = livroRepository;
            this._validatorCriarLivroCommand = validatorCriarLivroCommand;
            this._validatorAtualizarLivroCommand = validatorAtualizarLivroCommand;
            this._validatorQueryLivro = validatorQueryLivro;
            this._logger = logger;
        }

        public async Task<Livro?> ObterLivroPorIdAsync(ObterLivroQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                if (query == null || query.Id == null)
                    throw new ArgumentException("Id do livro inválido.");

                var validationResult = await this._validatorQueryLivro.ValidateAsync(query!, cancellationToken);

                if (!validationResult.IsValid)
                {
                    this._logger.LogWarning("Validação falhou para obter livro por id: {Id}, Título: {Titulo}, Autor: {Autor}   ", query.Id.Value, query.Titulo, query.Autor);
                    throw new ArgumentException("Dados do livro inválidos.");
                }

                Livro? response = await _livroRepository.GetLivroByIdAsync(query.Id.Value, cancellationToken);

                this._logger.LogInformation("ObterLivroPorIdAsync - Obtendo livro por id: {Id}, Título: {Titulo}, Autor: {Autor}   ", query.Id.Value, query.Titulo, query.Autor);

                return response;
            }
            catch (Exception ex)
            {
                this._logger.LogError("Erro ao obter livro por id: {Id}. ( Error: {exMessage} ) ", query?.Id, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<Livro>> ObterTodosLivrosAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Livro> response = await _livroRepository.GetAllLivrosAsync(cancellationToken);

                this._logger.LogInformation("ObterTodosLivrosAsync - Obtendo todos os livros.");

                return response;
            }
            catch (Exception ex)
            {
                this._logger.LogError("ObterTodosLivrosAsync - Não foi possível, buscar os dados de todos os Livros. ( Error: {exMessage} ) ", ex.Message);
                throw;
            }            
        }

        public async Task<Livro> AdicionarLivroAsync(CriarLivroCommand command,CancellationToken cancellationToken = default)
        {
            try
            {
                if (command == null)
                {
                    this._logger.LogWarning("AdicionarLivroAsync - Comando nulo para adicionar livro.");
                    throw new ArgumentException("Dados do livro inválidos.");
                }

                var validationResult = await _validatorCriarLivroCommand.ValidateAsync(command, cancellationToken);

                if (!validationResult.IsValid)
                {
                    this._logger.LogWarning("AdicionarLivroAsync - Validação falhou para adicionar livro. ( Título: {Titulo}, Autor: {Autor} ) ", command.Titulo, command.Autor);
                    throw new ArgumentException("Dados do livro inválidos.");
                }

                Livro novoLivro = new Livro(command.Titulo!, command.Autor!);

                if (novoLivro == null) { 
                    this._logger.LogWarning("AdicionarLivroAsync - Não foi possível criar o livro. ( Título: {Titulo}, Autor: {Autor} ) ", command.Titulo, command.Autor);
                    throw new ArgumentException("Não foi possível criar o livro.");
                }

                Livro response = await _livroRepository.AddLivroAsync(novoLivro, cancellationToken);

                this._logger.LogInformation("AdicionarLivroAsync - Livro adicionado com sucesso. ( Id: {Id}, Título: {Titulo}, Autor: {Autor} ) ", response.Id, response.Titulo, response.Autor);

                return response;
            }
            catch (Exception ex)
            {
                this._logger.LogError("AdicionarLivroAsync - Erro ao adicionar livro. ( Error: {exMessage} ) ", ex.Message);
                throw;
            }
            
        }

        public async Task<Livro> AtualizarLivroAsync(int id, AtualizarLivroCommand command, CancellationToken cancellationToken = default)
        {
            try
            {
                var validationResult = await _validatorAtualizarLivroCommand.ValidateAsync(command, cancellationToken);

                if (!validationResult.IsValid) { 
                    this._logger.LogWarning("AtualizarLivroAsync - Validação falhou para atualizar livro. ( Id: {Id}, Título: {Titulo}, Autor: {Autor} ) ", id, command.Titulo, command.Autor);
                    throw new ArgumentException("Dados do livro inválidos.");
                }

                Livro? livroAtualizar = await _livroRepository.GetLivroByIdAsync(id, cancellationToken);

                if (livroAtualizar == null) { 
                    this._logger.LogWarning("AtualizarLivroAsync - Livro não foi encontrado para atualização. ( Id: {Id} ) ", id);
                    throw new ArgumentException("Livro não foi encontrado para atualização.");
                }

                livroAtualizar.AtualizarLivro(command.Titulo, command.Autor);

                Livro response =  await _livroRepository.UpdateLivroAsync(id, livroAtualizar, cancellationToken);

                this._logger.LogInformation("AtualizarLivroAsync - Livro atualizado com sucesso. ( Id: {Id}, Título: {Titulo}, Autor: {Autor} ) ", response.Id, response.Titulo, response.Autor);

                return response;
            }
            catch (Exception ex)
            {
                this._logger.LogError("AtualizarLivroAsync - Erro ao atualizar livro com id: {Id}. ( Error: {exMessage} ) ", id, ex.Message);
                throw;
            }
        }

        public async Task<Livro> DeletarLivroAsync(int id, CancellationToken cancellationToken = default )
        {
            try
            {
                Livro response = await _livroRepository.DeleteLivroAsync(id, cancellationToken);

                this._logger.LogInformation("DeletarLivroAsync - Livro deletado com sucesso. ( Id: {Id}, Título: {Titulo}, Autor: {Autor} ) ", response.Id, response.Titulo, response.Autor);

                return response;    
            }
            catch (Exception ex)
            {
                this._logger.LogError("DeletarLivroAsync - Erro ao deletar livro com id: {Id}. ( Error: {exMessage} ) ", id, ex.Message);
                throw new ArgumentException("Não foi possível deletar o livro.", ex.Message);
            }
        }

        

    }
}
