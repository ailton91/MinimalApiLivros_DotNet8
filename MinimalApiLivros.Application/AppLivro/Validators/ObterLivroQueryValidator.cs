using FluentValidation;
using MinimalApiLivros.Application.AppLivro.Queries;

namespace MinimalApiLivros.Application.AppLivro.Validators
{
    public class ObterLivroQueryValidator : AbstractValidator<ObterLivroQuery>
    {
        public ObterLivroQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("O Id do livro é obrigatório.")
                .GreaterThan(0).WithMessage("O Id do livro deve ser maior que zero.");

            RuleFor(x => x.Titulo)
                .MaximumLength(200).WithMessage("O título do livro não pode exceder 200 caracteres.");

            RuleFor(x => x.Autor)
                .MaximumLength(100).WithMessage("O autor do livro não pode exceder 100 caracteres.");
        }
    }

}
