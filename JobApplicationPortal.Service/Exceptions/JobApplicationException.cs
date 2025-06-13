using Microsoft.Extensions.DependencyInjection;

namespace JobApplicationPortal.Service.Exceptions;

public class JobApplicationException : ApplicationException
{
    protected JobApplicationException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JobApplicationException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}

public class CandidateNotFoundException : JobApplicationException
{
    public CandidateNotFoundException() : base("candidate not found", "candidate_not_found_exception"){}
}

public class JobNotFoundOrNotOpenToApplyException : JobApplicationException
{
    public JobNotFoundOrNotOpenToApplyException() : base("Job not found or not active or deleted or not still open to apply.","job_not_found_or_not_applicable_exception"){}
}

public class JobAlreadyAppliedException : JobApplicationException
{
    public JobAlreadyAppliedException() : base("You have already applied for this job.","already_applied_exception"){}
}

public class NotEnoughExperienceException : JobApplicationException
{
    public NotEnoughExperienceException() : base("You do not have enough experience for this job.","not_enough_experience_exception"){}
}

public class ApplicationNotFoundException : JobApplicationException
{
    public ApplicationNotFoundException() : base("Application not found.", "application_not_found_exception"){}
}
