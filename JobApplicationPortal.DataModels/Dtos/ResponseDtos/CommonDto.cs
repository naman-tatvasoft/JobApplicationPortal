namespace JobApplicationPortal.DataModels.Dtos.ResponseDtos;

public class CommonDto<T>
{
    public int StatusCode {get; set;}
    public string Message {get; set; }
    public T Data { get; set; }
}
