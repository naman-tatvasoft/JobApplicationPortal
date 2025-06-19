using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.DataModels.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDTO>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        
         RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
