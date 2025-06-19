using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;

namespace JobApplicationPortal.DataModels.Validators
{
    public class JobDtoValidator : AbstractValidator<JobDto>
    {
        public JobDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("JobTitle is required.")
                .MaximumLength(50).WithMessage("JobTitle must not exceed 50 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(255).WithMessage("Description must not exceed 255 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.");
               
            RuleFor(x => x.ExperienceRequired)
                .InclusiveBetween(0, 40).WithMessage("Experience required must be between 0 and 40 years.");

            RuleFor(x => x.Vacancies)
                .GreaterThan(0).WithMessage("Vacancies must be greater than 0.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.");
        }
    }
}