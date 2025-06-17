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

public class EmployerNotFoundException : JobException
{
    public EmployerNotFoundException() : base("Employer not found", "employer_not_found_exception"){}
}

public class JobNameAlreadyExistException : JobException
{
    public JobNameAlreadyExistException() : base("Job with the same title already exists for this employer.", "job_name_exception"){}
}

public class SkillNotPresentException : JobException
{
    public SkillNotPresentException() : base("Such skill is not present", "skill_not_present_exception"){}
}

public class JobNotFoundException : JobException
{
    public JobNotFoundException() : base("Job not found", "job_not_found_exception"){}
}

public class JobNotByEmployerException : JobException
{
    public JobNotByEmployerException() : base("Job is not created by the employer.", "job_not_by_employer_exception"){}
}

public class JobAlreadyOpenedException : JobException
{
    public JobAlreadyOpenedException() : base("Job is already opened.", "job_already_opened_exception"){}
}

public class JobAlreadyDeleted : JobException
{
    public JobAlreadyDeleted() : base("Job is already deleted.", "job_already_deleted_exception"){}
}

public class JobPreferenceAlreadyExistsException : JobException
{
    public JobPreferenceAlreadyExistsException() : base("Job preference already exists for this candidate.", "job_preference_already_exists_exception"){}
}

public class JobPreferenceNotExistsException : JobException
{
    public JobPreferenceNotExistsException() : base("Job preference does not exist.", "job_preference_not_exists_exception"){}
}

public class CategoryNotFoundException : JobException
{
    public CategoryNotFoundException() : base("Category not found", "category_not_found_exception"){}
}

public class JobPreferenceNotBelongsToCandidateException : JobException
{
    public JobPreferenceNotBelongsToCandidateException() : base("Job preference does not belong to the candidate.", "job_preference_not_belongs_to_candidate_exception"){}
}