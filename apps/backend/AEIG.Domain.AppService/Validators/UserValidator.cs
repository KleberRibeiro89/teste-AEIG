using AEIG.Domain.Entities;
using FluentValidation;

namespace AEIG.Domain.AppService.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Nome).NotEmpty().MaximumLength(200);
        RuleFor(u => u.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(u => u.HashSenha).NotEmpty();
    }
}
