using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.DataModels.Validators;

public class JobPreferenceDtoValidator : AbstractValidator<JobPreferenceDto>
{
    public JobPreferenceDtoValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.ExperienceRequired)
            .InclusiveBetween(0, 40).WithMessage("Experience required must be between 0 and 40 years.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");
     }
}
