using FluentValidation;
using JobApplicationPortal.DataModels.Dtos.RequestDtos;
using Microsoft.AspNetCore.Http;

namespace JobApplicationPortal.Validators;

public class ApplicationDtoValidator : AbstractValidator<ApplicationDto>
{
    public ApplicationDtoValidator()
    {
        RuleFor(x => x.JobId)
            .NotNull().NotEmpty().WithMessage("JobId is required.");

        RuleFor(x => x.Experience)
            .NotNull().NotEmpty().WithMessage("Experience is required.")
            .InclusiveBetween(0, 40)
            .WithMessage("Experience required must be between 0 and 40 years.");

        RuleFor(x => x.NoteForEmployer)
            .MaximumLength(500).WithMessage("Note for employer must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.NoteForEmployer));

        RuleFor(x => x.CoverLetter)
            .NotNull().WithMessage("CoverLetter is required.")
            .Must(BeAValidFile).WithMessage("CoverLetter must be a valid PDF, DOC, or DOCX file under 5MB.");

        RuleFor(x => x.Resume)
            .NotNull().WithMessage("Resume is required.")
            .Must(BeAValidFile).WithMessage("Resume must be a valid PDF, DOC, or DOCX file under 5MB.");
    }

    private bool BeAValidFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension) && file.Length <= 5 * 1024 * 1024; 
    }
}
