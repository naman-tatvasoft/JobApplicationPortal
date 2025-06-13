namespace JobApplicationPortal.Service.Exceptions;

public abstract class AuthException : ApplicationException
{
    protected AuthException(string message, string errorCode) : base(message, errorCode)
    {
    }

    protected AuthException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
    {
    }


}

public class UnAuthenticatedException : AuthException
{
    public UnAuthenticatedException() : base("User is not authenticated.", "unauthenticated_exception"){}
}

public class RegistrationException : AuthException
{
    public RegistrationException() : base("Invalid registration data.", "registration_exception"){}
}

public class EmailAlreadyExistException : AuthException
{
    public EmailAlreadyExistException() : base("Email already exists.", "email_exist_exception"){}
}

public class LoginException : AuthException
{
    public LoginException() : base("Invalid login data.", "login_exception"){}
}

public class EmailDoesNotExistException : AuthException
{
    public EmailDoesNotExistException() : base("Email does not exists.", "email_not_exist_exception"){}
}

public class InvalidPasswordException : AuthException
{
    public InvalidPasswordException() : base("Invalid Password.", "invalid_password_exception"){}
}