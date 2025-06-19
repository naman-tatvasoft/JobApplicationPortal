using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.DataModels.Validators;

public class UpdateCandidateProfileDtoValidator : AbstractValidator<UpdateCandidateProfileDto>
{
     public UpdateCandidateProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");
            
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}
