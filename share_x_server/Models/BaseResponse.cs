namespace ShareXServer.Models;

public class BaseResponse<T>
{
    public bool Successful { get; set; }
    public string? ErrorMessage { get; set; }
    public T? Data { get; set; }
}