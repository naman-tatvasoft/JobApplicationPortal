namespace JobApplicationPortal.Service.Exceptions;

public class JobException : ApplicationException
{
    protected JobException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected JobException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }
}

public class EmployerNotFoundException : AuthException
{
    public EmployerNotFoundException() : base("Employer not found", "employer_not_found_exception"){}
}

public class JobNameAlreadyExistException : AuthException
{
    public JobNameAlreadyExistException() : base("Job with the same title already exists for this employer.", "job_name_exception"){}
}

public class SkillNotPresentException : AuthException
{
    public SkillNotPresentException() : base("Such skill is not present", "skill_not_present_exception"){}
}

public class JobNotFoundException : AuthException
{
    public JobNotFoundException() : base("Job not found", "job_not_found_exception"){}
}

public class JobNotByEmployerException : AuthException
{
    public JobNotByEmployerException() : base("Job is not created by the employer.", "job_not_by_employer_exception"){}
}

public class JobAlreadyOpenedException : AuthException
{
    public JobAlreadyOpenedException() : base("Job is already opened.", "job_already_opened_exception"){}
}

public class JobAlreadyDeleted : AuthException
{
    public JobAlreadyDeleted() : base("Job is already deleted.", "job_already_deleted_exception"){}
}