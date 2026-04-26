namespace VeterinarySystem.Responses;

public class ServiceResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ServiceResponse<T> Ok(T data, string message = "Operation Success.") =>
        new() { Success = true, Data = data, Message = message };

    public static ServiceResponse<T> Fail(string error) =>
        new() { Success = false, Message = error, Errors = new List<string> { error } };

    public static ServiceResponse<T> Fail(List<string> errors) =>
        new() { Success = false, Message = "Validations errors.", Errors = errors };
}

public class ServiceResponse : ServiceResponse<object>  
{
    public static ServiceResponse Ok(string message = "Operation success") =>
        new() { Success = true, Message = message };

    public new static ServiceResponse Fail(string error) =>
        new() { Success = false, Message = error, Errors = new List<string> { error } };
}