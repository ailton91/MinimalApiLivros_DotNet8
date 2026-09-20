using FluentValidation;
using MinimalApiLivros.Application.AppLivro.Commands;

namespace MinimalApiLivros.Application.AppLivro.Validators
{
    public class AtualizarLivroCommandValidator : AbstractValidator<AtualizarLivroCommand>
    {
        public AtualizarLivroCommandValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título do livro é obrigatório.")
                .MaximumLength(200).WithMessage("O título do livro não pode exceder 200 caracteres.");

            RuleFor(x => x.Autor)
                .NotEmpty().WithMessage("O autor do livro é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do autor não pode exceder 100 caracteres.");
        }
    }
}
