using FluentValidation;
using FinCorralApi.Application.DTOs;

namespace FinCorralApi.Application.Validators;

public sealed class ValidateContactDtoValidator : AbstractValidator<ValidateContactDto>
{
    public ValidateContactDtoValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().When(x => string.IsNullOrWhiteSpace(x.Email)).WithMessage("Teléfono o correo requerido.");
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone).Matches(@"^\+?[0-9\-\s]{6,24}$").When(x => !string.IsNullOrWhiteSpace(x.Phone)).WithMessage("Teléfono inválido.");
    }
}