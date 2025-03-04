namespace API.Domain.Models;

public class ResponseModel<T>
{
    public bool Succeeded { get; set; }
    public T Value { get; set; }
    public ErrorProperty Error { get; set; }
}

public class ErrorProperty
{
    public string Property { get; set; }
    public List<string> Description { get; set; } 
}