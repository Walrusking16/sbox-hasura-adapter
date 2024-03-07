namespace Database.GraphQL;

public class Response
{
	public string? Id { get; set; }
	public PayloadResponse Payload { get; set; } = new();
	public string Type { get; set; } = "";
}

public class PayloadResponse
{
	public object Data { get; set; }
	public object Errors { get; set; }
	public string Message { get; set; } = "";
}
