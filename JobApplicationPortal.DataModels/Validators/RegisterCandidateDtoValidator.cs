using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.DataModels.Validators;

public class RegisterCandidateDtoValidator : AbstractValidator<RegisterCandidateDto>
{
    public RegisterCandidateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$")
            .WithMessage("Password must contain at least 8 characters, including uppercase, lowercase, numbers and special characters");
    }
}