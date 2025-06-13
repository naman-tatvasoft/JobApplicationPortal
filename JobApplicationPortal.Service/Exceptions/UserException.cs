namespace JobApplicationPortal.Service.Exceptions;

public class UserException : ApplicationException
{
  protected UserException(string message, string errorCode) : base(message, errorCode)
  {
  }

  protected UserException(string message, string errorCode, Exception innerException) : base(message, errorCode, innerException)
  {
  }
}
